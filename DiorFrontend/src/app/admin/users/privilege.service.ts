import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PrivilegeDto } from '../../models/privilege.model';

@Injectable({ providedIn: 'root' })
export class PrivilegeService {
  private api = '/api/RoleDefinition_Privilege';
  constructor(private http: HttpClient) {}
  getByRoleIds(roleIds: number[]): Observable<PrivilegeDto[]> {
    return this.http.get<PrivilegeDto[]>(`${this.api}?roleIds=${roleIds.join(',')}`);
  }
}
