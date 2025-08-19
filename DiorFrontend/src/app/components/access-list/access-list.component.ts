import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AccessService } from '../../services/access.service';
import { AccessDto } from '../../models/access.model';

@Component({
  selector: 'app-access-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './access-list.component.html',
  styleUrls: ['./access-list.component.scss']
})
export class AccessListComponent implements OnInit {
selectAccess(_t21: AccessDto) {
throw new Error('Method not implemented.');
}

  accessList: AccessDto[] = [];
  selectedAccess: AccessDto | null = null;
  newAccess: Partial<AccessDto> = {
    badgePhysicalNumber: '',
    isActive: true,
    createdBy: 'admin'
  };
  error: string | null = null;
  success: string | null = null;

  constructor(private accessService: AccessService) {}

  ngOnInit(): void {
    this.loadAccesses();
  }

  loadAccesses(): void {
    this.accessService.getAll().subscribe({
      next: (data) => {
        this.accessList = data;
        this.error = null;
      },
      error: () => this.error = '❌ Erreur lors du chargement des badges.'
    });
  }

  selectedAccessFn(access: AccessDto): void {
    this.selectedAccess = { ...access };
  }

  cancelEdit(): void {
    this.selectedAccess = null;
  }

  updateAccess(): void {
    if (!this.selectedAccess) return;

    this.accessService.update(this.selectedAccess.id, this.selectedAccess).subscribe({
      next: () => {
        this.success = '✅ Badge mis à jour';
        this.loadAccesses();
        this.selectedAccess = null;
      },
      error: () => this.error = '❌ Erreur lors de la mise à jour du badge.'
    });
  }

  addAccess(): void {
    if (!this.newAccess.badgePhysicalNumber?.trim()) {
      this.error = 'Le numéro de badge est requis.';
      return;
    }

    this.accessService.create(this.newAccess as AccessDto).subscribe({
      next: () => {
        this.success = '✅ Nouveau badge ajouté';
        this.loadAccesses();
        this.newAccess = {
          badgePhysicalNumber: '',
          isActive: true,
          createdBy: 'admin'
        };
      },
      error: () => this.error = '❌ Erreur lors de l\'ajout du badge.'
    });
  }

  deleteAccess(id: number): void {
    if (!confirm('Supprimer ce badge ?')) return;

    this.accessService.delete(id).subscribe({
      next: () => {
        this.success = '✅ Badge supprimé';
        this.loadAccesses();
      },
      error: () => this.error = '❌ Erreur lors de la suppression du badge.'
    });
  }
}
