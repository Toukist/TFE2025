import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * Service CRUD pour la gestion des associations entre rôles et privilèges.
 *
 * Permet de créer, lire, mettre à jour et supprimer des associations
 * via les endpoints de l'API REST.
 */
@Injectable({ providedIn: 'root' })
export class RoleDefinitionPrivilegeService {
  private apiUrl = '/api/RoleDefinitionPrivilege';

  constructor(private http: HttpClient) {}

  /**
   * Récupère toutes les associations rôle-privilège
   * @returns Observable contenant un tableau d'associations
   */
  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  /**
   * Crée une nouvelle association rôle-privilège
   * @param rolePrivilege Données de l'association à créer
   * @returns Observable contenant l'association créée
   */
  add(rolePrivilege: any): Observable<any> {
    return this.http.post(this.apiUrl, rolePrivilege);
  }

  /**
   * Met à jour une association rôle-privilège existante
   * @param rolePrivilege Données de l'association à mettre à jour
   * @returns Observable contenant l'association mise à jour
   */
  update(rolePrivilege: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${rolePrivilege.id}`, rolePrivilege);
  }

  /**
   * Supprime une association rôle-privilège
   * @param id Identifiant de l'association à supprimer
   * @returns Observable indiquant le succès de l'opération
   */
  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
