import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RoleDefinition } from '../../models/role-definition.model';

@Injectable({ providedIn: 'root' })
export class RoleService {
  private api = '/api/RoleDefinition';
  constructor(private http: HttpClient) {}
  getAll(): Observable<RoleDefinition[]> { return this.http.get<RoleDefinition[]>(this.api); }
}
