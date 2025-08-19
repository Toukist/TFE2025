import { Injectable } from '@angular/core';
import { Observable, of, delay, map } from 'rxjs';
import { PerformanceData, TeamProductivity, MonthlyTrend, Alert, TopPerformer } from './performance.types';

@Injectable({
  providedIn: 'root'
})
export class PerformanceService {
  
  // Simule un appel HTTP avec delay
  getPerformanceMetrics(): Observable<PerformanceData> {
    console.log('🔄 Simulation HTTP: GET /api/performance');
    
    const mockData = this.generateMockPerformanceData();
    
    return of(mockData).pipe(
      delay(800), // Simule latence réseau
      map(data => {
        console.log('✅ HTTP Response: Performance data loaded', data);
        return data;
      })
    );
  }

  getTeamProductivity(): Observable<TeamProductivity[]> {
    console.log('🔄 Simulation HTTP: GET /api/team-productivity');
    
    return of(this.generateTeamProductivityData()).pipe(
      delay(600),
      map(data => {
        console.log('✅ HTTP Response: Team productivity loaded');
        return data;
      })
    );
  }

  getMonthlyTrends(): Observable<MonthlyTrend[]> {
    console.log('🔄 Simulation HTTP: GET /api/monthly-trends');
    
    return of(this.generateMonthlyTrendsData()).pipe(
      delay(700),
      map(data => {
        console.log('✅ HTTP Response: Monthly trends loaded');
        return data;
      })
    );
  }

  private generateMockPerformanceData(): PerformanceData {
    return {
      globalMetrics: {
        productivity: Math.floor(Math.random() * 20) + 80, // 80-100%
        efficiency: Math.floor(Math.random() * 15) + 85,   // 85-100%
        satisfaction: Math.floor(Math.random() * 10) + 90, // 90-100%
        completionRate: Math.floor(Math.random() * 15) + 85
      },
      dailyStats: {
        tasksCompleted: Math.floor(Math.random() * 20) + 45,
        tasksInProgress: Math.floor(Math.random() * 10) + 12,
        averageTime: +(Math.random() * 2 + 3).toFixed(1), // 3-5h
        qualityScore: Math.floor(Math.random() * 10) + 90
      },
      alerts: [
        { type: 'success', message: 'Objectifs mensuels atteints à 103%' },
        { type: 'warning', message: 'Équipe 2 en retard sur 2 projets' },
        { type: 'info', message: 'Nouvelle formation disponible' }
      ],
      topPerformers: this.generateTopPerformers()
    };
  }

  private generateTeamProductivityData(): TeamProductivity[] {
    return [
      { 
        teamName: 'Équipe 1 - Matin', 
        productivity: 92,
        tasksCompleted: 34,
        efficiency: 88,
        trend: 'up'
      },
      { 
        teamName: 'Équipe 2 - Après-midi', 
        productivity: 87,
        tasksCompleted: 28,
        efficiency: 91,
        trend: 'down'
      },
      { 
        teamName: 'Support', 
        productivity: 94,
        tasksCompleted: 19,
        efficiency: 96,
        trend: 'up'
      }
    ];
  }

  private generateMonthlyTrendsData(): MonthlyTrend[] {
    const months = ['Jan', 'Fév', 'Mar', 'Avr', 'Mai', 'Jun'];
    return months.map(month => ({
      month,
      productivity: Math.floor(Math.random() * 20) + 80,
      efficiency: Math.floor(Math.random() * 15) + 85,
      satisfaction: Math.floor(Math.random() * 10) + 90
    }));
  }

  private generateTopPerformers(): TopPerformer[] {
    return [
      { name: 'Marie Dubois', score: 98, department: 'Production' },
      { name: 'Jean Martin', score: 96, department: 'Qualité' },
      { name: 'Sophie Leroy', score: 94, department: 'Logistique' }
    ];
  }
}
