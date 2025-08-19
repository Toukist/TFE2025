import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError, of, forkJoin } from 'rxjs';
import { tap, catchError, timeout, retry, switchMap, map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { RoleDefinition } from '../models/role-definition.model';
import { RoleService } from './admin-routes/role.service';
import { AccessCompetency, UserAccessCompetency, AccessCompetencyName } from '../models/access-competency.model';

export interface LoginRequest {
  username: string;
  password: string;
  badgePhysicalNumber: string;
}

export interface LoginResponse {
  token?: string;
  success?: boolean;
  user?: any;
  userId?: number;
}

export interface UserRoleDto {
  id: number;
  roleDefinitionID: number;
  userID: number;
  lastEditBy: string;
  lastEditAt: string;
}

export interface RoleDefinitionDto {
  id: number;
  name: string;
  description?: string;
  parentRoleId?: number;
  isActive: boolean;
  createdAt: string;
  createdBy?: string;
  lastEditAt?: string;
  lastEditBy?: string;
}

export interface CompleteRole {
  userRoleId: number;
  roleDefinitionId: number;
  roleName: string;
  roleDescription: string;
  parentRoleId: number | null;
  isActive: boolean;
  lastEditBy: string;
  lastEditAt: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = 'https://localhost:7201/api';
  private readonly loginEndpoint = `${this.apiUrl}/auth/login`;
  private readonly timeoutDuration = 30000;  private readonly maxRetries = 1;
  private isAuthenticated = false;
  private currentUser: any = null;
  private userRoles: RoleDefinition[] = [];
    // 🔹 ÉTAPE 1: Propriétés pour les AccessCompetencies
  private accessCompetencies: string[] = []; // tableau contenant les noms des accès
  private userId = 0; // à conserver depuis la réponse du login
  private readonly accessCompetenciesSubject = new BehaviorSubject<string[]>([]);

  private readonly currentUserSubject = new BehaviorSubject<any>(null);
  private readonly userRolesSubject = new BehaviorSubject<RoleDefinition[]>([]);
  private readonly isLoadingSubject = new BehaviorSubject<boolean>(false);
  private readonly currentRoleSubject = new BehaviorSubject<RoleDefinition | null>(null);
  private readonly isAuthenticatedSubject = new BehaviorSubject<boolean>(this.isLoggedIn()); // Initialiser avec l'état actuel

  public readonly currentUser$ = this.currentUserSubject.asObservable();
  public readonly userRoles$ = this.userRolesSubject.asObservable();
  public readonly isLoading$ = this.isLoadingSubject.asObservable();
  public readonly currentRole$ = this.currentRoleSubject.asObservable();
  public readonly isLoggedIn$ = this.isAuthenticatedSubject.asObservable(); // Exposer isLoggedIn$
  
  // 🔹 OPTION BONUS 7: Exposer accessCompetencies réactif dans toute l'application
  public readonly accessCompetencies$ = this.accessCompetenciesSubject.asObservable();

  // AJOUT: Méthode publique pour définir l'état de chargement
  public setLoading(isLoading: boolean): void {
    this.isLoadingSubject.next(isLoading);
  }

  private readonly httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/json' })
  };

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router,
    @Inject(PLATFORM_ID) private readonly platformId: object,
    private readonly roleService: RoleService
  ) {
    this.restoreAuthState();
  }

  loginWithCredentials(username: string, password: string): Observable<LoginResponse> {
    if (!username?.trim() || !password?.trim()) {
      return throwError(() => new Error('Nom d\'utilisateur et mot de passe requis'));
    }

    const loginData: LoginRequest = {
      username: username.trim(),
      password,
      badgePhysicalNumber: ''
    };

    this.setLoading(true);

    return this.http.post<LoginResponse>(this.loginEndpoint, loginData, this.httpOptions).pipe(
      timeout(this.timeoutDuration),
      retry(this.maxRetries),
      tap(response => this.handleLoginSuccess(response)),
      catchError(error => this.handleLoginError(error)),
      tap(() => this.setLoading(false))
    );
  }

  // AJOUT: Méthode loginWithBadge
  loginWithBadge(badgePhysicalNumber: string): Observable<LoginResponse> {
    if (!badgePhysicalNumber?.trim()) {
      return throwError(() => new Error('Numéro de badge physique requis'));
    }
    // Vérification côté client si le badge est désactivé
    const currentUser = this.getCurrentUser();
    if (currentUser?.isActive === false) {
      return throwError(() => new Error('Ce badge est désactivé et ne peut plus être utilisé.'));
    }
    const loginData: LoginRequest = {
      username: '',
      password: '',
      badgePhysicalNumber: badgePhysicalNumber.trim()
    };
    this.setLoading(true);
    return this.http.post<LoginResponse>(this.loginEndpoint, loginData, this.httpOptions).pipe(
      timeout(this.timeoutDuration),
      retry(this.maxRetries),
      tap(response => {
        // Vérification côté serveur si le badge est désactivé
        if (response.user?.isActive === false) {
          throw new Error('Ce badge est désactivé et ne peut plus être utilisé.');
        }
        this.handleLoginSuccess(response);
      }),
      catchError(error => this.handleLoginError(error)),
      tap(() => this.setLoading(false))
    );
  }
  logout(): void {
    this.roleService.clearActiveRole();
    this.isAuthenticated = false;
    this.currentUser = null;
    this.userRoles = [];
    this.accessCompetencies = []; // 🔹 Réinitialiser les AccessCompetencies
    this.userId = 0; // 🔹 Réinitialiser l'userId
    this.isAuthenticatedSubject.next(false); // Mettre à jour l'observable

    if (isPlatformBrowser(this.platformId)) {
      localStorage.clear();
    }

    this.currentUserSubject.next(null);
    this.userRolesSubject.next([]);
    this.currentRoleSubject.next(null);
    this.accessCompetenciesSubject.next([]); // 🔹 Émettre une nouvelle valeur pour les AccessCompetencies

    this.router.navigate(['/login']);
  }
  // 🔹 ÉTAPE 2: Méthode pour charger les AccessCompetencies d'un utilisateur
  /**
   * 🔹 ÉTAPE 2: Méthode fetchAccessCompetencies selon spécifications
   * @param userId - ID de l'utilisateur
   * @returns Observable<void>
   */
  fetchAccessCompetencies(userId: number): Observable<void> {
    console.log(`[AuthService] Chargement des AccessCompetencies pour l'utilisateur ${userId}`);
    
    // Étape 1: Récupérer les UserAccessCompetency de l'utilisateur
    return this.http.get<UserAccessCompetency[]>(`${this.apiUrl}/UserAccessCompetency/list?userId=${userId}`)
      .pipe(
        timeout(this.timeoutDuration),
        switchMap((userAccessCompetencies: UserAccessCompetency[]) => {
          console.log(`[AuthService] UserAccessCompetencies trouvées:`, userAccessCompetencies);
          
          if (!userAccessCompetencies || userAccessCompetencies.length === 0) {
            console.log(`[AuthService] Aucune compétence d'accès trouvée pour l'utilisateur ${userId}`);
            this.accessCompetencies = [];
            this.accessCompetenciesSubject.next([]);
            return of(void 0);
          }

          // Étape 2: Récupérer les détails de chaque AccessCompetency pour obtenir les noms
          const accessCompetencyIds = userAccessCompetencies.map(uac => uac.accessCompetencyId);
          const accessCompetencyRequests = accessCompetencyIds.map(id => 
            this.http.get<AccessCompetency>(`${this.apiUrl}/AccessCompetency/${id}`)
          );

          return forkJoin(accessCompetencyRequests).pipe(
            map((accessCompetencies: AccessCompetency[]) => {
              console.log(`[AuthService] AccessCompetencies détaillées:`, accessCompetencies);
              
              // Stocker tous les noms (name: string) dans this.accessCompetencies
              const competencyNames = accessCompetencies
                .filter(ac => ac.isActive)
                .map(ac => ac.name);
              
              this.accessCompetencies = competencyNames;
              this.accessCompetenciesSubject.next(competencyNames);
              
              // Sauvegarder dans localStorage pour persistance
              if (isPlatformBrowser(this.platformId)) {
                localStorage.setItem('accessCompetencies', JSON.stringify(competencyNames));
              }
              
              console.log(`[AuthService] AccessCompetencies stockées:`, this.accessCompetencies);
              return void 0;
            })
          );
        }),
        catchError(error => {
          console.error('[AuthService] Erreur lors du chargement des AccessCompetencies:', error);
          this.accessCompetencies = [];
          this.accessCompetenciesSubject.next([]);
          return of(void 0);
        })
      );
  }

  /**
   * 🔹 ÉTAPE 2 - Méthode publique pour vérifier l'accès
   * Vérifie si l'utilisateur a une compétence d'accès spécifique
   * @param name - Nom de la compétence d'accès (utiliser AccessCompetencyName enum)
   * @returns true si l'utilisateur a cette compétence, false sinon
   */
  public hasAccess(name: string | AccessCompetencyName): boolean {
    const hasPermission = this.accessCompetencies.includes(name.toString());
    console.log(`[AuthService] Vérification d'accès '${name}': ${hasPermission}`);
    return hasPermission;
  }

  /**
   * Récupère toutes les compétences d'accès de l'utilisateur actuel
   * @returns Tableau des noms de compétences d'accès
   */
  public getAccessCompetencies(): string[] {
    return [...this.accessCompetencies]; // Retourner une copie pour éviter les modifications externes
  }

  isLoggedIn(): boolean {
    if (!isPlatformBrowser(this.platformId)) return this.isAuthenticated;
    const token = localStorage.getItem('authToken');
    // Vérifie que le token existe et a une longueur minimale (ex: JWT = 3 parties séparées par des points)
    return this.isAuthenticated || (!!token && token.split('.').length === 3);
  }

  getCurrentUser(): any {
    if (!this.currentUser && isPlatformBrowser(this.platformId)) {
      const userString = localStorage.getItem('currentUser');
      if (userString) {
        try {
          this.currentUser = JSON.parse(userString);
        } catch (e) {
          console.error('Erreur parsing localStorage user:', e);
        }
      }
    }
    return this.currentUser;
  }

  getCurrentUserId(): number | null {
    const currentUser = this.getCurrentUser();
    return currentUser?.id || null;
  }

  updateCurrentUser(user: any): void {
    this.currentUser = user;
    localStorage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  getUserRoles(): RoleDefinition[] {
    return this.userRoles;
  }
  getCurrentRole(): RoleDefinition | null {
    const stored = localStorage.getItem('currentRoleId');
    const id = stored ? +stored : null;
    // S'assurer que userRoles est initialisé si getCurrentUser est appelé avant fetchCompleteUserRoles
    if (!this.userRoles || this.userRoles.length === 0) {
        const rolesString = localStorage.getItem('userRoles');
        if (rolesString) {
            try {
                this.userRoles = JSON.parse(rolesString);
                this.userRolesSubject.next(this.userRoles);
                console.log('[DEBUG] Rôles utilisateur chargés:', this.userRoles.map(x => x.name));
            } catch (e) {
                console.error('Erreur parsing localStorage userRoles:', e);
            }
        }
    }
    return this.userRoles.find(r => r.id === id) || null;
  }

  setCurrentRoleById(roleId: number): void {
    const found = this.userRoles.find(r => r.id === roleId);
    if (found) {
      localStorage.setItem('currentRoleId', roleId.toString());
      this.currentRoleSubject.next(found);
    }
  }
  
  // AJOUT: Méthode hasRole
  public hasRole(roleName: string): boolean {
    const lowerRoleName = roleName.toLowerCase();
    // S'assurer que userRoles est initialisé
     if (!this.userRoles || this.userRoles.length === 0) {
        const rolesString = localStorage.getItem('userRoles');
        if (rolesString) {
            try {
                this.userRoles = JSON.parse(rolesString);
            } catch (e) {
                console.error('Erreur parsing localStorage userRoles:', e);
                return false;
            }
        } else {
            return false;
        }
    }
    return this.userRoles.some(role => role.name.toLowerCase() === lowerRoleName);
  }

  // AJOUT: Méthode hasAnyRole
  public hasAnyRole(roleNames: string[]): boolean {
    const lowerRoleNames = roleNames.map(name => name.toLowerCase());
    // S'assurer que userRoles est initialisé
    if (!this.userRoles || this.userRoles.length === 0) {
        const rolesString = localStorage.getItem('userRoles');
        if (rolesString) {
            try {
                this.userRoles = JSON.parse(rolesString);
            } catch (e) {
                console.error('Erreur parsing localStorage userRoles:', e);
                return false;
            }
        } else {
            return false;
        }
    }
    return this.userRoles.some(role => lowerRoleNames.includes(role.name.toLowerCase()));
  }

  // Vérifie si l'utilisateur a un privilège donné (exemple simple, à adapter selon ta logique)
  public hasPrivilege(privilege: string): boolean {
    // Exemple : stocker les privilèges dans currentUser.privileges: string[]
    const user = this.getCurrentUser();
    if (!user || !user.privileges) return false;
    return user.privileges.includes(privilege);
  }

  fetchCompleteUserRoles(userId: number): Promise<CompleteRole[]> {
    return Promise.all([
      this.http.get<UserRoleDto[]>(`${this.apiUrl}/UserRole?userId=${userId}`).toPromise(),
      this.http.get<RoleDefinitionDto[]>(`${this.apiUrl}/RoleDefinition`).toPromise()
    ]).then(([userRolesResponse, roleDefinitionsResponse]) => {
      if (!userRolesResponse || !roleDefinitionsResponse) {
        console.warn('Réponse manquante de userRoles ou roleDefinitions');
        this.userRolesSubject.next([]);
        return [];
      }

      // Filtrer explicitement sur le userID (sécurité supplémentaire)
      const filteredUserRoles = userRolesResponse.filter(userRole => userRole.userID === userId);

      // Croiser userRoles filtrés et roleDefinitions actifs pour ne garder que les rôles attribués à l'utilisateur
      const completeRoles: CompleteRole[] = filteredUserRoles
        .map(userRole => {
          const roleDef = roleDefinitionsResponse.find(rd => rd.id === userRole.roleDefinitionID && rd.isActive);
          if (!roleDef) return null;
          return {
            userRoleId: userRole.id,
            roleDefinitionId: roleDef.id,
            roleName: roleDef.name,
            roleDescription: roleDef.description || '',
            parentRoleId: roleDef.parentRoleId ?? null,
            isActive: roleDef.isActive,
            lastEditBy: userRole.lastEditBy,
            lastEditAt: userRole.lastEditAt
          };
        })
        .filter((role): role is CompleteRole => !!role);

      // La liste des rôles pour le front ne doit contenir que les rôles attribués à l'utilisateur
      const roleDefinitions: RoleDefinition[] = completeRoles.map(role => ({
        id: role.roleDefinitionId,
        name: role.roleName,
        description: role.roleDescription,
        parentRoleId: role.parentRoleId,
        isActive: role.isActive ?? true,
        createdAt: new Date().toISOString(),
        createdBy: 'system',
        lastEditBy: role.lastEditBy ?? 'system',
        lastEditAt: new Date().toISOString()
      }));      // Éliminer les doublons éventuels (par id)
      const uniqueRoleDefinitions = roleDefinitions.filter((role, index, self) =>
        index === self.findIndex(r => r.id === role.id)
      );

      this.userRoles = uniqueRoleDefinitions;
      this.userRolesSubject.next(uniqueRoleDefinitions);
      console.log('[DEBUG] Rôles utilisateur mis à jour:', uniqueRoleDefinitions.map(x => x.name));
      if (isPlatformBrowser(this.platformId)) {
        localStorage.setItem('userRoles', JSON.stringify(uniqueRoleDefinitions));
      }

      return completeRoles;
    }).catch(err => {
      console.error('Erreur lors de fetchCompleteUserRoles:', err);
      this.userRolesSubject.next([]);
      return [];
    });
  }
  public performRoleBasedRedirection(): void {
    console.log('[AuthService] Début redirection basée sur les rôles');
    
    // Ne prendre en compte que les rôles actifs
    const activeRoles = (this.userRoles || []).filter(r => r.isActive);
    console.log('[AuthService] Rôles actifs trouvés:', activeRoles);
    
    if (!activeRoles || activeRoles.length === 0) {
      console.log('[AuthService] Aucun rôle actif, redirection vers login');
      this.router.navigate(['/login']);
      return;
    }
    
    if (activeRoles.length === 1) {
      const roleName = activeRoles[0].name;
      const targetRoute = this.getRoleRoute(roleName);
      console.log(`[AuthService] Un seul rôle: ${roleName}, redirection vers: ${targetRoute}`);
      
      this.setCurrentRoleById(activeRoles[0].id);
      this.router.navigate([targetRoute]);
    } else {
      console.log('[AuthService] Plusieurs rôles, redirection vers choix-role');
      this.router.navigate(['/choix-role']);
    }
  }
  getRoleRoute(roleName: string): string {
    switch (roleName) {
      case 'admin': 
      case 'administrateur': 
        return '/admin/dashboard';
      case 'manager': 
      case 'gestionnaire': 
        return '/manager';
      case 'operateur': 
        return '/operateur';
      case 'rh': 
      case 'hr': 
        return '/rh';
      default: 
        console.warn(`Role inconnu: ${roleName}, redirection vers login`);
        return '/login';
    }
  }
  private handleLoginSuccess(response: LoginResponse): void {
    console.log('handleLoginSuccess - Réponse backend:', response);
    // Correction : accepte une réponse avec seulement token et user
    const hasToken = !!response.token;
    const hasUser = !!response.user;
    const userId = response.userId || response.user?.id;
    const isSuccess = response.success !== undefined ? response.success : hasToken;
    if (isSuccess && hasToken && hasUser && userId) {
      this.isAuthenticated = true;
      this.currentUser = response.user;
      this.isAuthenticatedSubject.next(true); // Mettre à jour l'observable

      if (isPlatformBrowser(this.platformId)) {
        if (response.token) {
          localStorage.setItem('authToken', response.token);
        }
        if (response.user) {
          localStorage.setItem('currentUser', JSON.stringify(response.user));
        }
        if (userId) {
          localStorage.setItem('currentUserId', userId.toString());
        }
      }

      this.currentUserSubject.next(this.currentUser);      if (userId) {
        // 🔹 ÉTAPE 4: Charger les rôles ET les AccessCompetencies avant redirection
        this.userId = userId; // Conserver l'userId
        
        this.fetchCompleteUserRoles(userId).then(() => {
          // Attendre que le chargement des droits soit terminé avant de rediriger vers le dashboard
          this.fetchAccessCompetencies(userId).subscribe({
            next: () => {
              console.log('[AuthService] AccessCompetencies chargées avec succès');
              this.performRoleBasedRedirection();
            },
            error: (error: any) => {
              console.error('[AuthService] Erreur lors du chargement des AccessCompetencies:', error);
              // Continuer même si le chargement des AccessCompetencies échoue
              this.performRoleBasedRedirection();
            }
          });
        });
      } else {
        this.logout();
      }
    }
  }

  private handleLoginError(error: HttpErrorResponse): Observable<never> {
    let message = 'Erreur de connexion';
    if (error.status === 401) message = 'Identifiants incorrects';
    if (error.status === 403) message = 'Accès interdit';
    return throwError(() => new Error(message));
  }
  // 🔹 AJOUT: Rendez la méthode restoreAuthState publique pour permettre son appel depuis les composants
  public restoreAuthState(): void {
    if (isPlatformBrowser(this.platformId)) {
      const token = localStorage.getItem('authToken');
      if (token) {
        this.isAuthenticated = true;
        this.isAuthenticatedSubject.next(true); // Mettre à jour l'observable
        const userString = localStorage.getItem('currentUser');
        if (userString) {
          try {
            this.currentUser = JSON.parse(userString);
            // Correction : émettre l'utilisateur restauré dans le BehaviorSubject
            this.currentUserSubject.next(this.currentUser);
          } catch (e) {
            console.error('Erreur parsing localStorage user:', e);
          }
        }

        // 🔹 AJOUT: Restaurer les AccessCompetencies depuis localStorage
        const accessCompetenciesString = localStorage.getItem('accessCompetencies');
        if (accessCompetenciesString) {
          try {
            this.accessCompetencies = JSON.parse(accessCompetenciesString);
            this.accessCompetenciesSubject.next(this.accessCompetencies);
            console.log('[AuthService] AccessCompetencies restaurées depuis localStorage:', this.accessCompetencies);
          } catch (e) {
            console.error('Erreur parsing localStorage accessCompetencies:', e);
            this.accessCompetencies = [];
          }
        }
      }
    }
  }
}
