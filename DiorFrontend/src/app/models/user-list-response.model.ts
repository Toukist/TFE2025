import { UserDto } from './user.model';

export interface UserListResponse {
  users: UserDto[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}
