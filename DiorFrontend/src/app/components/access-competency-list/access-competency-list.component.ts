import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccessCompetencyService } from '../../services/admin-routes/access-competency.service';
import { AccessCompetency } from '../../models/access-competency.model';

/**
 * Composant de gestion des zones/salles accessibles par badges
 * 
 * Permet de lister, créer, modifier et supprimer des zones/salles accessibles
 */
@Component({
  selector: 'app-access-competency-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './access-competency-list.component.html',
  styleUrls: ['./access-competency-list.component.scss']
})
export class AccessCompetencyListComponent {

  itemList: AccessCompetency[] = [];
  selectedItem: AccessCompetency | null = null;
  newItem: Partial<AccessCompetency> = {
    isActive: true
  };
  error: string | null = null;

  constructor(private service: AccessCompetencyService) {
    this.loadItems();
  }

  loadItems() {
    this.service.getAll().subscribe({
      next: (data: AccessCompetency[]) => { this.itemList = data; this.error = null; },
      error: () => { this.error = 'Erreur lors du chargement.'; }
    });
  }

  selectItem(item: AccessCompetency) {
    this.selectedItem = { ...item };
    this.error = null;
  }

  addItem() {
    if (!this.newItem.name || this.newItem.isActive === undefined) {
      this.error = 'Les champs "Nom" et "Activation" sont obligatoires.';
      return;
    }
    this.service.create(this.newItem as AccessCompetency).subscribe({
      next: () => { 
        this.loadItems(); 
        this.newItem = { isActive: true }; 
        this.error = null; 
      },
      error: () => { this.error = 'Erreur lors de l\'ajout.'; }
    });
  }

  updateItem() {
    if (!this.selectedItem) return;
    if (!this.selectedItem.name || this.selectedItem.isActive === undefined) {
      this.error = 'Les champs "Nom" et "Activation" sont obligatoires.';
      return;
    }
    this.service.update(this.selectedItem.id, this.selectedItem).subscribe({
      next: () => { this.loadItems(); this.selectedItem = null; this.error = null; },
      error: () => { this.error = 'Erreur lors de la modification.'; }
    });
  }

  deleteItem(id: number) {
    if (!confirm('Supprimer cette zone/salle ?')) return;
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

