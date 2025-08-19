import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RoleDefinition } from './role-definition.model';

@Injectable({ providedIn: 'root' })
export class RoleDefinitionService {
  private apiUrl = 'https://localhost:7201/api/RoleDefinition';

  constructor(private http: HttpClient) {}

  getRoles(): Observable<RoleDefinition[]> {
    return this.http.get<RoleDefinition[]>(this.apiUrl);
  }

  getRole(id: number): Observable<RoleDefinition> {
    return this.http.get<RoleDefinition>(`${this.apiUrl}/${id}`);
  }

  createRole(role: RoleDefinition): Observable<RoleDefinition> {
    return this.http.post<RoleDefinition>(this.apiUrl, role);
  }

  updateRole(id: number, role: RoleDefinition): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, role);
  }

  deleteRole(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
