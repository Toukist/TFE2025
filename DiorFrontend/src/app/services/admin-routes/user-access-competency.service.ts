import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AccessCompetency } from '../../models/access-competency.model';

/**
 * Service CRUD pour la gestion des associations entre badges et zones/salles.
 *
 * Permet de créer, lire, mettre à jour et supprimer des associations
 * entre badges utilisateurs et zones/salles accessibles via les endpoints de l'API REST.
 */
@Injectable({ providedIn: 'root' })
export class UserAccessCompetencyService {
  private apiUrl = environment.apiUrl + '/UserAccessCompetency';

  constructor(private http: HttpClient) {}

  /**
   * Récupère toutes les associations badge-zone
   * @returns Observable contenant un tableau d'associations
   */
  getAll(): Observable<AccessCompetency[]> {
    return this.http.get<AccessCompetency[]>(this.apiUrl).pipe(catchError(this.handleError));
  }

  /**
   * Récupère les associations d'access competencies pour un utilisateur spécifique
   * @param userId Identifiant de l'utilisateur
   * @returns Observable contenant un tableau d'associations pour l'utilisateur
   */
  getByUserId(userId: number): Observable<AccessCompetency[]> {
    return this.http.get<AccessCompetency[]>(`${this.apiUrl}/user/${userId}`).pipe(catchError(this.handleError));
  }
  /**
   * Crée une nouvelle association badge-zone
   * @param item Données de l'association à créer
   * @returns Observable indiquant le succès de l'opération
   */
  create(item: AccessCompetency): Observable<AccessCompetency> {
    return this.http.post<AccessCompetency>(this.apiUrl, item).pipe(catchError(this.handleError));
  }

  /**
   * Met à jour une association badge-zone existante
   * @param id Identifiant de l'association
   * @param item Nouvelles données de l'association
   * @returns Observable contenant l'association mise à jour
   */
  update(id: number, item: AccessCompetency): Observable<AccessCompetency> {
    return this.http.put<AccessCompetency>(`${this.apiUrl}/${id}`, item).pipe(catchError(this.handleError));
  }

  /**
   * Supprime une association badge-zone
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
