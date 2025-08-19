// Table : Privilege
// id: number (PK)
// name: string
// description?: string
// isConfigurableRead: boolean
// isConfigurableDelete: boolean
// isConfigurableAdd: boolean
// isConfigurableModify: boolean
// isConfigurableStatus: boolean
// isConfigurableExecution: boolean
// lastEditBy?: string
// lastEditAt?: string

/**
 * Modèle Privilege - Représente un privilège dans le système
 * 
 * @property {number} id - Identifiant unique du privilège (clé primaire)
 * @property {string} name - Nom du privilège
 * @property {string} description - Description détaillée du privilège
 */
export interface PrivilegeDto {
  id: number; // PK, obligatoire
  name: string; // obligatoire
  description: string; // nullable
  isConfigurableRead: boolean; // obligatoire
  isConfigurableDelete: boolean; // obligatoire
  isConfigurableAdd: boolean; // obligatoire
  isConfigurableModify: boolean; // obligatoire
  isConfigurableStatus: boolean; // obligatoire
  isConfigurableExecution: boolean; // obligatoire
  lastEditBy: string; // nullable
  lastEditAt?: string | Date; // nullable
}

/**
 * Alias métier si besoin d’un type distinct (sinon utiliser PrivilegeDto partout)
 */
export type Privilege = PrivilegeDto;
