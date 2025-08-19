import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TeamDto } from '../models/TeamDto';
import { UserDto } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class TeamService {
private readonly apiUrl = 'https://localhost:7201/api/Team';

  constructor(private http: HttpClient) {}

  getAllTeams(): Observable<TeamDto[]> {
    return this.http.get<TeamDto[]>(this.apiUrl);
  }

  getTeamMembers(teamId: number): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.apiUrl}/${teamId}/users`);
  }
}
