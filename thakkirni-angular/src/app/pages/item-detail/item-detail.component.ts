import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { LayoutComponent } from '../../components/layout/layout.component';
import { ItemsService } from '../../services/items.service';
import { UsersService } from '../../services/users.service';
import { AuthService } from '../../services/auth.service';
<<<<<<< HEAD
import { Item, ChatMessage, ItemAuditEvent, ItemStatus, ItemType } from '../../models/item.model';
=======
import { Item, ChatMessage } from '../../models/item.model';
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
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
<<<<<<< HEAD
  auditEvents: ItemAuditEvent[] = [];
  users: User[] = [];
  loading = true;
  messageText = '';
  selectedAttachment: File | null = null;
  sending = false;
  completing = false;
  deleting = false;
  activeTab: 'details' | 'chat' | 'members' | 'activity' = 'details';
  chatMarkedRead = false;

  showEditModal = false;
  saving = false;
  editForm = this.emptyEditForm();

=======
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
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
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
<<<<<<< HEAD
        this.loadAudit(id);
=======
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
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
<<<<<<< HEAD
      },
      error: () => {}
    });
  }

  loadAudit(id: number) {
    this.itemsService.getAudit(id).subscribe({
      next: (events) => {
        this.auditEvents = [...events];
        this.cdr.detectChanges();
=======
        this.itemsService.markMessagesRead(id).subscribe();
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
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

<<<<<<< HEAD
  setActiveTab(tab: 'details' | 'chat' | 'members' | 'activity') {
    this.activeTab = tab;
    if (tab === 'chat' && this.item && !this.chatMarkedRead) {
      this.itemsService.markMessagesRead(this.item.id).subscribe({
        next: () => {
          this.chatMarkedRead = true;
          if (this.item) {
            this.item = { ...this.item, unreadCount: 0, hasUnreadUpdates: false };
          }
          this.cdr.detectChanges();
        }
      });
    }
  }
=======
  // ── Edit modal ────────────────────────────────────────
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62

  openEdit() {
    if (!this.item) return;
    this.editForm = {
<<<<<<< HEAD
      type: this.item.type,
=======
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
      title: this.item.title,
      description: this.item.description ?? '',
      importance: this.item.importance,
      committeeType: this.item.committeeType ?? '',
<<<<<<< HEAD
      dueDate: this.item.dueDate ? new Date(this.item.dueDate).toISOString().split('T')[0] : '',
=======
      dueDate: this.item.dueDate
        ? new Date(this.item.dueDate).toISOString().split('T')[0]
        : '',
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
      memberIds: [...(this.item.memberIds ?? [])],
      assigneeIds: [...(this.item.assigneeIds ?? [])]
    };
    this.showEditModal = true;
    this.cdr.detectChanges();
  }

  toggleEditMember(userId: number) {
    const idx = this.editForm.memberIds.indexOf(userId);
<<<<<<< HEAD
    if (idx >= 0) {
      this.editForm.memberIds.splice(idx, 1);
      return;
    }
    this.editForm.memberIds.push(userId);
=======
    if (idx >= 0) this.editForm.memberIds.splice(idx, 1);
    else this.editForm.memberIds.push(userId);
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
  }

  toggleEditAssignee(userId: number) {
    const idx = this.editForm.assigneeIds.indexOf(userId);
<<<<<<< HEAD
    if (idx >= 0) {
      this.editForm.assigneeIds.splice(idx, 1);
      return;
    }
    this.editForm.assigneeIds.push(userId);
=======
    if (idx >= 0) this.editForm.assigneeIds.splice(idx, 1);
    else this.editForm.assigneeIds.push(userId);
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
  }

  saveEdit() {
    if (!this.item || !this.editForm.title.trim()) return;
    this.saving = true;

    const payload = {
<<<<<<< HEAD
      type: this.editForm.type,
      title: this.editForm.title.trim(),
      description: this.editForm.description,
      importance: this.editForm.importance,
      committeeType: this.editForm.type === 'COMMITTEE' ? this.editForm.committeeType || null : null,
=======
      type: this.item.type,
      title: this.editForm.title,
      description: this.editForm.description,
      importance: this.editForm.importance,
      committeeType: this.editForm.committeeType || null,
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
      dueDate: this.editForm.dueDate,
      memberIds: this.editForm.memberIds,
      assigneeIds: this.editForm.assigneeIds
    };

<<<<<<< HEAD
    this.itemsService.updateItem(this.item.id, payload as never).subscribe({
=======
    this.itemsService.updateItem(this.item.id, payload as any).subscribe({
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
      next: (updated) => {
        this.item = updated;
        this.showEditModal = false;
        this.saving = false;
<<<<<<< HEAD
        this.loadAudit(updated.id);
        this.cdr.detectChanges();
      },
      error: () => {
        this.saving = false;
        this.cdr.detectChanges();
      }
=======
        this.cdr.detectChanges();
      },
      error: () => { this.saving = false; }
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
    });
  }

  private emptyEditForm() {
    return {
<<<<<<< HEAD
      type: 'TASK' as ItemType,
=======
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
      title: '',
      description: '',
      importance: 'NORMAL',
      committeeType: '',
      dueDate: '',
      memberIds: [] as number[],
      assigneeIds: [] as number[]
    };
  }

<<<<<<< HEAD
  get nonMembers(): User[] {
    if (!this.item) return [];
    return this.users.filter((user) => !this.item?.memberIds?.includes(user.id));
=======
  // ── Member management ─────────────────────────────────

  /** Users that are not yet members of this item */
  get nonMembers(): User[] {
    if (!this.item) return [];
    return this.users.filter(u => !this.item!.memberIds?.includes(u.id));
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
  }

  addMember() {
    if (!this.item || !this.selectedAddUserId) return;
    this.memberActionBusy = true;
    this.memberActionError = null;

    this.itemsService.addMember(this.item.id, this.selectedAddUserId).subscribe({
<<<<<<< HEAD
      next: (updated) => {
        this.item = updated;
        this.selectedAddUserId = null;
        this.memberActionBusy = false;
        this.loadAudit(updated.id);
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.memberActionError = err?.error?.message ?? 'حدث خطأ أثناء إضافة العضو';
=======
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
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
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
<<<<<<< HEAD
      next: (updated) => {
        this.item = updated;
        this.memberActionBusy = false;
        this.loadAudit(updated.id);
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.memberActionError = err?.error?.message ?? 'حدث خطأ أثناء إزالة العضو';
=======
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
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
        this.memberActionBusy = false;
        this.cdr.detectChanges();
      }
    });
  }

<<<<<<< HEAD
  onAttachmentSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    this.selectedAttachment = input.files?.[0] ?? null;
  }

  clearAttachment() {
    this.selectedAttachment = null;
  }

  sendMessage() {
    if ((!this.messageText.trim() && !this.selectedAttachment) || !this.item) return;
    this.sending = true;

    this.itemsService.sendMessage(this.item.id, this.messageText, this.selectedAttachment).subscribe({
      next: (msg) => {
        this.messages = [...this.messages, msg];
        this.messageText = '';
        this.selectedAttachment = null;
        this.sending = false;
        this.loadAudit(this.item!.id);
        this.cdr.detectChanges();
      },
      error: () => {
        this.sending = false;
        this.cdr.detectChanges();
      }
    });
  }

  downloadAttachment(message: ChatMessage) {
    this.itemsService.downloadAttachment(message.id).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        window.open(url, '_blank');
        setTimeout(() => URL.revokeObjectURL(url), 1000);
      }
=======
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
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
    });
  }

  completeItem() {
    if (!this.item) return;
    this.completing = true;
    this.itemsService.completeItem(this.item.id).subscribe({
      next: (updated) => {
        this.item = updated;
        this.completing = false;
<<<<<<< HEAD
        this.loadAudit(updated.id);
        this.cdr.detectChanges();
      },
      error: () => {
        this.completing = false;
        this.cdr.detectChanges();
      }
=======
        this.cdr.detectChanges();
      },
      error: () => { this.completing = false; }
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
    });
  }

  deleteItem() {
    if (!this.item || !confirm('هل أنت متأكد من الحذف؟')) return;
    this.deleting = true;
    this.itemsService.deleteItem(this.item.id).subscribe({
      next: () => {
        this.router.navigate(['/todo']);
      },
<<<<<<< HEAD
      error: () => {
        this.deleting = false;
        this.cdr.detectChanges();
      }
=======
      error: () => { this.deleting = false; }
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
    });
  }

  getUserName(id: number): string {
<<<<<<< HEAD
    return this.users.find((user) => user.id === id)?.name || `مستخدم ${id}`;
  }

  getUserInitial(id: number): string {
    return this.getUserName(id).trim().charAt(0) || '؟';
  }

  getItemTypeLabel(type: ItemType): string {
    return type === 'TASK' ? 'مهمة' : 'لجنة';
  }

  getStatusLabel(status: ItemStatus): string {
    if (status === 'COMPLETED') return 'مكتملة';
    if (status === 'OVERDUE') return 'متأخرة';
    return 'قيد التنفيذ';
  }

  getStatusClass(status: ItemStatus): string {
    if (status === 'COMPLETED') return 'completed';
    if (status === 'OVERDUE') return 'overdue';
    return 'active';
=======
    return this.users.find(u => u.id === id)?.name || `مستخدم ${id}`;
  }

  getUserInitial(id: number): string {
    const name = this.getUserName(id);
    return name.charAt(0);
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
  }

  isCurrentUser(userId: number): boolean {
    return this.authService.currentUser?.id === userId;
  }

<<<<<<< HEAD
  get canManage(): boolean {
    return this.authService.canManageItems();
  }

  get canComplete(): boolean {
    return this.item?.status !== 'COMPLETED' && this.canManage;
=======
  get isAdmin(): boolean {
    return this.authService.isAdmin();
  }

  get canComplete(): boolean {
    return this.item?.status !== 'COMPLETED' && this.isAdmin;
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
  }
}
