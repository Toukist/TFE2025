import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccessCompetency } from '../../models/access-competency.model';

@Injectable({ providedIn: 'root' })
export class AccessCompetencyService {
  private api = '/api/AccessCompetency';
  constructor(private http: HttpClient) {}
  getAll(): Observable<AccessCompetency[]> { return this.http.get<AccessCompetency[]>(this.api); }
  getByUserId(userId: number): Observable<AccessCompetency[]> {
    return this.http.get<AccessCompetency[]>(`${this.api}/list?userId=${userId}`);
  }
}
