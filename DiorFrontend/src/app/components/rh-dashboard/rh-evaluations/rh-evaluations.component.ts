import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';

import { UserService } from '../../../services/user.service';
import { TeamService } from '../../../services/team.service';
import { UserDto } from '../../../models/user.model';
import { TeamDto } from '../../../models/TeamDto';

// Interfaces pour les évaluations
interface Evaluation {
  id: number;
  employeeName: string;
  employeeId: number;
  evaluatorName: string;
  evaluatorId: number;
  period: string; // ex: "2025-Q2"
  type: 'annual' | 'mid-year' | 'probation' | 'project';
  status: 'pending' | 'in-progress' | 'completed' | 'overdue';
  dueDate: string;
  completedDate?: string;
  overallScore?: number; // 1-10
  categories: EvaluationCategory[];
  comments?: string;
  goals?: Goal[];
}

interface EvaluationCategory {
  name: string;
  weight: number; // Poids en pourcentage
  score?: number; // 1-10
  comments?: string;
}

interface Goal {
  id: number;
  description: string;
  deadline: string;
  status: 'not-started' | 'in-progress' | 'completed' | 'cancelled';
  progress: number; // 0-100
}

interface EvaluationStats {
  totalEvaluations: number;
  pendingEvaluations: number;
  completedEvaluations: number;
  overdueEvaluations: number;
  averageScore: number;
  completionRate: number;
}

@Component({
  selector: 'app-rh-evaluations',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="rh-evaluations-container">
      <header class="page-header">
        <h1>Évaluations & Performance</h1>
        <p>Suivi des évaluations et de la performance des employés</p>
        <nav class="breadcrumb">
          <a routerLink="/rh">Tableau de Bord RH</a> > Évaluations
        </nav>
      </header>

      <div class="stats-overview">
        <div class="stat-card total">
          <div class="stat-icon">📊</div>
          <div class="stat-info">
            <span class="stat-value">{{ stats.totalEvaluations }}</span>
            <span class="stat-label">Évaluations</span>
          </div>
        </div>
        <div class="stat-card pending">
          <div class="stat-icon">⏳</div>
          <div class="stat-info">
            <span class="stat-value">{{ stats.pendingEvaluations }}</span>
            <span class="stat-label">En Attente</span>
          </div>
        </div>
        <div class="stat-card completed">
          <div class="stat-icon">✅</div>
          <div class="stat-info">
            <span class="stat-value">{{ stats.completedEvaluations }}</span>
            <span class="stat-label">Complétées</span>
          </div>
        </div>
        <div class="stat-card average-score">
          <div class="stat-icon">⭐</div>
          <div class="stat-info">
            <span class="stat-value">{{ stats.averageScore }}/10</span>
            <span class="stat-label">Score Moyen</span>
          </div>
        </div>
      </div>

      <div class="content-layout">
        <!-- Section Évaluations -->
        <section class="evaluations-section">
          <div class="section-header">
            <h2>Évaluations</h2>
            <div class="controls">
              <select (change)="filterByStatus($event)">
                <option value="">Tous les statuts</option>
                <option value="pending">En attente</option>
                <option value="in-progress">En cours</option>
                <option value="completed">Complétées</option>
                <option value="overdue">En retard</option>
              </select>
              <select (change)="filterByType($event)">
                <option value="">Tous les types</option>
                <option value="annual">Annuelle</option>
                <option value="mid-year">Mi-année</option>
                <option value="probation">Période d'essai</option>
                <option value="project">Projet</option>
              </select>
              <button class="btn-add">+ Nouvelle Évaluation</button>
            </div>
          </div>

          <div class="evaluations-grid">
            <div *ngFor="let evaluation of filteredEvaluations" 
                 class="evaluation-card" 
                 [class]="'status-' + evaluation.status">
              
              <div class="evaluation-header">
                <div class="employee-info">
                  <div class="avatar">
                    {{ getInitials(evaluation.employeeName) }}
                  </div>
                  <div class="details">
                    <h3>{{ evaluation.employeeName }}</h3>
                    <span class="evaluator">Évaluateur: {{ evaluation.evaluatorName }}</span>
                  </div>
                </div>
                <div class="evaluation-meta">
                  <span class="type" [class]="'type-' + evaluation.type">
                    {{ getTypeLabel(evaluation.type) }}
                  </span>
                  <span class="period">{{ evaluation.period }}</span>
                </div>
              </div>

              <div class="evaluation-status">
                <span class="status-badge" [class]="'status-' + evaluation.status">
                  {{ getStatusLabel(evaluation.status) }}
                </span>
                <span class="due-date" [class]="getDueDateClass(evaluation.dueDate, evaluation.status)">
                  Échéance: {{ formatDate(evaluation.dueDate) }}
                </span>
              </div>

              <div class="evaluation-score" *ngIf="evaluation.overallScore">
                <div class="score-display">
                  <span class="score-value">{{ evaluation.overallScore }}/10</span>
                  <div class="stars">
                    <span *ngFor="let star of getStars(evaluation.overallScore)" 
                          class="star" 
                          [class.filled]="star">★</span>
                  </div>
                </div>
              </div>

              <div class="evaluation-categories" *ngIf="evaluation.categories.length">
                <h4>Critères d'évaluation:</h4>
                <div class="categories-list">
                  <div *ngFor="let category of evaluation.categories" class="category-item">
                    <span class="category-name">{{ category.name }}</span>
                    <div class="category-score">
                      <span class="score" *ngIf="category.score">{{ category.score }}/10</span>
                      <span class="weight">({{ category.weight }}%)</span>
                    </div>
                  </div>
                </div>
              </div>

              <div class="evaluation-goals" *ngIf="evaluation.goals && evaluation.goals.length">
                <h4>Objectifs:</h4>
                <div class="goals-summary">
                  <span class="goals-count">{{ getCompletedGoals(evaluation.goals) }}/{{ evaluation.goals.length }} objectifs atteints</span>
                  <div class="goals-progress">
                    <div class="progress-bar" [style.width.%]="getGoalsProgress(evaluation.goals)"></div>
                  </div>
                </div>
              </div>

              <div class="evaluation-actions">
                <button class="btn-view">Voir Détails</button>
                <button *ngIf="evaluation.status === 'pending'" class="btn-start">Commencer</button>
                <button *ngIf="evaluation.status === 'in-progress'" class="btn-continue">Continuer</button>
                <button *ngIf="evaluation.status === 'completed'" class="btn-download">Télécharger</button>
              </div>
            </div>
          </div>
        </section>

        <!-- Sidebar Droite -->
        <aside class="sidebar">
          <!-- Section Top Performers -->
          <div class="top-performers-section">
            <h3>Meilleures Performances</h3>
            <div class="performers-ranking">
              <div *ngFor="let performer of topPerformers; let i = index" class="performer-item">
                <div class="rank-badge" [class]="getRankClass(i)">
                  #{{ i + 1 }}
                </div>
                <div class="performer-info">
                  <div class="avatar">{{ getInitials(performer.name) }}</div>
                  <div class="details">
                    <span class="name">{{ performer.name }}</span>
                    <div class="performance-indicators">
                      <span class="score">{{ performer.averageScore }}/10</span>
                      <div class="mini-stars">
                        <span *ngFor="let star of getStars(performer.averageScore)" 
                              class="mini-star" 
                              [class.filled]="star">★</span>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="improvement" [class]="performer.trend">
                  {{ performer.improvement > 0 ? '+' : '' }}{{ performer.improvement }}%
                </div>
              </div>
            </div>
          </div>

          <!-- Section Alertes -->
          <div class="alerts-section">
            <h3>Alertes & Rappels</h3>
            <div class="alerts-list">
              <div *ngFor="let alert of alerts" class="alert-item" [class]="'alert-' + alert.type">
                <div class="alert-icon">
                  <span *ngIf="alert.type === 'overdue'">⚠️</span>
                  <span *ngIf="alert.type === 'due-soon'">🔔</span>
                  <span *ngIf="alert.type === 'reminder'">📅</span>
                </div>
                <div class="alert-content">
                  <span class="alert-message">{{ alert.message }}</span>
                  <span class="alert-date">{{ alert.date }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Section Performance Analytics -->
          <div class="analytics-section">
            <h3>Analytics Performance</h3>
            <div class="analytics-metrics">
              <div class="metric-item">
                <span class="metric-label">Taux de completion</span>
                <div class="metric-bar">
                  <div class="bar" [style.width.%]="stats.completionRate"></div>
                </div>
                <span class="metric-value">{{ stats.completionRate }}%</span>
              </div>
              
              <div class="metric-item">
                <span class="metric-label">Évaluations en retard</span>
                <div class="metric-bar">
                  <div class="bar danger" [style.width.%]="getOverduePercentage()"></div>
                </div>
                <span class="metric-value">{{ stats.overdueEvaluations }}</span>
              </div>

              <div class="performance-distribution">
                <h4>Distribution des scores</h4>
                <div class="distribution-chart">
                  <div *ngFor="let range of scoreDistribution" class="score-range">
                    <span class="range-label">{{ range.label }}</span>
                    <div class="range-bar">
                      <div class="bar" [style.width.%]="range.percentage"></div>
                    </div>
                    <span class="range-count">{{ range.count }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </aside>
      </div>
    </div>
  `,
  styles: [`
    .rh-evaluations-container {
      padding: 2rem;
      max-width: 1400px;
      margin: 0 auto;
    }

    .page-header {
      margin-bottom: 2rem;
      h1 { color: #333; margin-bottom: 0.5rem; }
      p { color: #666; margin-bottom: 1rem; }
      .breadcrumb a { color: #9c27b0; text-decoration: none; }
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

      &.total { border-left-color: #9c27b0; }
      &.pending { border-left-color: #ff9800; }
      &.completed { border-left-color: #4caf50; }
      &.average-score { border-left-color: #ffc107; }

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

    .evaluations-section {
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
      flex-wrap: wrap;
      gap: 1rem;

      h2 { margin: 0; color: #333; }

      .controls {
        display: flex;
        gap: 1rem;
        align-items: center;

        select {
          padding: 0.5rem;
          border: 1px solid #ddd;
          border-radius: 4px;
          background: white;
        }

        .btn-add {
          background: #9c27b0;
          color: white;
          border: none;
          padding: 0.5rem 1rem;
          border-radius: 4px;
          cursor: pointer;
          font-weight: 500;
          transition: background 0.2s;

          &:hover { background: #7b1fa2; }
        }
      }
    }

    .evaluations-grid {
      display: grid;
      gap: 1.5rem;
    }

    .evaluation-card {
      border: 1px solid #eee;
      border-radius: 8px;
      padding: 1.5rem;
      transition: all 0.2s;
      border-left: 4px solid;

      &.status-pending { border-left-color: #ff9800; }
      &.status-in-progress { border-left-color: #2196f3; }
      &.status-completed { border-left-color: #4caf50; }
      &.status-overdue { border-left-color: #f44336; }

      &:hover {
        box-shadow: 0 4px 12px rgba(0,0,0,0.1);
        transform: translateY(-2px);
      }
    }

    .evaluation-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 1rem;

      .employee-info {
        display: flex;
        align-items: center;
        gap: 1rem;

        .avatar {
          width: 50px;
          height: 50px;
          border-radius: 50%;
          background: #9c27b0;
          color: white;
          display: flex;
          align-items: center;
          justify-content: center;
          font-weight: bold;
        }

        .details {
          h3 {
            margin: 0 0 0.25rem 0;
            color: #333;
            font-size: 1.1rem;
          }

          .evaluator {
            color: #666;
            font-size: 0.85rem;
          }
        }
      }

      .evaluation-meta {
        text-align: right;

        .type {
          display: block;
          padding: 0.25rem 0.5rem;
          border-radius: 12px;
          font-size: 0.75rem;
          font-weight: bold;
          margin-bottom: 0.5rem;

          &.type-annual { background: #e8f5e8; color: #2e7d32; }
          &.type-mid-year { background: #e3f2fd; color: #1976d2; }
          &.type-probation { background: #fff3e0; color: #f57c00; }
          &.type-project { background: #f3e5f5; color: #7b1fa2; }
        }

        .period {
          color: #666;
          font-size: 0.85rem;
        }
      }
    }

    .evaluation-status {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;

      .status-badge {
        padding: 0.25rem 0.75rem;
        border-radius: 12px;
        font-size: 0.8rem;
        font-weight: 500;

        &.status-pending { background: #fff3e0; color: #f57c00; }
        &.status-in-progress { background: #e3f2fd; color: #1976d2; }
        &.status-completed { background: #e8f5e8; color: #2e7d32; }
        &.status-overdue { background: #ffebee; color: #c62828; }
      }

      .due-date {
        font-size: 0.85rem;
        
        &.normal { color: #666; }
        &.warning { color: #ff9800; font-weight: 500; }
        &.overdue { color: #f44336; font-weight: bold; }
      }
    }

    .evaluation-score {
      margin-bottom: 1rem;

      .score-display {
        display: flex;
        align-items: center;
        gap: 1rem;

        .score-value {
          font-size: 1.5rem;
          font-weight: bold;
          color: #9c27b0;
        }

        .stars {
          display: flex;
          .star {
            color: #ddd;
            font-size: 1.2rem;
            &.filled { color: #ffc107; }
          }
        }
      }
    }

    .evaluation-categories {
      margin-bottom: 1rem;

      h4 {
        margin: 0 0 0.5rem 0;
        color: #333;
        font-size: 0.9rem;
      }

      .categories-list {
        display: grid;
        gap: 0.5rem;

        .category-item {
          display: flex;
          justify-content: space-between;
          align-items: center;
          padding: 0.5rem;
          background: #f9f9f9;
          border-radius: 4px;
          font-size: 0.85rem;

          .category-name {
            color: #333;
          }

          .category-score {
            display: flex;
            gap: 0.5rem;
            align-items: center;

            .score {
              font-weight: bold;
              color: #9c27b0;
            }

            .weight {
              color: #666;
              font-size: 0.8rem;
            }
          }
        }
      }
    }

    .evaluation-goals {
      margin-bottom: 1rem;

      h4 {
        margin: 0 0 0.5rem 0;
        color: #333;
        font-size: 0.9rem;
      }

      .goals-summary {
        display: flex;
        align-items: center;
        gap: 1rem;

        .goals-count {
          font-size: 0.85rem;
          color: #666;
          min-width: fit-content;
        }

        .goals-progress {
          flex: 1;
          height: 6px;
          background: #e0e0e0;
          border-radius: 3px;

          .progress-bar {
            height: 100%;
            background: #4caf50;
            border-radius: 3px;
            transition: width 0.3s ease;
          }
        }
      }
    }

    .evaluation-actions {
      display: flex;
      gap: 0.5rem;

      button {
        padding: 0.5rem 1rem;
        border: 1px solid #ddd;
        border-radius: 4px;
        background: white;
        cursor: pointer;
        font-size: 0.85rem;
        transition: all 0.2s;

        &:hover { background: #f5f5f5; }

        &.btn-view { border-color: #2196f3; color: #2196f3; }
        &.btn-start { border-color: #4caf50; color: #4caf50; }
        &.btn-continue { border-color: #ff9800; color: #ff9800; }
        &.btn-download { border-color: #9c27b0; color: #9c27b0; }
      }
    }

    .sidebar {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .top-performers-section,
    .alerts-section,
    .analytics-section {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);

      h3 {
        margin: 0 0 1rem 0;
        color: #333;
        font-size: 1.1rem;
      }
    }

    .performer-item {
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 0.75rem;
      border: 1px solid #eee;
      border-radius: 6px;
      margin-bottom: 0.75rem;

      .rank-badge {
        width: 30px;
        height: 30px;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        font-weight: bold;
        font-size: 0.85rem;

        &.rank-1 { background: #ffd700; color: #333; }
        &.rank-2 { background: #c0c0c0; color: #333; }
        &.rank-3 { background: #cd7f32; color: white; }
        &.rank-other { background: #e0e0e0; color: #666; }
      }

      .performer-info {
        flex: 1;
        display: flex;
        align-items: center;
        gap: 0.75rem;

        .avatar {
          width: 35px;
          height: 35px;
          border-radius: 50%;
          background: #9c27b0;
          color: white;
          display: flex;
          align-items: center;
          justify-content: center;
          font-size: 0.8rem;
          font-weight: bold;
        }

        .details {
          .name {
            display: block;
            font-weight: 500;
            color: #333;
            margin-bottom: 0.25rem;
          }

          .performance-indicators {
            display: flex;
            align-items: center;
            gap: 0.5rem;

            .score {
              font-size: 0.85rem;
              font-weight: bold;
              color: #9c27b0;
            }

            .mini-stars {
              display: flex;
              .mini-star {
                color: #ddd;
                font-size: 0.8rem;
                &.filled { color: #ffc107; }
              }
            }
          }
        }
      }

      .improvement {
        font-size: 0.8rem;
        font-weight: bold;
        padding: 0.2rem 0.4rem;
        border-radius: 8px;

        &.positive { background: #e8f5e8; color: #2e7d32; }
        &.negative { background: #ffebee; color: #c62828; }
        &.neutral { background: #f5f5f5; color: #666; }
      }
    }

    .alert-item {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.75rem;
      border-radius: 6px;
      margin-bottom: 0.5rem;

      &.alert-overdue { background: #ffebee; border-left: 3px solid #f44336; }
      &.alert-due-soon { background: #fff3e0; border-left: 3px solid #ff9800; }
      &.alert-reminder { background: #e3f2fd; border-left: 3px solid #2196f3; }

      .alert-icon {
        font-size: 1.2rem;
      }

      .alert-content {
        flex: 1;

        .alert-message {
          display: block;
          font-size: 0.85rem;
          color: #333;
          margin-bottom: 0.25rem;
        }

        .alert-date {
          font-size: 0.75rem;
          color: #666;
        }
      }
    }

    .metric-item {
      margin-bottom: 1rem;

      .metric-label {
        display: block;
        font-size: 0.85rem;
        color: #666;
        margin-bottom: 0.5rem;
      }

      .metric-bar {
        height: 8px;
        background: #e0e0e0;
        border-radius: 4px;
        margin-bottom: 0.25rem;

        .bar {
          height: 100%;
          background: #9c27b0;
          border-radius: 4px;
          transition: width 0.3s ease;

          &.danger { background: #f44336; }
        }
      }

      .metric-value {
        font-size: 0.8rem;
        color: #333;
        font-weight: 500;
      }
    }

    .performance-distribution {
      margin-top: 1.5rem;

      h4 {
        margin: 0 0 1rem 0;
        color: #333;
        font-size: 0.95rem;
      }

      .score-range {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        margin-bottom: 0.5rem;

        .range-label {
          min-width: 60px;
          font-size: 0.8rem;
          color: #666;
        }

        .range-bar {
          flex: 1;
          height: 6px;
          background: #e0e0e0;
          border-radius: 3px;

          .bar {
            height: 100%;
            background: #9c27b0;
            border-radius: 3px;
          }
        }

        .range-count {
          min-width: 30px;
          text-align: right;
          font-size: 0.8rem;
          color: #333;
        }
      }
    }

    @media (max-width: 1024px) {
      .content-layout {
        grid-template-columns: 1fr;
      }

      .section-header {
        flex-direction: column;
        align-items: stretch;

        .controls {
          flex-wrap: wrap;
        }
      }

      .evaluation-header {
        flex-direction: column;
        gap: 1rem;

        .evaluation-meta {
          text-align: left;
        }
      }

      .performer-item {
        flex-wrap: wrap;
        gap: 0.5rem;
      }
    }
  `]
})
export class RhEvaluationsComponent implements OnInit {
  private userService = inject(UserService);
  private teamService = inject(TeamService);

  // Données simulées
  allEvaluations: Evaluation[] = [
    {
      id: 1,
      employeeName: 'Marie Dubois',
      employeeId: 1,
      evaluatorName: 'Pierre Manager',
      evaluatorId: 2,
      period: '2025-Q2',
      type: 'annual',
      status: 'completed',
      dueDate: '2025-06-30',
      completedDate: '2025-06-25',
      overallScore: 8.5,
      categories: [
        { name: 'Performance technique', weight: 40, score: 9, comments: 'Excellente maîtrise' },
        { name: 'Travail en équipe', weight: 30, score: 8, comments: 'Très collaborative' },
        { name: 'Initiative', weight: 30, score: 8.5, comments: 'Proactive' }
      ],
      comments: 'Excellente performance globale.',
      goals: [
        { id: 1, description: 'Maîtriser Angular 18', deadline: '2025-08-01', status: 'completed', progress: 100 },
        { id: 2, description: 'Former 2 juniors', deadline: '2025-09-01', status: 'in-progress', progress: 60 }
      ]
    },
    {
      id: 2,
      employeeName: 'Jean Durand',
      employeeId: 3,
      evaluatorName: 'Sophie Lead',
      evaluatorId: 4,
      period: '2025-Q2',
      type: 'mid-year',
      status: 'in-progress',
      dueDate: '2025-07-15',
      categories: [
        { name: 'Qualité du code', weight: 50, score: 7 },
        { name: 'Respect des délais', weight: 30, score: 6 },
        { name: 'Communication', weight: 20 }
      ],
      goals: [
        { id: 3, description: 'Améliorer les tests unitaires', deadline: '2025-08-15', status: 'in-progress', progress: 40 }
      ]
    },
    {
      id: 3,
      employeeName: 'Sophie Laurent',
      employeeId: 5,
      evaluatorName: 'Marie Manager',
      evaluatorId: 6,
      period: '2025-Q2',
      type: 'probation',
      status: 'pending',
      dueDate: '2025-07-01',
      categories: [
        { name: 'Adaptation', weight: 40 },
        { name: 'Apprentissage', weight: 35 },
        { name: 'Intégration équipe', weight: 25 }
      ],
      goals: []
    },
    {
      id: 4,
      employeeName: 'Pierre Martin',
      employeeId: 7,
      evaluatorName: 'Claire Director',
      evaluatorId: 8,
      period: '2025-Q1',
      type: 'project',
      status: 'overdue',
      dueDate: '2025-06-15',
      overallScore: 7.2,
      categories: [
        { name: 'Gestion projet', weight: 60, score: 7.5 },
        { name: 'Leadership', weight: 40, score: 6.8 }
      ],
      goals: [
        { id: 4, description: 'Livrer le projet X', deadline: '2025-06-30', status: 'completed', progress: 100 }
      ]
    }
  ];

  filteredEvaluations: Evaluation[] = this.allEvaluations;

  stats: EvaluationStats = {
    totalEvaluations: 0,
    pendingEvaluations: 0,
    completedEvaluations: 0,
    overdueEvaluations: 0,
    averageScore: 0,
    completionRate: 0
  };

  topPerformers = [
    { name: 'Marie Dubois', averageScore: 8.5, improvement: 5, trend: 'positive' },
    { name: 'Pierre Martin', averageScore: 7.2, improvement: -3, trend: 'negative' },
    { name: 'Jean Durand', averageScore: 6.5, improvement: 0, trend: 'neutral' }
  ];

  alerts = [
    { type: 'overdue', message: 'Évaluation de Pierre Martin en retard', date: '15/06/2025' },
    { type: 'due-soon', message: 'Évaluation de Sophie Laurent due dans 2 jours', date: '01/07/2025' },
    { type: 'reminder', message: 'Programmer évaluations Q3', date: '15/07/2025' }
  ];

  scoreDistribution = [
    { label: '9-10', percentage: 25, count: 1 },
    { label: '7-8', percentage: 50, count: 2 },
    { label: '5-6', percentage: 25, count: 1 },
    { label: '0-4', percentage: 0, count: 0 }
  ];

  ngOnInit() {
    this.calculateStats();
  }

  calculateStats() {
    this.stats.totalEvaluations = this.allEvaluations.length;
    this.stats.pendingEvaluations = this.allEvaluations.filter(e => e.status === 'pending').length;
    this.stats.completedEvaluations = this.allEvaluations.filter(e => e.status === 'completed').length;
    this.stats.overdueEvaluations = this.allEvaluations.filter(e => e.status === 'overdue').length;
    
    const completedWithScores = this.allEvaluations.filter(e => e.overallScore);
    this.stats.averageScore = completedWithScores.length > 0 
      ? parseFloat((completedWithScores.reduce((sum, e) => sum + (e.overallScore || 0), 0) / completedWithScores.length).toFixed(1))
      : 0;
    
    this.stats.completionRate = this.stats.totalEvaluations > 0 
      ? Math.round((this.stats.completedEvaluations / this.stats.totalEvaluations) * 100)
      : 0;
  }

  filterByStatus(event: any) {
    const status = event.target.value;
    this.applyFilters(status, null);
  }

  filterByType(event: any) {
    const type = event.target.value;
    this.applyFilters(null, type);
  }

  private applyFilters(status: string | null, type: string | null) {
    this.filteredEvaluations = this.allEvaluations.filter(evaluation => {
      const statusMatch = !status || evaluation.status === status;
      const typeMatch = !type || evaluation.type === type;
      return statusMatch && typeMatch;
    });
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase();
  }

  getTypeLabel(type: string): string {
    const labels: {[key: string]: string} = {
      'annual': 'Annuelle',
      'mid-year': 'Mi-année',
      'probation': 'Période d\'essai',
      'project': 'Projet'
    };
    return labels[type] || type;
  }

  getStatusLabel(status: string): string {
    const labels: {[key: string]: string} = {
      'pending': 'En attente',
      'in-progress': 'En cours',
      'completed': 'Complétée',
      'overdue': 'En retard'
    };
    return labels[status] || status;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR', { 
      day: '2-digit', 
      month: '2-digit', 
      year: 'numeric' 
    });
  }

  getDueDateClass(dueDate: string, status: string): string {
    if (status === 'overdue') return 'overdue';
    
    const due = new Date(dueDate);
    const now = new Date();
    const daysLeft = Math.ceil((due.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));
    
    if (daysLeft <= 3) return 'warning';
    return 'normal';
  }

  getStars(score: number): boolean[] {
    const fullStars = Math.floor(score / 2); // Score sur 10 -> étoiles sur 5
    const stars: boolean[] = [];
    for (let i = 0; i < 5; i++) {
      stars.push(i < fullStars);
    }
    return stars;
  }

  getCompletedGoals(goals: Goal[]): number {
    return goals.filter(g => g.status === 'completed').length;
  }

  getGoalsProgress(goals: Goal[]): number {
    if (goals.length === 0) return 0;
    const totalProgress = goals.reduce((sum, g) => sum + g.progress, 0);
    return totalProgress / goals.length;
  }

  getRankClass(index: number): string {
    switch (index) {
      case 0: return 'rank-1';
      case 1: return 'rank-2';
      case 2: return 'rank-3';
      default: return 'rank-other';
    }
  }

  getOverduePercentage(): number {
    return this.stats.totalEvaluations > 0 
      ? Math.round((this.stats.overdueEvaluations / this.stats.totalEvaluations) * 100)
      : 0;
  }
}
