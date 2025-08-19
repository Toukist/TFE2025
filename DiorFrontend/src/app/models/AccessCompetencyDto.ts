export interface AccessCompetencyDto {
  id: number;
  name: string;
  parentId?: number;
  isActive: boolean;
  createdAt: string | Date;
  createdBy: string;
  lastEditAt?: string | Date;
  lastEditBy?: string;
}
