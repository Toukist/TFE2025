import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserRoleDto } from '../../models/user-role.model';

@Injectable({ providedIn: 'root' })
export class UserRoleService {
  private api = '/api/UserRole';
  constructor(private http: HttpClient) {}

  getByUserId(userId: number): Observable<UserRoleDto[]> {
    return this.http.get<UserRoleDto[]>(`${this.api}?userId=${userId}`);
  }
  assignRoles(userId: number, roleIds: number[]): Observable<any> {
    return this.http.post(`${this.api}/assign`, { userId, roleIds });
  }
}
