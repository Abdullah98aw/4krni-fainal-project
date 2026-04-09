import { Routes } from '@angular/router';
<<<<<<< HEAD
import { authGuard, adminGuard, managerGuard } from './guards/auth.guard';
=======
import { authGuard, adminGuard } from './guards/auth.guard';
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'todo',
    loadComponent: () => import('./pages/items-list/items-list.component').then(m => m.ItemsListComponent),
    canActivate: [authGuard],
    data: { status: 'ACTIVE' }
  },
  {
    path: 'overdue',
    loadComponent: () => import('./pages/items-list/items-list.component').then(m => m.ItemsListComponent),
    canActivate: [authGuard],
    data: { status: 'OVERDUE' }
  },
  {
    path: 'completed',
    loadComponent: () => import('./pages/items-list/items-list.component').then(m => m.ItemsListComponent),
    canActivate: [authGuard],
    data: { status: 'COMPLETED' }
  },
  {
    path: 'items/:id',
    loadComponent: () => import('./pages/item-detail/item-detail.component').then(m => m.ItemDetailComponent),
    canActivate: [authGuard]
  },
  {
    path: 'users',
    loadComponent: () => import('./pages/users/users.component').then(m => m.UsersComponent),
    canActivate: [authGuard, adminGuard]
  },
  {
<<<<<<< HEAD
    path: 'organizations-admin',
    loadComponent: () => import('./pages/organizations-admin/organizations-admin.component').then(m => m.OrganizationsAdminComponent),
    canActivate: [authGuard, adminGuard]
  },
  {
    path: 'managers',
    loadComponent: () => import('./pages/managers/managers.component').then(m => m.ManagersComponent),
    canActivate: [authGuard, managerGuard]
=======
    path: 'managers',
    loadComponent: () => import('./pages/managers/managers.component').then(m => m.ManagersComponent),
    canActivate: [authGuard, adminGuard]
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
  },
  { path: '**', redirectTo: '/dashboard' }
];
