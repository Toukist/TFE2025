import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuditLogDto } from '../models/AuditLogDto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {
  private readonly apiUrl = 'https://localhost:7201/api/AuditLog';


  constructor(private http: HttpClient) {}

  getAll(): Observable<AuditLogDto[]> {
    return this.http.get<AuditLogDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<AuditLogDto> {
    return this.http.get<AuditLogDto>(`${this.apiUrl}/${id}`);
  }

  search(userId?: number, action?: string): Observable<AuditLogDto[]> {
    const params: string[] = [];
    if (userId) params.push(`userId=${userId}`);
    if (action) params.push(`action=${action}`);
    return this.http.get<AuditLogDto[]>(`${this.apiUrl}/search?${params.join('&')}`);
  }
}
