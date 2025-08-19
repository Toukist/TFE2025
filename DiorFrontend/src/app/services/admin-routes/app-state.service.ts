import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { 
  NotificationType, 
  Notification, 
  UserPreferences, 
  ThemeMode, 
  LanguageCode 
} from '../../models/common.types';

/**
 * État global de l'application
 */
export interface AppState {
  // États de chargement
  isLoading: boolean;
  loadingOperations: Set<string>;
  
  // Notifications
  notifications: Notification[];
  
  // Préférences utilisateur
  userPreferences: UserPreferences;
  
  // État de l'interface
  sidebarCollapsed: boolean;
  mobileMenuOpen: boolean;
  
  // Connectivité
  isOnline: boolean;
  
  // Métadonnées de l'application
  version: string;
  buildDate: Date;
}

/**
 * Service de gestion d'état global de l'application
 * Centralise la gestion des états transversaux
 */
@Injectable({
  providedIn: 'root'
})
export class AppStateService {
  
  // État initial
  private readonly initialState: AppState = {
    isLoading: false,
    loadingOperations: new Set(),
    notifications: [],
    userPreferences: {
      theme: 'auto',
      language: 'fr',
      notifications: {
        email: true,
        push: true,
        inApp: true
      },
      pagination: {
        defaultPageSize: 20
      }
    },
    sidebarCollapsed: false,
    mobileMenuOpen: false,
    isOnline: navigator.onLine,
    version: '1.0.0',
    buildDate: new Date()
  };

  // Subjects pour l'état
  private readonly stateSubject = new BehaviorSubject<AppState>(this.initialState);
  private readonly loadingOperationsSubject = new BehaviorSubject<Set<string>>(new Set());
  private readonly notificationsSubject = new BehaviorSubject<Notification[]>([]);

  // Observables publics
  public readonly state$ = this.stateSubject.asObservable();
  public readonly isLoading$ = this.loadingOperationsSubject.pipe(
    map(operations => operations.size > 0)
  );
  public readonly notifications$ = this.notificationsSubject.asObservable();

  constructor() {
    this.initializeNetworkStatusListener();
    this.loadUserPreferences();
  }

  // === GESTION DU CHARGEMENT ===

  /**
   * Démarre une opération de chargement
   */
  startLoading(operationId: string): void {
    const operations = new Set(this.loadingOperationsSubject.value);
    operations.add(operationId);
    this.loadingOperationsSubject.next(operations);
    this.updateState({ isLoading: operations.size > 0 });
  }

  /**
   * Termine une opération de chargement
   */
  stopLoading(operationId: string): void {
    const operations = new Set(this.loadingOperationsSubject.value);
    operations.delete(operationId);
    this.loadingOperationsSubject.next(operations);
    this.updateState({ isLoading: operations.size > 0 });
  }

  /**
   * Vérifie si une opération spécifique est en cours
   */
  isOperationLoading(operationId: string): Observable<boolean> {
    return this.loadingOperationsSubject.pipe(
      map(operations => operations.has(operationId))
    );
  }

  // === GESTION DES NOTIFICATIONS ===

  /**
   * Ajoute une notification
   */
  addNotification(
    type: NotificationType, 
    title: string, 
    message: string, 
    autoRemove = true
  ): string {
    const notification: Notification = {
      id: this.generateNotificationId(),
      type,
      title,
      message,
      timestamp: new Date(),
      read: false
    };

    const notifications = [...this.notificationsSubject.value, notification];
    this.notificationsSubject.next(notifications);
    this.updateState({ notifications });

    // Auto-suppression après 5 secondes pour les notifications non-erreur
    if (autoRemove && type !== 'error') {
      setTimeout(() => this.removeNotification(notification.id), 5000);
    }

    return notification.id;
  }

  /**
   * Interface générique pour afficher une notification
   */
  showNotification(notification: { type: NotificationType; title?: string; message: string }): string {
    const title = notification.title || this.getDefaultTitle(notification.type);
    return this.addNotification(notification.type, title, notification.message);
  }

  /**
   * Supprime une notification
   */
  removeNotification(notificationId: string): void {
    const notifications = this.notificationsSubject.value.filter(n => n.id !== notificationId);
    this.notificationsSubject.next(notifications);
    this.updateState({ notifications });
  }

  /**
   * Marque une notification comme lue
   */
  markNotificationAsRead(notificationId: string): void {
    const notifications = this.notificationsSubject.value.map(n => 
      n.id === notificationId ? { ...n, read: true } : n
    );
    this.notificationsSubject.next(notifications);
    this.updateState({ notifications });
  }

  /**
   * Supprime toutes les notifications
   */
  clearAllNotifications(): void {
    this.notificationsSubject.next([]);
    this.updateState({ notifications: [] });
  }

  // === GESTION DES PRÉFÉRENCES ===

  /**
   * Met à jour les préférences utilisateur
   */
  updateUserPreferences(preferences: Partial<UserPreferences>): void {
    const currentPreferences = this.stateSubject.value.userPreferences;
    const newPreferences = { ...currentPreferences, ...preferences };
    
    this.updateState({ userPreferences: newPreferences });
    this.saveUserPreferences(newPreferences);
  }

  /**
   * Change le thème de l'application
   */
  setTheme(theme: ThemeMode): void {
    this.updateUserPreferences({ theme });
    this.applyTheme(theme);
  }

  /**
   * Change la langue de l'application
   */
  setLanguage(language: LanguageCode): void {
    this.updateUserPreferences({ language });
  }

  // === GESTION DE L'INTERFACE ===

  /**
   * Bascule l'état de la sidebar
   */
  toggleSidebar(): void {
    const currentState = this.stateSubject.value;
    this.updateState({ sidebarCollapsed: !currentState.sidebarCollapsed });
  }

  /**
   * Ouvre/ferme le menu mobile
   */
  toggleMobileMenu(): void {
    const currentState = this.stateSubject.value;
    this.updateState({ mobileMenuOpen: !currentState.mobileMenuOpen });
  }

  /**
   * Ferme le menu mobile
   */
  closeMobileMenu(): void {
    this.updateState({ mobileMenuOpen: false });
  }

  // === MÉTHODES UTILITAIRES ===

  /**
   * Notifications raccourcies
   */
  showSuccess(title: string, message: string): string {
    return this.addNotification('success', title, message);
  }

  showError(title: string, message: string): string {
    return this.addNotification('error', title, message, false);
  }

  showWarning(title: string, message: string): string {
    return this.addNotification('warning', title, message);
  }

  showInfo(title: string, message: string): string {
    return this.addNotification('info', title, message);
  }

  /**
   * Réinitialise l'état de l'application
   */
  resetState(): void {
    this.stateSubject.next(this.initialState);
    this.loadingOperationsSubject.next(new Set());
    this.notificationsSubject.next([]);
  }

  /**
   * Récupère l'état actuel
   */
  getCurrentState(): AppState {
    return this.stateSubject.value;
  }

  // === MÉTHODES PRIVÉES ===

  /**
   * Met à jour l'état global
   */
  private updateState(partialState: Partial<AppState>): void {
    const currentState = this.stateSubject.value;
    const newState = { ...currentState, ...partialState };
    this.stateSubject.next(newState);
  }

  /**
   * Génère un ID unique pour les notifications
   */
  private generateNotificationId(): string {
    return `notification-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  }

  /**
   * Retourne un titre par défaut selon le type de notification
   */
  private getDefaultTitle(type: NotificationType): string {
    const titles = {
      success: 'Succès',
      error: 'Erreur',
      warning: 'Attention',
      info: 'Information'
    };
    return titles[type];
  }

  /**
   * Initialise l'écoute du statut réseau
   */
  private initializeNetworkStatusListener(): void {
    window.addEventListener('online', () => {
      this.updateState({ isOnline: true });
      this.showSuccess('Connexion', 'Connexion internet rétablie');
    });

    window.addEventListener('offline', () => {
      this.updateState({ isOnline: false });
      this.showWarning('Connexion', 'Connexion internet perdue');
    });
  }

  /**
   * Charge les préférences utilisateur depuis le localStorage
   */
  private loadUserPreferences(): void {
    try {
      const saved = localStorage.getItem('userPreferences');
      if (saved) {
        const preferences = JSON.parse(saved);
        this.updateState({ userPreferences: { ...this.initialState.userPreferences, ...preferences } });
        this.applyTheme(preferences.theme || 'auto');
      }
    } catch (error) {
      console.error('Erreur lors du chargement des préférences:', error);
    }
  }

  /**
   * Sauvegarde les préférences utilisateur dans le localStorage
   */
  private saveUserPreferences(preferences: UserPreferences): void {
    try {
      localStorage.setItem('userPreferences', JSON.stringify(preferences));
    } catch (error) {
      console.error('Erreur lors de la sauvegarde des préférences:', error);
    }
  }

  /**
   * Applique le thème à l'interface
   */
  private applyTheme(theme: ThemeMode): void {
    const root = document.documentElement;
    
    if (theme === 'auto') {
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      root.setAttribute('data-theme', prefersDark ? 'dark' : 'light');
    } else {
      root.setAttribute('data-theme', theme);
    }
  }
}
