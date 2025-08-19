import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

export interface UserCoordonnees {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  badgePhysicalNumber?: string;
  accessId?: number;
  userAccessId?: number;
  isActive?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class CoordonneesService {
  private readonly apiUrl = 'https://localhost:7201/api';
  private readonly httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(private readonly http: HttpClient) {}

  getUserCoordonnees(): Observable<UserCoordonnees> {
    return this.http.get<UserCoordonnees>(`${this.apiUrl}/User`).pipe(
      catchError(this.handleError)
    );
  }

  getUserBadgeInfo(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/UserAccess/current`).pipe(
      catchError(this.handleError)
    );
  }

  updateUserCoordonnees(data: Partial<UserCoordonnees>): Observable<any> {
    return this.http.put(`${this.apiUrl}/User`, data, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  signalerBadgePerdu(userAccessId: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/Access/${userAccessId}/disable`, {}, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  reactiverBadge(userAccessId: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/Access/${userAccessId}/enable`, {}, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  // Optionnel : validation téléphone
  isValidPhoneFrench(value: string): boolean {
    return /^(\+33|0)[1-9](\d{2}){4}$/.test(value.replace(/\s/g, ''));
  }

  formatPhoneNumber(value: string): string {
    const digits = value.replace(/\D/g, '');
    return digits.replace(/(\d{2})(?=\d)/g, '$1 ').trim();
  }

  private handleError(error: any): Observable<never> {
    const message = error?.error?.message || 'Erreur inattendue.';
    return throwError(() => new Error(message));
  }
}
