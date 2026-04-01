import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LayoutComponent } from '../../components/layout/layout.component';
import { UsersService } from '../../services/users.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, LayoutComponent],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  users: User[] = [];
  loading = true;
  showModal = false;
  saving = false;
  editingUser: Partial<User> = {};
  isEditing = false;

  constructor(private usersService: UsersService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.usersService.getUsers().subscribe({
      next: (users) => {
        this.users = [...users];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openCreate() {
    this.editingUser = { role: 'USER' };
    this.isEditing = false;
    this.showModal = true;
    this.cdr.detectChanges();
  }

  openEdit(user: User) {
    this.editingUser = { ...user };
    this.isEditing = true;
    this.showModal = true;
    this.cdr.detectChanges();
  }

  save() {
    this.saving = true;
    const action = this.isEditing
      ? this.usersService.updateUser(this.editingUser.id!, this.editingUser)
      : this.usersService.createUser(this.editingUser);

    action.subscribe({
      next: () => {
        this.showModal = false;
        this.saving = false;
        this.loadUsers();
      },
      error: () => { this.saving = false; }
    });
  }

  deleteUser(id: number) {
    if (!confirm('هل أنت متأكد من حذف هذا المستخدم؟')) return;
    this.usersService.deleteUser(id).subscribe({
      next: () => { this.loadUsers(); },
      error: () => {}
    });
  }
}
