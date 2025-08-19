import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

/**
 * ApiBaseService centralises HTTP calls to the backend.  All services
 * should extend this class to build CRUD methods with the configured
 * base URL.  When changing the backend host or port, update
 * environment.apiUrl instead of hard‑coding URLs in multiple places.
 */
@Injectable({ providedIn: 'root' })
export class ApiBaseService {
  protected readonly apiUrl = environment.apiUrl;

  constructor(protected http: HttpClient) {}

  /**
   * Performs a GET request to `${apiUrl}/${path}` and returns an
   * observable of the response.
   */
  protected get<T>(path: string, params?: HttpParams): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}/${path}`, { params });
  }

  /**
   * Performs a POST request to `${apiUrl}/${path}` with the given body.
   */
  protected post<T>(path: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}/${path}`, body);
  }

  /**
   * Performs a PUT request to `${apiUrl}/${path}` with the given body.
   */
  protected put<T>(path: string, body: any): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}/${path}`, body);
  }

  /**
   * Performs a DELETE request to `${apiUrl}/${path}`.
   */
  protected delete<T>(path: string): Observable<T> {
    return this.http.delete<T>(`${this.apiUrl}/${path}`);
  }
}