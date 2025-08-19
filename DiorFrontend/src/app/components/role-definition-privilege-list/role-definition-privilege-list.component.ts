import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RoleDefinitionPrivilegeService } from '../../services/admin-routes/role-definition-privilege.service';
import { RoleDefinitionPrivilege, RoleDefinition } from '../../models/role-definition.model';
import { RoleDefinitionService } from '../../services/admin-routes/role-definition.service';
import { PrivilegeService } from '../../services/admin-routes/privilege.service';
import { PrivilegeDto } from '../../models/privilege.model';

/**
 * Composant de gestion des associations entre rôles et privilèges
 * 
 * Permet de lister, créer, modifier et supprimer des associations entre rôles et privilèges
 */
@Component({
  selector: 'app-role-definition-privilege-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './role-definition-privilege-list.component.html',
  styleUrls: ['./role-definition-privilege-list.component.scss']
})
export class RoleDefinitionPrivilegeListComponent {

  itemList: RoleDefinitionPrivilege[] = [];
  roleList: RoleDefinition[] = [];
  privilegeList: PrivilegeDto[] = [];
  selectedItem: RoleDefinitionPrivilege | null = null;
  newItem: Partial<RoleDefinitionPrivilege> = {
    isActive: true,
    isRead: false,
    isAdd: false,
    isModify: false,
    isStatus: false,
    isExecution: false
  };
  error: string | null = null;

  constructor(
    private service: RoleDefinitionPrivilegeService,
    private roleService: RoleDefinitionService,
    private privilegeService: PrivilegeService
  ) {
    this.loadItems();
    this.loadRoles();
    this.loadPrivileges();
  }
  loadItems() {
    this.service.getAll().subscribe({
      next: (data: RoleDefinitionPrivilege[]) => { this.itemList = data; this.error = null; },
      error: () => { this.error = 'Erreur lors du chargement des associations.'; }
    });
  }

  loadRoles() {
    this.roleService.getAll().subscribe({
      next: (data: RoleDefinition[]) => { this.roleList = data; },
      error: () => { this.error = 'Erreur lors du chargement des rôles.'; }
    });
  }

  loadPrivileges() {
    this.privilegeService.getAll().subscribe({
      next: (data: PrivilegeDto[]) => { this.privilegeList = data; },
      error: () => { this.error = 'Erreur lors du chargement des privilèges.'; }
    });
  }

  selectItem(item: RoleDefinitionPrivilege) {
    this.selectedItem = { ...item };
    this.error = null;
  }

  getRoleName(roleId: number): string {
    const role = this.roleList.find(r => r.id === roleId);
    return role ? role.name : `Rôle #${roleId}`;
  }

  getPrivilegeName(privilegeId: number): string {
    const privilege = this.privilegeList.find(p => p.id === privilegeId);
    return privilege?.name ?? `Privilège #${privilegeId}`;
  }

  addItem() {
    if (!this.newItem.roleDefinitionId || !this.newItem.privilegeId) {
      this.error = 'Le rôle et le privilège sont obligatoires.';
      return;
    }

    // Initialiser les valeurs par défaut si non définies
    this.newItem.isActive = this.newItem.isActive ?? true;
    this.newItem.isRead = this.newItem.isRead ?? false;
    this.newItem.isAdd = this.newItem.isAdd ?? false;
    this.newItem.isModify = this.newItem.isModify ?? false;
    this.newItem.isStatus = this.newItem.isStatus ?? false;
    this.newItem.isExecution = this.newItem.isExecution ?? false;
    
    // Ajouter la date et l'utilisateur de dernière modification
    this.newItem.lastEditAt = new Date().toISOString();
    this.newItem.lastEditBy = 'Utilisateur actuel'; // À remplacer par l'utilisateur connecté

    this.service.add(this.newItem as RoleDefinitionPrivilege).subscribe({
      next: () => { 
        this.loadItems(); 
        this.newItem = {
          isActive: true,
          isRead: false,
          isAdd: false,
          isModify: false,
          isStatus: false,
          isExecution: false
        }; 
        this.error = null; 
      },
      error: () => { this.error = 'Erreur lors de l\'ajout.'; }
    });
  }

  updateItem() {
    if (!this.selectedItem) return;
    
    if (!this.selectedItem.roleDefinitionId || !this.selectedItem.privilegeId) {
      this.error = 'Le rôle et le privilège sont obligatoires.';
      return;
    }
    
    // Mettre à jour la date et l'utilisateur de dernière modification
    this.selectedItem.lastEditAt = new Date().toISOString();
    this.selectedItem.lastEditBy = 'Utilisateur actuel'; // À remplacer par l'utilisateur connecté
    
    this.service.update(this.selectedItem).subscribe({
      next: () => { this.loadItems(); this.selectedItem = null; this.error = null; },
      error: () => { this.error = 'Erreur lors de la modification.'; }
    });
  }

  deleteItem(id: number) {
    if (!confirm('Supprimer cette association ?')) return;
    this.service.delete(id).subscribe({
      next: () => { this.loadItems(); this.error = null; },
      error: () => { this.error = 'Erreur lors de la suppression.'; }
    });
  }

  cancelEdit() {
    this.selectedItem = null;
    this.error = null;
  }
}
