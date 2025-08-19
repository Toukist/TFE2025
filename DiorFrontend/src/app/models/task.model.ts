// src/app/models/task.model.ts

export interface Task {
  id: number;
  title?: string;
  description?: string;
  status?: string;
  assignedToUserId: number;
  createdByUserId: number;
  createdAt: string;
  createdBy?: string;
  lastEditAt?: string;
  lastEditBy?: string;
  teamId?: number;
}
