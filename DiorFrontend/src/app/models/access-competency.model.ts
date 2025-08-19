// Modèle fusionné pour AccessCompetency et UserAccessCompetency

export interface AccessCompetency {
  id: number;
  name: string;
  isActive: boolean;
  createdAt: string | Date;
  createdBy?: string;
  lastEditAt?: string | Date;
  lastEditBy?: string;
}

export interface UserAccessCompetency {
  id: number;
  userId: number;
  accessCompetencyId: number;
  createdAt: string | Date;
  createdBy?: string;
  lastEditAt?: string | Date;
  lastEditBy?: string;
}

export type AccessCompetencyName = string;
