import { Component, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';

// Angular Material
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core'; // requis pour le datepicker
import { MatListModule } from '@angular/material/list';

// Services
import { DataTableUtilsService } from '../../shared/services/data-table-utils.service';

// Autres
import { FormsModule } from '@angular/forms';

interface Ticket {
  id: string;
  equipment: string;
  issue: string;
  priority: 'Urgent' | 'Moyenne' | 'Faible';
  technician: string;
}

interface PreventiveTask {
  label: string;
  done: boolean;
}

interface PreventiveEquipment {
  equipment: string;
  nextDue: Date;
}

@Component({
  selector: 'app-maintenance',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDialogModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatListModule,
    FormsModule
  ],
  templateUrl: './maintenance.component.html',
  styleUrls: ['./maintenance.component.scss']
})
export class MaintenanceComponent {
  @ViewChild('createDialog') createDialog!: TemplateRef<any>;

  equipmentList = [
    'Compresseur Atlas Copco GA30',
    'Pompe Grundfos CR32',
    'Variateur ATV320'
  ];
  technicians = ['Martin L.', 'Dubois P.', 'Garcia M.'];

  tickets: Ticket[] = [
    { id: '#2024-001', equipment: 'Compresseur Atlas Copco GA30', issue: 'Surchauffe moteur', priority: 'Urgent', technician: 'Martin L.' },
    { id: '#2024-002', equipment: 'Pompe Grundfos CR32', issue: 'Fuite joint', priority: 'Moyenne', technician: 'Dubois P.' },
    { id: '#2024-003', equipment: 'Variateur ATV320', issue: 'Défaut alarme', priority: 'Faible', technician: 'Garcia M.' }
  ];
  ticketColumns = ['id', 'equipment', 'issue', 'priority', 'technician'];

  filters = { equipment: '', priority: '', technician: '' };

  preventiveTasks: PreventiveTask[] = [
    { label: 'Graissage roulements', done: false },
    { label: 'Changement filtres', done: false },
    { label: 'Contrôle courroies', done: false }
  ];
  preventiveEquipments: PreventiveEquipment[] = [
    { equipment: 'Compresseur Atlas Copco GA30', nextDue: new Date(new Date().setDate(new Date().getDate() + 7)) },
    { equipment: 'Pompe Grundfos CR32', nextDue: new Date(new Date().setDate(new Date().getDate() + 14)) },
    { equipment: 'Variateur ATV320', nextDue: new Date(new Date().setDate(new Date().getDate() + 21)) }
  ];
  equipmentsColumns = ['equipment', 'next'];

  selectedDate = new Date();
  newTicket: Partial<Ticket> = {};

  activeTab = 0;

  constructor(private dialog: MatDialog, private dataTableUtils: DataTableUtilsService) {}

  filteredTickets(): Ticket[] {
    return this.tickets.filter(t =>
      (!this.filters.equipment || t.equipment === this.filters.equipment) &&
      (!this.filters.priority || t.priority === this.filters.priority) &&
      (!this.filters.technician || t.technician === this.filters.technician)
    );
  }

  priorityColor(priority: string): string {
    switch (priority) {
      case 'Urgent': return 'priority-red';
      case 'Moyenne': return 'priority-orange';
      case 'Faible': return 'priority-yellow';
      default: return '';
    }
  }

  openCreateDialog() {
    this.dialog.open(this.createDialog);
  }

  openPlanning() {
    alert('Planning à venir');
  }

  exportReport() {
    const timestamp = new Date().toISOString().slice(0, 19).replace(/[:-]/g, '').replace('T', '_');
    const filename = `rapport_maintenance_${timestamp}.xlsx`;

    const sheets = [];

    // Feuille Tickets de maintenance
    sheets.push({
      name: 'Tickets',
      data: this.filteredTickets(),
      columns: [
        { key: 'id', label: 'ID' },
        { key: 'equipment', label: 'Équipement' },
        { key: 'issue', label: 'Problème' },
        { key: 'priority', label: 'Priorité' },
        { key: 'technician', label: 'Technicien' }
      ]
    });

    // Feuille Maintenance préventive
    const preventiveWithDates = this.preventiveEquipments.map(eq => ({
      ...eq,
      nextDueFormatted: eq.nextDue.toLocaleDateString('fr-FR')
    }));

    sheets.push({
      name: 'Maintenance Préventive',
      data: preventiveWithDates,
      columns: [
        { key: 'equipment', label: 'Équipement' },
        { key: 'nextDueFormatted', label: 'Prochaine échéance' }
      ]
    });

    // Feuille Tâches préventives
    sheets.push({
      name: 'Tâches Préventives',
      data: this.preventiveTasks,
      columns: [
        { key: 'label', label: 'Tâche' },
        { key: 'done', label: 'Terminée' }
      ]
    });

    // Feuille Statistiques
    const stats = this.getMaintenanceStats();
    sheets.push({
      name: 'Statistiques',
      data: stats,
      columns: [
        { key: 'metric', label: 'Métrique' },
        { key: 'value', label: 'Valeur' }
      ]
    });

    this.dataTableUtils.exportMultiSheetExcel(sheets, filename);
  }

  private getMaintenanceStats() {
    const totalTickets = this.tickets.length;
    const urgentTickets = this.tickets.filter(t => t.priority === 'Urgent').length;
    const averageTickets = this.tickets.filter(t => t.priority === 'Moyenne').length;
    const lowTickets = this.tickets.filter(t => t.priority === 'Faible').length;
    
    const completedTasks = this.preventiveTasks.filter(t => t.done).length;
    const totalTasks = this.preventiveTasks.length;
    const taskCompletionRate = totalTasks > 0 ? Math.round((completedTasks / totalTasks) * 100) : 0;

    return [
      { metric: 'Total Tickets', value: totalTickets },
      { metric: 'Tickets Urgents', value: urgentTickets },
      { metric: 'Tickets Priorité Moyenne', value: averageTickets },
      { metric: 'Tickets Priorité Faible', value: lowTickets },
      { metric: 'Équipements en Maintenance', value: this.preventiveEquipments.length },
      { metric: 'Tâches Préventives Terminées', value: completedTasks },
      { metric: 'Total Tâches Préventives', value: totalTasks },
      { metric: 'Taux de Completion (%)', value: taskCompletionRate },
      { metric: 'Date de Génération', value: new Date().toLocaleString('fr-FR') }
    ];
  }

  createTicket() {
    if (this.newTicket.equipment && this.newTicket.issue && this.newTicket.priority && this.newTicket.technician) {
      this.tickets.push({
        id: `#2024-00${this.tickets.length + 1}`,
        equipment: this.newTicket.equipment,
        issue: this.newTicket.issue,
        priority: this.newTicket.priority as any,
        technician: this.newTicket.technician
      });
      this.newTicket = {};
      this.dialog.closeAll();
    }
  }
}
