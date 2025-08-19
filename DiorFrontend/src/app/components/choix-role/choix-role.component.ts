import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { RoleService } from '../../services/admin-routes/role.service'; 
import { RoleDefinition } from '../../models/role-definition.model';
import { UserDto } from '../../models/user.model';
import { Observable, Subject } from 'rxjs';
import { takeUntil, tap, filter, first } from 'rxjs/operators'; // first ajouté

@Component({
  selector: 'app-choix-role',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './choix-role.component.html',
  styleUrl: './choix-role.component.scss'
})
export class ChoixRoleComponent implements OnInit, OnDestroy {
  userRoles$: Observable<RoleDefinition[]>;
  currentUser$: Observable<UserDto | null>;
  isLoading$: Observable<boolean>;
  localUserRoles: RoleDefinition[] = []; 

  private destroy$ = new Subject<void>();

  constructor(
    private readonly authService: AuthService,
    private readonly roleService: RoleService, 
    private readonly router: Router
  ) {
    this.userRoles$ = this.authService.userRoles$;
    this.currentUser$ = this.authService.currentUser$;
    this.isLoading$ = this.authService.isLoading$;
  }

  ngOnInit(): void {
    this.userRoles$.pipe(
      takeUntil(this.destroy$),
      tap(roles => {
        this.localUserRoles = roles; 
        console.log('[ChoixRoleComponent] Rôles utilisateur chargés:', roles);
        this.handleRoleNavigation(roles);
      })
    ).subscribe();

    this.authService.isLoggedIn$.pipe(
      first(), // Utiliser first() pour vérifier une seule fois au chargement
      filter(isLoggedIn => isLoggedIn !== true), // Si pas connecté (isLoggedIn n'est pas true)
      tap(() => {
        console.warn('[ChoixRoleComponent] Utilisateur non connecté, redirection vers /login.');
        this.router.navigate(['/login']);
      })
    ).subscribe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private handleRoleNavigation(roles: RoleDefinition[]): void {
    if (!roles || roles.length === 0) { // Vérification supplémentaire de roles
      console.warn('[ChoixRoleComponent] Aucun rôle disponible. Redirection vers /login.');
      this.router.navigate(['/login']); 
    } else if (roles.length === 1) {
      console.log('[ChoixRoleComponent] Un seul rôle disponible, sélection automatique et redirection.');
      this.selectRoleAndNavigate(roles[0].name);
    }
  }

  selectRoleAndNavigate(roleName: string): void {
    this.authService.setLoading(true); 
    const roleToSelect = this.localUserRoles.find(r => r.name.toLowerCase() === roleName.toLowerCase());

    if (roleToSelect) {
      this.authService.setCurrentRoleById(roleToSelect.id); // AuthService gère la mise à jour du rôle actif
      const route = this.authService.getRoleRoute(roleName); 
      console.log(`[ChoixRoleComponent] Sélection du rôle: ${roleName}. Redirection vers ${route}`);
      this.router.navigate([route]).finally(() => {
        this.authService.setLoading(false);
      });
    } else {
      console.error(`[ChoixRoleComponent] Rôle '${roleName}' non trouvé dans les rôles de l'utilisateur.`);
      this.authService.setLoading(false);
      // Gérer l'erreur, peut-être rediriger vers une page d'erreur ou /login
      this.router.navigate(['/login']);
    }
  }

  getRoleIcon(roleName: string): string {
    const iconMap: Record<string, string> = {
      'admin': '👑', 'administrateur': '👑', 'manager': '👨‍💼', 'gestionnaire': '👨‍💼',
      'rh': '👥', 'ressources humaines': '👥', 'operateur': '⚙️', 'operator': '⚙️'
    };
    return iconMap[roleName.toLowerCase()] || '👤';
  }

  getRoleDescription(roleName: string): string {
    const descriptionMap: Record<string, string> = {
      'admin': 'Accès complet au système', 'administrateur': 'Accès complet au système',
      'manager': 'Gestion d\'équipe et projets', 'gestionnaire': 'Gestion d\'équipe et projets',
      'rh': 'Gestion des ressources humaines', 'ressources humaines': 'Gestion des ressources humaines',
      'operateur': 'Opérations quotidiennes', 'operator': 'Opérations quotidiennes'
    };
    return descriptionMap[roleName.toLowerCase()] || 'Accès utilisateur standard';
  }

  getRoleDisplayName(roleName: string): string {
    const displayMap: Record<string, string> = {
      'admin': 'Administrateur', 'administrateur': 'Administrateur',
      'manager': 'Manager', 'gestionnaire': 'Manager',
      'rh': 'Ressources Humaines', 'ressources humaines': 'Ressources Humaines',
      'operateur': 'Opérateur', 'operator': 'Opérateur'
    };
    return displayMap[roleName.toLowerCase()] || roleName;
  }

  logout(): void {
    this.authService.logout(); 
  }
}
