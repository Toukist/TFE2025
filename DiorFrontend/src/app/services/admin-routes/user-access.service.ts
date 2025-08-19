import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UserAccess } from '../../models/user-access.model';

/**
 * Service CRUD pour la gestion des associations entre utilisateurs et badges.
 * 
 * Permet de créer, lire, mettre à jour et supprimer des associations
 * entre utilisateurs et badges via les endpoints de l'API REST.
 */
@Injectable({ providedIn: 'root' })
export class UserAccessService {
  private apiUrl = '/api/User_Access';

  constructor(private http: HttpClient) {}

  /**
   * Récupère toutes les associations utilisateur-badge
   * @returns Observable contenant un tableau d'associations
   */
  getAll(): Observable<UserAccess[]> {
    return this.http.get<UserAccess[]>(this.apiUrl).pipe(catchError(this.handleError));
  }

  /**
   * Crée une nouvelle association utilisateur-badge
   * @param item Données de l'association à créer
   * @returns Observable contenant l'association créée
   */
  create(item: UserAccess): Observable<UserAccess> {
    return this.http.post<UserAccess>(this.apiUrl, item).pipe(catchError(this.handleError));
  }

  /**
   * Met à jour une association utilisateur-badge existante
   * @param id Identifiant de l'association
   * @param item Nouvelles données de l'association
   * @returns Observable contenant l'association mise à jour
   */
  update(id: number, item: UserAccess): Observable<UserAccess> {
    return this.http.put<UserAccess>(`${this.apiUrl}/${id}`, item).pipe(catchError(this.handleError));
  }

  /**
   * Supprime une association utilisateur-badge
   * @param id Identifiant de l'association à supprimer
   * @returns Observable indiquant le succès de l'opération
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(catchError(this.handleError));
  }

  /**
   * Gestion des erreurs HTTP
   * @param error Erreur HTTP reçue
   * @returns Observable contenant l'erreur
   */
  private handleError(error: HttpErrorResponse) {
    return throwError(() => error);
  }
}
