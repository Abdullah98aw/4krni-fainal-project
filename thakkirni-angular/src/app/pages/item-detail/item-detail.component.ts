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
