import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { ManagerDashboardService } from '../manager-dashboard/manager-dashboard.service';
import { Project, Team, CreateProjectRequest } from '../manager-dashboard/manager-dashboard.types';

@Component({
  selector: 'app-manager-projets',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  template: `
    <div class="manager-projets">
      <div class="page-header">
        <h1>Gestion des Projets</h1>
        <p>Créez et suivez l'avancement de vos projets d'équipe</p>
        <nav class="breadcrumb">
          <a routerLink="/manager">Dashboard</a>
          <span> > </span>
          <span>Projets</span>
        </nav>
      </div>

      <div class="page-content">
        <!-- Formulaire de création -->
        <div class="project-form-section">
          <h2>🚀 Nouveau Projet</h2>
          <form [formGroup]="projectForm" (ngSubmit)="createProject()" class="project-form">
            <div class="form-row">
              <div class="form-group">
                <label for="title">Titre du projet *</label>
                <input id="title" type="text" formControlName="title" placeholder="Nom du projet">
                <div *ngIf="isFieldInvalid('title')" class="error-text">{{ getErrorMessage('title') }}</div>
              </div>
              <div class="form-group">
                <label for="priority">Priorité *</label>
                <select id="priority" formControlName="priority">
                  <option value="low">🟢 Basse</option>
                  <option value="medium">🟡 Moyenne</option>
                  <option value="high">🔴 Haute</option>
                </select>
              </div>
            </div>

            <div class="form-group">
              <label for="description">Description *</label>
              <textarea id="description" formControlName="description" placeholder="Description détaillée du projet" rows="4"></textarea>
              <div *ngIf="isFieldInvalid('description')" class="error-text">{{ getErrorMessage('description') }}</div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="teamId">Équipe assignée *</label>
                <select id="teamId" formControlName="teamId">
                  <option value="">Sélectionner une équipe</option>
                  <option *ngFor="let team of teams" [value]="team.id">
                    {{ team.name }} ({{ team.membersCount || 0 }} membres)
                  </option>
                </select>
                <div *ngIf="isFieldInvalid('teamId')" class="error-text">{{ getErrorMessage('teamId') }}</div>
              </div>
              <div class="form-group">
                <label for="deadline">Date limite *</label>
                <input id="deadline" type="date" formControlName="deadline">
                <div *ngIf="isFieldInvalid('deadline')" class="error-text">{{ getErrorMessage('deadline') }}</div>
              </div>
            </div>

            <div class="form-actions">
              <button type="button" (click)="resetForm()" class="btn btn--secondary">Effacer</button>
              <button type="submit" [disabled]="projectForm.invalid || loading" class="btn btn--primary">
                <span *ngIf="loading" class="loading-spinner"></span>
                ✨ Créer le projet
              </button>
            </div>
          </form>
        </div>

        <!-- Liste des projets -->
        <div class="projects-section">
          <div class="section-header">
            <h2>📊 Projets Actifs</h2>
            <div class="filters">
              <button *ngFor="let filter of statusFilters" 
                      (click)="setStatusFilter(filter.value)"
                      [class.active]="selectedStatus === filter.value"
                      class="filter-btn">
                {{ filter.label }}
              </button>
            </div>
          </div>

          <div class="projects-grid">
            <div *ngFor="let project of filteredProjects" class="project-card" [class]="'priority-' + project.priority">
              <div class="project-header">
                <h3>{{ project.title }}</h3>
                <span class="priority-badge" [class]="'priority-' + project.priority">
                  {{ getPriorityLabel(project.priority) }}
                </span>
              </div>
              
              <p class="project-description">{{ project.description }}</p>
              
              <div class="project-meta">
                <div class="meta-item">
                  <span class="label">Équipe:</span>
                  <span class="value">{{ project.teamName }}</span>
                </div>
                <div class="meta-item">
                  <span class="label">Créé le:</span>
                  <span class="value">{{ formatDate(project.createdAt) }}</span>
                </div>
                <div class="meta-item">
                  <span class="label">Échéance:</span>
                  <span class="value" [class.urgent]="isUrgent(project.deadline)">{{ formatDate(project.deadline) }}</span>
                </div>
              </div>

              <div *ngIf="project.progress !== undefined" class="progress-section">
                <div class="progress-header">
                  <span>Progression</span>
                  <span class="progress-value">{{ project.progress }}%</span>
                </div>
                <div class="progress-bar">
                  <div class="progress-fill" [style.width.%]="project.progress"></div>
                </div>
              </div>

              <div class="project-actions">
                <button (click)="editProject(project)" class="btn btn--small btn--secondary">✏️ Modifier</button>
                <button (click)="viewDetails(project)" class="btn btn--small btn--primary">👁️ Détails</button>
              </div>
            </div>
          </div>

          <div *ngIf="filteredProjects.length === 0" class="empty-state">
            <div class="empty-state__icon">📊</div>
            <div class="empty-state__text">Aucun projet trouvé</div>
            <div class="empty-state__subtext">Créez votre premier projet ci-dessus</div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./manager-projets.component.scss']
})
export class ManagerProjetsComponent implements OnInit {
  private dashboardService = inject(ManagerDashboardService);
  private fb = inject(FormBuilder);

  projectForm: FormGroup;
  projects: Project[] = [];
  filteredProjects: Project[] = [];
  teams: Team[] = [];
  loading = false;
  selectedStatus = 'all';

  statusFilters = [
    { value: 'all', label: 'Tous' },
    { value: 'pending', label: 'En attente' },
    { value: 'in-progress', label: 'En cours' },
    { value: 'completed', label: 'Terminés' }
  ];

  constructor() {
    this.projectForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      teamId: [null, [Validators.required]],
      priority: ['medium', [Validators.required]],
      deadline: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadData();
    this.setupFormValidation();
  }

  private loadData(): void {
    this.dashboardService.getProjects().subscribe({
      next: (projects) => {
        this.projects = projects;
        this.applyFilters();
      },
      error: (error) => {
        console.error('Erreur lors du chargement des projets:', error);
      }
    });

    this.dashboardService.getTeams().subscribe({
      next: (teams) => {
        this.teams = teams;
      },
      error: (error) => {
        console.error('Erreur lors du chargement des équipes:', error);
      }
    });
  }

  private setupFormValidation(): void {
    this.projectForm.get('deadline')?.valueChanges.subscribe(value => {
      if (value) {
        const deadline = new Date(value);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        
        if (deadline < today) {
          this.projectForm.get('deadline')?.setErrors({ pastDate: true });
        }
      }
    });
  }

  createProject(): void {
    if (this.projectForm.valid) {
      this.loading = true;
      const formData = this.projectForm.value;
      
      const projectRequest: CreateProjectRequest = {
        ...formData,
        deadline: new Date(formData.deadline)
      };

      this.dashboardService.sendProjectToTeam(projectRequest).subscribe({
        next: (project) => {
          this.projects = [project, ...this.projects];
          this.applyFilters();
          this.resetForm();
          this.loading = false;
          console.log('Projet créé avec succès!');
        },
        error: (error) => {
          console.error('Erreur lors de la création:', error);
          this.loading = false;
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  setStatusFilter(status: string): void {
    this.selectedStatus = status;
    this.applyFilters();
  }

  private applyFilters(): void {
    let filtered = this.projects;
    
    if (this.selectedStatus !== 'all') {
      filtered = filtered.filter(project => project.status === this.selectedStatus);
    }
    
    this.filteredProjects = filtered;
  }

  resetForm(): void {
    this.projectForm.reset({ priority: 'medium' });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.projectForm.controls).forEach(key => {
      this.projectForm.get(key)?.markAsTouched();
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const control = this.projectForm.get(fieldName);
    return !!(control?.invalid && control.touched);
  }

  getErrorMessage(fieldName: string): string {
    const control = this.projectForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['required']) return `${fieldName} est requis`;
      if (control.errors['minlength']) return `Minimum ${control.errors['minlength'].requiredLength} caractères`;
      if (control.errors['pastDate']) return 'La date ne peut pas être dans le passé';
    }
    return '';
  }

  formatDate(date: Date | string): string {
    const d = new Date(date);
    return d.toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  getPriorityLabel(priority?: string): string {
    switch (priority) {
      case 'high': return 'Haute';
      case 'medium': return 'Moyenne';
      case 'low': return 'Basse';
      default: return priority ?? '';
    }
  }

  isUrgent(deadline: Date | string): boolean {
    const deadlineDate = new Date(deadline);
    const now = new Date();
    const diffDays = Math.ceil((deadlineDate.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));
    return diffDays <= 3 && diffDays >= 0;
  }

  editProject(project: Project): void {
    console.log('Modifier projet:', project);
  }

  viewDetails(project: Project): void {
    console.log('Voir détails:', project);
  }
}
