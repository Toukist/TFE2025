import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Observable, Subject } from 'rxjs';
import { map, takeUntil, first } from 'rxjs/operators';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="unauthorized-container">
      <div class="unauthorized-content">
        <div class="icon">🚫</div>
        <h1>Accès non autorisé</h1>
        <p>Vous n'avez pas les permissions nécessaires pour accéder à cette page.</p>
        <div class="actions">
          <button class="btn-primary" (click)="goBack()">Retour</button>
          <button class="btn-secondary" (click)="goToRoleSelection()" *ngIf="(canChangeRole$ | async)">
            Changer de rôle
          </button>
          <button class="btn-danger" (click)="logout()">Se déconnecter</button>
        </div>
        <div *ngIf="(authService.currentUser$ | async) as user" class="user-info">
          <p>Utilisateur : {{ user.firstName }} {{ user.lastName }}</p>
          <!-- Correction pour l'affichage des rôles -->
          <p *ngIf="(authService.userRoles$ | async) as roles">Rôles : {{ formatRoles(roles) }}</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .unauthorized-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh; /* Fallback pour anciens navigateurs */
      min-height: 100dvh; /* Hauteur dynamique du viewport */
      background: #f0f2f5; /* Couleur de fond plus douce */
      padding: 2rem;
      box-sizing: border-box;
    }

    .unauthorized-content {
      text-align: center;
      background: white;
      padding: 2.5rem 3rem; /* Augmentation du padding */
      border-radius: 12px;
      box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1); /* Ombre plus prononcée */
      max-width: 550px; /* Légère augmentation de la largeur max */
      width: 100%;
    }

    .icon {
      font-size: 4.5rem; /* Icône plus grande */
      margin-bottom: 1.5rem;
      color: #e74c3c; /* Couleur de l'icône */
    }

    h1 {
      color: #34495e; /* Couleur de titre plus sobre */
      margin-bottom: 1rem;
      font-size: 2.2rem; /* Taille de titre augmentée */
      font-weight: 600;
    }

    p {
      color: #555; /* Couleur de texte plus foncée pour lisibilité */
      margin-bottom: 2rem;
      font-size: 1.1rem;
      line-height: 1.6;
    }

    .actions {
      display: flex;
      gap: 1rem;
      justify-content: center;
      flex-wrap: wrap; /* Permet aux boutons de passer à la ligne sur petits écrans */
      margin-bottom: 1.5rem; /* Espace avant les infos utilisateur */
    }

    .btn-primary, .btn-secondary, .btn-danger {
      padding: 0.8rem 1.6rem; /* Padding augmenté */
      border: none;
      border-radius: 8px; /* Coins plus arrondis */
      font-size: 1rem;
      font-weight: 500; /* Graisse de police */
      cursor: pointer;
      transition: all 0.25s ease-in-out;
      text-transform: uppercase; /* Majuscules pour les boutons */
      letter-spacing: 0.5px;
    }

    .btn-primary {
      background-color: #3498db;
      color: white;
    }
    .btn-primary:hover {
      background-color: #2980b9;
      transform: translateY(-2px); /* Effet de surélévation */
      box-shadow: 0 4px 10px rgba(52, 152, 219, 0.3);
    }

    .btn-secondary {
      background-color: #95a5a6;
      color: white;
    }
    .btn-secondary:hover {
      background-color: #7f8c8d;
      transform: translateY(-2px);
      box-shadow: 0 4px 10px rgba(149, 165, 166, 0.3);
    }

    .btn-danger {
      background-color: #e74c3c;
      color: white;
    }
    .btn-danger:hover {
      background-color: #c0392b;
      transform: translateY(-2px);
      box-shadow: 0 4px 10px rgba(231, 76, 60, 0.3);
    }
    
    .user-info {
      margin-top: 2rem;
      padding-top: 1rem;
      border-top: 1px solid #eee;
      font-size: 0.9rem;
      color: #7f8c8d;
    }
    .user-info p {
      margin-bottom: 0.5rem;
      font-size: 0.9rem; /* Assurer la cohérence */
    }
  `]
})
export class UnauthorizedComponent implements OnInit, OnDestroy {
  canChangeRole$: Observable<boolean>;
  private destroy$ = new Subject<void>();

  constructor(
    public authService: AuthService, 
    private router: Router
  ) {
    this.canChangeRole$ = this.authService.userRoles$.pipe(
      map(roles => roles.length > 1)
    );
  }

  ngOnInit(): void {
    this.authService.isLoggedIn$.pipe(
      takeUntil(this.destroy$),
      map(loggedIn => loggedIn === true) // Assurer que c'est un booléen pour le log
      ).subscribe(loggedIn => {
      console.log('[UnauthorizedComponent] User logged in:', loggedIn);
    });
    this.authService.userRoles$.pipe(takeUntil(this.destroy$)).subscribe(roles => {
      console.log('[UnauthorizedComponent] User roles:', roles.map(r => r.name));
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  goBack(): void {
    this.authService.userRoles$.pipe(first()).subscribe(roles => {
      if (roles.length > 1) {
        this.router.navigate(['/choix-role']);
      } else if (roles.length === 1) {
        const defaultRoute = this.authService.getRoleRoute(roles[0].name);
        this.router.navigate([defaultRoute]);
      } else {
        this.router.navigate(['/login']); 
      }
    });
  }

  goToRoleSelection(): void {
    this.router.navigate(['/choix-role']);
  }

  logout(): void {
    this.authService.logout(); 
  }

  formatRoles(roles: any[]): string {
    if (!roles || roles.length === 0) {
      return 'Aucun rôle';
    }
    return roles.map(r => r && r.name ? r.name : '').filter(name => !!name).join(', ');
  }
}
