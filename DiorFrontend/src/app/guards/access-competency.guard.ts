import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class AccessCompetencyGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean | UrlTree {
    const competency = route.data['accessCompetency'] as string | undefined;
    if (!competency) return true; // rien d'exigé

    if (this.auth.hasAccess(competency)) {
      return true;
    }

    return this.router.parseUrl('/unauthorized');
  }
}
