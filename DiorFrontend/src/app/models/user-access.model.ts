// Modèle unique pour les associations utilisateur-badge
// À adapter si d'autres propriétés sont utilisées dans les templates ou services

export interface UserAccess {
  id: number;
  userId: number;
  badgeId: number;
  badgePhysicalNumber?: string;
  // Ajoutez ici toute propriété utilisée dans les composants/templates
  // ex: dateGranted?: string;
}
