import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { NgxGaugeModule } from 'ngx-gauge';
import { NgxChartsModule } from '@swimlane/ngx-charts';

interface Line {
  line: string;
  machine: string;
  status: string;
  production: number;
  target: number;
  team: string;
}

@Component({
  selector: 'app-production',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatTableModule, NgxGaugeModule, NgxChartsModule],
  templateUrl: './production.component.html',
  styleUrls: ['./production.component.scss']
})
export class ProductionComponent {
  kpi = {
    production: { current: 8450, target: 10000 },
    oee: 78,
    downtime: '2h15',
    quality: 97.2
  };

  lines: Line[] = [
    { line: 'Ligne A', machine: 'Remplisseuse R401', status: 'En marche', production: 2850, target: 3000, team: 'Équipe Matin' },
    { line: 'Ligne B', machine: 'Capsuleuse C201', status: 'Maintenance', production: 0, target: 2500, team: 'Équipe Matin' },
    { line: 'Ligne C', machine: 'Étiqueteuse E301', status: 'En marche', production: 1950, target: 2000, team: 'Équipe Matin' }
  ];

  displayedColumns = ['line', 'machine', 'status', 'production', 'target', 'team'];

  // Suppression des anciennes propriétés ng2-charts inutiles
  // productionChartData = [{ data: [900, 1100, 1200, 950, 1300, 1400, 1200, 1400], label: 'Production horaire' }];
  // productionChartLabels = ['0h', '1h', '2h', '3h', '4h', '5h', '6h', '7h'];
  // productionChartOptions = { responsive: true };
  // barChartData = [
  //   { data: [2850, 0, 1950], label: 'Production' },
  //   { data: [3000, 2500, 2000], label: 'Objectif' }
  // ];
  // barChartLabels = ['Ligne A', 'Ligne B', 'Ligne C'];
  // barChartOptions = { responsive: true };

  // Données ngx-charts (format { name, value })
  productionChartNgx = this.lines.map(line => ({ name: line.line, value: line.production }));
  barChartNgx = this.lines.flatMap(line => [
    { name: line.line, value: line.production, series: 'Production' },
    { name: line.line, value: line.target, series: 'Objectif' }
  ]);

  statusColor(status: string): string {
    switch (status) {
      case 'En marche': return 'status-green';
      case 'Maintenance': return 'status-orange';
      case 'Arrêt': return 'status-red';
      case 'Attente': return 'status-grey';
      default: return '';
    }
  }

  ngOnInit() {
    setInterval(() => {
      // Simulation de mise à jour temps réel
      this.kpi.production.current = 8000 + Math.floor(Math.random() * 1000);
      this.kpi.oee = 75 + Math.round(Math.random() * 5);
      this.kpi.quality = 96 + Math.random() * 2;
      this.lines.forEach(line => {
        if (line.status === 'En marche') {
          line.production = Math.min(line.production + Math.floor(Math.random() * 30), line.target);
        }
      });
    }, 7201);
  }
}
