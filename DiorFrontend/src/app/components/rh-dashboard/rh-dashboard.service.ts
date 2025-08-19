import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, forkJoin } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { UserDto as User } from '../../models/user.model';
import { TeamDto as Team } from '../../models/TeamDto';
import { Task } from '../../models/task.model';
import { Notification } from '../../models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class RhDashboardService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl; // Utilise l'URL de l'environnement

  // Endpoints de l'API
  private usersUrl = `${this.apiUrl}/User`;
  private teamsUrl = `${this.apiUrl}/Team`;
  private tasksUrl = `${this.apiUrl}/Task`;
  private notificationsUrl = `${this.apiUrl}/Notification`;

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.usersUrl).pipe(
      map(users => users.map(u => ({ ...u, initials: this.getInitials(u.firstName, u.lastName) }))),
      catchError(this.handleError)
    );
  }

  getTeams(): Observable<Team[]> {
    return this.http.get<Team[]>(this.teamsUrl).pipe(
      catchError(this.handleError)
    );
  }

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(this.tasksUrl).pipe(
      catchError(this.handleError)
    );
  }

  getNotifications(): Observable<Notification[]> {
    return this.http.get<Notification[]>(this.notificationsUrl).pipe(
      catchError(this.handleError)
    );
  }

  // Méthode pour charger toutes les données nécessaires au dashboard
  getDashboardData(): Observable<any> {
    return forkJoin({
      users: this.getUsers(),
      teams: this.getTeams(),
      tasks: this.getTasks(),
      // Ajoutez d'autres appels API ici si nécessaire (ex: formations, évaluations)
    }).pipe(
      map(({ users, teams, tasks }) => {
        // Calculer les statistiques pour chaque card
        const personnelStats = this.calculatePersonnelStats(users, teams);
        const planificationStats = this.calculatePlanificationStats(tasks, teams);
        const formationStats = this.simulateFormationStats(users); // Données simulées
        const evaluationStats = this.simulateEvaluationStats(users); // Données simulées

        return {
          personnel: personnelStats,
          planification: planificationStats,
          formation: formationStats,
          evaluations: evaluationStats,
          teams: teams, // Pour les chips
        };
      }),
      catchError(this.handleError)
    );
  }

  private calculatePersonnelStats(users: User[], teams: Team[]): any {
    const activeEmployees = users.filter(u => u.isActive).length;
    const latestUsers = users
      .filter(u => !!u.createdAt)
      .sort((a, b) => new Date(b.createdAt as string | number | Date).getTime() - new Date(a.createdAt as string | number | Date).getTime())
      .slice(0, 5);
    return {
      totalEmployees: users.length,
      activeEmployees: activeEmployees,
      totalTeams: teams.length,
      latestUsers: latestUsers,
    };
  }

  private calculatePlanificationStats(tasks: Task[], teams: Team[]): any {
    const pendingTasks = tasks.filter(t => t.status === 'pending').length;
    const inProgressTasks = tasks.filter(t => t.status === 'in-progress').length;
    const completedTasks = tasks.filter(t => t.status === 'completed').length;
    const overallProgress = tasks.length > 0 ? (completedTasks / tasks.length) * 100 : 0;

    // Calcul de la charge par équipe (exemple simple)
    const teamLoad = teams.map(team => {
      const teamTasks = tasks.filter(task => task.teamId === team.id && task.status !== 'completed');
      return { teamName: team.name, load: teamTasks.length }; // 'load' pourrait être un % ou un nombre de tâches
    });

    return {
      totalTasks: tasks.length,
      pendingTasks,
      inProgressTasks,
      completedTasks,
      overallProgress: parseFloat(overallProgress.toFixed(1)),
      teamLoad
    };
  }

  // Simulation pour les données de formation
  private simulateFormationStats(users: User[]): any {
    const simulatedTrainings: any[] = [
      { id: 1, title: 'Gestion de Projet Agile', participants: 25, completed: 15, deadline: '2025-07-30' },
      { id: 2, title: 'Sécurité des Données', participants: 18, completed: 18, deadline: '2025-08-15' },
      { id: 3, title: 'Nouvelles Normes RGPD', participants: 30, completed: 10, deadline: '2025-09-01' },
    ];
    const topPerformers = users.slice(0, 3).map(u => ({ ...u, formationsCompleted: Math.floor(Math.random() * 5) + 1 })); // Simulé

    return {
      activeTrainings: simulatedTrainings.filter(t => new Date(t.deadline) > new Date()).length,
      certificationsAchieved: simulatedTrainings.reduce((sum, t) => sum + t.completed, 0),
      upcomingDeadlines: simulatedTrainings.filter(t => new Date(t.deadline) > new Date() && new Date(t.deadline) < new Date(Date.now() + 30 * 24 * 60 * 60 * 1000)).length,
      topPerformers: topPerformers,
    };
  }

  // Simulation pour les données d'évaluation
  private simulateEvaluationStats(users: User[]): any {
    const simulatedEvaluations: any[] = users.map((user, index) => ({
      id: index + 1,
      employeeName: `${user.firstName} ${user.lastName}`,
      status: Math.random() > 0.3 ? 'completed' : 'pending',
      score: Math.random() > 0.3 ? parseFloat((Math.random() * 2 + 3).toFixed(1)) : undefined, // Score entre 3 et 5
      evaluator: 'Manager X'
    }));

    const completedEvals = simulatedEvaluations.filter(e => e.status === 'completed' && e.score !== undefined);
    const averageScore = completedEvals.length > 0 ? completedEvals.reduce((sum, e) => sum + (e.score || 0), 0) / completedEvals.length : 0;
    const topRatedEmployees = users.slice(0, 3).map(u => ({
      user: u,
      rating: parseFloat((Math.random() * 1.5 + 3.5).toFixed(1)) // Simulé entre 3.5 et 5
    })).sort((a,b) => b.rating - a.rating);

    return {
      pendingEvaluations: simulatedEvaluations.filter(e => e.status === 'pending').length,
      completedEvaluations: completedEvals.length,
      averageScore: parseFloat(averageScore.toFixed(1)),
      topRatedEmployees: topRatedEmployees,
    };
  }

  private getInitials(firstName?: string, lastName?: string): string {
    const first = firstName ? firstName[0] : '';
    const last = lastName ? lastName[0] : '';
    return `${first}${last}`.toUpperCase();
  }

  private handleError(error: HttpErrorResponse) {
    console.error('API Error:', error);
    let errorMessage = 'Une erreur inconnue est survenue.';
    if (error.error instanceof ErrorEvent) {
      // Erreur côté client
      errorMessage = `Erreur : ${error.error.message}`;
    } else {
      // Erreur côté serveur
      errorMessage = `Code d\'erreur ${error.status}: ${error.message}`;
      if (error.status === 0) {
        errorMessage = 'Impossible de contacter le serveur. Vérifiez votre connexion ou l\'URL de l\'API.';
      }
    }
    return throwError(() => new Error(errorMessage));
  }
}
