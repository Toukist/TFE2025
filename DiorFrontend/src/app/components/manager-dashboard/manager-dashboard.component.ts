import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil, combineLatest, map, startWith } from 'rxjs';

import { ManagerDashboardService } from './manager-dashboard.service';
import {
  User, Team, Project, DashboardStats, PerformanceMetrics, 
  NavigationItem, ProjectForm, TeamOption, CreateProjectRequest, 
  ManagerDashboardData, Performance
} from './manager-dashboard.types';

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './manager-dashboard.component.html',
  styleUrls: ['./manager-dashboard.component.scss']
})
export class ManagerDashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private dashboardService = inject(ManagerDashboardService);
  // ========== ÉTAT DU COMPOSANT ==========
  dashboardData: ManagerDashboardData | null = null;
  users: User[] = [];
  teams: Team[] = [];
  projects: Project[] = [];
  performance: Performance | null = null;
  agendaItems: any[] = [];
  
  loading$ = this.dashboardService.loading$;
  error: string | null = null;
  
  // ========== FORMULAIRES ==========
  projectForm: FormGroup;
  showProjectForm = false;
  
  // ========== NAVIGATION ==========
  navigationItems: NavigationItem[] = [
    {
      path: '/manager/equipe',
      label: 'Gestion Équipe',
      icon: '👥',
      color: 'primary',
      count: 0
    },
    {
      path: '/manager/projets',
      label: 'Projets Actifs',
      icon: '📊',
      color: 'secondary',
      count: 0
    },
    {
      path: '/manager/performance',
      label: 'Performance',
      icon: '📈',
      color: 'accent',
      count: 0
    }
  ];

  // ========== DONNÉES DÉRIVÉES ==========
  recentUsers$ = combineLatest([
    this.dashboardService.getUsers().pipe(startWith([])),
  ]).pipe(
    map(([users]) => users
      .filter(user => user.isActive)
      .sort((a, b) => {        if (!a.lastConnection || !b.lastConnection) return 0;
        return new Date(b.lastConnection).getTime() - new Date(a.lastConnection).getTime();
      })
      .slice(0, 5)
    )
  );

  recentProjects$ = combineLatest([
    this.dashboardService.getProjects().pipe(startWith([])),
  ]).pipe(
    map(([projects]) => projects
      .filter(project => project.status !== 'completed')
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 3)
    )
  );

  teamOptions$ = combineLatest([
    this.dashboardService.getTeams().pipe(startWith([])),
  ]).pipe(
    map(([teams]) => teams.map(team => ({
      id: team.id,
      name: team.name,
      description: team.description,
      membersCount: team.membersCount || 0
    } as TeamOption)))
  );

  constructor() {
    this.projectForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      teamId: [null, [Validators.required]],
      priority: ['medium', [Validators.required]],
      deadline: ['', [Validators.required]]
    });
  }

  // ========== LIFECYCLE ==========
  ngOnInit(): void {
    this.loadDashboardData();
    this.setupFormValidation();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ========== CHARGEMENT DES DONNÉES ==========
  private loadDashboardData(): void {
    this.error = null;

    // Charger les stats principales
    this.dashboardService.getDashboardData()
      .pipe(takeUntil(this.destroy$))
      .subscribe({        next: (data) => {
          this.dashboardData = data;
          this.updateNavigationCounts(data.stats);
        },
        error: (error) => {
          console.error('Erreur lors du chargement des stats:', error);
          this.error = 'Impossible de charger les statistiques du dashboard';
        }
      });

    // Charger les utilisateurs
    this.dashboardService.getUsers()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (users) => {
          this.users = users;
        },
        error: (error) => {
          console.error('Erreur lors du chargement des utilisateurs:', error);
        }
      });

    // Charger les équipes
    this.dashboardService.getTeams()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (teams) => {
          this.teams = teams;
        },
        error: (error) => {
          console.error('Erreur lors du chargement des équipes:', error);
        }
      });

    // Charger les projets
    this.dashboardService.getProjects()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (projects) => {
          this.projects = projects;
        },
        error: (error) => {
          console.error('Erreur lors du chargement des projets:', error);
        }
      });

    // Charger les métriques de performance
    this.dashboardService.getPerformanceMetrics()
      .pipe(takeUntil(this.destroy$))
      .subscribe({        next: (performance) => {
          this.performance = performance;
        },
        error: (error) => {
          console.error('Erreur lors du chargement des métriques:', error);
        }
      });
  }

  private updateNavigationCounts(stats: DashboardStats): void {
    this.navigationItems = this.navigationItems.map(item => {
      switch (item.path) {
        case '/manager/equipe':
          return { ...item, count: stats.team.onlineNow };
        case '/manager/projets':
          return { ...item, count: stats.projects.activeProjects };
        case '/manager/performance':
          return { ...item, count: stats.performance.monthlyTrend };
        default:
          return item;
      }
    });
  }

  // ========== GESTION DU FORMULAIRE PROJET ==========
  private setupFormValidation(): void {
    this.projectForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(values => {
        // Validation personnalisée pour la date
        const deadlineControl = this.projectForm.get('deadline');
        if (deadlineControl?.value) {
          const deadline = new Date(deadlineControl.value);
          const today = new Date();
          today.setHours(0, 0, 0, 0);
          
          if (deadline < today) {
            deadlineControl.setErrors({ pastDate: true });
          }
        }
      });
  }

  toggleProjectForm(): void {
    this.showProjectForm = !this.showProjectForm;
    if (!this.showProjectForm) {
      this.resetProjectForm();
    }
  }
  submitProject(): void {
    if (this.projectForm.valid) {
      const formData = this.projectForm.value as ProjectForm;
      
      // Convertir le formulaire en CreateProjectRequest
      const projectRequest: CreateProjectRequest = {
        ...formData,
        deadline: new Date(formData.deadline)
      };
      
      this.dashboardService.sendProjectToTeam(projectRequest)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (project) => {
            console.log('Projet créé avec succès:', project);
            this.projects = [project, ...this.projects];
            this.resetProjectForm();
            this.showProjectForm = false;
            
            // Notification de succès (à implémenter)
            this.showNotification('Projet créé avec succès!', 'success');
          },
          error: (error) => {
            console.error('Erreur lors de la création du projet:', error);
            this.showNotification('Erreur lors de la création du projet', 'error');
          }
        });
    } else {
      this.markFormGroupTouched(this.projectForm);
    }
  }

  private resetProjectForm(): void {
    this.projectForm.reset({
      priority: 'medium'
    });
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  // ========== NAVIGATION ==========
  navigateTo(path: string): void {
    this.router.navigate([path]);
  }

  // ========== UTILITAIRES ==========
  getErrorMessage(fieldName: string): string {
    const control = this.projectForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['required']) {
        return `${fieldName} est requis`;
      }
      if (control.errors['minlength']) {
        return `${fieldName} doit contenir au moins ${control.errors['minlength'].requiredLength} caractères`;
      }
      if (control.errors['pastDate']) {
        return 'La date limite ne peut pas être dans le passé';
      }
    }
    return '';
  }

  isFieldInvalid(fieldName: string): boolean {
    const control = this.projectForm.get(fieldName);
    return !!(control?.invalid && control.touched);
  }

  formatDate(date: Date | string): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'online': return '#22c55e';
      case 'away': return '#f97316';
      case 'offline': return '#94a3b8';
      default: return '#94a3b8';
    }
  }

  getPriorityClass(priority: string): string {
    return `projects-card__priority--${priority}`;
  }

  getProgressWidth(progress: number = 0): string {
    return `${Math.min(Math.max(progress, 0), 100)}%`;
  }

  getTrendIcon(trend: 'up' | 'down' | 'stable'): string {
    switch (trend) {
      case 'up': return '↗️';
      case 'down': return '↘️';
      case 'stable': return '➡️';
      default: return '➡️';
    }
  }

  getTrendClass(trend: 'up' | 'down' | 'stable'): string {
    return `mini-stat__trend--${trend}`;
  }

  getAlertClass(type: string): string {
    return `performance-card__alert--${type}`;
  }

  // ========== ACTIONS ==========
  refreshData(): void {
    this.loadDashboardData();
  }

  handleProjectClick(project: Project): void {
    // Navigation vers le détail du projet (à implémenter)
    console.log('Naviguer vers le projet:', project);
  }

  handleUserClick(user: User): void {
    // Navigation vers le profil utilisateur (à implémenter)
    console.log('Naviguer vers l\'utilisateur:', user);
  }

  handleTeamClick(team: Team): void {
    // Navigation vers la gestion de l'équipe
    this.router.navigate(['/manager/equipe'], { queryParams: { teamId: team.id } });
  }

  handleAlertAction(alert: any): void {
    if (alert.action) {
      // Exécuter l'action de l'alerte (à implémenter selon le contexte)
      console.log('Action alerte:', alert);
    }
  }

  // ========== NOTIFICATIONS ==========
  private showNotification(message: string, type: 'success' | 'error' | 'info'): void {
    // Implémentation basique - peut être remplacée par un service de notification
    console.log(`[${type.toUpperCase()}] ${message}`);
    
    // Ici vous pouvez intégrer une librairie de notifications comme ngx-toastr
    // ou créer votre propre système de notifications
  }

  // ========== GETTERS POUR LE TEMPLATE ==========
  get hasData(): boolean {
    return !!(this.dashboardData && this.users.length > 0);
  }

  get activeUsersCount(): number {
    return this.users.filter(user => user.status === 'online').length;
  }

  get activeProjectsCount(): number {
    return this.projects.filter(project => project.status === 'in-progress').length;
  }

  get upcomingDeadlines(): Project[] {
    const now = new Date();
    const nextWeek = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000);
    
    return this.projects.filter(project => {
      const deadline = new Date(project.deadline);
      return deadline >= now && deadline <= nextWeek && project.status !== 'completed';
    });
  }
  get teamEfficiencyAverage(): number {
    if (this.teams.length === 0) return 0;
    const total = this.teams.reduce((sum, team) => sum + (team.presenceRate || 75), 0);
    return Math.round(total / this.teams.length);
  }
}
