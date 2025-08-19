import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class PrivilegeGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean | UrlTree {
    const privilege = route.data['privilege'];
    if (privilege && this.auth.hasPrivilege(privilege)) {
      return true;
    }
    return this.router.parseUrl('/unauthorized');
  }
}
