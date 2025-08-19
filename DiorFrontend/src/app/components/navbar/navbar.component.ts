import { Component, OnInit, OnDestroy, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { RoleDefinition } from '../../models/role-definition.model';
import { UserDto } from '../../models/user.model';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ],
  templateUrl: './navbar.component.html',
})
export class NavbarComponent implements OnInit, OnDestroy, AfterViewInit {
  isLoggedIn$: Observable<boolean>;
  currentUser$: Observable<UserDto | null>;
  userRoles$: Observable<RoleDefinition[]>;
  currentRole$: Observable<RoleDefinition | null>;
  menuOpen = false;
  currentRoleId: number | null = null;
  private destroy$ = new Subject<void>();

  constructor(
    public authService: AuthService,
    public router: Router,
    private cdr: ChangeDetectorRef
  ) {
    this.isLoggedIn$ = this.authService.isLoggedIn$;
    this.currentUser$ = this.authService.currentUser$;
    this.userRoles$ = this.authService.userRoles$;
    this.currentRole$ = this.authService.currentRole$;
  }

  ngOnInit(): void {
    this.currentRole$.pipe(takeUntil(this.destroy$)).subscribe(role => {
      this.currentRoleId = role?.id ?? null;
    });
    document.addEventListener('click', this.handleClickOutside.bind(this), true);
  }

  ngAfterViewInit(): void {
    this.cdr.detectChanges(); // évite NG0100
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    document.removeEventListener('click', this.handleClickOutside.bind(this), true);
  }

  handleClickOutside(event: MouseEvent) {
    if (this.menuOpen) {
      const menu = document.querySelector('.z-50');
      if (menu && !menu.contains(event.target as Node)) {
        this.menuOpen = false;
      }
    }
  }

  hasRole(roleName: string, roles: RoleDefinition[] | undefined | null = []): boolean {
    if (!roles) return false;
    return roles.some(r => r.name?.toLowerCase() === roleName.toLowerCase());
  }

  getCurrentRoleName(): string {
    const currentRole = this.authService.getCurrentRole();
    return currentRole?.name?.toLowerCase() || '';
  }

  isRole(roleName: string): boolean {
    return this.getCurrentRoleName() === roleName.toLowerCase();
  }

  getUserFirstName(user: UserDto | null): string {
    return user?.firstName || '';
  }

  getUserLastName(user: UserDto | null): string {
    return user?.lastName || '';
  }

  getUserUsername(user: UserDto | null): string {
    return user?.name || 'Non défini';
  }

  getUserInitial(user: UserDto | null): string {
    return user?.firstName?.charAt(0) || 'U';
  }

  getRoleName(role: RoleDefinition | null): string {
    return role?.name || 'Aucun rôle';
  }

  isUserActive(user: UserDto | null): boolean {
    return user?.isActive ?? false;
  }

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  updateUserInfo(): void {
    alert('Informations utilisateur enregistrées !');
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getAgendaUrl(): string {
    const currentRole = this.authService.getCurrentRole();
    const roleName = currentRole?.name?.toLowerCase();

    switch (roleName) {
      case 'admin':
      case 'administrateur':
        return '/admin/agenda';
      case 'manager':
      case 'gestionnaire':
        return '/manager/agenda';
      case 'rh':
      case 'hr':
        return '/rh/agenda';
      case 'operateur':
        return '/operateur/agenda';
      default:
        return '/agenda';
    }
  }

  getHomeUrl(): string {
    const currentRole = this.authService.getCurrentRole();
    const roleName = currentRole?.name?.toLowerCase();

    switch (roleName) {
      case 'admin':
      case 'administrateur':
        return '/admin-dashboard';
      case 'manager':
      case 'gestionnaire':
        return '/manager';
      case 'rh':
      case 'hr':
        return '/rh';
      case 'operateur':
        return '/operateur-dashboard';
      default:
        return '/home';
    }
  }
}
