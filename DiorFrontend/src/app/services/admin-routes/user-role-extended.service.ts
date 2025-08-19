import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserRole } from '../../models/user-role.model';
import { RoleDefinition } from '../../models/role-definition.model';

@Injectable({
  providedIn: 'root'
})
export class UserRoleService {
  private apiUrl = 'https://localhost:7201/api';

  constructor(private http: HttpClient) {}

  getUserRoles(userId: number): Observable<UserRole[]> {
    return this.http.get<UserRole[]>(`${this.apiUrl}/user-roles/user/${userId}`);
  }

  getUserRoleDefinitions(userId: number): Observable<RoleDefinition[]> {
    return this.http.get<RoleDefinition[]>(`${this.apiUrl}/users/${userId}/roles`);
  }

  assignRoleToUser(userId: number, roleDefinitionId: number): Observable<UserRole> {
    return this.http.post<UserRole>(`${this.apiUrl}/user-roles`, {
      userId,
      roleDefinitionId
    });
  }

  removeRoleFromUser(userRoleId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/user-roles/${userRoleId}`);
  }
}
