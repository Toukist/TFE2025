// Déprécié : ancien modèle spécifique à l'admin.
// On aligne maintenant sur UserDto unique défini dans src/app/models/user.model.ts
// Pour limiter les refactors massifs immédiats, on garde un alias exporté.

export type { UserDto as User } from '../../models/user.model';

