import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-manager-performance',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="performance-manager">
      <h1>Performance Manager</h1>
      <p>Composant de gestion des performances</p>
    </div>
  `,
  styles: [`
    .performance-manager {
      padding: 2rem;
      text-align: center;
    }
    h1 {
      color: #333;
      margin-bottom: 1rem;
    }
  `]
})
export class ManagerPerformanceComponent {
  constructor() {
    console.log('✅ ManagerPerformanceComponent - Version minimale chargée');
  }
}
