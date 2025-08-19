// src/app/models/contract.model.ts

export interface Contract {
  id: number;
  userId: number;
  fileName?: string;
  fileUrl?: string;
  uploadedAt: string;
  uploadedBy?: string;
}
