import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { authGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },

  { path: 'login', component: LoginComponent, title: 'Connexion - DiorSystem' },
  {
    path: 'choix-role',
    loadComponent: () => import('./components/choix-role/choix-role.component').then(m => m.ChoixRoleComponent),
    canActivate: [authGuard],
    title: 'Choix du rôle - DiorSystem'
  },
  {
  path: 'admin/audit-log',
  loadComponent: () => import('./admin/reports/audit-log.component').then(m => m.AuditLogComponent),
  canActivate: [authGuard, RoleGuard],
  data: { roles: ['admin', 'administrateur'] },
  title: 'Journal d’audit - Admin'
},


  // Nouvelle interface CRUD utilisateurs simplifiée
  {    path: 'admin/users',
    loadComponent: () => import('./components/admin-users/admin-users.component').then(m => m.AdminUsersComponent),
    canActivate: [authGuard, RoleGuard],
    data: { roles: ['admin', 'administrateur'] },
    title: 'Gestion des utilisateurs - Admin'
  },

  {
    path: 'admin/dashboard',
    canActivate: [authGuard, RoleGuard],
    data: { roles: ['admin', 'administrateur'] },
    loadChildren: () => import('./components/admin-dashboard/admin.module').then(m => m.AdminModule),
    title: 'Admin - DiorSystem'
  },
  { path: 'admin', redirectTo: 'admin/dashboard', pathMatch: 'full' },

  {
    path: 'manager',
    canActivate: [authGuard, RoleGuard],
    data: { roles: ['manager', 'gestionnaire'] },
    loadChildren: () => import('./components/manager-dashboard/manager.module').then(m => m.ManagerModule),
    title: 'Manager - DiorSystem'
  },
  {
    path: 'rh',
    canActivate: [authGuard, RoleGuard],
    data: { roles: ['rh', 'hr'] },
    loadChildren: () => import('./components/rh-dashboard/rh.module').then(m => m.RhModule),
    title: 'RH - DiorSystem'
  },
  {
    path: 'operateur',
    canActivate: [authGuard, RoleGuard],
    data: { roles: ['operateur'] },
    loadChildren: () => import('./components/operateur-dashboard/operateur.module').then(m => m.OperateurModule),
    title: 'Opérateur - DiorSystem'
  },

  // Redirections héritées
  { path: 'admin-dashboard', redirectTo: 'admin' },
  { path: 'manager-dashboard', redirectTo: 'manager' },
  { path: 'rh-dashboard', redirectTo: 'rh' },
  { path: 'operateur-dashboard', redirectTo: 'operateur' },

  {
    path: 'home',
    loadComponent: () => import('./components/home/home.component').then(m => m.HomeComponent),
    canActivate: [authGuard],
    title: 'Accueil - DiorSystem'
  },
  {
    path: 'mes-coordonnees',
    loadComponent: () => import('./components/mes-coordonnees/mes-coordonnees.component').then(m => m.MesCoordonneesComponent),
    canActivate: [authGuard],
    title: 'Mes coordonnées - DiorSystem'
  },
  {
    path: 'unauthorized',
    loadComponent: () => import('./components/unauthorized/unauthorized.component').then(m => m.UnauthorizedComponent),
    title: 'Accès non autorisé - DiorSystem'
  },
  {
    path: 'teams',
    loadComponent: () => import('./components/team-list/team-list.component').then(m => m.TeamListComponent)
  },
  {
    path: 'team/:id/members',
    loadComponent: () => import('./components/team-members/team-members.component').then(m => m.TeamMembersComponent)
  },
  {
    path: 'users/collaborateur',
    canActivate: [authGuard, RoleGuard],
    data: { roles: ['admin', 'manager', 'rh'] },
    loadComponent: () => import('./components/collaborateur-users/collaborateur-users.component').then(m => m.CollaborateurUsersComponent),
    title: 'Collaborateurs - DiorSystem'
  },
{ path: '**', redirectTo: 'login' }
];
