import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserRoleService } from '../../services/admin-routes/user-role.service';
import { UserRole } from '../../models/user-role.model';
import { UserService } from '../../services/user.service';
import { RoleDefinitionService } from '../../services/admin-routes/role-definition.service';
import { UserDto } from '../../models/user.model';
import { RoleDefinition } from '../../models/role-definition.model';

/**
 * Composant de gestion des associations entre utilisateurs et rôles
 * 
 * Permet de lister, créer, modifier et supprimer des associations entre utilisateurs et rôles
 */
@Component({
  selector: 'app-user-role-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './user-role-list.component.html',
  styleUrls: ['./user-role-list.component.scss']
})
export class UserRoleListComponent {
  itemList: UserRole[] = [];
  userList: UserDto[] = [];
  roleList: RoleDefinition[] = [];
  selectedItem: UserRole | null = null;
  newItem: Partial<UserRole> = {};
  error: string | null = null;

  constructor(
    private service: UserRoleService,
    private userService: UserService,
    private roleService: RoleDefinitionService
  ) {
    this.loadItems();
    this.loadUsers();
    this.loadRoles();
  }

  loadItems() {
    this.service.getAll().subscribe({
      next: (data: UserRole[]) => { this.itemList = data; this.error = null; },
      error: () => { this.error = 'Erreur lors du chargement.'; }
    });
  }

  loadUsers() {
    this.userService.getAll().subscribe({
      next: (data: UserDto[]) => { this.userList = data; },
      error: () => { this.error = 'Erreur lors du chargement des utilisateurs.'; }
    });
  }

  loadRoles() {
    this.roleService.getAll().subscribe({
      next: (data: RoleDefinition[]) => { this.roleList = data; },
      error: () => { this.error = 'Erreur lors du chargement des rôles.'; }
    });
  }

  getUserName(userId: number): string {
    const user = this.userList.find(u => u.id === userId);
    return user ? `${user.firstName} ${user.lastName}` : `Utilisateur #${userId}`;
  }

  getRoleName(roleId: number): string {
    const role = this.roleList.find(r => r.id === roleId);
    return role ? role.name : `Rôle #${roleId}`;
  }

  selectItem(item: UserRole) {
    this.selectedItem = { ...item };
    this.error = null;
  }

  addItem() {
    if (!this.newItem.userId || !this.newItem.roleDefinitionId) {
      this.error = 'L\'utilisateur et le rôle sont obligatoires.';
      return;
    }

    // Ajouter la date et l'utilisateur de dernière modification
    this.newItem.lastEditAt = new Date().toISOString();
    this.newItem.lastEditBy = 'Utilisateur actuel'; // À remplacer par l'utilisateur connecté
    
    this.service.add(this.newItem as UserRole).subscribe({
      next: () => { this.loadItems(); this.newItem = {}; this.error = null; },
      error: () => { this.error = 'Erreur lors de l\'ajout.'; }
    });
  }

  updateItem() {
    if (!this.selectedItem) return;
    
    if (!this.selectedItem.userId || !this.selectedItem.roleDefinitionId) {
      this.error = 'L\'utilisateur et le rôle sont obligatoires.';
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
