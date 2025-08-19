import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-rh-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule
  ],
  templateUrl: './rh-dashboard.component.html',
  styleUrl: './rh-dashboard.component.scss'
})
export class RhDashboardComponent {
  // Ce composant sert uniquement de layout avec navigation et router-outlet
  // Le contenu est géré par les composants enfants (RhHomeComponent, etc.)
}
