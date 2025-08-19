import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment'; // Chemin relatif
import { Notification } from '../models/notification.model';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private apiUrl = `${environment.apiUrl}/Notification`;

  constructor(private http: HttpClient) {}

  /**
   * Récupère les notifications pour un utilisateur donné.
   */
  getByUserId(userId: number): Observable<Notification[]> {
    return this.http.get<Notification[]>(`${this.apiUrl}?userId=${userId}`)
      .pipe(catchError(this.handleError));
  }

  /**
   * Marque une notification comme lue.
   */
  markAsRead(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/read`, {})
      .pipe(catchError(this.handleError));
  }

  /**
   * Supprime une notification.
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  /**
   * Gestion des erreurs HTTP.
   */
  private handleError(error: HttpErrorResponse) {
    if (error.status === 404) {
      return throwError(() => new Error('Notification non trouvée.'));
    } else if (error.status === 0) {
      return throwError(() => new Error('Impossible de contacter le serveur.'));
    } else {
      return throwError(() => new Error(error.message));
    }
  }
}
