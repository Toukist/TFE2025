import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserAccess } from '../../models/user-access.model';

@Injectable({ providedIn: 'root' })
export class UserAccessService {
  private api = '/api/UserAccess';
  constructor(private http: HttpClient) {}

  getByUserId(userId: number): Observable<UserAccess[]> {
    return this.http.get<UserAccess[]>(`${this.api}?userId=${userId}`);
  }
  assignAccess(userId: number, accessIds: number[]): Observable<any> {
    return this.http.post(`${this.api}/assign`, { userId, accessIds });
  }
}
