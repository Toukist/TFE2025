import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContractService } from '../../services/contract.service';
import { Contract } from '../../models/contract.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-contract-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './contract-list.component.html',
  styleUrls: ['./contract-list.component.css']
})
export class ContractListComponent implements OnInit {  contracts: Contract[] = [];
  searchUser = '';
  selectedFile: File | null = null;
  uploading = false;
  canViewContracts: any;
  searchTerm = '';

  constructor(
    private contractService: ContractService,
    private auth: AuthService
  ) {}

  ngOnInit() {
    if (this.hasRHPrivilege()) {
      this.loadContracts();
    }
  }

  loadContracts() {
    this.contractService.getContracts().subscribe(contracts => {
      this.contracts = contracts;
    });
  }

  get filteredContracts(): Contract[] {
    const search = this.searchTerm.trim().toLowerCase();
    return search
      ? this.contracts.filter(c =>
          c.fileName?.toLowerCase().includes(search) ||
          c.userId.toString().includes(search)
        )
      : this.contracts;
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file && (file.type === 'application/pdf' || file.name.endsWith('.docx'))) {
      this.selectedFile = file;
    } else {
      this.selectedFile = null;
      alert('Seuls les fichiers PDF ou DOCX sont autorisés.');
    }
  }

  uploadContract(userId: number) {
    if (!this.selectedFile) return;
    this.uploading = true;
    this.contractService.uploadContract(this.selectedFile, userId).subscribe({
      next: () => {
        this.uploading = false;
        this.selectedFile = null;
        this.loadContracts();
      },
      error: () => {
        this.uploading = false;
        alert('Erreur lors de l\'upload.');
      }
    });
  }

  deleteContract(id: number) {
    if (confirm('Supprimer ce contrat ?')) {
      this.contractService.deleteContract(id).subscribe(() => this.loadContracts());
    }
  }

  onDelete(contractId: number) {
    this.contractService.deleteContract(contractId).subscribe(() => {
      this.contracts = this.contracts.filter(c => c.id !== contractId);
    });
  }

  onFileUpload(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input?.files && input.files.length > 0) {
      const file = input.files[0];
      this.contractService.uploadContract(file, 999).subscribe(newContract => {
        this.contracts = [...this.contracts, newContract];
      });
    }
  }

  hasRHPrivilege(): boolean {
    return this.auth.hasPrivilege('RH');
  }
}
