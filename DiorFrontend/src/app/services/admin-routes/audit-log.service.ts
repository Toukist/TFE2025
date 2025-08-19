import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuditLogService {
  private apiUrl = 'https://localhost:7201/api/AuditLog';
  constructor(private http: HttpClient) {}
  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }
  // Ajoute ici d'autres méthodes si besoin (filtrage côté serveur, etc.)
}
