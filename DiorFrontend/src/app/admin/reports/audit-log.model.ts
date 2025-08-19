export interface AuditLog {
  id: number;
  userId: number;
  action: string;
  entity: string;
  timestamp: string;
  details: string;
}
