import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminDashboardComponent } from './admin-dashboard.component';
import { AdminHomeComponent } from './admin-home.component';
import { authGuard } from '../../guards/auth.guard';
import { RoleGuard } from '../../guards/role.guard';
import { AdminUsersComponent } from '../admin-users/admin-users.component';

const routes: Routes = [
  {
    path: '',
    component: AdminDashboardComponent,
    canActivate: [authGuard, RoleGuard],
    data: { roles: ['admin', 'administrateur'] },
    children: [
      { 
        path: '', 
        component: AdminHomeComponent,
        title: 'Dashboard Admin'
      },
      { 
        path: 'agenda', 
        loadComponent: () => import('../agenda-page/agenda-page.component').then(m => m.AgendaPageComponent),
        canActivate: [authGuard, RoleGuard],
        data: { roles: ['admin', 'administrateur'] },
        title: 'Agenda - Admin'
      },
      { 
        path: 'user-access', 
        loadComponent: () => import('../user-access-management/user-access-management.component').then(m => m.UserAccessManagementComponent),
        canActivate: [authGuard, RoleGuard],
        data: { roles: ['admin', 'administrateur'] },
        title: 'Gestion des accès'
      },
      { 
        path: 'roles', 
        loadComponent: () => import('../../admin/roles/role-list.component').then(m => m.RoleListComponent),
        canActivate: [authGuard, RoleGuard],
        data: { roles: ['admin', 'administrateur'] },
        title: 'Gestion des rôles'
      },
      { 
        path: 'settings', 
        loadComponent: () => import('../../admin/settings/settings.component').then(m => m.SettingsComponent),
        canActivate: [authGuard, RoleGuard],
        data: { roles: ['admin', 'administrateur'] },
        title: 'Paramètres'
      },
      { 
        path: 'reports', 
        loadComponent: () => import('../../admin/reports/audit-log.component').then(m => m.AuditLogComponent),
        canActivate: [authGuard, RoleGuard],
        data: { roles: ['admin', 'administrateur'] },
        title: 'Rapports et logs'
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule {}
