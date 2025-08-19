// Supprimé : fusion et déplacement dans manager/
// Voir manager/ pour la version à jour.

// Types utilisés par ManagerDashboardComponent

export interface User {
  teamId: number;
teamName: any;
phone: any;
role: any;
avatar: any;
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  status?: string;
  lastConnection?: string | Date;
  isActive?: boolean;
}

export interface Team {
  id: number;
  name: string;
  description?: string;
  membersCount?: number;
  presenceRate?: number;
}

export interface Project {
  id: number;
  title: string;
  description: string;
  teamId: number;
  teamName?: string;
  status: string;
  createdAt: string | Date;
  deadline: string | Date;
  priority?: string;
  createdBy?: string;
  progress?: number;
}

export interface DashboardStats {
  team: {
    totalMembers: number;
    activeToday: number;
    attendanceRate: number;
    onlineNow: number;
  };
  projects: {
    activeProjects: number;
    tasksInProgress: number;
    upcomingDeadlines: number;
    recentlySent: Project[];
  };
  performance: {
    productivity: number;
    completionRate: number;
    averageTaskTime: number;
    monthlyTrend: number;
  };
}

export interface PerformanceMetrics {
  // À compléter selon usage
}

export interface NavigationItem {
  path: string;
  label: string;
  icon: string;
  color: string;
  count: number;
}

export interface ProjectForm {
  title: string;
  description: string;
  teamId: number;
  priority: string;
  deadline: string | Date;
}

export interface TeamOption {
  id: number;
  name: string;
  description?: string;
  membersCount?: number;
}

export interface CreateProjectRequest extends ProjectForm {
  deadline: Date;
}

export interface ManagerDashboardData {
  stats: DashboardStats;
  // autres propriétés selon usage
}

export interface Performance {
  // À compléter selon usage
}

export interface Task {
  id: number;
  title?: string;
  description?: string;
  status?: string;
  assignedToUserId?: number;
  assignedTo?: number;
  createdByUserId?: number;
  createdAt: string | Date;
  completedAt?: string | Date;
  createdBy?: string;
  lastEditAt?: string;
  lastEditBy?: string;
  estimatedHours?: number;
  actualHours?: number;
  priority?: string;
}
