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

  constructor(private dialog: MatDialog) {}

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
    alert('Export rapport à venir');
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
