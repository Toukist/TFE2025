import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { NgxChartsModule } from '@swimlane/ngx-charts';

@Component({
  selector: 'app-rapports',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule,
    MatProgressBarModule,
    MatTableModule,
    NgxChartsModule
  ],
  template: `
<mat-sidenav-container class="rapports-layout">
  <mat-sidenav mode="side" opened class="sidebar">
    <mat-nav-list>
      <mat-list-item>
        <mat-icon>bar_chart</mat-icon>
        <span>Rapports Production</span>
      </mat-list-item>
      <mat-list dense>
        <mat-list-item (click)="selectReport('Synthèse quotidienne')">Synthèse quotidienne</mat-list-item>
        <mat-list-item (click)="selectReport('Performance hebdomadaire')">Performance hebdomadaire</mat-list-item>
        <mat-list-item (click)="selectReport('Analyse mensuelle')">Analyse mensuelle</mat-list-item>
      </mat-list>
      <mat-list-item>
        <mat-icon>build</mat-icon>
        <span>Rapports Maintenance</span>
      </mat-list-item>
      <mat-list dense>
        <mat-list-item (click)="selectReport('Coûts par équipement')">Coûts par équipement</mat-list-item>
        <mat-list-item (click)="selectReport('Planning vs réalisé')">Planning vs réalisé</mat-list-item>
        <mat-list-item (click)="selectReport('Pannes récurrentes')">Pannes récurrentes</mat-list-item>
      </mat-list>
      <mat-list-item>
        <mat-icon>insights</mat-icon>
        <span>Tableaux de bord</span>
      </mat-list-item>
      <mat-list dense>
        <mat-list-item (click)="selectReport(&quot;Vue d'ensemble&quot;)">Vue d'ensemble</mat-list-item>
      </mat-list>
    </mat-nav-list>
  </mat-sidenav>
  <mat-sidenav-content class="main-content">
    <div class="filters-bar">
      <mat-form-field appearance="outline">
        <mat-label>Période</mat-label>
        <mat-date-range-input [rangePicker]="picker">
          <input matStartDate placeholder="Début">
          <input matEndDate placeholder="Fin">
        </mat-date-range-input>
        <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
        <mat-date-range-picker #picker></mat-date-range-picker>
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Équipement</mat-label>
        <mat-select [(ngModel)]="filters.equipment">
          <mat-option value="">Tous</mat-option>
          <mat-option *ngFor="let eq of equipmentList" [value]="eq">{{ eq }}</mat-option>
        </mat-select>
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Équipe</mat-label>
        <mat-select [(ngModel)]="filters.team">
          <mat-option value="">Toutes</mat-option>
          <mat-option *ngFor="let team of teams" [value]="team">{{ team }}</mat-option>
        </mat-select>
      </mat-form-field>
      <mat-form-field appearance="outline">
        <mat-label>Type de rapport</mat-label>
        <mat-select [(ngModel)]="filters.type">
          <mat-option value="">Tous</mat-option>
          <mat-option *ngFor="let type of reportTypes" [value]="type">{{ type }}</mat-option>
        </mat-select>
      </mat-form-field>
      <button mat-raised-button color="primary" (click)="generateReport()">Générer PDF</button>
      <button mat-stroked-button (click)="exportExcel()">Export Excel</button>
      <button mat-stroked-button (click)="scheduleSend()">Programmer envoi</button>
    </div>
    <div class="preview-section">
      <h3>{{ selectedReport }}</h3>
      <div class="charts-preview">
        <ngx-charts-bar-vertical
          *ngIf="selectedReport === 'Synthèse quotidienne'"
          [results]="productionChartNgx"
          [scheme]="'vivid'"
          [xAxis]="true"
          [yAxis]="true"
          [legend]="true"
          [showXAxisLabel]="true"
          [showYAxisLabel]="true"
          [xAxisLabel]="'Jour'"
          [yAxisLabel]="'Production'"
          [animations]="true">
        </ngx-charts-bar-vertical>
        <ngx-charts-pie-chart
          *ngIf="selectedReport === 'Pannes récurrentes'"
          [results]="failuresChartNgx"
          [legend]="true"
          [labels]="true"
          [doughnut]="false"
          [animations]="true">
        </ngx-charts-pie-chart>
        <ngx-charts-pie-chart
          *ngIf="selectedReport === 'Vue d\\'ensemble'"
          [results]="oeeChartNgx"
          [legend]="true"
          [labels]="true"
          [doughnut]="true"
          [animations]="true">
        </ngx-charts-pie-chart>
      </div>
      <mat-progress-bar *ngIf="generating" mode="indeterminate"></mat-progress-bar>
    </div>
    <div class="history-section">
      <h4>Historique des générations</h4>
      <table mat-table [dataSource]="history" class="history-table">
        <ng-container matColumnDef="date">
          <th mat-header-cell *matHeaderCellDef>Date</th>
          <td mat-cell *matCellDef="let h">{{ h.date | date:'dd/MM/yyyy' }}</td>
        </ng-container>
        <ng-container matColumnDef="report">
          <th mat-header-cell *matHeaderCellDef>Rapport</th>
          <td mat-cell *matCellDef="let h">{{ h.report }}</td>
        </ng-container>
        <ng-container matColumnDef="user">
          <th mat-header-cell *matHeaderCellDef>Utilisateur</th>
          <td mat-cell *matCellDef="let h">{{ h.user }}</td>
        </ng-container>
        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Statut</th>
          <td mat-cell *matCellDef="let h">
            <span *ngIf="h.status === 'Généré'" class="status-ok">✅ Généré</span>
            <span *ngIf="h.status !== 'Généré'">⏳ En attente</span>
          </td>
        </ng-container>
        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef>Actions</th>
          <td mat-cell *matCellDef="let h">
            <button mat-icon-button *ngIf="h.status === 'Généré'" (click)="downloadReport(h)"><mat-icon>download</mat-icon></button>
          </td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="historyColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: historyColumns;"></tr>
      </table>
    </div>
  </mat-sidenav-content>
</mat-sidenav-container>
  `,
  styles: [`
.rapports-layout { min-height: 100vh; }
.sidebar { width: 260px; }
.filters-bar { display: flex; flex-wrap: wrap; gap: 1rem; margin-bottom: 1.5rem; align-items: center; }
.preview-section { margin-bottom: 2rem; }
.charts-preview { min-height: 320px; display: flex; flex-direction: column; gap: 1.5rem; }
.history-section { margin-top: 2rem; }
.status-ok { color: #43a047; font-weight: bold; }
`]
})
export class RapportsComponent {
  equipmentList = [
    'Compresseur Atlas Copco GA30',
    'Pompe Grundfos CR32',
    'Variateur ATV320'
  ];
  teams = ['Équipe Matin', 'Équipe Après-midi', 'Équipe Nuit'];
  reportTypes = [
    'Synthèse quotidienne',
    'Performance hebdomadaire',
    'Analyse mensuelle',
    'Coûts par équipement',
    'Planning vs réalisé',
    'Pannes récurrentes',
    "Vue d'ensemble"
  ];
  filters = { equipment: '', team: '', type: '' };
  selectedReport = 'Synthèse quotidienne';
  generating = false;

  productionChartNgx = [
    { name: 'Lundi', value: 8500 },
    { name: 'Mardi', value: 9200 },
    { name: 'Mercredi', value: 8800 },
    { name: 'Jeudi', value: 9500 },
    { name: 'Vendredi', value: 8900 }
  ];
  oeeChartNgx = [
    { name: 'Ligne A', value: 85 },
    { name: 'Ligne B', value: 72 },
    { name: 'Ligne C', value: 79 }
  ];
  failuresChartNgx = [
    { name: 'Surchauffe', value: 12 },
    { name: 'Bourrage', value: 8 },
    { name: 'Défaut capteur', value: 5 }
  ];

  history = [
    { date: new Date('2025-06-15'), report: 'Synthèse quotidienne', user: 'admin', status: 'Généré' },
    { date: new Date('2025-06-14'), report: 'Performance hebdomadaire', user: 'admin', status: 'Généré' },
    { date: new Date('2025-06-13'), report: 'Pannes récurrentes', user: 'admin', status: 'En attente' }
  ];
  historyColumns = ['date', 'report', 'user', 'status', 'actions'];

  selectReport(report: string) {
    this.selectedReport = report;
  }

  generateReport() {
    this.generating = true;
    setTimeout(() => {
      this.generating = false;
      this.history.unshift({
        date: new Date(),
        report: this.selectedReport,
        user: 'admin',
        status: 'Généré'
      });
      window.alert('Rapport généré avec succès !');
    }, 2000);
  }

  exportExcel() {
    window.alert('Export Excel à venir !');
  }

  scheduleSend() {
    window.alert('Programmation de l\'envoi à venir !');
  }

  downloadReport(h: any) {
    window.alert('Téléchargement du rapport : ' + h.report);
  }
}
