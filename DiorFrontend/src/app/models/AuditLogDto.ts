export interface AuditLogDto {
  id: number;
  userId: number;
  userName?: string;
  action: string;
  tableName: string;
  recordId: number;
  details?: string;
  timestamp: string;
}
