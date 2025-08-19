import { Injectable, inject } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
// import { RoleService } from '../services/admin-routes/role.service'; // Moins susceptible d'être nécessaire ici
import { Observable, of, switchMap, take, map } from 'rxjs'; // map ajouté

/**
 * Guard basé sur une classe pour vérifier les rôles
 */
@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
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
          console.log('🛡️ RoleGuard - Non authentifié, redirection vers /login avec returnUrl:', state.url);
          this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
          return of(false);
        }

        const requiredRoles = route.data?.['roles'] as string[] | undefined;
        if (!requiredRoles || requiredRoles.length === 0) {
          console.log('🛡️ RoleGuard - Aucun rôle spécifique requis, accès autorisé.');
          return of(true);
        }

        // Utiliser les rôles de userRoles$ et la méthode synchrone hasAnyRole
        return this.authService.userRoles$.pipe(
          take(1),
          map(userRoles => { // userRoles est RoleDefinition[]
            const hasRequiredRole = this.authService.hasAnyRole(requiredRoles); // Appel synchrone
            console.log('🛡️ RoleGuard - Rôles requis:', requiredRoles, 'Rôles utilisateur:', userRoles.map(r=>r.name), 'Autorisation:', hasRequiredRole);
            if (hasRequiredRole) {
              return true;
            }
            console.log('❌ RoleGuard - Accès refusé. Rôles insuffisants. Redirection vers /unauthorized.');
            this.router.navigate(['/unauthorized']); 
            return false;
          })
        );
      })
    );
  }
}

/**
 * Fonction helper pour créer un guard de rôle spécifique (functional guard)
 */
export function createRoleGuard(roles: string[]) {
  return (route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> => {
    const authService = inject(AuthService);
    const router = inject(Router);

    return authService.isLoggedIn$.pipe(
      take(1),
      switchMap(isLoggedIn => {
        if (!isLoggedIn) {
          console.log('🚫 Functional RoleGuard - Non authentifié, redirection vers /login');
          router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
          return of(false);
        }
        // Utiliser la méthode synchrone hasAnyRole après s'être assuré que les rôles sont chargés (via userRoles$ si nécessaire)
        // Pour simplifier ici, on suppose que userRoles$ est à jour ou que hasAnyRole gère l'état interne.
        const hasAccess = authService.hasAnyRole(roles);
        if (hasAccess) {
          console.log('✅ Functional RoleGuard - Accès autorisé pour les rôles:', roles);
          return of(true);
        } else {
          console.log('🚫 Functional RoleGuard - Accès refusé - Rôles insuffisants. Requis:', roles);
          authService.userRoles$.pipe(take(1)).subscribe(currentRoles => {
            console.log('🚫 Functional RoleGuard - Rôles actuels:', currentRoles.map(r => r.name));
          });
          router.navigate(['/unauthorized']);
          return of(false);
        }
      })
    );
  };
}

// Export des guards pour compatibilité
export const roleGuard = RoleGuard; // Conserver l'export de la classe si elle est utilisée directement

// Guards pré-configurés pour les rôles courants
export const adminGuard = createRoleGuard(['admin', 'administrateur']);
export const managerGuard = createRoleGuard(['manager', 'gestionnaire']);
export const rhGuard = createRoleGuard(['rh', 'ressources humaines']);
export const operateurGuard = createRoleGuard(['operateur', 'operator']);
// ... autres guards spécifiques si nécessaire
