// Table : User_Role
// id: number (PK)
// roleDefinitionId: number (FK)
// userId: number (FK)
// lastEditBy?: string
// lastEditAt?: string

/**
 * Modèle User_Role - Association entre un utilisateur et un rôle
 * 
 * @property {number} id - Identifiant unique de l'association (clé primaire)
 * @property {number} userId - Référence à l'utilisateur (clé étrangère vers User)
 * @property {number} roleDefinitionId - Référence au rôle (clé étrangère vers RoleDefinition)
 * @property {string} lastEditBy - Nom de la personne ayant effectué la dernière modification
 * @property {string|Date} lastEditAt - Date de la dernière modification
 */
export interface UserRole {
  id: number;
  userId: number;
  roleDefinitionId: number;
  lastEditBy?: string;
  lastEditAt?: string | Date;
}

/**
 * Représente un rôle attribué à un utilisateur
 */
export interface UserRoleDto {
  id: number;
  roleDefinitionID: number;
  userID: number;
  lastEditBy?: string;
  lastEditAt: string | Date;
}
