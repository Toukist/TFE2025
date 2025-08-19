import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable, of, switchMap, take } from 'rxjs'; // map ajouté

/**
 * 🔐 Guard spécialisé pour les modules de gestion des utilisateurs
 * Autorise uniquement les rôles Admin et RH
 */
@Injectable({
  providedIn: 'root'
})
export class UserManagementGuard implements CanActivate {
  
  private readonly AUTHORIZED_ROLES = ['admin', 'administrateur', 'rh', 'hr', 'human-resources'];

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    
    return this.authService.isLoggedIn$.pipe( // Utilisation de isLoggedIn$ public
      take(1),
      switchMap(isLoggedIn => {
        if (!isLoggedIn) {
          console.log('🚫 UserManagementGuard - Non authentifié, redirection vers /login');
          this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
          return of(false);
        }

        // Utiliser la méthode synchrone hasAnyRole
        const hasAccess = this.authService.hasAnyRole(this.AUTHORIZED_ROLES);
        if (hasAccess) {
          console.log('✅ UserManagementGuard - Accès autorisé pour la gestion des utilisateurs');
          return of(true);
        } else {
          console.log('🚫 UserManagementGuard - Rôle insuffisant pour la gestion des utilisateurs');
          this.router.navigate(['/unauthorized']);
          return of(false);
        }
      })
    );
  }

  /**
   * Vérification rapide des permissions depuis les composants (observable)
   */
  canManageUsers(): Observable<boolean> {
    return this.authService.isLoggedIn$.pipe(
      switchMap(isLoggedIn => {
        if (!isLoggedIn) {
          return of(false);
        }
        // Retourne un observable basé sur la valeur synchrone
        return of(this.authService.hasAnyRole(this.AUTHORIZED_ROLES));
      })
    );
  }

  /**
   * Récupération des rôles autorisés pour l'affichage
   */
  getAuthorizedRoles(): string[] {
    return [...this.AUTHORIZED_ROLES];
  }
}
