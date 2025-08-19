import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccessCompetency } from '../models/access-competency.model';

/**
 * 🔹 Service pour gérer les AccessCompetencies
 * Fournit les opérations CRUD pour les compétences d'accès
 */
@Injectable({
  providedIn: 'root'
})
export class AccessCompetencyService {
  private readonly apiUrl = 'https://localhost:7201/api';
  private readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    })
  };

  constructor(private http: HttpClient) {}

  /**
   * Récupère toutes les compétences d'accès disponibles
   * @returns Observable<AccessCompetency[]>
   */
  getAllAccessCompetencies(): Observable<AccessCompetency[]> {
    return this.http.get<AccessCompetency[]>(`${this.apiUrl}/AccessCompetency`, this.httpOptions);
  }

  /**
   * Récupère les compétences d'accès attribuées à un utilisateur
   * @param userId - ID de l'utilisateur
   * @returns Observable<AccessCompetency[]>
   */
  getUserAccessCompetencies(userId: number): Observable<AccessCompetency[]> {
    return this.http.get<AccessCompetency[]>(
      `${this.apiUrl}/UserAccessCompetency/list?userId=${userId}`,
      this.httpOptions
    );
  }

  /**
   * Ajoute une compétence d'accès à un utilisateur
   * @param userId - ID de l'utilisateur
   * @param competencyId - ID de la compétence d'accès
   * @returns Observable<any>
   */  addUserAccessCompetency(userId: number, competencyId: number): Observable<object> {
    const body = {
      userId: userId,
      accessCompetencyId: competencyId,
      assignedBy: 'Admin', // Vous pouvez récupérer l'utilisateur actuel
      assignedAt: new Date().toISOString()
    };

    return this.http.post(`${this.apiUrl}/UserAccessCompetency`, body, this.httpOptions);
  }

  /**
   * Supprime une compétence d'accès d'un utilisateur
   * @param id - ID de l'UserAccessCompetency à supprimer
   * @returns Observable<object>
   */
  deleteUserAccessCompetency(id: number): Observable<object> {
    return this.http.delete(`${this.apiUrl}/UserAccessCompetency/${id}`, this.httpOptions);
  }

  /**
   * Récupère une compétence d'accès spécifique par ID
   * @param id - ID de la compétence d'accès
   * @returns Observable<AccessCompetency>
   */
  getAccessCompetencyById(id: number): Observable<AccessCompetency> {
    return this.http.get<AccessCompetency>(`${this.apiUrl}/AccessCompetency/${id}`, this.httpOptions);
  }
}
