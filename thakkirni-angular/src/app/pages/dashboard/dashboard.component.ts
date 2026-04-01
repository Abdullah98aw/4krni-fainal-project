import { Component, OnInit, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LayoutComponent } from '../../components/layout/layout.component';
import { ItemsService } from '../../services/items.service';
import { UsersService } from '../../services/users.service';
import { AuthService } from '../../services/auth.service';
import { Item } from '../../models/item.model';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, LayoutComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default
})
export class DashboardComponent implements OnInit {
  items: Item[] = [];
  users: User[] = [];
  loading = true;
  expandedUser: number | null = null;

  constructor(
    private itemsService: ItemsService,
    private usersService: UsersService,
    public authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    this.itemsService.getItems().subscribe({
      next: (items) => {
        this.items = [...items];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Items API error:', err);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });

    if (this.authService.isAdmin()) {
      this.usersService.getUsers().subscribe({
        next: (users) => {
          this.users = [...users];
          this.cdr.detectChanges();
        },
        error: (err) => { console.error('Users API error:', err); }
      });
    }
  }

  get totalItems() { return this.items.length; }
  get todoItems() { return this.items.filter(i => i.status === 'TODO').length; }
  get overdueItems() { return this.items.filter(i => i.status === 'OVERDUE').length; }
  get completedItems() { return this.items.filter(i => i.status === 'COMPLETED').length; }

  getUserStats() {
    return this.users.map(user => {
      const assigned = this.items.filter(i => i.assigneeIds?.includes(user.id));
      const completed = assigned.filter(i => i.status === 'COMPLETED');
      const overdue = assigned.filter(i => i.status === 'OVERDUE');
      const todo = assigned.filter(i => i.status === 'TODO');
      const rate = assigned.length > 0 ? Math.round((completed.length / assigned.length) * 100) : 0;
      return { user, total: assigned.length, completed: completed.length, overdue: overdue.length, todo: todo.length, rate };
    }).sort((a, b) => b.total - a.total);
  }

  toggleUser(userId: number) {
    this.expandedUser = this.expandedUser === userId ? null : userId;
  }

  getUserItems(userId: number) {
    return this.items.filter(i => i.createdById === userId || i.memberIds?.includes(userId));
  }
}
