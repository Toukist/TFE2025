import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserDto } from '../../models/user.model';
import { environment } from '../../../environments/environment';
import { UserSearchParams } from '@app/models/user-search-params.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  // Utilise l'URL relative afin que le proxy de dev ou l'env prod s'applique
  private readonly apiUrl = `${environment.apiUrl}/User`;
  private readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    })
  };

  constructor(private http: HttpClient) {}

  /**
   * Récupère tous les utilisateurs
   * GET /api/User
   */
  getAll(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(this.apiUrl, this.httpOptions);
  }

  getFullUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.apiUrl}/full`, this.httpOptions);
  }

  /**
   * Recherche des utilisateurs avec filtres
   */
  getUsers(searchParams: UserSearchParams): Observable<any> {
    // Adapter selon l'API réelle (GET ou POST)
    return this.http.post<any>(`${this.apiUrl}/search`, searchParams, this.httpOptions);
  }

  /**
   * Récupère un utilisateur par son ID
   * GET /api/User/{id}
   */
  getById(id: number): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/${id}`, this.httpOptions);
  }

  /**
   * Crée un nouvel utilisateur
   * POST /api/User
   */
  create(user: UserDto): Observable<any> {
    return this.http.post<any>(this.apiUrl, user, this.httpOptions);
  }

  /**
   * Met à jour un utilisateur existant
   * PUT /api/User/{id}
   */
  update(id: number, user: UserDto): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, user, this.httpOptions);
  }

  /**
   * Supprime un utilisateur
   * DELETE /api/User/{id}
   */
  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`, this.httpOptions);
  }
}
