import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { LayoutComponent } from '../../components/layout/layout.component';
import { ItemsService } from '../../services/items.service';
import { UsersService } from '../../services/users.service';
import { AuthService } from '../../services/auth.service';
import { Item, CreateItemRequest, ItemStatus } from '../../models/item.model';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-items-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, LayoutComponent],
  templateUrl: './items-list.component.html',
  styleUrls: ['./items-list.component.scss']
})
export class ItemsListComponent implements OnInit {
  items: Item[] = [];
  filteredItems: Item[] = [];
  users: User[] = [];
  loading = true;
  status: ItemStatus = 'ACTIVE';
  searchQuery = '';
  filterType = '';
  showCreateModal = false;
  creating = false;

  newItem: CreateItemRequest = {
    type: 'TASK',
    title: '',
    description: '',
    importance: 'NORMAL',
    dueDate: '',
    memberIds: [],
    assigneeIds: []
  };

  titles: Record<ItemStatus, string> = {
    'ACTIVE': 'قيد التنفيذ',
    'OVERDUE': 'المتأخرة',
    'COMPLETED': 'المكتملة'
  };

  constructor(
    private itemsService: ItemsService,
    private usersService: UsersService,
    public authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.status = data['status'] || 'TODO';
      this.loadData();
    });
  }

  loadData() {
    this.loading = true;
    this.itemsService.getItems().subscribe({
      next: (items) => {
        this.items = items.filter(i => i.status === this.status);
        this.applyFilters();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => { console.error('Items error:', err); this.loading = false; this.cdr.detectChanges(); }
    });
    this.usersService.getUsers().subscribe({
      next: (users) => { this.users = [...users]; this.cdr.detectChanges(); },
      error: () => {}
    });
  }

  applyFilters() {
    let result = [...this.items];
    if (this.searchQuery) {
      const q = this.searchQuery.toLowerCase();
      result = result.filter(i => i.title.toLowerCase().includes(q) || i.itemNumber.toLowerCase().includes(q));
    }
    if (this.filterType) {
      result = result.filter(i => i.type === this.filterType);
    }
    this.filteredItems = result;
    this.cdr.detectChanges();
  }

  createItem() {
    if (!this.newItem.title || !this.newItem.dueDate) return;
    this.creating = true;
    this.itemsService.createItem(this.newItem).subscribe({
      next: () => {
        this.showCreateModal = false;
        this.creating = false;
        this.resetNewItem();
        this.loadData();
      },
      error: () => { this.creating = false; }
    });
  }

  resetNewItem() {
    this.newItem = {
      type: 'TASK',
      title: '',
      description: '',
      importance: 'NORMAL',
      dueDate: '',
      memberIds: [],
      assigneeIds: []
    };
  }

  toggleMember(userId: number) {
    const idx = this.newItem.memberIds.indexOf(userId);
    if (idx >= 0) {
      this.newItem.memberIds.splice(idx, 1);
    } else {
      this.newItem.memberIds.push(userId);
    }
  }

  toggleAssignee(userId: number) {
    const idx = this.newItem.assigneeIds.indexOf(userId);
    if (idx >= 0) {
      this.newItem.assigneeIds.splice(idx, 1);
    } else {
      this.newItem.assigneeIds.push(userId);
    }
  }

  getUserName(id: number): string {
    return this.users.find(u => u.id === id)?.name || '';
  }

  get pageTitle(): string {
    return this.titles[this.status];
  }

  get isAdmin(): boolean {
    return this.authService.isAdmin();
  }
}
