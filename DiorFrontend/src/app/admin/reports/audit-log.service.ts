import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuditLogDto } from '../../models/AuditLogDto';

@Injectable({ providedIn: 'root' })
export class AuditLogService {
  private apiUrl = 'https://localhost:7201/api/AuditLog';

  constructor(private http: HttpClient) {}

  // Appel avec filtres optionnels
  getLogs(params?: { userId?: number; startDate?: string; endDate?: string }): Observable<AuditLogDto[]> {
    let httpParams = new HttpParams();
    if (params) {
      if (params.userId) httpParams = httpParams.set('userId', params.userId.toString());
      if (params.startDate) httpParams = httpParams.set('startDate', params.startDate);
      if (params.endDate) httpParams = httpParams.set('endDate', params.endDate);
    }
    return this.http.get<AuditLogDto[]>(this.apiUrl, { params: httpParams });
  }

  // Appel direct pour tout récupérer
  getAll(): Observable<AuditLogDto[]> {
    return this.getLogs();
  }
}
