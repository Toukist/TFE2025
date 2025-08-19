import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { RoleDefinition } from '../../models/role-definition.model';
import { BaseHttpService } from './base-http.service';

/**
 * Service optimisé pour la gestion des définitions de rôles
 * Étend BaseHttpService pour les opérations CRUD standardisées
 */
@Injectable({
  providedIn: 'root'
})
export class RoleDefinitionService extends BaseHttpService<RoleDefinition, RoleDefinition> {

  constructor(protected override readonly http: HttpClient) {
    super(http, 'RoleDefinition');
  }

  /**
   * Récupère uniquement les rôles actifs (non supprimés)
   */
  getActiveRoles(): Observable<RoleDefinition[]> {
    return this.getAll({ isActive: true });
  }

  /**
   * Recherche de rôles par nom
   */
  searchByName(name: string): Observable<RoleDefinition[]> {
    return this.search(name);
  }

  /**
   * Vérifie si un nom de rôle est disponible
   */
  checkRoleNameAvailability(name: string, excludeRoleId?: number): Observable<boolean> {
    let url = `${this.apiUrl}/check-name/${encodeURIComponent(name)}`;
    if (excludeRoleId) {
      url += `?excludeRoleId=${excludeRoleId}`;
    }
    
    return this.http.get<{ available: boolean }>(url, this.httpOptions).pipe(
      map(response => response.available)
    );
  }

  /**
   * Soft delete - marque un rôle comme supprimé
   */
  softDelete(id: number): Observable<RoleDefinition> {
    return this.patch(id, { isActive: false } as Partial<RoleDefinition>);
  }

  /**
   * Restaure un rôle supprimé
   */
  restore(id: number): Observable<RoleDefinition> {
    return this.patch(id, { isActive: true } as Partial<RoleDefinition>);
  }

  /**
   * Récupère les rôles avec leurs privilèges associés
   */
  getRolesWithPrivileges(): Observable<RoleDefinition[]> {
    return this.http.get<RoleDefinition[]>(`${this.apiUrl}/with-privileges`, this.httpOptions).pipe(
      map(response => this.mapResponse(response))
    );
  }

  /**
   * Récupère les statistiques des rôles
   */
  getRoleStatistics(): Observable<{
    total: number;
    active: number;
    deleted: number;
    rolesWithUsers: number;
  }> {
    return this.http.get<any>(`${this.apiUrl}/statistics`, this.httpOptions);
  }

  /**
   * Clone un rôle existant avec un nouveau nom
   */
  cloneRole(roleId: number, newName: string, newDescription?: string): Observable<RoleDefinition> {
    return this.http.post<RoleDefinition>(`${this.apiUrl}/${roleId}/clone`, {
      name: newName,
      description: newDescription
    }, this.httpOptions).pipe(
      map(response => this.mapResponse(response))
    );
  }

  /**
   * Mappe les données des rôles pour ajuster les types de dates
   */
  protected override mapResponse<R>(response: R): R {
    if (Array.isArray(response)) {
      return response.map(role => this.mapRoleDates(role)) as R;
    } else if (response && typeof response === 'object') {
      return this.mapRoleDates(response) as R;
    }
    return response;
  }

  /**
   * Convertit les dates string en objets Date si nécessaire
   */
  private mapRoleDates(role: any): any {
    if (role && role.lastEditAt && typeof role.lastEditAt === 'string') {
      return {
        ...role,
                lastEditAt: new Date(role.lastEditAt)
      };
    }
    return role;
  }
}
