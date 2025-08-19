import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Contract } from '../models/contract.model';

@Injectable({ providedIn: 'root' })
export class ContractService {
  // Version locale de développement : contrats RH fictifs
  private testContracts: Contract[] = [
    {
      id: 1,
      userId: 101,
      fileName: 'contrat_test_1.pdf',
      fileUrl: 'assets/test-contracts/contrat_test_1.pdf',
      uploadedAt: '2025-06-10T09:00:00Z'
    },
    {
      id: 2,
      userId: 102,
      fileName: 'contrat_test_2.docx',
      fileUrl: 'assets/test-contracts/contrat_test_2.docx',
      uploadedAt: '2025-06-10T10:30:00Z'
    }
  ];

  getContracts(): Observable<Contract[]> {
    return of(this.testContracts);
  }

  uploadContract(file: File, userId: number): Observable<Contract> {
    const fakeContract: Contract = {
      id: Math.floor(Math.random() * 1000),
      userId,
      fileName: file.name,
      fileUrl: `assets/test-contracts/${file.name}`,
      uploadedAt: new Date().toISOString()
    };
    this.testContracts.push(fakeContract);
    return of(fakeContract);
  }

  deleteContract(contractId: number): Observable<void> {
    this.testContracts = this.testContracts.filter(c => c.id !== contractId);
    return of(void 0);
  }
}
