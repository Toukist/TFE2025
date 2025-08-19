import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { PrivilegeDto } from '../../models/privilege.model';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

/**
 * Service CRUD pour la gestion des privilèges.
 *
 * Permet de créer, lire, mettre à jour et supprimer des privilèges
 * via les endpoints de l'API REST.
 */
@Injectable({ providedIn: 'root' })
export class PrivilegeService {
  private apiUrl = '/api/Privilege';

  constructor(private http: HttpClient) {}

  /**
   * Récupère tous les privilèges
   * @returns Observable contenant un tableau de privilèges
   */
  getAll(): Observable<PrivilegeDto[]> {
    return this.http.get<PrivilegeDto[]>(this.apiUrl).pipe(catchError(this.handleError));
  }

  /**
   * Crée un nouveau privilège
   * @param privilege Données du privilège à créer
   * @returns Observable contenant le privilège créé
   */
  create(privilege: PrivilegeDto): Observable<PrivilegeDto> {
    return this.http.post<PrivilegeDto>(this.apiUrl, privilege).pipe(catchError(this.handleError));
  }

  /**
   * Met à jour un privilège existant
   * @param id Identifiant du privilège
   * @param privilege Nouvelles données du privilège
   * @returns Observable contenant le privilège mis à jour
   */
  update(id: number, privilege: PrivilegeDto): Observable<PrivilegeDto> {
    return this.http.put<PrivilegeDto>(`${this.apiUrl}/${id}`, privilege).pipe(catchError(this.handleError));
  }

  /**
   * Supprime un privilège
   * @param id Identifiant du privilège à supprimer
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
