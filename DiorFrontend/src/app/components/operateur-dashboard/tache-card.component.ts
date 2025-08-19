import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Tache } from './mes-taches.component';

@Component({
  selector: 'app-tache-card',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatTooltipModule
  ],
  template: `
<mat-card class="tache-card" [ngClass]="task.priorite.toLowerCase()">
  <div class="card-header flex items-center justify-between">
    <div class="flex items-center gap-2">
      <span class="font-semibold">{{ task.titre }}</span>
      <mat-chip [color]="chipColor(task.priorite)" selected>{{ task.priorite }}</mat-chip>
    </div>
    <span *ngIf="task.echeance" class="text-xs text-gray-500">⏰ {{ task.echeance }}</span>
  </div>
  <div class="card-body mt-2">
    <div class="text-sm text-gray-700 mb-1">{{ task.description }}</div>
    <div class="text-xs text-gray-500" *ngIf="task.equipement">
      <mat-icon inline fontSize="small">precision_manufacturing</mat-icon> {{ task.equipement }}
    </div>
    <div class="text-xs text-gray-500" *ngIf="task.assignePar">
      <mat-icon inline fontSize="small">person</mat-icon> Assignée par {{ task.assignePar }}
    </div>
  </div>
  <div class="card-footer flex items-center justify-between mt-2">
    <div class="flex gap-2">
      <button mat-icon-button color="primary" matTooltip="Démarrer" *ngIf="task.statut === 'A faire'">
        <mat-icon>play_arrow</mat-icon>
      </button>
      <button mat-icon-button color="accent" matTooltip="Pause" *ngIf="task.statut === 'En cours'">
        <mat-icon>pause</mat-icon>
      </button>
      <button mat-icon-button color="success" matTooltip="Terminer" *ngIf="task.statut !== 'Terminé'">
        <mat-icon>check_circle</mat-icon>
      </button>
      <button mat-icon-button matTooltip="Commenter">
        <mat-icon>chat_bubble</mat-icon>
      </button>
    </div>
    <button mat-icon-button matTooltip="Joindre une photo">
      <mat-icon>photo_camera</mat-icon>
    </button>
  </div>
</mat-card>
  `,
  styles: [`
.tache-card {
  margin-bottom: 12px;
  border-left: 4px solid transparent;
  &.urgente { border-color: #e53935; }
  &.haute { border-color: #fb8c00; }
  &.moyenne { border-color: #1976d2; }
  &.faible { border-color: #43a047; }
  .mat-chip {
    font-weight: bold;
    font-size: 0.85em;
    margin-left: 4px;
  }
  .card-header { margin-bottom: 4px; }
  .card-footer { margin-top: 8px; }
}
  `]
})
export class TacheCardComponent {
  @Input({ required: true }) task!: Tache;

  chipColor(prio: Tache['priorite']) {
    switch (prio) {
      case 'Urgente': return 'warn';
      case 'Haute': return 'accent';
      case 'Moyenne': return 'primary';
      default: return 'success';
    }
  }
}
