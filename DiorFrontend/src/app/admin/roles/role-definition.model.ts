export interface RoleDefinition {
  id: number;
  name: string;
  description?: string;
  parentRoleId?: number;
  isActive: boolean;
}
