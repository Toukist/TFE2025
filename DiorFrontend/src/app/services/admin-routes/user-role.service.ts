import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * Service CRUD pour la gestion des associations entre utilisateurs et rôles.
 *
 * Permet de créer, lire, mettre à jour et supprimer des associations utilisateur-rôle
 * via les endpoints de l'API REST.
 */
@Injectable({ providedIn: 'root' })
export class UserRoleService {
  private apiUrl = '/api/UserRole';

  constructor(private http: HttpClient) {}

  /**
   * Récupère toutes les associations utilisateur-rôle
   * @returns Observable contenant un tableau d'associations
   */
  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  /**
   * Crée une nouvelle association utilisateur-rôle
   * @param userRole Données de l'association à créer
   * @returns Observable contenant l'association créée
   */
  add(userRole: any): Observable<any> {
    return this.http.post(this.apiUrl, userRole);
  }

  /**
   * Met à jour une association utilisateur-rôle existante
   * @param userRole Nouvelles données de l'association
   * @returns Observable contenant l'association mise à jour
   */
  update(userRole: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${userRole.id}`, userRole);
  }

  /**
   * Supprime une association utilisateur-rôle
   * @param id Identifiant de l'association à supprimer
   * @returns Observable indiquant le succès de l'opération
   */
  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
