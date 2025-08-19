/**
 * Modèle ProjetDto - Interface TypeScript correspondant au DTO backend
 * 
 * @property {number} id - Identifiant unique du projet
 * @property {string} nom - Nom du projet
 * @property {string} description - Description détaillée du projet
 * @property {Date | string} dateDebut - Date de début du projet
 * @property {Date | string} dateFin - Date de fin du projet
 * @property {number} teamId - ID de l'équipe assignée
 * @property {string} teamName - Nom de l'équipe (propriété calculée)
 * @property {Date | string} createdAt - Date de création
 * @property {string} createdBy - Créé par
 * @property {Date | string} lastEditAt - Date de dernière modification
 * @property {string} lastEditBy - Dernière modification par
 */
export interface ProjetDto {
  id: number;
  nom: string;
  description?: string;
  dateDebut?: Date | string;
  dateFin?: Date | string;
  teamId: number;
  teamName?: string;
  createdAt: Date | string;
  createdBy: string;
  lastEditAt?: Date | string;
  lastEditBy?: string;
  progress?: number; // Ajout de la propriété optionnelle progress
}

/**
 * Interface pour la création/édition de projet (formulaire)
 */
export interface CreateProjetRequest {
  nom: string;
  description?: string;
  dateDebut?: Date | string;
  dateFin?: Date | string;
  teamId: number;
}

/**
 * Interface pour la réponse de l'API
 */
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  error?: string;
}

/**
 * Énumération des statuts de projet (pour extension future)
 */
export enum StatutProjet {
  PLANIFIE = 'PLANIFIE',
  EN_COURS = 'EN_COURS',
  TERMINE = 'TERMINE',
  SUSPENDU = 'SUSPENDU',
  ANNULE = 'ANNULE'
}

/**
 * Interface étendue pour les données de projet avec calculs métier
 */
export interface ProjetDetaille extends ProjetDto {
  dureeEnJours?: number;
  progression?: number; // Pourcentage 0-100
  statut?: StatutProjet;
  estEnRetard?: boolean;
  joursRestants?: number;
}

/**
 * Interface pour les filtres de recherche
 */
export interface ProjetFilters {
  teamId?: number;
  statut?: StatutProjet;
  dateDebutMin?: Date;
  dateDebutMax?: Date;
  searchTerm?: string;
}
