import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { interval, Subscription } from 'rxjs';

import { PerformanceService } from '../../services/performance.service';
import { PerformanceData, Alert, TopPerformer, TeamProductivity, MonthlyTrend } from '../../services/performance.types';

@Component({
  selector: 'app-manager-performance',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './manager-performance.component.html',
  styleUrls: ['./manager-performance.component.scss']
})
export class ManagerPerformanceComponent implements OnInit, OnDestroy {
  private performanceService = inject(PerformanceService);
  
  // Signaux pour l'état du composant
  performanceData = signal<PerformanceData | null>(null);
  teamProductivity = signal<TeamProductivity[]>([]);
  monthlyTrends = signal<MonthlyTrend[]>([]);
  alerts = signal<Alert[]>([]);
  topPerformers = signal<TopPerformer[]>([]);
  loading = signal<boolean>(false);
  lastUpdated = signal<Date>(new Date());
  
  private refreshSubscription?: Subscription;
  private readonly REFRESH_INTERVAL = 30000; // 30 secondes

  ngOnInit(): void {
    console.log('🚀 [Manager Performance] Initialisation du composant Performance Manager');
    this.loadAllData();
    this.startAutoRefresh();
  }

  ngOnDestroy(): void {
    console.log('🔚 [Manager Performance] Destruction du composant - arrêt de l\'auto-refresh');
    this.refreshSubscription?.unsubscribe();
  }
  /**
   * Charge toutes les données de performance
   */
  async loadAllData(): Promise<void> {
    this.loading.set(true);
    console.log('📊 [Manager Performance] Chargement de toutes les données...');
    
    try {
      // Souscription aux observables et attente des données
      const performance = await this.performanceService.getPerformanceMetrics().toPromise();
      const productivity = await this.performanceService.getTeamProductivity().toPromise();
      const trends = await this.performanceService.getMonthlyTrends().toPromise();
      
      if (performance) {
        this.performanceData.set(performance);
        this.alerts.set(performance.alerts);
        this.topPerformers.set(performance.topPerformers);
      }
      
      if (productivity) {
        this.teamProductivity.set(productivity);
      }
      
      if (trends) {
        this.monthlyTrends.set(trends);
      }
      
      this.lastUpdated.set(new Date());
      
      console.log('✅ [Manager Performance] Toutes les données chargées avec succès');
    } catch (error) {
      console.error('❌ [Manager Performance] Erreur lors du chargement:', error);
    } finally {
      this.loading.set(false);
    }
  }

  /**
   * Démarre l'auto-refresh des données
   */
  private startAutoRefresh(): void {
    console.log(`⏱️ [Manager Performance] Auto-refresh démarré (${this.REFRESH_INTERVAL / 1000}s)`);
    this.refreshSubscription = interval(this.REFRESH_INTERVAL).subscribe(() => {
      console.log('🔄 [Manager Performance] Auto-refresh en cours...');
      this.loadAllData();
    });
  }

  /**
   * Rafraîchir manuellement les données
   */
  async refreshData(): Promise<void> {
    console.log('🔄 [Manager Performance] Refresh manuel déclenché');
    await this.loadAllData();
  }

  /**
   * Retourne la classe CSS selon la performance
   */
  getPerformanceClass(value: number, threshold: number = 80): string {
    if (value >= threshold) return 'excellent';
    if (value >= 60) return 'good';
    if (value >= 40) return 'average';
    return 'poor';
  }

  /**
   * Retourne la classe CSS selon la sévérité de l'alerte
   */
  getAlertClass(severity: 'info' | 'warning' | 'critical'): string {
    const classes = {
      info: 'alert-info',
      warning: 'alert-warning', 
      critical: 'alert-critical'
    };
    return classes[severity] || 'alert-info';
  }
  /**
   * Retourne la couleur selon le statut
   */
  getStatusColor(status: string): string {
    const colors: { [key: string]: string } = {
      'En cours': '#22c55e',
      'Terminé': '#3b82f6',
      'En retard': '#ef4444',
      'En attente': '#f59e0b'
    };
    return colors[status] || '#6b7280';
  }

  /**
   * Retourne la classe CSS selon la tendance
   */
  getTrendClass(trend: 'up' | 'down' | 'stable'): string {
    const classes = {
      up: 'trend-up',
      down: 'trend-down',
      stable: 'trend-stable'
    };
    return classes[trend] || 'trend-stable';
  }

  /**
   * Retourne la classe CSS pour la barre de progression
   */
  getProgressBarClass(percentage: number): string {
    if (percentage >= 90) return 'progress-excellent';
    if (percentage >= 75) return 'progress-good';
    if (percentage >= 50) return 'progress-average';
    return 'progress-poor';
  }

  /**
   * Formate un montant en devise
   */
  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('fr-FR', {
      style: 'currency',
      currency: 'EUR'
    }).format(amount);
  }

  /**
   * Formate un pourcentage
   */
  formatPercentage(value: number): string {
    return `${value.toFixed(1)}%`;
  }

  /**
   * Formate les durées
   */
  formatDuration(hours: number): string {
    if (hours < 1) {
      return `${Math.round(hours * 60)}min`;
    }
    return `${hours.toFixed(1)}h`;
  }

  /**
   * Formate une date
   */
  formatDate(date: Date): string {
    return new Intl.DateTimeFormat('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(date);
  }
}
