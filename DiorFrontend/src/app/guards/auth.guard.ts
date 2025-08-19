import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Guard basé sur une classe pour vérifier l'authentification
 * Solution de contournement pour les problèmes d'injection dans les functional guards
 */
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    const isAuth = this.authService.isLoggedIn();
    console.log('🛡️ AuthGuard - isAuthenticated:', isAuth);
    
    if (isAuth) {
      return true;
    }
    
    console.log('❌ AuthGuard - Redirection vers /login avec returnUrl:', state.url);
    this.router.navigate(['/login'], { 
      queryParams: { returnUrl: state.url } 
    });
    return false;
  }
}

// Export de la function guard pour compatibilité
export const authGuard = AuthGuard;
