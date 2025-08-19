import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RoleDefinitionService } from '../../services/admin-routes/role-definition.service';
import { RoleDefinition } from '../../models/role-definition.model';

/**
 * Composant de gestion des définitions de rôles
 * 
 * Permet de lister, créer, modifier et supprimer des rôles
 */
@Component({
  selector: 'app-role-definition-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './role-definition-list.component.html',
  styleUrls: ['./role-definition-list.component.scss']
})
export class RoleDefinitionListComponent {

  roleList: RoleDefinition[] = [];
  selectedRole: RoleDefinition | null = null;
  newRole: Partial<RoleDefinition> = {
    isActive: true
  };
  error: string | null = null;

  constructor(private roleService: RoleDefinitionService) {
    this.loadRoles();
  }
  loadRoles() {
    this.roleService.getAll().subscribe({
      next: (data: RoleDefinition[]) => { this.roleList = data; this.error = null; },
      error: () => { this.error = 'Erreur lors du chargement.'; }
    });
  }

  selectRole(role: RoleDefinition) {
    this.selectedRole = { ...role };
    this.error = null;
  }

  addRole() {
    if (!this.newRole.name) {
      this.error = 'Le nom du rôle est obligatoire.';
      return;
    }
    if (this.newRole.isActive === undefined) {
      this.newRole.isActive = true;
    }
   
    const now = new Date();
    const dto: RoleDefinition = {
      id: 0,
      name: this.newRole.name!,
      description: this.newRole.description || '',
      isActive: this.newRole.isActive!,
      createdAt: now,
      createdBy: 'Utilisateur actuel', // À remplacer par l'utilisateur connecté
      lastEditAt: now,
      lastEditBy: 'Utilisateur actuel' // À remplacer par l'utilisateur connecté
    };
    this.roleService.create(dto).subscribe({
      next: () => {
        this.loadRoles();
        this.newRole = { isActive: true };
        this.error = null;
      },
      error: () => { this.error = 'Erreur lors de l\'ajout.'; }
    });
  }

  updateRole() {
    if (!this.selectedRole) return;
    if (!this.selectedRole.name) {
      this.error = 'Le nom du rôle est obligatoire.';
      return;
    }
    const now = new Date();
    const dto: RoleDefinition = {
      id: this.selectedRole.id,
      name: this.selectedRole.name,
      description: this.selectedRole.description || '',
      isActive: this.selectedRole.isActive,
      createdAt: new Date(this.selectedRole.createdAt || now),
      createdBy: this.selectedRole.createdBy || 'Utilisateur inconnu',
      lastEditAt: now,
      lastEditBy: 'Utilisateur actuel' // À remplacer par l'utilisateur connecté
    };
    this.roleService.update(this.selectedRole.id, dto).subscribe({
      next: () => { this.loadRoles(); this.selectedRole = null; this.error = null; },
      error: () => { this.error = 'Erreur lors de la modification.'; }
    });
  }

  deleteRole(id: number) {
    if (!confirm('Supprimer ce rôle ?')) return;
    this.roleService.delete(id).subscribe({
      next: () => { this.loadRoles(); this.error = null; },
      error: () => { this.error = 'Erreur lors de la suppression.'; }
    });
  }

  cancelEdit() {
    this.selectedRole = null;
    this.error = null;
  }
}

