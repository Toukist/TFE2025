export interface RoleDto {
  id: number;
  name: string;
}

export interface UserFullDto {
  accessCompetencies?: { id?: number; userId?: number; accessCompetencyId: number; name?: string }[];
  userId: number;
  id: number; // Pour compatibilité UserDto
  Name?: string; // Pour compatibilité UserDto
  name: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  badgePhysicalNumber?: number | null;
  teamId?: number; // Pour compatibilité UserDto
  teamName: string;
  roles: RoleDto[];
  isActive: boolean;
  lastEditAt: string;
  lastEditBy: string;
}
