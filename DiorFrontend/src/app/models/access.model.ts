/**
 * Représente un badge physique (Access)
 */
export interface AccessDto {
  /**
   * Identifiant unique de l'accès (clé primaire)
   */
  id: number;

  /**
   * Numéro physique du badge (ex : code RFID)
   */
  badgePhysicalNumber: string;

  /**
   * Statut actif/inactif du badge
   */
  isActive: boolean;

  /**
   * Date de création du badge
   */
  createdAt: string | Date;

  /**
   * Nom de la personne ayant créé l'entrée (souvent un admin)
   */
  createdBy?: string | null;
}

// Suppression du modèle UserAccessDto (remplacé par UserAccess)
