import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatInputModule } from '@angular/material/input';
import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { TacheCardComponent } from './tache-card.component';

export interface Tache {
  id: string;
  titre: string;
  description: string;
  priorite: 'Urgente' | 'Haute' | 'Moyenne' | 'Faible';
  assignePar?: string;
  equipement?: string;
  statut: 'A faire' | 'En cours' | 'Terminé';
  echeance?: string;
  tempsEstime?: string;
  tempsDebut?: string;
  dateFin?: string;
  type: string;
}

@Component({
  selector: 'app-mes-taches',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatChipsModule,
    MatFormFieldModule,
    MatBadgeModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatInputModule,
    DragDropModule,
    TacheCardComponent
  ],
  template: `
  <div class="taches-container mat-typography">
    <div class="header flex flex-col md:flex-row md:items-center md:justify-between gap-4 mb-6">
      <div class="flex items-center gap-2">
        <button mat-fab color="primary" aria-label="Nouvelle tâche">
          <mat-icon>add</mat-icon>
        </button>
        <div class="mat-chip-list">
          <mat-chip
            *ngFor="let f of statusFilters"
            [color]="filterStatus() === f ? 'primary' : ''"
            (click)="setFilter(f)">
            {{ f }}
          </mat-chip>
        </div>
      </div>
      <div class="flex items-center gap-4">
        <span matBadge="{{activeCount()}}" matBadgeColor="primary" aria-label="Tâches actives">
          <mat-icon>assignment</mat-icon> {{activeCount()}} actives
        </span>
        <span matBadge="{{lateCount()}}" matBadgeColor="warn" aria-label="Tâches en retard">
          <mat-icon>error</mat-icon> {{lateCount()}} en retard
        </span>
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Rechercher</mat-label>
          <input matInput type="text" [ngModel]="search()" (ngModelChange)="setSearch($event)" autocomplete="off" />
          <button mat-icon-button matSuffix *ngIf="search()" (click)="setSearch('')" aria-label="Effacer">
            <mat-icon>close</mat-icon>
          </button>
        </mat-form-field>
      </div>
    </div>

    @if (isLoading()) {
      <div class="flex justify-center items-center h-64">
        <mat-spinner></mat-spinner>
      </div>
    } @else {
      <div class="kanban-board grid grid-cols-1 md:grid-cols-3 gap-4">
        <div class="kanban-col bg-surface p-3 rounded-xl shadow">
          <h3 class="font-semibold text-lg mb-2 flex items-center gap-2">
            <mat-icon color="primary">pending_actions</mat-icon> À faire ({{todo().length}})
          </h3>
          <div
            cdkDropList
            [cdkDropListData]="todo()"
            class="kanban-list min-h-[120px]"
            (cdkDropListDropped)="drop($event, 'A faire')"
            aria-label="Tâches à faire"
          >
            @for (task of todo(); track task.id) {
              <app-tache-card [task]="task" cdkDrag></app-tache-card>
            }
          </div>
        </div>
        <div class="kanban-col bg-surface p-3 rounded-xl shadow">
          <h3 class="font-semibold text-lg mb-2 flex items-center gap-2">
            <mat-icon color="accent">play_circle</mat-icon> En cours ({{doing().length}})
          </h3>
          <div
            cdkDropList
            [cdkDropListData]="doing()"
            class="kanban-list min-h-[120px]"
            (cdkDropListDropped)="drop($event, 'En cours')"
            aria-label="Tâches en cours"
          >
            @for (task of doing(); track task.id) {
              <app-tache-card [task]="task" cdkDrag></app-tache-card>
            }
          </div>
        </div>
        <div class="kanban-col bg-surface p-3 rounded-xl shadow">
          <h3 class="font-semibold text-lg mb-2 flex items-center gap-2">
            <mat-icon color="success">check_circle</mat-icon> Terminé ({{done().length}})
          </h3>
          <div
            cdkDropList
            [cdkDropListData]="done()"
            class="kanban-list min-h-[120px]"
            (cdkDropListDropped)="drop($event, 'Terminé')"
            aria-label="Tâches terminées"
          >
            @for (task of done(); track task.id) {
              <app-tache-card [task]="task" cdkDrag></app-tache-card>
            }
          </div>
        </div>
      </div>
    }
  </div>
  `,
  styles: [`
.taches-container {
  padding: 1.5rem;
  .header {
    margin-bottom: 2rem;
    .mat-chip-list {
      margin-left: 1rem;
    }
    .search-field {
      min-width: 220px;
    }
  }
  .kanban-board {
    margin-top: 1rem;
    .kanban-col {
      background: var(--md-sys-color-surface, #fff);
      border-radius: 1rem;
      min-height: 300px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.04);
      padding: 1rem;
    }
    .kanban-list {
      min-height: 120px;
    }
  }
}
  `]
})
export class MesTachesComponent {
  isLoading = signal(false);
  statusFilters: Array<'Toutes' | 'A faire' | 'En cours' | 'Terminé' | 'Urgente'> = ['Toutes','A faire','En cours','Terminé','Urgente'];

  tasks = signal<Tache[]>([
    {
      id: 'T001',
      titre: 'Contrôle qualité Ligne A',
      description: 'Vérification conformité lot #LOT-2024-156',
      priorite: 'Haute',
      assignePar: 'Chef équipe Martin',
      equipement: 'Remplisseuse R401',
      statut: 'A faire',
      echeance: '2024-06-15 14:00',
      tempsEstime: '30min',
      type: 'Contrôle'
    },
    {
      id: 'T002',
      titre: 'Maintenance préventive',
      description: 'Graissage roulements Convoyeur CV101',
      priorite: 'Moyenne',
      assignePar: 'Technicien Dubois',
      equipement: 'Convoyeur CV101',
      statut: 'En cours',
      tempsDebut: '2024-06-15 13:15',
      type: 'Maintenance'
    },
    {
      id: 'T003',
      titre: 'Nettoyage poste de travail',
      description: 'Désinfection zone conditionnement',
      priorite: 'Faible',
      statut: 'Terminé',
      dateFin: '2024-06-15 12:45',
      type: 'Nettoyage'
    }
  ]);

  filterStatus = signal<'Toutes' | 'A faire' | 'En cours' | 'Terminé' | 'Urgente'>('Toutes');
  search = signal('');

  setFilter(status: 'Toutes' | 'A faire' | 'En cours' | 'Terminé' | 'Urgente') { this.filterStatus.set(status); }
  setSearch(val: string) { this.search.set(val); }

  filteredTasks = computed(() => {
    let list = this.tasks();
    if (this.filterStatus() !== 'Toutes') {
      if (this.filterStatus() === 'Urgente') {
        list = list.filter(t => t.priorite === 'Urgente');
      } else {
        list = list.filter(t => t.statut === this.filterStatus());
      }
    }
    if (this.search().trim()) {
      const s = this.search().toLowerCase();
      list = list.filter(t =>
        t.titre.toLowerCase().includes(s) ||
        t.description?.toLowerCase().includes(s) ||
        t.equipement?.toLowerCase().includes(s)
      );
    }
    return list;
  });

  todo = computed(() => this.filteredTasks().filter(t => t.statut === 'A faire'));
  doing = computed(() => this.filteredTasks().filter(t => t.statut === 'En cours'));
  done = computed(() => this.filteredTasks().filter(t => t.statut === 'Terminé'));

  activeCount = computed(() => this.tasks().filter(t => t.statut !== 'Terminé').length);
  lateCount = computed(() => this.tasks().filter(t => t.statut !== 'Terminé' && t.priorite === 'Urgente').length);

  isLoadingFn = () => this.isLoading();

  drop(event: CdkDragDrop<Tache[]>, statut: 'A faire' | 'En cours' | 'Terminé') {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      const task = event.previousContainer.data[event.previousIndex];
      task.statut = statut;
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
    }
  }
}
