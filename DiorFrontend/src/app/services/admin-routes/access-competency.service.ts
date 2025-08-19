import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { AccessCompetency } from '../../models/access-competency.model';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

/**
 * Service CRUD pour la gestion des zones/salles accessibles par badge.
 *
 * Permet de créer, lire, mettre à jour et supprimer des zones/salles
 * via les endpoints de l'API REST.
 */
@Injectable({ providedIn: 'root' })
export class AccessCompetencyService {
  private apiUrl = environment.apiUrl + '/AccessCompetency';

  constructor(private http: HttpClient) {}

  /**
   * Récupère toutes les zones/salles
   * @returns Observable contenant un tableau de zones/salles
   */
  getAll(): Observable<AccessCompetency[]> {
    return this.http.get<AccessCompetency[]>(this.apiUrl).pipe(catchError(this.handleError));
  }

  /**
   * Crée une nouvelle zone/salle
   * @param item Données de la zone/salle à créer
   * @returns Observable contenant la zone/salle créée
   */
  create(item: AccessCompetency): Observable<AccessCompetency> {
    return this.http.post<AccessCompetency>(this.apiUrl, item).pipe(catchError(this.handleError));
  }

  /**
   * Met à jour une zone/salle existante
   * @param id Identifiant de la zone/salle
   * @param item Nouvelles données de la zone/salle
   * @returns Observable contenant la zone/salle mise à jour
   */
  update(id: number, item: AccessCompetency): Observable<AccessCompetency> {
    return this.http.put<AccessCompetency>(`${this.apiUrl}/${id}`, item).pipe(catchError(this.handleError));
  }

  /**
   * Supprime une zone/salle
   * @param id Identifiant de la zone/salle à supprimer
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
