import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AccessDto } from '../models/access.model';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AccessService {
  private readonly apiUrl = 'https://localhost:7201/api/Access';
  private readonly userAccessApiUrl = 'https://localhost:7201/api/UserAccess/by-user';

  constructor(private http: HttpClient) {}

  /** GET all accesses */
  getAll(): Observable<AccessDto[]> {
    return this.http.get<AccessDto[]>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  /** GET single access by id */
  getById(id: number): Observable<AccessDto> {
    return this.http.get<AccessDto>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  /** POST create new access */
  create(access: AccessDto): Observable<AccessDto> {
    return this.http.post<AccessDto>(this.apiUrl, access)
      .pipe(catchError(this.handleError));
  }

  /** PUT update access */
  update(id: number, access: AccessDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, access)
      .pipe(catchError(this.handleError));
  }

  /** DELETE access */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  /** PATCH disable own badge */
  disableMyBadge(): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/self-disable`, {})
      .pipe(catchError(this.handleError));
  }

  /** PATCH enable own badge */
  enableMyBadge(): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/self-enable`, {})
      .pipe(catchError(this.handleError));
  }

  /** GET the accessId assigned to a user (nouvelle version propre) */
  getUserAccess(userId: number): Observable<{ accessId: number }> {
    return this.http.get<{ accessId: number }>(`${this.userAccessApiUrl}?userId=${userId}`)
      .pipe(catchError(this.handleError));
  }

  /** Error handler */
  private handleError(error: any) {
    console.error('AccessService error:', error);
    return throwError(() => new Error('Erreur côté serveur'));
  }
}
