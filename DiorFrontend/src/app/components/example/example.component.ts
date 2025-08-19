import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { PermissionService } from '../../services/permission.service';

/**
 * 🔹 ÉTAPE 5: Exemple d'utilisation dans un composant Angular
 */
@Component({
  selector: 'app-example',
  standalone: true,
  imports: [CommonModule],
  template: `
    <!-- Utilisation directe avec authService.hasAccess() -->
    <button *ngIf="authService.hasAccess('CONTRACT_MANAGEMENT')" class="btn">
      Voir les contrats
    </button>
      <!-- Utilisation avec PermissionService -->
    <div *ngIf="permissionService.canUsePartyKit()" class="chat-section">
      Chat PartyKit disponible
    </div>
    
    <!-- Autres exemples -->
    <div *ngIf="authService.hasAccess('USER_MANAGEMENT')" class="admin-section">
      Interface d'administration des utilisateurs
    </div>
    
    <button *ngIf="permissionService.canManageContracts()" 
            (click)="openContractManagement()">
      Gérer les contrats
    </button>
  `,
  styles: [`
    .btn {
      padding: 0.5rem 1rem;
      background-color: #007bff;
      color: white;
      border: none;
      border-radius: 0.25rem;
      cursor: pointer;
    }
    
    .admin-section {
      padding: 1rem;
      background-color: #f8f9fa;
      border: 1px solid #dee2e6;
      border-radius: 0.375rem;
      margin: 1rem 0;
    }
  `]
})
export class ExampleComponent {

  constructor(
    public authService: AuthService,        // Exposer en public pour utilisation dans template
    public permissionService: PermissionService
  ) {}

  openContractManagement(): void {
    console.log('Ouverture de la gestion des contrats');
  }
}
