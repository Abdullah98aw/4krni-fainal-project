import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LayoutComponent } from '../../components/layout/layout.component';
import { UsersService } from '../../services/users.service';
import { OrganizationService, Agency, Department, Section } from '../../services/organization.service';
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

  // Organization lookup lists
  agencies: Agency[] = [];
  departments: Department[] = [];
  sections: Section[] = [];

  constructor(
    private usersService: UsersService,
    private orgService: OrganizationService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadUsers();
    this.loadAgencies();
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

  loadAgencies() {
    this.orgService.getAgencies().subscribe({
      next: (agencies) => {
        this.agencies = agencies;
        this.cdr.detectChanges();
      },
      error: () => {}
    });
  }

  onAgencyChange(agencyId: number | undefined) {
    this.editingUser.departmentId = undefined;
    this.editingUser.departmentName = undefined;
    this.editingUser.sectionId = undefined;
    this.editingUser.sectionName = undefined;
    this.departments = [];
    this.sections = [];

    if (agencyId && agencyId > 0) {
      this.orgService.getDepartmentsByAgency(agencyId).subscribe({
        next: (depts) => {
          this.departments = depts;
          this.cdr.detectChanges();
        },
        error: () => {}
      });
    }
  }

  onDepartmentChange(departmentId: number | undefined) {
    this.editingUser.sectionId = undefined;
    this.editingUser.sectionName = undefined;
    this.sections = [];

    if (departmentId && departmentId > 0) {
      this.orgService.getSectionsByDepartment(departmentId).subscribe({
        next: (sects) => {
          this.sections = sects;
          this.cdr.detectChanges();
        },
        error: () => {}
      });
    }
  }

  openCreate() {
    this.editingUser = { role: 'USER' };
    this.departments = [];
    this.sections = [];
    this.isEditing = false;
    this.showModal = true;
    this.cdr.detectChanges();
  }

  openEdit(user: User) {
    this.editingUser = { ...user };
    this.departments = [];
    this.sections = [];
    this.isEditing = true;
    this.showModal = true;

    // Pre-load departments and sections for the existing user's org assignment
    if (user.agencyId) {
      this.orgService.getDepartmentsByAgency(user.agencyId).subscribe({
        next: (depts) => {
          this.departments = depts;
          if (user.departmentId) {
            this.orgService.getSectionsByDepartment(user.departmentId).subscribe({
              next: (sects) => {
                this.sections = sects;
                this.cdr.detectChanges();
              },
              error: () => {}
            });
          }
          this.cdr.detectChanges();
        },
        error: () => {}
      });
    }

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
