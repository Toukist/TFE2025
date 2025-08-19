import { Routes } from '@angular/router';
import { UserListComponent } from './users/user-list.component';
import { RoleListComponent } from './roles/role-list.component';
import { SettingsComponent } from './settings/settings.component';

export const adminRoutes: Routes = [
  { path: 'users', component: UserListComponent },
  { path: 'roles', component: RoleListComponent },
  { path: 'settings', component: SettingsComponent },

  {
    path: 'audit-log',
    loadComponent: () =>
      import('./reports/audit-log.component').then((m) => m.AuditLogComponent),
  },

  { path: '', redirectTo: 'users', pathMatch: 'full' },
];
