export interface PerformanceData {
  globalMetrics: {
    productivity: number;
    efficiency: number;
    satisfaction: number;
    completionRate: number;
  };
  dailyStats: {
    tasksCompleted: number;
    tasksInProgress: number;
    averageTime: number;
    qualityScore: number;
  };
  alerts: Alert[];
  topPerformers: TopPerformer[];
}

export interface Alert {
  type: 'success' | 'warning' | 'info' | 'error';
  message: string;
}

export interface TopPerformer {
  name: string;
  score: number;
  department: string;
}

export interface TeamProductivity {
  teamName: string;
  productivity: number;
  tasksCompleted: number;
  efficiency: number;
  trend: 'up' | 'down' | 'stable';
}

export interface MonthlyTrend {
  month: string;
  productivity: number;
  efficiency: number;
  satisfaction: number;
}
