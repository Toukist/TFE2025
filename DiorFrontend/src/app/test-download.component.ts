import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DataTableUtilsService } from './shared/services/data-table-utils.service';

@Component({
  selector: 'app-test-download',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6 max-w-4xl mx-auto">
      <h1 class="text-2xl font-bold mb-6">Test des fonctionnalités de téléchargement</h1>
      
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div class="bg-white p-6 rounded-lg shadow">
          <h2 class="text-lg font-semibold mb-4">Export CSV</h2>
          <p class="text-gray-600 mb-4">Exporter les données de test au format CSV</p>
          <button 
            class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
            (click)="exportCSV()">
            Télécharger CSV
          </button>
        </div>

        <div class="bg-white p-6 rounded-lg shadow">
          <h2 class="text-lg font-semibold mb-4">Export Excel</h2>
          <p class="text-gray-600 mb-4">Exporter les données de test au format Excel</p>
          <button 
            class="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
            (click)="exportExcel()">
            Télécharger Excel
          </button>
        </div>

        <div class="bg-white p-6 rounded-lg shadow">
          <h2 class="text-lg font-semibold mb-4">Export PDF</h2>
          <p class="text-gray-600 mb-4">Exporter les données de test au format PDF</p>
          <button 
            class="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
            (click)="exportPDF()">
            Télécharger PDF
          </button>
        </div>

        <div class="bg-white p-6 rounded-lg shadow">
          <h2 class="text-lg font-semibold mb-4">Rapport Multi-feuilles</h2>
          <p class="text-gray-600 mb-4">Exporter un rapport Excel avec plusieurs feuilles</p>
          <button 
            class="bg-purple-600 text-white px-4 py-2 rounded hover:bg-purple-700"
            (click)="exportMultiSheet()">
            Télécharger Rapport
          </button>
        </div>
      </div>

      <div class="mt-8 bg-gray-50 p-4 rounded-lg">
        <h3 class="font-semibold mb-2">Données de test :</h3>
        <div class="overflow-x-auto">
          <table class="min-w-full table-auto">
            <thead>
              <tr class="bg-gray-200">
                <th class="px-4 py-2 text-left">ID</th>
                <th class="px-4 py-2 text-left">Nom</th>
                <th class="px-4 py-2 text-left">Email</th>
                <th class="px-4 py-2 text-left">Date</th>
                <th class="px-4 py-2 text-left">Statut</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let item of testData" class="border-b">
                <td class="px-4 py-2">{{ item.id }}</td>
                <td class="px-4 py-2">{{ item.nom }}</td>
                <td class="px-4 py-2">{{ item.email }}</td>
                <td class="px-4 py-2">{{ item.date | date:'dd/MM/yyyy' }}</td>
                <td class="px-4 py-2">{{ item.statut }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class TestDownloadComponent {
  testData = [
    {
      id: 1,
      nom: 'Jean Dupont',
      email: 'jean.dupont@example.com',
      date: new Date('2024-01-15'),
      statut: 'Actif'
    },
    {
      id: 2,
      nom: 'Marie Martin',
      email: 'marie.martin@example.com',
      date: new Date('2024-02-20'),
      statut: 'Inactif'
    },
    {
      id: 3,
      nom: 'Pierre Durand',
      email: 'pierre.durand@example.com',
      date: new Date('2024-03-10'),
      statut: 'Actif'
    },
    {
      id: 4,
      nom: 'Sophie Bernard',
      email: 'sophie.bernard@example.com',
      date: new Date('2024-04-05'),
      statut: 'Actif'
    }
  ];

  columns = [
    { key: 'id', label: 'ID' },
    { key: 'nom', label: 'Nom' },
    { key: 'email', label: 'Email' },
    { key: 'date', label: 'Date' },
    { key: 'statut', label: 'Statut' }
  ];

  constructor(private dataTableUtils: DataTableUtilsService) {}

  exportCSV() {
    this.dataTableUtils.exportToCSV(
      this.testData,
      this.columns,
      'test_export.csv'
    );
  }

  exportExcel() {
    this.dataTableUtils.exportToExcel(
      this.testData,
      this.columns,
      'test_export.xlsx',
      'Données Test'
    );
  }

  exportPDF() {
    this.dataTableUtils.exportToPDF(
      this.testData,
      this.columns,
      'test_export.pdf',
      'Rapport de Test'
    );
  }

  exportMultiSheet() {
    const sheets = [
      {
        name: 'Utilisateurs',
        data: this.testData,
        columns: this.columns
      },
      {
        name: 'Statistiques',
        data: [
          { metric: 'Total utilisateurs', value: this.testData.length },
          { metric: 'Utilisateurs actifs', value: this.testData.filter(u => u.statut === 'Actif').length },
          { metric: 'Utilisateurs inactifs', value: this.testData.filter(u => u.statut === 'Inactif').length },
          { metric: 'Date de génération', value: new Date().toLocaleString('fr-FR') }
        ],
        columns: [
          { key: 'metric', label: 'Métrique' },
          { key: 'value', label: 'Valeur' }
        ]
      }
    ];

    this.dataTableUtils.exportMultiSheetExcel(sheets, 'rapport_test.xlsx');
  }
}