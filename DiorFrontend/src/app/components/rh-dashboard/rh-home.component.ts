import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Observable, EMPTY } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';

import { RhDashboardService } from './rh-dashboard.service';

@Component({
  selector: 'app-rh-home',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="isLoading" class="loading-spinner-container">
      <p>Chargement des données...</p>
    </div>

    <div *ngIf="error" class="error-message">
      <p>{{ error }}</p>
      <button (click)="loadDashboardData()">Réessayer</button>
    </div>

    <div *ngIf="!isLoading && !error && dashboardData" class="dashboard-grid">
      <!-- Card Personnel -->
      <div class="dashboard-card personnel-card" (click)="navigateTo('/rh/personnel')">
        <div class="card-header">
          <span class="icon-container">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/></svg>
          </span>
          <h3>Personnel</h3>
        </div>
        <div class="card-content">
          <div class="stats-grid">
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.personnel.totalEmployees }}</span>
              <span class="stat-label">Employés</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.personnel.activeEmployees }}</span>
              <span class="stat-label">Actifs</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.personnel.totalTeams }}</span>
              <span class="stat-label">Équipes</span>
            </div>
          </div>
          <p class="section-subtitle">Équipes :</p>
          <div class="team-chips">
            <span *ngFor="let team of dashboardData.teams | slice:0:5" class="chip">{{ team.name }}</span>
            <span *ngIf="dashboardData.teams.length > 5" class="chip">+{{ dashboardData.teams.length - 5 }}</span>
          </div>
          <p class="section-subtitle">Derniers ajouts :</p>
          <div class="avatar-list">
            <div *ngFor="let user of dashboardData.personnel.latestUsers" class="avatar" [title]="user.firstName + ' ' + user.lastName">
              <img *ngIf="user.avatarUrl" [src]="user.avatarUrl" [alt]="user.firstName + ' ' + user.lastName">
              <span *ngIf="!user.avatarUrl">{{ user.initials }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Card Planification -->
      <div class="dashboard-card planification-card" (click)="navigateTo('/rh/planification')">
        <div class="card-header">
          <span class="icon-container">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M19 3h-1V1h-2v2H8V1H6v2H5c-1.11 0-1.99.9-1.99 2L3 19c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm0 16H5V8h14v11zM7 10h5v5H7z"/></svg>
          </span>
          <h3>Planification</h3>
        </div>
        <div class="card-content">
          <div class="stats-grid">
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.planification.totalTasks }}</span>
              <span class="stat-label">Tâches</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.planification.pendingTasks }}</span>
              <span class="stat-label">En attente</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.planification.completedTasks }}</span>
              <span class="stat-label">Terminées</span>
            </div>
          </div>
          <p class="section-subtitle">Progression globale :</p>
          <div class="progress-bar-container">
            <div class="progress-bar" [style.width.%]="dashboardData.planification.overallProgress"></div>
          </div>
          <p class="progress-label">{{ dashboardData.planification.overallProgress }}%</p>
          <p class="section-subtitle">Charge par équipe :</p>
          <div class="team-load-indicators">
            <div *ngFor="let load of dashboardData.planification.teamLoad | slice:0:3" class="team-load-item">
              <span class="team-name">{{ load.teamName }}</span>
              <span class="load-value">{{ load.load }} tâches</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Card Formation -->
      <div class="dashboard-card formation-card" (click)="navigateTo('/rh/formation')">
        <div class="card-header">
          <span class="icon-container">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M12 3L1 9l11 6 9-4.91V17h2V9L12 3zm0 8.53L4.74 8 12 4.8l7.26 3.2L12 11.53zM5 13.18v3.13c0 .52.18.98.49 1.34.31.36.75.55 1.26.55h10.5c.51 0 .95-.19 1.26-.55.31-.36.49-.82.49-1.34v-3.13L12 16.38 5 13.18z"/></svg>
          </span>
          <h3>Formation</h3>
        </div>
        <div class="card-content">
          <div class="stats-grid">
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.formation.activeTrainings }}</span>
              <span class="stat-label">Actives</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.formation.certificationsAchieved }}</span>
              <span class="stat-label">Certif.</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.formation.upcomingDeadlines }}</span>
              <span class="stat-label">Échéances</span>
            </div>
          </div>
          <p class="section-subtitle">Top Performers :</p>
          <div class="top-performers-list">
            <div *ngFor="let performer of dashboardData.formation.topPerformers" class="performer-item">
              <div class="avatar">
                <img *ngIf="performer.avatarUrl" [src]="performer.avatarUrl" [alt]="performer.firstName">
                <span *ngIf="!performer.avatarUrl">{{ getInitials(performer) }}</span>
              </div>
              <span class="name">{{ performer.firstName }} {{ performer.lastName }}</span>
              <span class="detail">{{ 0 }} formations</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Card Évaluations -->
      <div class="dashboard-card evaluations-card" (click)="navigateTo('/rh/evaluations')">
        <div class="card-header">
          <span class="icon-container">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z"/></svg>
          </span>
          <h3>Évaluations</h3>
        </div>
        <div class="card-content">
          <div class="stats-grid">
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.evaluations.pendingEvaluations }}</span>
              <span class="stat-label">En attente</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.evaluations.completedEvaluations }}</span>
              <span class="stat-label">Complétées</span>
            </div>
            <div class="stat-item">
              <span class="stat-value">{{ dashboardData.evaluations.averageScore }}/5</span>
              <span class="stat-label">Score moyen</span>
            </div>
          </div>
          <p class="section-subtitle">Mieux Notés :</p>
          <div class="top-rated-list">
            <div *ngFor="let rated of dashboardData.evaluations.topRatedEmployees; let i = index" class="rated-item">
              <span class="rank">#{{i + 1}}</span>
              <div class="avatar">
                <img *ngIf="rated.user.avatarUrl" [src]="rated.user.avatarUrl" [alt]="rated.user.firstName">
                <span *ngIf="!rated.user.avatarUrl">{{ getInitials(rated.user) }}</span>
              </div>
              <span class="name">{{ rated.user.firstName }} {{ rated.user.lastName }}</span>
              <span class="rating"><span class="stars">★★★★★</span> {{ rated.rating }}/5</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div *ngIf="!isLoading && !error && !dashboardData" class="empty-state">
      <p>Aucune donnée à afficher pour le moment.</p>
    </div>
  `
})
export class RhHomeComponent implements OnInit {
  private rhDashboardService = inject(RhDashboardService);
  private router = inject(Router);

  dashboardData: {
    personnel: {
      totalEmployees: number;
      activeEmployees: number;
      totalTeams: number;
      latestUsers: { firstName: string; lastName: string; avatarUrl?: string; initials?: string }[];
    };
    teams: { name: string }[];
    planification: {
      totalTasks: number;
      pendingTasks: number;
      completedTasks: number;
      overallProgress: number;
      teamLoad: { teamName: string; load: number }[];
    };
    formation: {
      activeTrainings: number;
      certificationsAchieved: number;
      upcomingDeadlines: number;
      topPerformers: { firstName: string; lastName: string; avatarUrl?: string }[];
    };
    evaluations: {
      pendingEvaluations: number;
      completedEvaluations: number;
      averageScore: number;
      topRatedEmployees: { user: { firstName: string; lastName: string; avatarUrl?: string }; rating: number }[];
    };
  } | null = null;
  isLoading = false;
  error: string | null = null;

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.isLoading = true;
    this.error = null;

    this.rhDashboardService.getDashboardData()
      .pipe(
        catchError(error => {
          console.error('Erreur lors du chargement des données du dashboard:', error);
          this.error = error.message || 'Une erreur est survenue lors du chargement des données.';
          return EMPTY;
        }),
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe(data => {
        this.dashboardData = data;
        console.log('Données du dashboard chargées:', data);
      });
  }
  navigateTo(path: string): void {
    console.log('🔄 Navigation vers:', path);
    this.router.navigate([path]).then(success => {
      console.log('✅ Navigation réussie:', success);
    }).catch(error => {
      console.error('❌ Erreur de navigation:', error);
    });
  }

  getInitials(user: { firstName: string; lastName: string }): string {
    return `${user.firstName[0]}${user.lastName[0]}`.toUpperCase();
  }
}
