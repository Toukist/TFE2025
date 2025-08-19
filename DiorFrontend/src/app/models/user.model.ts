// src/app/models/user.model.ts

// Modèle aligné sur backend (spécification CRUD Admin)
export interface UserDto {
  // Identifiants (certains endpoints renvoient id et userId)
  id: number;                     // int64
  userId?: number;                // optionnel, harmonisation "full" vs basique

  // Etat & identité coeur (considérés toujours présents côté backend)
  isActive: boolean;
  firstName: string;
  lastName: string;
  email: string;

  // Champs optionnels backend
  userName?: string;              // nullable côté API
  phone?: string;
  teamId?: number | null;         // nullable
  teamName?: string | null;       // nullable
  badgePhysicalNumber?: number | null; // nullable (int32 swagger)
  roles?: string[];               // array de strings pour simplicité, jamais null (conversion garantie côté service)
  name?: string;                  // alias éventuel renvoyé par l'API
  Name?: string;                  // legacy (certaines vues utilisent encore Name)

  // Champs tracabilité backend éventuellement présents
  createdAt?: string | Date;
  updatedAt?: string | Date;
  lastEditAt?: string | Date | null;
  lastEditBy?: string;

  // Champs purement UI / enrichissements (jamais exigés par l'API)
  avatar?: string;
  status?: string;                // online / away / offline (simulé)
  lastConnection?: string | Date;
  role?: string;                  // rôle "principal" dérivé de roles[]
  accessCompetencies?: any[];     // collections enrichies
  password?: string;              // seulement côté formulaire création/édition
}

export interface CreateUserDto {
  userName?: string;  // rendu optionnel pour flexibilité frontend
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  teamId?: number | null;
  roles?: string[];
  isActive?: boolean; // défaut true côté front
  password?: string;  // si création requiert mot de passe
}

export interface UpdateUserDto {
  id?: number;
  userName?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  phone?: string;
  teamId?: number | null;
  roles?: string[];
  isActive?: boolean;
}
