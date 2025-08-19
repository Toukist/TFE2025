import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { RoleDefinition } from '../../models/role-definition.model';

interface User {
isActive: any;
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  isActivate: boolean;
  lastEditAt: Date;
}

interface Task {
  id: number;
  title: string;
  status: string;
  assignedToUserId: number;
  createdByUserId: number;
  createdAt: Date;
  dueDate?: Date;
}

interface Contract {
  id: number;
  userId: number;
  fileName: string;
  uploadedAt: Date;
  description: string;
}

interface Notification {
  id: number;
  userId: number;
  type: string;
  message: string;
  isRead: boolean;
  createdAt: Date;
}

interface ReportData {
  users: User[];
  tasks: Task[];
  roles: RoleDefinition[];
  contracts: Contract[];
  notifications: Notification[];
}

interface UserReport {
  totalUsers: number;
  activeUsers: number;
  inactiveUsers: number;
  usersByRole: { [key: string]: number };
  recentUsers: User[];
}

interface TaskReport {
  totalTasks: number;
  tasksByStatus: { [key: string]: number };
  overdueTasks: number;
  tasksThisMonth: number;
  topAssignees: { name: string; count: number }[];
}

interface SystemReport {
  totalContracts: number;
  unreadNotifications: number;
  totalNotifications: number;
  recentActivity: any[];
}

// Correction du type pour l'activité récente
interface RecentActivity {
  type: string;
  user: string;
  action: string;
  date: Date | string;
}

@Component({
  selector: 'app-admin-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="min-h-screen bg-gray-50 p-6">
      <!-- Header cohérent avec dashboard admin -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 mb-2">Rapports</h1>
        <p class="text-gray-600">Consultation des logs et rapports</p>
      </div>

      <!-- Loading State -->
      @if (loading()) {
        <div class="flex justify-center items-center h-64">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
        </div>
      } @else {

      <!-- Filtres et Actions -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6">
        <div class="flex flex-wrap items-center gap-4">
          <div class="flex-1 min-w-64">
            <label class="block text-sm font-medium text-gray-700 mb-2">Période de rapport</label>
            <select 
              [(ngModel)]="selectedPeriod" 
              (ngModelChange)="onPeriodChange()"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500">
              <option value="today">Aujourd'hui</option>
              <option value="week">Cette semaine</option>
              <option value="month">Ce mois</option>
              <option value="quarter">Ce trimestre</option>
              <option value="year">Cette année</option>
              <option value="all">Toutes les données</option>
            </select>
          </div>
          
          <div class="flex gap-2">
            <button 
              (click)="refreshReports()"
              class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
              <svg class="h-4 w-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
              </svg>
              Actualiser
            </button>
            
            <button 
              (click)="exportReport()"
              class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
              <svg class="h-4 w-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
              Exporter Excel
            </button>
          </div>
        </div>
      </div>

      <!-- Vue d'ensemble - KPIs principaux -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <div class="flex items-center">
            <div class="flex-1">
              <p class="text-sm font-medium text-gray-600">Utilisateurs Actifs</p>
              <p class="text-2xl font-bold text-blue-600">{{ userReport()?.activeUsers || 0 }}</p>
              <p class="text-xs text-gray-500 mt-1">
                sur {{ userReport()?.totalUsers || 0 }} total
              </p>
            </div>
            <div class="p-3 bg-blue-100 rounded-full">
              <svg class="h-6 w-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0z"></path>
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <div class="flex items-center">
            <div class="flex-1">
              <p class="text-sm font-medium text-gray-600">Tâches Actives</p>
              <p class="text-2xl font-bold text-green-600">{{ getActiveTasks() }}</p>
              <p class="text-xs text-gray-500 mt-1">
                {{ taskReport()?.overdueTasks || 0 }} en retard
              </p>
            </div>
            <div class="p-3 bg-green-100 rounded-full">
              <svg class="h-6 w-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4"></path>
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <div class="flex items-center">
            <div class="flex-1">
              <p class="text-sm font-medium text-gray-600">Notifications</p>
              <p class="text-2xl font-bold text-orange-600">{{ systemReport()?.unreadNotifications || 0 }}</p>
              <p class="text-xs text-gray-500 mt-1">
                non lues sur {{ systemReport()?.totalNotifications || 0 }}
              </p>
            </div>
            <div class="p-3 bg-orange-100 rounded-full">
              <svg class="h-6 w-6 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-5 5v-5zM4 19h6v-2H4v2zM4 15h8v-2H4v2zM4 11h10V9H4v2z"></path>
              </svg>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <div class="flex items-center">
            <div class="flex-1">
              <p class="text-sm font-medium text-gray-600">Contrats</p>
              <p class="text-2xl font-bold text-purple-600">{{ systemReport()?.totalContracts || 0 }}</p>
              <p class="text-xs text-gray-500 mt-1">
                documents stockés
              </p>
            </div>
            <div class="p-3 bg-purple-100 rounded-full">
              <svg class="h-6 w-6 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Rapports détaillés -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        
        <!-- Rapport Utilisateurs -->
        <div class="bg-white rounded-lg shadow-sm border border-gray-200">
          <div class="p-6 border-b border-gray-200">
            <h3 class="text-lg font-semibold text-gray-900 flex items-center">
              <svg class="h-5 w-5 mr-2 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.25 2.25 0 11-4.5 0 2.25 2.25 0 014.5 0z"></path>
              </svg>
              Rapport Utilisateurs
            </h3>
          </div>
          <div class="p-6">
            <!-- Répartition par rôle -->
            <div class="mb-6">
              <h4 class="text-sm font-medium text-gray-700 mb-3">Répartition par rôle</h4>
              <div class="space-y-2">
                @for (roleItem of getRoleDistribution(); track roleItem.role) {
                  <div class="flex justify-between items-center">
                    <span class="text-sm text-gray-600">{{ roleItem.role }}</span>
                    <div class="flex items-center">
                      <div class="w-24 bg-gray-200 rounded-full h-2 mr-3">
                        <div 
                          class="bg-blue-600 h-2 rounded-full" 
                          [style.width.%]="roleItem.percentage">
                        </div>
                      </div>
                      <span class="text-sm font-medium text-gray-900 w-8">{{ roleItem.count }}</span>
                    </div>
                  </div>
                }
              </div>
            </div>

            <!-- Utilisateurs récents -->
            <div>
              <h4 class="text-sm font-medium text-gray-700 mb-3">Utilisateurs récents</h4>
              <div class="space-y-2">
                @for (user of userReport()?.recentUsers?.slice(0, 5); track user.id) {
                  <div class="flex items-center justify-between text-sm">
                    <div class="flex items-center">
                      <div class="w-8 h-8 bg-gray-300 rounded-full flex items-center justify-center mr-3">
                        <span class="text-xs font-medium">{{ user.firstName.charAt(0) }}{{ user.lastName.charAt(0) }}</span>
                      </div>
                      <div>
                        <p class="font-medium text-gray-900">{{ user.firstName }} {{ user.lastName }}</p>
                        <p class="text-gray-500">{{ user.email }}</p>
                      </div>
                    </div>
                    <span 
                      class="px-2 py-1 text-xs rounded-full"
                      [class]="user.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                      {{ user.isActive ? 'Actif' : 'Inactif' }}
                    </span>
                  </div>
                }
              </div>
            </div>
          </div>
        </div>

        <!-- Rapport Tâches -->
        <div class="bg-white rounded-lg shadow-sm border border-gray-200">
          <div class="p-6 border-b border-gray-200">
            <h3 class="text-lg font-semibold text-gray-900 flex items-center">
              <svg class="h-5 w-5 mr-2 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4"></path>
              </svg>
              Rapport Tâches
            </h3>
          </div>
          <div class="p-6">
            <!-- Répartition par statut -->
            <div class="mb-6">
              <h4 class="text-sm font-medium text-gray-700 mb-3">Répartition par statut</h4>
              <div class="grid grid-cols-2 gap-4">
                @for (statusItem of getTaskStatusDistribution(); track statusItem.status) {
                  <div class="text-center">
                    <div 
                      class="w-16 h-16 rounded-full mx-auto mb-2 flex items-center justify-center text-white font-bold"
                      [class]="getStatusColor(statusItem.status)">
                      {{ statusItem.count }}
                    </div>
                    <p class="text-xs text-gray-600">{{ statusItem.status }}</p>
                  </div>
                }
              </div>
            </div>

            <!-- Top assignés -->
            <div>
              <h4 class="text-sm font-medium text-gray-700 mb-3">Opérateurs les plus sollicités</h4>
              <div class="space-y-2">
                @for (assignee of taskReport()?.topAssignees?.slice(0, 5); track assignee.name) {
                  <div class="flex justify-between items-center">
                    <span class="text-sm text-gray-600">{{ assignee.name }}</span>
                    <div class="flex items-center">
                      <div class="w-16 bg-gray-200 rounded-full h-2 mr-3">
                        <div 
                          class="bg-green-600 h-2 rounded-full" 
                          [style.width.%]="(assignee.count / getMaxAssigneeCount()) * 100">
                        </div>
                      </div>
                      <span class="text-sm font-medium text-gray-900 w-6">{{ assignee.count }}</span>
                    </div>
                  </div>
                }
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Activité récente -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200">
        <div class="p-6 border-b border-gray-200">
          <h3 class="text-lg font-semibold text-gray-900 flex items-center">
            <svg class="h-5 w-5 mr-2 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            Activité Récente
          </h3>
        </div>
        <div class="p-6">
          <div class="space-y-4">
            @for (activity of getRecentActivity(); track $index) {
              <div class="flex items-start space-x-3">
                <div 
                  class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center"
                  [class]="getActivityColor(activity.type)">
                  <svg class="h-4 w-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" [attr.d]="getActivityIcon(activity.type)"></path>
                  </svg>
                </div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm text-gray-900">
                    <span class="font-medium">{{ activity.user }}</span>
                    {{ activity.action }}
                  </p>
                  <p class="text-xs text-gray-500">{{ formatDate(activity.date) }}</p>
                </div>
              </div>
            }
          </div>
          
          @if (getRecentActivity().length === 0) {
            <div class="text-center text-gray-500 py-8">
              <svg class="h-12 w-12 mx-auto mb-2 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"></path>
              </svg>
              <p>Aucune activité récente trouvée</p>
            </div>
          }
        </div>
      </div>

      <!-- Section Notifications système -->
      <div class="mt-8 bg-white rounded-lg shadow-sm border border-gray-200">
        <div class="p-6 border-b border-gray-200">
          <h3 class="text-lg font-semibold text-gray-900 flex items-center">
            <svg class="h-5 w-5 mr-2 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-5 5v-5zM4 19h6v-2H4v2zM4 15h8v-2H4v2zM4 11h10V9H4v2z"></path>
            </svg>
            Notifications Non Lues
          </h3>
        </div>
        <div class="p-6">
          <div class="space-y-3">
            @for (notification of getUnreadNotifications(); track notification.id) {
              <div class="flex items-start space-x-3 p-3 bg-gray-50 rounded-lg">
                <div 
                  class="flex-shrink-0 w-6 h-6 rounded-full flex items-center justify-center"
                  [class]="getNotificationColor(notification.type)">
                  <span class="text-xs text-white font-bold">!</span>
                </div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm text-gray-900">{{ notification.message }}</p>
                  <p class="text-xs text-gray-500">{{ formatDate(notification.createdAt) }} - Type: {{ notification.type }}</p>
                </div>
              </div>
            } @empty {
              <div class="text-center text-gray-500 py-8">
                <svg class="h-12 w-12 mx-auto mb-2 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
                </svg>
                <p>Toutes les notifications ont été lues</p>
              </div>
            }
          </div>
        </div>
      </div>

      }
    </div>
  `
})
export class AdminReportsComponent implements OnInit {
  private readonly http = inject(HttpClient);

  // Signals
  loading = signal(false);
  error = signal<string | null>(null);
  reportData = signal<ReportData | null>(null);
  selectedPeriod = signal('month');

  // Computed reports
  userReport = computed(() => {
    const data = this.reportData();
    if (!data?.users) return null;

    const totalUsers = data.users.length;
    const activeUsers = data.users.filter(u => u.isActive).length;
    const inactiveUsers = totalUsers - activeUsers;
    
    // Simulation répartition par rôle (à adapter selon votre API)
    const usersByRole = {
      'Operateur': Math.floor(activeUsers * 0.6),
      'Manager': Math.floor(activeUsers * 0.2),
      'Admin': Math.floor(activeUsers * 0.1),
      'Maintenance': Math.floor(activeUsers * 0.1)
    };

    const recentUsers = data.users
      .sort((a, b) => new Date(b.lastEditAt).getTime() - new Date(a.lastEditAt).getTime())
      .slice(0, 10);

    return {
      totalUsers,
      activeUsers,
      inactiveUsers,
      usersByRole,
      recentUsers
    };
  });

  taskReport = computed(() => {
    const data = this.reportData();
    if (!data?.tasks || !data?.users) return null;

    const totalTasks = data.tasks.length;
    const now = new Date();
    const monthStart = new Date(now.getFullYear(), now.getMonth(), 1);
    
    const tasksByStatus = data.tasks.reduce((acc, task) => {
      acc[task.status] = (acc[task.status] || 0) + 1;
      return acc;
    }, {} as { [key: string]: number });

    const overdueTasks = data.tasks.filter(task => 
      task.dueDate && new Date(task.dueDate) < now && task.status !== 'Terminé'
    ).length;

    const tasksThisMonth = data.tasks.filter(task => 
      new Date(task.createdAt) >= monthStart
    ).length;

    // Top assignés
    const assigneeCounts = data.tasks.reduce((acc, task) => {
      const user = data.users.find(u => u.id === task.assignedToUserId);
      if (user) {
        const name = `${user.firstName} ${user.lastName}`;
        acc[name] = (acc[name] || 0) + 1;
      }
      return acc;
    }, {} as { [key: string]: number });

    const topAssignees = Object.entries(assigneeCounts)
      .map(([name, count]) => ({ name, count }))
      .sort((a, b) => b.count - a.count)
      .slice(0, 10);

    return {
      totalTasks,
      tasksByStatus,
      overdueTasks,
      tasksThisMonth,
      topAssignees
    };
  });

  systemReport = computed(() => {
    const data = this.reportData();
    if (!data) return null;

    return {
      totalContracts: data.contracts?.length || 0,
      unreadNotifications: data.notifications?.filter(n => !n.isRead).length || 0,
      totalNotifications: data.notifications?.length || 0,
      recentActivity: [] // à compléter selon besoin
    };
  });

  ngOnInit(): void {
    this.loadReports();
  }

  loadReports(): void {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      users: this.http.get<User[]>('/api/user'),
      tasks: this.http.get<Task[]>('/api/Task'),
      roles: this.http.get<RoleDefinition[]>('/api/RoleDefinition'),
      contracts: this.http.get<Contract[]>('/api/Contract'),
      notifications: this.http.get<Notification[]>('/api/NOTIFICATION')
    }).subscribe({
      next: (data) => {
        this.reportData.set(data);
        this.loading.set(false);
      },
      error: (error) => {
        this.error.set('Erreur lors du chargement des rapports');
        this.loading.set(false);
        console.error('Erreur:', error);
      }
    });
  }

  refreshReports(): void {
    this.loadReports();
  }

  onPeriodChange(): void {
    this.loadReports();
  }

  exportReport(): void {
    const data = this.reportData();
    if (!data) return;
    const reportContent = {
      'Utilisateurs': data.users.length,
      'Tâches': data.tasks.length,
      'Contrats': data.contracts.length,
      'Notifications': data.notifications.length,
      'Période': this.selectedPeriod(),
      'Généré le': new Date().toISOString()
    };
    console.log('Export rapport:', reportContent);
    alert('Fonctionnalité d\'export Excel à implémenter');
  }

  // Méthodes utilitaires à compléter selon le template
  getActiveTasks() {
    return this.taskReport()?.tasksByStatus['En cours'] || 0;
  }
  getRoleDistribution() {
    const ur = this.userReport();
    if (!ur) return [];
    const total = ur.activeUsers || 1;
    return Object.entries(ur.usersByRole).map(([role, count]) => ({
      role,
      count,
      percentage: Math.round((count / total) * 100)
    }));
  }
  getTaskStatusDistribution() {
    const tr = this.taskReport();
    if (!tr) return [];
    const total = tr.totalTasks || 1;
    return Object.entries(tr.tasksByStatus).map(([status, count]) => ({
      status,
      count,
      percentage: Math.round((count / total) * 100)
    }));
  }
  getStatusColor(status: string) {
    switch (status) {
      case 'En cours': return 'bg-green-600';
      case 'Terminé': return 'bg-blue-600';
      case 'En attente': return 'bg-yellow-500';
      case 'En retard': return 'bg-red-600';
      default: return 'bg-gray-400';
    }
  }
  getMaxAssigneeCount() {
    const tr = this.taskReport();
    if (!tr || !tr.topAssignees.length) return 1;
    return tr.topAssignees[0].count;
  }
  getRecentActivity(): RecentActivity[] {
    const data = this.reportData();
    if (!data) return [];
    // Fusionner tâches récentes et notifications récentes
    const taskActivities: RecentActivity[] = data.tasks.slice(-5).map(t => ({
      type: 'task',
      user: this.getUserNameById(t.assignedToUserId),
      action: `a reçu la tâche "${t.title}"`,
      date: t.createdAt
    }));
    const notificationActivities: RecentActivity[] = data.notifications.slice(-5).map(n => ({
      type: 'notification',
      user: this.getUserNameById(n.userId),
      action: `a reçu une notification : ${n.message}`,
      date: n.createdAt
    }));
    return [...taskActivities, ...notificationActivities].sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()).slice(0, 8);
  }
  getNotificationColor(type: string): string {
    switch (type) {
      case 'ALERT': return 'bg-red-600';
      case 'INFO': return 'bg-blue-600';
      case 'WARNING': return 'bg-yellow-500';
      default: return 'bg-gray-400';
    }
  }
  getActivityColor(type: string) {
    switch (type) {
      case 'login': return 'bg-blue-600';
      case 'task': return 'bg-green-600';
      case 'contract': return 'bg-purple-600';
      case 'notification': return 'bg-orange-600';
      default: return 'bg-gray-400';
    }
  }
  getActivityIcon(type: string) {
    switch (type) {
      case 'login': return 'M12 8v4l3 3';
      case 'task': return 'M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2';
      case 'contract': return 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z';
      case 'notification': return 'M15 17h5l-5 5v-5zM4 19h6v-2H4v2zM4 15h8v-2H4v2zM4 11h10V9H4v2z';
      default: return '';
    }
  }
  getUnreadNotifications() {
    const data = this.reportData();
    if (!data) return [];
    return data.notifications?.filter(n => !n.isRead) || [];
  }
  formatDate(date: Date | string) {
    return new Date(date).toLocaleString('fr-FR');
  }
  getUserNameById(userId: number): string {
    const data = this.reportData();
    if (!data?.users) return 'Utilisateur';
    const user = data.users.find(u => u.id === userId);
    return user ? `${user.firstName} ${user.lastName}` : 'Utilisateur';
  }
}
