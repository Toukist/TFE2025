import { Component, OnInit, OnDestroy, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { RoleDefinition } from '../../../models/role-definition.model';
import { UserDto } from '../../../models/user.model';
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

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  getCurrentRoleName(): string | null {
    let roleName: string | null = null;
    this.currentRole$.subscribe(role => {
      roleName = role?.name ?? null;
    }).unsubscribe();
    return roleName;
  }

  getHomeUrl(): string {
    const role = this.getCurrentRoleName();
    switch (role) {
      case 'admin':
        return '/admin';
      case 'manager':
        return '/manager';
      case 'rh':
        return '/rh';
      case 'operateur':
        return '/operateur';
      default:
        return '/';
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
