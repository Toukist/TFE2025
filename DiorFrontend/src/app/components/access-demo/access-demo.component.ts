import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { PermissionService } from '../../services/permission.service';

/**
 * 🔹 ÉTAPE 5 - Exemple d'utilisation dans les composants Angular
 * Ce composant montre comment utiliser les AccessCompetencies dans l'interface
 */
@Component({
  selector: 'app-access-demo',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="access-demo-container">
      <h2>🔹 Démonstration des contrôles d'accès</h2>
      
      <!-- UTILISATION DIRECTE avec *ngIf et authService.hasAccess() -->
      <section class="demo-section">
        <h3>Utilisation directe dans le template :</h3>
        
        <!-- 🔹 ÉTAPE 5: Exemples d'utilisation -->
        <button *ngIf="authService.hasAccess('CONTRACT_MANAGEMENT')" 
                class="btn btn-primary">
          Voir les contrats
        </button>
        
        <div *ngIf="authService.hasAccess('PARTYKIT_ACCESS')" 
             class="chat-container">
          <!-- <app-chat></app-chat> -->
          <p>💬 Chat PartyKit activé pour cet utilisateur</p>
        </div>
        
        <button *ngIf="authService.hasAccess('USER_MANAGEMENT')" 
                class="btn btn-secondary"
                (click)="manageUsers()">
          Gérer les utilisateurs
        </button>
        
        <button *ngIf="authService.hasAccess('BADGE_MANAGEMENT')" 
                class="btn btn-warning"
                (click)="manageBadges()">
          Gérer les badges
        </button>
      </section>
      
      <!-- UTILISATION avec PermissionService -->
      <section class="demo-section">
        <h3>Utilisation avec PermissionService :</h3>
        
        <button *ngIf="permissionService.canUsePartyKit()" 
                class="btn btn-info">
          Accès PartyKit via PermissionService
        </button>
        
        <button *ngIf="permissionService.canManageContracts()" 
                class="btn btn-success">
          Gestion contrats via PermissionService
        </button>
        
        <div *ngIf="permissionService.hasAdminAccess()" 
             class="admin-panel">
          <h4>🛡️ Panel Administrateur</h4>
          <p>Vous avez des droits d'administration</p>
        </div>
      </section>
      
      <!-- AFFICHAGE des permissions actuelles -->
      <section class="demo-section">
        <h3>Permissions actuelles :</h3>
        <div class="permissions-list">
          <p><strong>Nombre de permissions :</strong> {{ currentPermissions.length }}</p>
          <ul *ngIf="currentPermissions.length > 0">
            <li *ngFor="let permission of currentPermissions">
              {{ permission }}
            </li>
          </ul>
          <p *ngIf="currentPermissions.length === 0" class="no-permissions">
            Aucune permission spéciale détectée
          </p>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .access-demo-container {
      max-width: 800px;
      margin: 0 auto;
      padding: 2rem;
    }
    
    .demo-section {
      margin-bottom: 2rem;
      padding: 1rem;
      border: 1px solid #dee2e6;
      border-radius: 0.375rem;
      background-color: #f8f9fa;
    }
    
    .btn {
      margin: 0.25rem;
      padding: 0.5rem 1rem;
      border: none;
      border-radius: 0.25rem;
      cursor: pointer;
      font-weight: 500;
    }
    
    .btn-primary { background-color: #007bff; color: white; }
    .btn-secondary { background-color: #6c757d; color: white; }
    .btn-success { background-color: #28a745; color: white; }
    .btn-warning { background-color: #ffc107; color: #212529; }
    .btn-info { background-color: #17a2b8; color: white; }
    
    .chat-container {
      background-color: #e7f3ff;
      padding: 1rem;
      border-radius: 0.25rem;
      border-left: 4px solid #007bff;
      margin: 0.5rem 0;
    }
    
    .admin-panel {
      background-color: #fff3cd;
      padding: 1rem;
      border-radius: 0.25rem;
      border-left: 4px solid #ffc107;
      margin: 0.5rem 0;
    }
    
    .permissions-list {
      background-color: white;
      padding: 1rem;
      border-radius: 0.25rem;
    }
    
    .no-permissions {
      color: #6c757d;
      font-style: italic;
    }
    
    ul {
      margin: 0.5rem 0;
      padding-left: 1.5rem;
    }
    
    li {
      margin: 0.25rem 0;
      color: #495057;
      font-family: monospace;
    }
  `]
})
export class AccessDemoComponent implements OnInit {
  currentPermissions: string[] = [];

  constructor(
    public authService: AuthService,        // 🔹 Public pour utilisation dans le template
    public permissionService: PermissionService
  ) {}

  ngOnInit() {
    // S'abonner aux changements de permissions pour mise à jour temps réel
    this.authService.accessCompetencies$.subscribe(permissions => {
      this.currentPermissions = permissions;
      console.log('[AccessDemo] Permissions mises à jour:', permissions);
    });
  }

  manageUsers() {
    console.log('Navigation vers la gestion des utilisateurs');
    alert('Redirection vers la gestion des utilisateurs');
  }

  manageBadges() {
    console.log('Navigation vers la gestion des badges');
    alert('Redirection vers la gestion des badges');
  }
}
