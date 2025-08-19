/**
 * Types et interfaces communs pour l'application DiorSystem
 */

// Types de base
export type EntityId = number;
export type Timestamp = string | Date;

// Interface de base pour les entités avec suivi des modifications
export interface BaseEntity {
  id: EntityId;
  lastEditBy?: string;
  lastEditAt?: Timestamp;
}

// Interface pour les entités supprimables (soft delete)
export interface SoftDeletableEntity extends BaseEntity {
  isDeleted: boolean;
}

// Types pour les états de chargement
export type LoadingState = 'idle' | 'loading' | 'success' | 'error';

// Interface pour les réponses d'API avec gestion des erreurs
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
  message?: string;
}

// Types pour la pagination
export interface PaginationParams {
  page: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
}

// Types pour les filtres
export interface FilterParams {
  searchTerm?: string;  dateFrom?: Date;
  dateTo?: Date;
  isActive?: boolean;
  [key: string]: unknown;
}

// Types pour les permissions et rôles
export type PermissionLevel = 'read' | 'write' | 'admin';
export type RoleName = 'admin' | 'manager' | 'editor' | 'user' | 'viewer';

// Types pour les notifications
export type NotificationType = 'success' | 'error' | 'warning' | 'info';

export interface Notification {
  id: string;
  type: NotificationType;
  title: string;
  message: string;
  timestamp: Date;
  read: boolean;
}

// Types pour les thèmes et l'UI
export type ThemeMode = 'light' | 'dark' | 'auto';
export type LanguageCode = 'fr' | 'en' | 'de' | 'nl';

// Interface pour les préférences utilisateur
export interface UserPreferences {
  theme: ThemeMode;
  language: LanguageCode;
  notifications: {
    email: boolean;
    push: boolean;
    inApp: boolean;
  };
  pagination: {
    defaultPageSize: number;
  };
}

// Types pour les événements d'audit
export interface AuditEvent {
  id: EntityId;
  userId: EntityId;
  action: string;
  entityType: string;  entityId: EntityId;
  oldValue?: unknown;
  newValue?: unknown;
  timestamp: Date;
  ipAddress?: string;
  userAgent?: string;
}

// Types pour la validation
export interface ValidationError {
  field: string;
  message: string;
  code?: string;
}

export interface ValidationResult {
  isValid: boolean;
  errors: ValidationError[];
}

// Utilitaires de type
export type Optional<T, K extends keyof T> = Omit<T, K> & Partial<Pick<T, K>>;
export type RequiredFields<T, K extends keyof T> = T & Required<Pick<T, K>>;
export type DeepPartial<T> = {
  [P in keyof T]?: T[P] extends object ? DeepPartial<T[P]> : T[P];
};

// Types pour les formulaires
export interface FormField<T = unknown> {
  value: T;
  valid: boolean;
  touched: boolean;
  errors: ValidationError[];
}

export interface FormState<T extends Record<string, unknown>> {
  fields: { [K in keyof T]: FormField<T[K]> };
  isValid: boolean;
  isSubmitting: boolean;
  submitErrors: ValidationError[];
}
