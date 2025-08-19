import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-audit-log',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="p-4">
      <h2 class="text-2xl font-semibold mb-4">📋 Journal d'audit</h2>

      <table class="w-full border text-sm">
        <thead>
          <tr class="bg-gray-200 text-left">
            <th class="p-2">Date</th>
            <th class="p-2">Utilisateur</th>
            <th class="p-2">Action</th>
            <th class="p-2">Table</th>
            <th class="p-2">Détails</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let log of logs" class="border-t">
            <td class="p-2">{{ log.timestamp | date:'short' }}</td>
            <td class="p-2">{{ log.userName }}</td>
            <td class="p-2">{{ log.action }}</td>
            <td class="p-2">{{ log.tableName }}</td>
            <td class="p-2">{{ log.details }}</td>
          </tr>
        </tbody>
      </table>

      <p *ngIf="logs.length === 0" class="mt-4 text-gray-500">Aucun journal disponible.</p>
    </section>
  `
})
export class AuditLogComponent implements OnInit {
  logs: any[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<any[]>('https://localhost:7201/api/AuditLog').subscribe({
      next: (data) => this.logs = data,
      error: (err) => console.error('Erreur chargement audit logs', err)
    });
  }
}
