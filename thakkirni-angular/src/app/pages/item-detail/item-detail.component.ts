import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { LayoutComponent } from '../../components/layout/layout.component';
import { ItemsService } from '../../services/items.service';
import { UsersService } from '../../services/users.service';
import { AuthService } from '../../services/auth.service';
import { Item, ChatMessage } from '../../models/item.model';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-item-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, LayoutComponent],
  templateUrl: './item-detail.component.html',
  styleUrls: ['./item-detail.component.scss']
})
export class ItemDetailComponent implements OnInit {
  item: Item | null = null;
  messages: ChatMessage[] = [];
  users: User[] = [];
  loading = true;
  messageText = '';
  sending = false;
  completing = false;
  deleting = false;
  activeTab: 'details' | 'chat' = 'details';

  // ── Edit modal state ──────────────────────────────────
  showEditModal = false;
  saving = false;
  editForm: {
    title: string;
    description: string;
    importance: string;
    committeeType: string;
    dueDate: string;
    memberIds: number[];
    assigneeIds: number[];
  } = this.emptyEditForm();

  // ── Member management state ───────────────────────────
  selectedAddUserId: number | null = null;
  memberActionBusy = false;
  memberActionError: string | null = null;

  constructor(
    private itemsService: ItemsService,
    private usersService: UsersService,
    public authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadItem(id);
    this.loadUsers();
  }

  loadItem(id: number) {
    this.itemsService.getItem(id).subscribe({
      next: (item) => {
        this.item = item;
        this.loading = false;
        this.cdr.detectChanges();
        this.loadMessages(id);
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
        this.router.navigate(['/dashboard']);
      }
    });
  }

  loadMessages(id: number) {
    this.itemsService.getMessages(id).subscribe({
      next: (msgs) => {
        this.messages = [...msgs];
        this.cdr.detectChanges();
        this.itemsService.markMessagesRead(id).subscribe();
      },
      error: () => {}
    });
  }

  loadUsers() {
    this.usersService.getUsers().subscribe({
      next: (users) => {
        this.users = [...users];
        this.cdr.detectChanges();
      },
      error: () => {}
    });
  }

  // ── Edit modal ────────────────────────────────────────

  openEdit() {
    if (!this.item) return;
    this.editForm = {
      title: this.item.title,
      description: this.item.description ?? '',
      importance: this.item.importance,
      committeeType: this.item.committeeType ?? '',
      dueDate: this.item.dueDate
        ? new Date(this.item.dueDate).toISOString().split('T')[0]
        : '',
      memberIds: [...(this.item.memberIds ?? [])],
      assigneeIds: [...(this.item.assigneeIds ?? [])]
    };
    this.showEditModal = true;
    this.cdr.detectChanges();
  }

  toggleEditMember(userId: number) {
    const idx = this.editForm.memberIds.indexOf(userId);
    if (idx >= 0) this.editForm.memberIds.splice(idx, 1);
    else this.editForm.memberIds.push(userId);
  }

  toggleEditAssignee(userId: number) {
    const idx = this.editForm.assigneeIds.indexOf(userId);
    if (idx >= 0) this.editForm.assigneeIds.splice(idx, 1);
    else this.editForm.assigneeIds.push(userId);
  }

  saveEdit() {
    if (!this.item || !this.editForm.title.trim()) return;
    this.saving = true;

    const payload = {
      type: this.item.type,
      title: this.editForm.title,
      description: this.editForm.description,
      importance: this.editForm.importance,
      committeeType: this.editForm.committeeType || null,
      dueDate: this.editForm.dueDate,
      memberIds: this.editForm.memberIds,
      assigneeIds: this.editForm.assigneeIds
    };

    this.itemsService.updateItem(this.item.id, payload as any).subscribe({
      next: (updated) => {
        this.item = updated;
        this.showEditModal = false;
        this.saving = false;
        this.cdr.detectChanges();
      },
      error: () => { this.saving = false; }
    });
  }

  private emptyEditForm() {
    return {
      title: '',
      description: '',
      importance: 'NORMAL',
      committeeType: '',
      dueDate: '',
      memberIds: [] as number[],
      assigneeIds: [] as number[]
    };
  }

  // ── Member management ─────────────────────────────────

  /** Users that are not yet members of this item */
  get nonMembers(): User[] {
    if (!this.item) return [];
    return this.users.filter(u => !this.item!.memberIds?.includes(u.id));
  }

  addMember() {
    if (!this.item || !this.selectedAddUserId) return;
    this.memberActionBusy = true;
    this.memberActionError = null;

    this.itemsService.addMember(this.item.id, this.selectedAddUserId).subscribe({
      next: () => {
        if (this.item && this.selectedAddUserId) {
          this.item = {
            ...this.item,
            memberIds: [...(this.item.memberIds ?? []), this.selectedAddUserId]
          };
        }
        this.selectedAddUserId = null;
        this.memberActionBusy = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.memberActionError = err?.error?.message ?? 'حدث خطأ';
        this.memberActionBusy = false;
        this.cdr.detectChanges();
      }
    });
  }

  removeMember(userId: number) {
    if (!this.item) return;
    this.memberActionBusy = true;
    this.memberActionError = null;

    this.itemsService.removeMember(this.item.id, userId).subscribe({
      next: () => {
        if (this.item) {
          this.item = {
            ...this.item,
            memberIds: (this.item.memberIds ?? []).filter(id => id !== userId)
          };
        }
        this.memberActionBusy = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.memberActionError = err?.error?.message ?? 'حدث خطأ';
        this.memberActionBusy = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ── Existing actions ──────────────────────────────────

  sendMessage() {
    if (!this.messageText.trim() || !this.item) return;
    this.sending = true;
    this.itemsService.sendMessage(this.item.id, this.messageText).subscribe({
      next: (msg) => {
        this.messages = [...this.messages, msg];
        this.messageText = '';
        this.sending = false;
        this.cdr.detectChanges();
      },
      error: () => { this.sending = false; }
    });
  }

  completeItem() {
    if (!this.item) return;
    this.completing = true;
    this.itemsService.completeItem(this.item.id).subscribe({
      next: (updated) => {
        this.item = updated;
        this.completing = false;
        this.cdr.detectChanges();
      },
      error: () => { this.completing = false; }
    });
  }

  deleteItem() {
    if (!this.item || !confirm('هل أنت متأكد من الحذف؟')) return;
    this.deleting = true;
    this.itemsService.deleteItem(this.item.id).subscribe({
      next: () => {
        this.router.navigate(['/todo']);
      },
      error: () => { this.deleting = false; }
    });
  }

  getUserName(id: number): string {
    return this.users.find(u => u.id === id)?.name || `مستخدم ${id}`;
  }

  getUserInitial(id: number): string {
    const name = this.getUserName(id);
    return name.charAt(0);
  }

  isCurrentUser(userId: number): boolean {
    return this.authService.currentUser?.id === userId;
  }

  get isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  get canComplete(): boolean {
    return this.item?.status !== 'COMPLETED' && this.isAdmin;
  }
}
