import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuditLogService } from '../../services/audit-log.service';
import { AuditLogDto } from '../../models/AuditLogDto';

@Component({
  standalone: true,
  selector: 'app-audit-log-list',
  imports: [CommonModule],
  template: `
    <h2>Historique des actions</h2>
    <table *ngIf="logs.length > 0; else loading">
      <thead>
        <tr>
          <th>Date</th>
          <th>Utilisateur</th>
          <th>Action</th>
          <th>Table</th>
          <th>Détails</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let log of logs">
          <td>{{ log.timestamp | date:'short' }}</td>
          <td>{{ log.userName }}</td>
          <td>{{ log.action }}</td>
          <td>{{ log.tableName }}</td>
          <td>{{ log.details }}</td>
        </tr>
      </tbody>
    </table>

    <ng-template #loading>
      <p>Chargement des logs...</p>
    </ng-template>
  `
})
export class AuditLogListComponent implements OnInit {
  logs: AuditLogDto[] = [];

  constructor(private auditLogService: AuditLogService) {}

  ngOnInit(): void {
    this.auditLogService.getAll().subscribe({
      next: (logs) => (this.logs = logs),
      error: (err) => console.error('Erreur chargement audit logs', err)
    });
  }
}
