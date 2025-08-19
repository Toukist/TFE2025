// Table : RoleDefinition
// id: number (PK)
// name: string
// description?: string
// isDeleted: boolean
// lastEditBy?: string
// lastEditAt?: string

/**
 * Modèle RoleDefinition - Représente un rôle dans le système
 * 
 * @property {number} id - Identifiant unique du rôle (clé primaire)
 * @property {string} name - Nom du rôle
 * @property {string} description - Description détaillée du rôle
 * @property {boolean} isDeleted - Indique si le rôle est supprimé
 * @property {string} lastEditBy - Nom de la personne ayant effectué la dernière modification
 * @property {string|Date} lastEditAt - Date de la dernière modification
 */
export interface RoleDefinition {
  id: number;
  name: string;
  description?: string;
  parentRoleId?: number | null;
  isActive: boolean;
  isDeleted?: boolean; // Pour compatibilité éventuelle
  createdAt: Date | string;
  createdBy: string;
  lastEditAt?: Date | string;
  lastEditBy?: string;
}

/**
 * Association entre un rôle et un privilège
 */
export interface RoleDefinitionPrivilege {
  id: number;
  roleDefinitionId: number;
  privilegeId: number;
  isActive: boolean;
  isRead: boolean;
  isAdd: boolean;
  isModify: boolean;
  isStatus: boolean;
  isExecution: boolean;
  createdAt?: Date | string;
  createdBy?: string;
  lastEditAt?: Date | string;
  lastEditBy?: string;
}
