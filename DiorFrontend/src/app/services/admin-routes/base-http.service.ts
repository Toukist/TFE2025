import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { map, catchError, timeout, retry, finalize } from 'rxjs/operators';
import { 
  PaginatedResponse, 
  PaginationParams, 
  FilterParams,
  LoadingState,
  BaseEntity,
  EntityId
} from '../../models/common.types';

/**
 * Service de base abstrait pour toutes les opérations HTTP
 * Fournit des méthodes CRUD standardisées et la gestion d'erreurs
 */
@Injectable()
export abstract class BaseHttpService<T extends BaseEntity, TDto = T> {
  protected readonly baseUrl = 'https://localhost:7201/api';
  protected readonly timeoutDuration = 15000; // 15 secondes
  protected readonly maxRetries = 2;

  // État de chargement
  private readonly loadingStateSubject = new BehaviorSubject<LoadingState>('idle');
  public readonly loadingState$ = this.loadingStateSubject.asObservable();

  // Headers HTTP par défaut
  protected readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'X-Requested-With': 'XMLHttpRequest'
    })
  };

  constructor(
    protected readonly http: HttpClient,
    protected readonly resourcePath: string
  ) {}

  /**
   * URL complète de la ressource
   */
  protected get apiUrl(): string {
    return `${this.baseUrl}/${this.resourcePath}`;
  }

  /**
   * Récupère tous les éléments
   */
  getAll(params?: FilterParams): Observable<T[]> {
    this.setLoadingState('loading');
    
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach(key => {
        const value = params[key];
        if (value !== undefined && value !== null) {
          httpParams = httpParams.set(key, value.toString());
        }
      });
    }

    return this.http.get<T[]>(this.apiUrl, { 
      ...this.httpOptions,
      params: httpParams
    }).pipe(
      timeout(this.timeoutDuration),
      retry(this.maxRetries),
      map(response => this.mapResponse(response)),
      catchError(error => this.handleError(error)),
      finalize(() => this.setLoadingState('idle'))
    );
  }

  /**
   * Récupère les éléments avec pagination
   */
  getAllPaginated(pagination: PaginationParams, filters?: FilterParams): Observable<PaginatedResponse<T>> {
    this.setLoadingState('loading');
    
    let httpParams = new HttpParams()
      .set('page', pagination.page.toString())
      .set('pageSize', pagination.pageSize.toString());

    if (pagination.sortBy) {
      httpParams = httpParams.set('sortBy', pagination.sortBy);
    }
    if (pagination.sortOrder) {
      httpParams = httpParams.set('sortOrder', pagination.sortOrder);
    }

    if (filters) {
      Object.keys(filters).forEach(key => {
        const value = filters[key];
        if (value !== undefined && value !== null) {
          httpParams = httpParams.set(key, value.toString());
        }
      });
    }

    return this.http.get<PaginatedResponse<T>>(`${this.apiUrl}/paginated`, {
      ...this.httpOptions,
      params: httpParams
    }).pipe(
      timeout(this.timeoutDuration),
      retry(this.maxRetries),
      map(response => this.mapPaginatedResponse(response)),
      catchError(error => this.handleError(error)),
      finalize(() => this.setLoadingState('idle'))
    );
  }

  /**
   * Récupère un élément par son ID
   */
  getById(id: EntityId): Observable<T> {
    this.setLoadingState('loading');

    return this.http.get<T>(`${this.apiUrl}/${id}`, this.httpOptions).pipe(
      timeout(this.timeoutDuration),
      retry(this.maxRetries),
      map(response => this.mapResponse(response)),
      catchError(error => this.handleError(error)),
      finalize(() => this.setLoadingState('idle'))
    );
  }

  /**
   * Crée un nouvel élément
   */
  create(item: TDto): Observable<T> {
    this.setLoadingState('loading');

    return this.http.post<T>(this.apiUrl, item, this.httpOptions).pipe(
      timeout(this.timeoutDuration),
      map(response => this.mapResponse(response)),
      catchError(error => this.handleError(error)),
      finalize(() => this.setLoadingState('idle'))
    );
  }

  /**
   * Met à jour un élément existant
   */
  update(id: EntityId, item: TDto): Observable<T> {
    this.setLoadingState('loading');

    return this.http.put<T>(`${this.apiUrl}/${id}`, item, this.httpOptions).pipe(
      timeout(this.timeoutDuration),
      map(response => this.mapResponse(response)),
      catchError(error => this.handleError(error)),
      finalize(() => this.setLoadingState('idle'))
    );
  }

  /**
   * Met à jour partiellement un élément
   */
  patch(id: EntityId, changes: Partial<TDto>): Observable<T> {
    this.setLoadingState('loading');

    return this.http.patch<T>(`${this.apiUrl}/${id}`, changes, this.httpOptions).pipe(
      timeout(this.timeoutDuration),
      map(response => this.mapResponse(response)),
      catchError(error => this.handleError(error)),
      finalize(() => this.setLoadingState('idle'))
    );
  }

  /**
   * Supprime un élément
   */
  delete(id: EntityId): Observable<void> {
    this.setLoadingState('loading');

    return this.http.delete<void>(`${this.apiUrl}/${id}`, this.httpOptions).pipe(
      timeout(this.timeoutDuration),
      catchError(error => this.handleError(error)),
      finalize(() => this.setLoadingState('idle'))
    );
  }

  /**
   * Supprime plusieurs éléments
   */
  deleteMultiple(ids: EntityId[]): Observable<void> {
    this.setLoadingState('loading');

    return this.http.request<void>('DELETE', this.apiUrl, {
      ...this.httpOptions,
      body: { ids }
    }).pipe(
      timeout(this.timeoutDuration),
      catchError(error => this.handleError(error)),
      finalize(() => this.setLoadingState('idle'))
    );
  }

  /**
   * Recherche d'éléments avec terme de recherche
   */
  search(searchTerm: string, pagination?: PaginationParams): Observable<T[]> {
    this.setLoadingState('loading');

    let httpParams = new HttpParams().set('q', searchTerm);
    
    if (pagination) {
      httpParams = httpParams
        .set('page', pagination.page.toString())
        .set('pageSize', pagination.pageSize.toString());
    }

    return this.http.get<T[]>(`${this.apiUrl}/search`, {
      ...this.httpOptions,
      params: httpParams
    }).pipe(
      timeout(this.timeoutDuration),
      retry(this.maxRetries),
      map(response => this.mapResponse(response)),
      catchError(error => this.handleError(error)),
      finalize(() => this.setLoadingState('idle'))
    );
  }

  /**
   * Vérifie si un élément existe
   */
  exists(id: EntityId): Observable<boolean> {
    return this.http.head(`${this.apiUrl}/${id}`, {
      observe: 'response'
    }).pipe(
      map(response => response.status === 200),
      catchError(() => throwError(() => false))
    );
  }

  /**
   * Compte le nombre total d'éléments
   */
  count(filters?: FilterParams): Observable<number> {
    let httpParams = new HttpParams();
    
    if (filters) {
      Object.keys(filters).forEach(key => {
        const value = filters[key];
        if (value !== undefined && value !== null) {
          httpParams = httpParams.set(key, value.toString());
        }
      });
    }

    return this.http.get<{ count: number }>(`${this.apiUrl}/count`, {
      ...this.httpOptions,
      params: httpParams
    }).pipe(
      map(response => response.count),
      catchError(error => this.handleError(error))
    );
  }

  // === MÉTHODES PROTÉGÉES À SURCHARGER ===

  /**
   * Mappe la réponse de l'API (à surcharger si nécessaire)
   */
  protected mapResponse<R>(response: R): R {
    return response;
  }

  /**
   * Mappe la réponse paginée (à surcharger si nécessaire)
   */
  protected mapPaginatedResponse(response: PaginatedResponse<T>): PaginatedResponse<T> {
    return response;
  }

  /**
   * Gestion des erreurs HTTP
   */
  protected handleError(error: HttpErrorResponse): Observable<never> {
    console.error(`Erreur API ${this.resourcePath}:`, error);
    
    this.setLoadingState('error');
    
    let errorMessage = 'Une erreur est survenue';
    
    if (error.status === 0) {
      errorMessage = 'Impossible de joindre le serveur';
    } else if (error.status === 400) {
      errorMessage = 'Requête invalide';
    } else if (error.status === 401) {
      errorMessage = 'Non autorisé';
    } else if (error.status === 403) {
      errorMessage = 'Accès interdit';
    } else if (error.status === 404) {
      errorMessage = 'Ressource non trouvée';
    } else if (error.status === 409) {
      errorMessage = 'Conflit de données';
    } else if (error.status >= 500) {
      errorMessage = 'Erreur serveur';
    }

    return throwError(() => new Error(errorMessage));
  }

  /**
   * Définit l'état de chargement
   */
  private setLoadingState(state: LoadingState): void {
    this.loadingStateSubject.next(state);
  }

  /**
   * Réinitialise l'état du service
   */
  public resetState(): void {
    this.setLoadingState('idle');
  }

  /**
   * Indique si le service est en cours de chargement
   */
  public get isLoading(): boolean {
    return this.loadingStateSubject.value === 'loading';
  }
}
