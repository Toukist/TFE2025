import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';

/**
 * 🔹 ÉTAPE 6 - Service de permissions
 * Ce service propose des méthodes spécifiques pour vérifier les permissions
 * de manière centralisée sans noms codés en dur dans les composants
 */
@Injectable({
  providedIn: 'root'
})
export class PermissionService {

  constructor(private authService: AuthService) {}

  /**
   * Vérifie si l'utilisateur peut accéder au chat PartyKit
   * @returns boolean
   */
  canUsePartyKit(): boolean {
    return this.authService.hasAccess('PARTYKIT_ACCESS');
  }

  /**
   * Vérifie si l'utilisateur peut gérer les contrats
   * @returns boolean
   */
  canManageContracts(): boolean {
    return this.authService.hasAccess('CONTRACT_MANAGEMENT');
  }

  /**
   * Vérifie si l'utilisateur peut gérer les utilisateurs
   * @returns boolean
   */
  canManageUsers(): boolean {
    return this.authService.hasAccess('USER_MANAGEMENT');
  }

  /**
   * Vérifie si l'utilisateur peut gérer les rôles
   * @returns boolean
   */
  canManageRoles(): boolean {
    return this.authService.hasAccess('ROLE_MANAGEMENT');
  }

  /**
   * Vérifie si l'utilisateur est administrateur système
   * @returns boolean
   */
  isSystemAdmin(): boolean {
    return this.authService.hasAccess('SYSTEM_ADMIN');
  }

  /**
   * Vérifie si l'utilisateur peut voir les logs d'audit
   * @returns boolean
   */
  canViewAuditLogs(): boolean {
    return this.authService.hasAccess('AUDIT_LOG_VIEW');
  }

  /**
   * Vérifie si l'utilisateur peut gérer les badges
   * @returns boolean
   */
  canManageBadges(): boolean {
    return this.authService.hasAccess('BADGE_MANAGEMENT');
  }

  /**
   * Vérifie si l'utilisateur peut gérer les notifications
   * @returns boolean
   */
  canManageNotifications(): boolean {
    return this.authService.hasAccess('NOTIFICATION_MANAGEMENT');
  }

  /**
   * Vérifie si l'utilisateur a un accès administrateur général
   * @returns boolean
   */
  hasAdminAccess(): boolean {
    return this.authService.hasAccess('SYSTEM_ADMIN') || 
           this.authService.hasAccess('USER_MANAGEMENT') || 
           this.authService.hasAccess('ROLE_MANAGEMENT');
  }

  /**
   * Méthode générique pour vérifier une permission personnalisée
   * @param permissionName - Nom de la permission à vérifier
   * @returns boolean
   */
  hasCustomPermission(permissionName: string): boolean {
    return this.authService.hasAccess(permissionName);
  }
}
