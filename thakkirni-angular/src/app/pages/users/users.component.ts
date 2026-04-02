import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
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

  /** Holds the last API error message to display in the error banner */
  apiError: string | null = null;

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

  // ─────────────────────────────────────────────
  // Cascading dropdown handlers
  // ─────────────────────────────────────────────

  onAgencyChange(agencyId: number | undefined) {
    // Always reset child selections when Agency changes
    this.editingUser.departmentId = undefined;
    this.editingUser.sectionId = undefined;
    this.departments = [];
    this.sections = [];
    this.apiError = null;

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
    // Always reset Section when Department changes
    this.editingUser.sectionId = undefined;
    this.sections = [];
    this.apiError = null;

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

  // ─────────────────────────────────────────────
  // Modal open helpers
  // ─────────────────────────────────────────────

  openCreate() {
    this.editingUser = { role: 'USER' };
    this.departments = [];
    this.sections = [];
    this.apiError = null;
    this.isEditing = false;
    this.showModal = true;
    this.cdr.detectChanges();
  }

  openEdit(user: User) {
    this.editingUser = { ...user };
    this.departments = [];
    this.sections = [];
    this.apiError = null;
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

  // ─────────────────────────────────────────────
  // Client-side form validity guard
  // ─────────────────────────────────────────────

  isFormValid(): boolean {
    const name = (this.editingUser.name ?? '').trim();
    const nationalId = (this.editingUser.nationalId ?? '').trim();
    const role = (this.editingUser.role ?? '').trim();
    return name.length > 0 && nationalId.length > 0 && role.length > 0;
  }

  // ─────────────────────────────────────────────
  // Save (create or update)
  // ─────────────────────────────────────────────

  save() {
    if (!this.isFormValid()) return;

    this.saving = true;
    this.apiError = null;

    const action = this.isEditing
      ? this.usersService.updateUser(this.editingUser.id!, this.editingUser)
      : this.usersService.createUser(this.editingUser);

    action.subscribe({
      next: () => {
        this.showModal = false;
        this.saving = false;
        this.loadUsers();
      },
      error: (err: HttpErrorResponse) => {
        this.saving = false;
        this.apiError = this.extractErrorMessage(err);
        this.cdr.detectChanges();
      }
    });
  }

  // ─────────────────────────────────────────────
  // Delete
  // ─────────────────────────────────────────────

  deleteUser(id: number) {
    if (!confirm('هل أنت متأكد من حذف هذا المستخدم؟')) return;
    this.usersService.deleteUser(id).subscribe({
      next: () => { this.loadUsers(); },
      error: () => {}
    });
  }

  // ─────────────────────────────────────────────
  // Private helpers
  // ─────────────────────────────────────────────

  /**
   * Extracts a user-friendly Arabic error message from an HTTP error response.
   * Handles both structured { errors: string[] } and plain string responses.
   */
  private extractErrorMessage(err: HttpErrorResponse): string {
    if (err.error) {
      // Structured backend validation response: { errors: ["..."] }
      if (err.error.errors && Array.isArray(err.error.errors) && err.error.errors.length > 0) {
        return err.error.errors.join(' | ');
      }
      // Single message string
      if (typeof err.error === 'string') {
        return err.error;
      }
      // { message: "..." }
      if (err.error.message) {
        return err.error.message;
      }
    }
    // Fallback by HTTP status
    if (err.status === 400) return 'البيانات المدخلة غير صحيحة. يرجى مراجعة الحقول.';
    if (err.status === 409) return 'رقم الهوية مستخدم بالفعل.';
    if (err.status === 401 || err.status === 403) return 'غير مصرح لك بتنفيذ هذه العملية.';
    return 'حدث خطأ غير متوقع. يرجى المحاولة مجدداً.';
  }
}
