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
      // Chargement des données de performance principales
      this.performanceService.getPerformanceMetrics().subscribe({
        next: (data) => {
          this.performanceData.set(data);
          console.log('✅ [Manager Performance] Données de performance chargées');
        },
        error: (error) => console.error('❌ [Manager Performance] Erreur performance:', error)
      });

      // Chargement de la productivité des équipes
      this.performanceService.getTeamProductivity().subscribe({
        next: (data) => {
          this.teamProductivity.set(data);
          console.log('✅ [Manager Performance] Productivité équipes chargée');
        },
        error: (error) => console.error('❌ [Manager Performance] Erreur productivité:', error)
      });

      // Chargement des tendances mensuelles
      this.performanceService.getMonthlyTrends().subscribe({
        next: (data) => {
          this.monthlyTrends.set(data);
          console.log('✅ [Manager Performance] Tendances mensuelles chargées');
        },
        error: (error) => console.error('❌ [Manager Performance] Erreur tendances:', error)
      });      // Simulation d'alertes et top performers (données mockées directement)
      this.alerts.set([
        { type: 'warning', message: 'Productivité en baisse équipe Frontend' },
        { type: 'info', message: 'Nouveau record mensuel atteint' },
        { type: 'success', message: 'Objectifs trimestriels dépassés' }
      ]);

      this.topPerformers.set([
        { name: 'Marie Dubois', score: 98.5, department: 'Development' },
        { name: 'Pierre Martin', score: 96.2, department: 'Design' },
        { name: 'Sophie Laurent', score: 94.8, department: 'Management' }
      ]);

      this.lastUpdated.set(new Date());
      console.log('✅ [Manager Performance] Toutes les données chargées avec succès');
    } catch (error) {
      console.error('❌ [Manager Performance] Erreur lors du chargement des données:', error);
    } finally {
      this.loading.set(false);
    }
  }

  /**
   * Démarre l'auto-refresh des données
   */
  private startAutoRefresh(): void {
    console.log(`🔄 [Manager Performance] Démarrage de l'auto-refresh (${this.REFRESH_INTERVAL / 1000}s)`);
    
    this.refreshSubscription = interval(this.REFRESH_INTERVAL).subscribe(() => {
      console.log('🔄 [Manager Performance] Auto-refresh déclenché');
      this.loadAllData();
    });
  }

  /**
   * Rafraîchit manuellement toutes les données
   */
  refreshData(): void {
    console.log('🔄 [Manager Performance] Rafraîchissement manuel déclenché');
    this.loadAllData();
  }

  /**
   * Récupère la classe CSS pour les alertes
   */
  getAlertClass(alertType: string): string {
    const baseClass = 'alert';
    switch (alertType) {
      case 'error':
        return `${baseClass} alert-error`;
      case 'warning':
        return `${baseClass} alert-warning`;
      case 'info':
        return `${baseClass} alert-info`;
      case 'success':
        return `${baseClass} alert-success`;
      default:
        return baseClass;
    }
  }

  /**
   * Formate les nombres avec séparateurs
   */
  formatNumber(value: number): string {
    return new Intl.NumberFormat('fr-FR').format(value);
  }

  /**
   * Formate les pourcentages
   */
  formatPercentage(value: number): string {
    return `${value.toFixed(1)}%`;
  }

  /**
   * Récupère la classe CSS pour les tendances
   */
  getTrendClass(percentage: number): string {
    if (percentage > 0) return 'trend-up';
    if (percentage < 0) return 'trend-down';
    return 'trend-stable';
  }

  /**
   * Récupère l'icône pour les tendances
   */
  getTrendIcon(percentage: number): string {
    if (percentage > 0) return '↗️';
    if (percentage < 0) return '↘️';
    return '→';
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
