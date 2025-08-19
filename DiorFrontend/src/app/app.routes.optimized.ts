import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { authGuard } from './guards/auth.guard';
import { adminGuard, managerGuard, createRoleGuard } from './guards/role.guard';

// Guards personnalisés pour des cas spécifiques
const userGuard = createRoleGuard(['user', 'utilisateur', 'manager', 'gestionnaire', 'admin', 'administrateur']);
const moderatorGuard = createRoleGuard(['admin', 'administrateur', 'manager', 'gestionnaire']);

export const routes: Routes = [
  // Redirection par défaut
  { 
    path: '', 
    pathMatch: 'full', 
    redirectTo: 'login' 
  },
  
  // Page de connexion (accessible sans authentification)
  { 
    path: 'login', 
    component: LoginComponent 
  },
  
  // Route de choix de rôle (accessible après authentification)
  { 
    path: 'choix-role', 
    loadComponent: () => import('./components/choix-role/choix-role.component').then(m => m.ChoixRoleComponent),
    canActivate: [authGuard],
    title: 'Choix du rôle - DiorSystem'
  },
  
  // === TABLEAUX DE BORD PAR RÔLE ===
  { 
    path: 'admin', 
    loadComponent: () => import('./components/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent),
    canActivate: [adminGuard],
    title: 'Administration - DiorSystem'
  },
  { 
    path: 'manager', 
    loadComponent: () => import('./components/manager-dashboard/manager-dashboard.component').then(m => m.ManagerDashboardComponent),
    canActivate: [managerGuard],
    title: 'Gestion - DiorSystem'
  },
  { 
    path: 'user', 
    loadComponent: () => import('./components/user-dashboard/user-dashboard.component').then(m => m.UserDashboardComponent),
    canActivate: [userGuard],
    title: 'Utilisateur - DiorSystem'
  },
  
  // === PAGES GÉNÉRALES ===
  { 
    path: 'home', 
    loadComponent: () => import('./components/home/home.component').then(m => m.HomeComponent),
    canActivate: [authGuard],
    title: 'Accueil - DiorSystem'
  },
    // === GESTION DES UTILISATEURS ET RÔLES ===
  { 
    path: 'users', 
    loadComponent: () => import('./components/users/users-page.component').then(m => m.UsersPageComponent),
    canActivate: [moderatorGuard],
    title: 'Gestion des utilisateurs - DiorSystem'
  },
  { 
    path: 'user-roles', 
    loadComponent: () => import('./components/user-role-list/user-role-list.component').then(m => m.UserRoleListComponent),
    canActivate: [moderatorGuard],
    title: 'Rôles des utilisateurs - DiorSystem'
  },
  
  // === ADMINISTRATION SYSTÈME (Admin uniquement) ===
  { 
    path: 'access', 
    loadComponent: () => import('./components/access-list/access-list.component').then(m => m.AccessListComponent),
    canActivate: [adminGuard],
    title: 'Gestion des accès - DiorSystem'
  },
  { 
    path: 'privileges', 
    loadComponent: () => import('./components/privilege-list/privilege-list.component').then(m => m.PrivilegeListComponent),
    canActivate: [adminGuard],
    title: 'Gestion des privilèges - DiorSystem'
  },
  { 
    path: 'role-definitions', 
    loadComponent: () => import('./components/role-definition-list/role-definition-list.component').then(m => m.RoleDefinitionListComponent),
    canActivate: [adminGuard],
    title: 'Définitions des rôles - DiorSystem'
  },
  { 
    path: 'role-definition-privileges', 
    loadComponent: () => import('./components/role-definition-privilege-list/role-definition-privilege-list.component').then(m => m.RoleDefinitionPrivilegeListComponent),
    canActivate: [adminGuard],
    title: 'Privilèges des rôles - DiorSystem'
  },
  
  // === COMPÉTENCES ET ACCÈS ===
  { 
    path: 'access-competencies', 
    loadComponent: () => import('./components/access-competency-list/access-competency-list.component').then(m => m.AccessCompetencyListComponent),
    canActivate: [moderatorGuard],
    title: 'Compétences d\'accès - DiorSystem'
  },
  { 
    path: 'user-access-competencies', 
    loadComponent: () => import('./components/user-access-competency-list/user-access-competency-list.component').then(m => m.UserAccessCompetencyListComponent),
    canActivate: [moderatorGuard],
    title: 'Compétences des utilisateurs - DiorSystem'
  },
  
  // === PAGES SYSTÈME ===
  { 
    path: 'unauthorized', 
    loadComponent: () => import('./components/unauthorized/unauthorized.component').then(m => m.UnauthorizedComponent),
    title: 'Accès non autorisé - DiorSystem'
  },
  
  // Route par défaut pour les URLs non reconnues
  { 
    path: '**', 
    redirectTo: 'home' 
  }
];
