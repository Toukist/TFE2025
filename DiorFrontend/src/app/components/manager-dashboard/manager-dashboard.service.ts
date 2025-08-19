import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, forkJoin, of, BehaviorSubject } from 'rxjs';
import { map, catchError, shareReplay } from 'rxjs/operators';
import {
  User,
  Team,
  Task,
  Project,
  Performance,
  DashboardStats,
  CreateProjectRequest,
  ManagerDashboardData
} from './manager-dashboard.types';

@Injectable({
  providedIn: 'root'
})
export class ManagerDashboardService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7201/api';
  
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  /**
   * Récupère tous les utilisateurs
   */
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/user`).pipe(
      map(users => users.map(user => ({
        ...user,
        isOnline: Math.random() > 0.3, // Simulation statut en ligne
        avatar: this.generateAvatar(user.firstName, user.lastName)
      }))),
      catchError(this.handleError)
    );
  }

  /**
   * Récupère toutes les équipes
   */
  getTeams(): Observable<Team[]> {
    return this.http.get<Team[]>(`${this.apiUrl}/Team`).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Récupère toutes les tâches
   */
  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/Task`).pipe(
      catchError(() => {
        // Données simulées si l'endpoint n'existe pas
        return of(this.generateMockTasks());
      })
    );
  }

  /**
   * Récupère les projets
   */
  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.apiUrl}/Projet`).pipe(
      catchError(() => {
        // Données simulées si l'endpoint n'existe pas
        return of(this.generateMockProjects());
      })
    );
  }

  /**
   * Envoie un projet à une équipe
   */
  sendProjectToTeam(project: CreateProjectRequest): Observable<any> {
    const projectData = {
      ...project,
      createdBy: 'Bruno Dupuis',
      createdAt: new Date(),
      status: 'pending'
    };

    return this.http.post(`${this.apiUrl}/Projet`, projectData).pipe(
      catchError(() => {
        // Simulation de succès si l'endpoint n'existe pas
        console.log('Projet envoyé (simulé):', projectData);
        return of({ success: true, id: Math.floor(Math.random() * 1000) });
      })
    );
  }

  /**
   * Récupère les métriques de performance
   */
  getPerformanceMetrics(): Observable<Performance> {
    return this.http.get<Performance>(`${this.apiUrl}/Performance`).pipe(
      catchError(() => {
        // Données simulées
        return of(this.generateMockPerformance());
      })
    );
  }

  /**
   * Charge toutes les données du dashboard
   */
  getDashboardData(): Observable<ManagerDashboardData> {
    return forkJoin({
      users: this.getUsers(),
      teams: this.getTeams(),
      tasks: this.getTasks(),
      projects: this.getProjects(),
      performance: this.getPerformanceMetrics()
    }).pipe(
      map(data => ({
        ...data,
        stats: this.calculateStats(data)
      })),
      shareReplay(1),
      catchError(this.handleError)
    );
  }

  /**
   * Calcule les statistiques du dashboard
   */
  private calculateStats(data: any): DashboardStats {
    const { users, teams, tasks, projects } = data;    // Stats équipe
    const totalMembers = users.length;
    const activeToday = users.filter((u: User) => u.isActive).length;
    const onlineNow = users.filter((u: User) => u.status === 'online').length;
    const attendanceRate = totalMembers > 0 ? (activeToday / totalMembers) * 100 : 0;

    // Stats projets
    const activeProjects = projects.filter((p: Project) => p.status === 'in-progress').length;
    const tasksInProgress = tasks.filter((t: Task) => t.status === 'in-progress').length;
    const upcomingDeadlines = projects.filter((p: Project) => {
      const deadline = new Date(p.deadline);
      const now = new Date();
      const daysDiff = Math.ceil((deadline.getTime() - now.getTime()) / (1000 * 3600 * 24));
      return daysDiff <= 7 && daysDiff > 0;
    }).length;

    // Stats performance
    const completedTasks = tasks.filter((t: Task) => t.status === 'completed');
    const completionRate = tasks.length > 0 ? (completedTasks.length / tasks.length) * 100 : 0;
    const averageTaskTime = completedTasks.reduce((sum: number, task: Task) => 
      sum + (task?.actualHours ?? task?.estimatedHours ?? 0), 0) / Math.max(completedTasks.length, 1);
    const productivity = Math.min(100, (completionRate + (100 - averageTaskTime * 2)) / 2);

    return {
      team: {
        totalMembers,
        activeToday,
        attendanceRate: Math.round(attendanceRate),
        onlineNow
      },
      projects: {
        activeProjects,
        tasksInProgress,
        upcomingDeadlines,
        recentlySent: projects.slice(0, 3)
      },
      performance: {
        productivity: Math.round(productivity),
        completionRate: Math.round(completionRate),
        averageTaskTime: Math.round(averageTaskTime * 10) / 10,
        monthlyTrend: Math.round((Math.random() - 0.5) * 20)
      }
    };
  }

  /**
   * Génère un avatar basé sur les initiales
   */
  private generateAvatar(firstName: string, lastName: string): string {
    const initials = `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
    const colors = ['#FF6B6B', '#4ECDC4', '#45B7D1', '#96CEB4', '#FFEAA7', '#DDA0DD', '#98D8C8'];
    const color = colors[Math.floor(Math.random() * colors.length)];
    
    // Retourne une URL data pour un SVG avec les initiales
    const svg = `
      <svg width="40" height="40" xmlns="http://www.w3.org/2000/svg">
        <circle cx="20" cy="20" r="20" fill="${color}"/>
        <text x="50%" y="50%" text-anchor="middle" dy="0.3em" font-family="Arial, sans-serif" font-size="14" font-weight="bold" fill="white">${initials}</text>
      </svg>
    `;
    return `data:image/svg+xml;base64,${btoa(svg)}`;
  }

  /**
   * Génère des tâches simulées
   */
  private generateMockTasks(): Task[] {
    return [
      {
        id: 1,
        title: 'Mise à jour interface utilisateur',
        description: 'Refonte de l\'interface principale',
        assignedTo: 1,
        status: 'in-progress',
        priority: 'high',
        estimatedHours: 8,
        actualHours: 6,
        createdAt: new Date('2025-06-10')
      },
      {
        id: 2,
        title: 'Tests de performance',
        description: 'Optimisation des requêtes base de données',
        assignedTo: 2,
        status: 'completed',
        priority: 'medium',
        estimatedHours: 4,
        actualHours: 3.5,
        createdAt: new Date('2025-06-12'),
        completedAt: new Date('2025-06-14')
      },
      {
        id: 3,
        title: 'Documentation API',
        description: 'Rédaction de la documentation technique',
        assignedTo: 3,
        status: 'todo',
        priority: 'low',
        estimatedHours: 6,
        createdAt: new Date('2025-06-15')
      }
    ];
  }

  /**
   * Génère des projets simulés
   */
  private generateMockProjects(): Project[] {
    return [
      {
        id: 1,
        title: 'Migration base de données',
        description: 'Migration vers la nouvelle version de PostgreSQL',
        teamId: 1,
        teamName: 'Équipe opérateurs matin',
        priority: 'high',
        deadline: new Date('2025-06-25'),
        status: 'in-progress',
        createdBy: 'Bruno Dupuis',
        createdAt: new Date('2025-06-10'),
        progress: 65
      },
      {
        id: 2,
        title: 'Formation sécurité',
        description: 'Session de formation sur les nouvelles procédures',
        teamId: 2,
        teamName: 'Équipe opérateurs après-midi',
        priority: 'medium',
        deadline: new Date('2025-06-30'),
        status: 'pending',
        createdBy: 'Bruno Dupuis',
        createdAt: new Date('2025-06-12'),
        progress: 20
      }
    ];
  }

  /**
   * Génère des métriques de performance simulées
   */
  private generateMockPerformance(): Performance {
    return {
      productivity: 78,
      completionRate: 85,
      averageTaskTime: 4.2,
      topPerformers: [],
      monthlyComparison: {
        current: 78,
        previous: 72,
        change: 8.3
      },
      weeklyComparison: {
        current: 82,
        previous: 79,
        change: 3.8
      },
      alerts: [
        'Échéance proche pour le projet Migration BD',
        '2 tâches en retard cette semaine'
      ],
      recommendations: [
        'Augmenter la fréquence des stand-ups',
        'Réviser l\'estimation des tâches complexes'
      ]
    };
  }

  /**
   * Gestion centralisée des erreurs
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('Erreur API Dashboard Manager:', error);
    
    let errorMessage = 'Une erreur est survenue';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Erreur client: ${error.error.message}`;
    } else {
      errorMessage = `Erreur serveur ${error.status}: ${error.message}`;
    }
    
    return throwError(() => new Error(errorMessage));
  }
}
