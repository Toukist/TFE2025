import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PrivilegeService } from '../../services/admin-routes/privilege.service';
import { PrivilegeDto } from '../../models/privilege.model';

/**
 * Composant de gestion des privilèges
 * 
 * Permet de lister, créer, modifier et supprimer des privilèges
 */
@Component({
  selector: 'app-privilege-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './privilege-list.component.html',
  styleUrls: ['./privilege-list.component.scss']
})
export class PrivilegeListComponent {

  privilegeList: PrivilegeDto[] = [];
  selectedPrivilege: PrivilegeDto | null = null;
  newPrivilege: Partial<PrivilegeDto> = {
    isConfigurableRead: false,
    isConfigurableDelete: false,
    isConfigurableAdd: false,
    isConfigurableModify: false,
    isConfigurableStatus: false,
    isConfigurableExecution: false
  };
  error: string | null = null;

  constructor(private privilegeService: PrivilegeService) {
    this.loadPrivileges();
  }
  loadPrivileges() {
    this.privilegeService.getAll().subscribe({
      next: (data: PrivilegeDto[]) => { this.privilegeList = data; this.error = null; },
      error: () => { this.error = 'Erreur lors du chargement.'; }
    });
  }

  selectPrivilege(privilege: PrivilegeDto) {
    this.selectedPrivilege = { ...privilege };
    this.error = null;
  }

  addPrivilege() {
    if (
      !this.newPrivilege.name || 
      this.newPrivilege.isConfigurableRead === undefined ||
      this.newPrivilege.isConfigurableDelete === undefined ||
      this.newPrivilege.isConfigurableAdd === undefined ||
      this.newPrivilege.isConfigurableModify === undefined ||
      this.newPrivilege.isConfigurableStatus === undefined ||
      this.newPrivilege.isConfigurableExecution === undefined
    ) {
      this.error = 'Les champs obligatoires doivent être remplis.';
      return;
    }
    
    // Ajouter la date et l'utilisateur de dernière modification
    this.newPrivilege.lastEditAt = new Date().toISOString();
    this.newPrivilege.lastEditBy = 'Utilisateur actuel'; // À remplacer par l'utilisateur connecté
    
    this.privilegeService.create(this.newPrivilege as PrivilegeDto).subscribe({
      next: () => { 
        this.loadPrivileges(); 
        this.newPrivilege = {
          isConfigurableRead: false,
          isConfigurableDelete: false,
          isConfigurableAdd: false,
          isConfigurableModify: false,
          isConfigurableStatus: false,
          isConfigurableExecution: false
        }; 
        this.error = null; 
      },
      error: () => { this.error = 'Erreur lors de l\'ajout.'; }
    });
  }

  updatePrivilege() {
    if (!this.selectedPrivilege) return;
    
    // Mettre à jour la date et l'utilisateur de dernière modification
    this.selectedPrivilege.lastEditAt = new Date().toISOString();
    this.selectedPrivilege.lastEditBy = 'Utilisateur actuel'; // À remplacer par l'utilisateur connecté
    
    this.privilegeService.update(this.selectedPrivilege.id, this.selectedPrivilege).subscribe({
      next: () => { this.loadPrivileges(); this.selectedPrivilege = null; this.error = null; },
      error: () => { this.error = 'Erreur lors de la modification.'; }
    });
  }

  deletePrivilege(id: number) {
    if (!confirm('Supprimer ce privilège ?')) return;
    this.privilegeService.delete(id).subscribe({
      next: () => { this.loadPrivileges(); this.error = null; },
      error: () => { this.error = 'Erreur lors de la suppression.'; }
    });
  }

  cancelEdit() {
    this.selectedPrivilege = null;
    this.error = null;
  }
}

