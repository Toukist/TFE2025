export interface Tache {
  id: string;
  titre: string;
  description: string;
  priorite: 'Urgente' | 'Haute' | 'Moyenne' | 'Faible';
  assignePar?: string;
  equipement?: string;
  statut: 'A faire' | 'En cours' | 'Terminé';
  echeance?: string;
  tempsEstime?: string;
  tempsDebut?: string;
  dateFin?: string;
  type: string;
}

// Supprimé : fusion et déplacement dans operateur/
// Voir operateur/ pour la version à jour.
