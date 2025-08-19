import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';

import { UserService } from '../../../services/user.service';
import { TeamService } from '../../../services/team.service';
import { UserDto } from '../../../models/user.model';
import { TeamDto } from '../../../models/TeamDto';

// Interface pour les tâches simulées
interface MockTask {
  id: number;
  title: string;
  description: string;
  status: 'pending' | 'in-progress' | 'completed';
  assignedTo?: string;
  teamName?: string;
  priority: 'low' | 'medium' | 'high';
  dueDate: string;
  progress: number; // 0-100
}

@Component({
  selector: 'app-rh-planification',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="rh-planification-container">
      <header class="page-header">
        <h1>Planification & Tâches</h1>
        <p>Gestion des tâches et suivi des projets</p>
        <nav class="breadcrumb">
          <a routerLink="/rh">Tableau de Bord RH</a> > Planification
        </nav>
      </header>

      <div class="stats-overview">
        <div class="stat-card total">
          <div class="stat-icon">📋</div>
          <div class="stat-info">
            <span class="stat-value">{{ totalTasks }}</span>
            <span class="stat-label">Total Tâches</span>
          </div>
        </div>
        <div class="stat-card pending">
          <div class="stat-icon">⏳</div>
          <div class="stat-info">
            <span class="stat-value">{{ pendingTasks }}</span>
            <span class="stat-label">En Attente</span>
          </div>
        </div>
        <div class="stat-card progress">
          <div class="stat-icon">🔄</div>
          <div class="stat-info">
            <span class="stat-value">{{ inProgressTasks }}</span>
            <span class="stat-label">En Cours</span>
          </div>
        </div>
        <div class="stat-card completed">
          <div class="stat-icon">✅</div>
          <div class="stat-info">
            <span class="stat-value">{{ completedTasks }}</span>
            <span class="stat-label">Terminées</span>
          </div>
        </div>
      </div>

      <div class="content-layout">
        <!-- Section Tâches -->
        <section class="tasks-section">
          <div class="section-header">
            <h2>Tâches</h2>
            <div class="filters">
              <select (change)="filterByStatus($event)">
                <option value="">Tous les statuts</option>
                <option value="pending">En attente</option>
                <option value="in-progress">En cours</option>
                <option value="completed">Terminées</option>
              </select>
              <select (change)="filterByTeam($event)">
                <option value="">Toutes les équipes</option>
                <option *ngFor="let team of teams$ | async" [value]="team.name">{{ team.name }}</option>
              </select>
            </div>
          </div>

          <div class="tasks-grid">
            <div *ngFor="let task of filteredTasks" class="task-card" [class]="'status-' + task.status">
              <div class="task-header">
                <h3>{{ task.title }}</h3>
                <span class="priority" [class]="'priority-' + task.priority">{{ task.priority }}</span>
              </div>
              <p class="task-description">{{ task.description }}</p>
              <div class="task-meta">
                <span class="assigned-to" *ngIf="task.assignedTo">👤 {{ task.assignedTo }}</span>
                <span class="team" *ngIf="task.teamName">🏢 {{ task.teamName }}</span>
                <span class="due-date">📅 {{ formatDate(task.dueDate) }}</span>
              </div>
              <div class="task-progress">
                <div class="progress-bar-container">
                  <div class="progress-bar" [style.width.%]="task.progress"></div>
                </div>
                <span class="progress-text">{{ task.progress }}%</span>
              </div>
              <div class="task-status">
                <span class="status-badge" [class]="'status-' + task.status">
                  {{ getStatusLabel(task.status) }}
                </span>
              </div>
            </div>
          </div>
        </section>

        <!-- Section Équipes Charge -->
        <aside class="team-load-section">
          <h2>Charge par Équipe</h2>
          <div class="team-load-list">
            <div *ngFor="let teamLoad of teamLoads" class="team-load-item">
              <div class="team-info">
                <h3>{{ teamLoad.teamName }}</h3>
                <span class="member-count">{{ teamLoad.memberCount }} membres</span>
              </div>
              <div class="load-metrics">
                <div class="load-bar-container">
                  <div class="load-bar" [style.width.%]="teamLoad.loadPercentage" [class]="getLoadClass(teamLoad.loadPercentage)"></div>
                </div>
                <span class="load-text">{{ teamLoad.activeTasks }} tâches actives</span>
              </div>
            </div>
          </div>

          <div class="upcoming-deadlines">
            <h3>Échéances Proches</h3>
            <div class="deadline-list">
              <div *ngFor="let deadline of upcomingDeadlines" class="deadline-item">
                <div class="deadline-info">
                  <span class="task-title">{{ deadline.title }}</span>
                  <span class="days-left" [class]="getUrgencyClass(deadline.daysLeft)">
                    {{ deadline.daysLeft }} jours restants
                  </span>
                </div>
              </div>
            </div>
          </div>
        </aside>
      </div>
    </div>
  `,
  styles: [`
    .rh-planification-container {
      padding: 2rem;
      max-width: 1400px;
      margin: 0 auto;
    }

    .page-header {
      margin-bottom: 2rem;
      h1 { color: #333; margin-bottom: 0.5rem; }
      p { color: #666; margin-bottom: 1rem; }
      .breadcrumb a { color: #009688; text-decoration: none; }
    }

    .stats-overview {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1.5rem;
      margin-bottom: 2rem;
    }

    .stat-card {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      display: flex;
      align-items: center;
      border-left: 4px solid;

      &.total { border-left-color: #2196f3; }
      &.pending { border-left-color: #ff9800; }
      &.progress { border-left-color: #9c27b0; }
      &.completed { border-left-color: #4caf50; }

      .stat-icon {
        font-size: 2rem;
        margin-right: 1rem;
      }

      .stat-info {
        display: flex;
        flex-direction: column;
        .stat-value {
          font-size: 2rem;
          font-weight: bold;
          color: #333;
        }
        .stat-label {
          color: #666;
          font-size: 0.9rem;
        }
      }
    }

    .content-layout {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 2rem;
    }

    .tasks-section, .team-load-section {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .section-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
      
      h2 { margin: 0; color: #333; }
      
      .filters {
        display: flex;
        gap: 1rem;
        select {
          padding: 0.5rem;
          border: 1px solid #ddd;
          border-radius: 4px;
          background: white;
        }
      }
    }

    .tasks-grid {
      display: grid;
      gap: 1rem;
    }

    .task-card {
      border: 1px solid #eee;
      border-radius: 8px;
      padding: 1.5rem;
      transition: all 0.2s;
      border-left: 4px solid;

      &.status-pending { border-left-color: #ff9800; }
      &.status-in-progress { border-left-color: #9c27b0; }
      &.status-completed { border-left-color: #4caf50; }

      &:hover {
        box-shadow: 0 4px 12px rgba(0,0,0,0.1);
        transform: translateY(-2px);
      }
    }

    .task-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.5rem;

      h3 {
        margin: 0;
        color: #333;
        font-size: 1.1rem;
      }

      .priority {
        padding: 0.25rem 0.5rem;
        border-radius: 12px;
        font-size: 0.75rem;
        font-weight: bold;
        text-transform: uppercase;

        &.priority-low { background: #e8f5e8; color: #2e7d32; }
        &.priority-medium { background: #fff3e0; color: #f57c00; }
        &.priority-high { background: #ffebee; color: #c62828; }
      }
    }

    .task-description {
      color: #666;
      margin-bottom: 1rem;
      font-size: 0.9rem;
    }

    .task-meta {
      display: flex;
      gap: 1rem;
      margin-bottom: 1rem;
      font-size: 0.85rem;
      color: #777;

      span {
        display: flex;
        align-items: center;
        gap: 0.25rem;
      }
    }

    .task-progress {
      display: flex;
      align-items: center;
      gap: 1rem;
      margin-bottom: 1rem;

      .progress-bar-container {
        flex: 1;
        height: 8px;
        background: #e0e0e0;
        border-radius: 4px;
        overflow: hidden;

        .progress-bar {
          height: 100%;
          background: #009688;
          transition: width 0.3s ease;
        }
      }

      .progress-text {
        font-size: 0.85rem;
        color: #666;
        min-width: 40px;
      }
    }

    .status-badge {
      padding: 0.25rem 0.75rem;
      border-radius: 12px;
      font-size: 0.8rem;
      font-weight: 500;

      &.status-pending { background: #fff3e0; color: #f57c00; }
      &.status-in-progress { background: #f3e5f5; color: #7b1fa2; }
      &.status-completed { background: #e8f5e8; color: #2e7d32; }
    }

    .team-load-list {
      margin-bottom: 2rem;
    }

    .team-load-item {
      margin-bottom: 1.5rem;
      padding: 1rem;
      border: 1px solid #eee;
      border-radius: 6px;

      .team-info {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 0.75rem;

        h3 { margin: 0; font-size: 1rem; color: #333; }
        .member-count { font-size: 0.8rem; color: #666; }
      }

      .load-metrics {
        .load-bar-container {
          height: 6px;
          background: #e0e0e0;
          border-radius: 3px;
          margin-bottom: 0.5rem;

          .load-bar {
            height: 100%;
            border-radius: 3px;
            transition: width 0.3s ease;

            &.low { background: #4caf50; }
            &.medium { background: #ff9800; }
            &.high { background: #f44336; }
          }
        }

        .load-text {
          font-size: 0.85rem;
          color: #666;
        }
      }
    }

    .upcoming-deadlines {
      h3 { color: #333; margin-bottom: 1rem; }
    }

    .deadline-item {
      padding: 0.75rem;
      border: 1px solid #eee;
      border-radius: 6px;
      margin-bottom: 0.5rem;

      .deadline-info {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;

        .task-title {
          font-weight: 500;
          color: #333;
        }

        .days-left {
          font-size: 0.8rem;
          
          &.urgent { color: #f44336; font-weight: bold; }
          &.warning { color: #ff9800; }
          &.normal { color: #666; }
        }
      }
    }

    @media (max-width: 968px) {
      .content-layout {
        grid-template-columns: 1fr;
      }
      
      .stats-overview {
        grid-template-columns: repeat(2, 1fr);
      }
      
      .section-header .filters {
        flex-direction: column;
        gap: 0.5rem;
      }
    }
  `]
})
export class RhPlanificationComponent implements OnInit {
  private userService = inject(UserService);
  private teamService = inject(TeamService);

  teams$: Observable<TeamDto[]> = this.teamService.getAllTeams();
  
  // Données simulées pour les tâches
  mockTasks: MockTask[] = [
    {
      id: 1,
      title: 'Refonte du système RH',
      description: 'Mise à jour complète de l\'interface utilisateur et des fonctionnalités.',
      status: 'in-progress',
      assignedTo: 'Marie Dubois',
      teamName: 'IT',
      priority: 'high',
      dueDate: '2025-07-15',
      progress: 65
    },
    {
      id: 2,
      title: 'Formation équipe commerciale',
      description: 'Organisation d\'une formation sur les nouvelles techniques de vente.',
      status: 'pending',
      assignedTo: 'Pierre Martin',
      teamName: 'Commercial',
      priority: 'medium',
      dueDate: '2025-07-30',
      progress: 0
    },
    {
      id: 3,
      title: 'Audit sécurité informatique',
      description: 'Vérification complète de la sécurité des systèmes informatiques.',
      status: 'completed',
      assignedTo: 'Sophie Laurent',
      teamName: 'IT',
      priority: 'high',
      dueDate: '2025-06-20',
      progress: 100
    },
    {
      id: 4,
      title: 'Mise à jour documentation',
      description: 'Révision et mise à jour de toute la documentation technique.',
      status: 'in-progress',
      assignedTo: 'Jean Durand',
      teamName: 'Documentation',
      priority: 'low',
      dueDate: '2025-08-10',
      progress: 30
    },
    {
      id: 5,
      title: 'Recrutement développeur senior',
      description: 'Processus de recrutement pour un poste de développeur senior.',
      status: 'pending',
      assignedTo: 'Claire Moreau',
      teamName: 'RH',
      priority: 'medium',
      dueDate: '2025-07-25',
      progress: 15
    }
  ];

  filteredTasks: MockTask[] = this.mockTasks;
  
  // Statistiques calculées
  totalTasks = this.mockTasks.length;
  pendingTasks = this.mockTasks.filter(t => t.status === 'pending').length;
  inProgressTasks = this.mockTasks.filter(t => t.status === 'in-progress').length;
  completedTasks = this.mockTasks.filter(t => t.status === 'completed').length;

  teamLoads: any[] = [];
  upcomingDeadlines: any[] = [];

  ngOnInit() {
    this.calculateTeamLoads();
    this.calculateUpcomingDeadlines();
  }

  filterByStatus(event: any) {
    const status = event.target.value;
    if (status) {
      this.filteredTasks = this.mockTasks.filter(task => task.status === status);
    } else {
      this.filteredTasks = this.mockTasks;
    }
  }

  filterByTeam(event: any) {
    const teamName = event.target.value;
    if (teamName) {
      this.filteredTasks = this.mockTasks.filter(task => task.teamName === teamName);
    } else {
      this.filteredTasks = this.mockTasks;
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR', { 
      day: '2-digit', 
      month: '2-digit', 
      year: 'numeric' 
    });
  }

  getStatusLabel(status: string): string {
    const labels: {[key: string]: string} = {
      'pending': 'En attente',
      'in-progress': 'En cours',
      'completed': 'Terminée'
    };
    return labels[status] || status;
  }

  calculateTeamLoads() {
    const teams = ['IT', 'Commercial', 'RH', 'Documentation'];
    this.teamLoads = teams.map(teamName => {
      const teamTasks = this.mockTasks.filter(task => task.teamName === teamName);
      const activeTasks = teamTasks.filter(task => task.status !== 'completed').length;
      const loadPercentage = Math.min((activeTasks / 5) * 100, 100); // Max 5 tâches = 100%
      
      return {
        teamName,
        memberCount: Math.floor(Math.random() * 8) + 3, // Simulé entre 3 et 10
        activeTasks,
        loadPercentage: Math.round(loadPercentage)
      };
    });
  }

  calculateUpcomingDeadlines() {
    const now = new Date();
    this.upcomingDeadlines = this.mockTasks
      .filter(task => task.status !== 'completed')
      .map(task => {
        const dueDate = new Date(task.dueDate);
        const daysLeft = Math.ceil((dueDate.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));
        return {
          title: task.title,
          daysLeft: daysLeft
        };
      })
      .filter(deadline => deadline.daysLeft <= 30 && deadline.daysLeft > 0)
      .sort((a, b) => a.daysLeft - b.daysLeft);
  }

  getLoadClass(percentage: number): string {
    if (percentage < 50) return 'low';
    if (percentage < 80) return 'medium';
    return 'high';
  }

  getUrgencyClass(daysLeft: number): string {
    if (daysLeft <= 3) return 'urgent';
    if (daysLeft <= 7) return 'warning';
    return 'normal';
  }
}
