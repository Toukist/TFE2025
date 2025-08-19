import { UserDto } from './user.model';
import { RoleDefinition } from './role-definition.model';

export interface LoginResponseDto {
  user: UserDto;
  roles: RoleDefinition[];
  token: string;
}
