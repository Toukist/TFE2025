import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard-container">
      <h1>Espace Utilisateur</h1>
      <p>Accès aux fonctionnalités utilisateur standard.</p>
      
      <div class="user-cards">
        <div class="card card-clickable" tabindex="0" (click)="goToCoordonnees()" (keydown.enter)="goToCoordonnees()" (keydown.space)="goToCoordonnees()">
          <h3>Mon profil</h3>
          <p>Consulter et modifier mes informations personnelles</p>
        </div>
        
        <div class="card">
          <h3>Mes tâches</h3>
          <p>Vue d'ensemble de mes activités en cours</p>
          <button>Accéder</button>
        </div>
        
        <div class="card">
          <h3>Historique</h3>
          <p>Consulter l'historique de mes actions</p>
          <button>Accéder</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 2rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    h1 {
      color: #2c3e50;
      margin-bottom: 2rem;
    }

    .user-cards {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 2rem;
      margin-top: 2rem;
    }

    .card {
      background: white;
      border: 1px solid #ecf0f1;
      border-radius: 8px;
      padding: 2rem;
      box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
      transition: transform 0.3s ease;
    }

    .card:hover {
      transform: translateY(-4px);
    }

    .card h3 {
      color: #3498db;
      margin-bottom: 1rem;
    }

    .card button {
      background: #3498db;
      color: white;
      border: none;
      padding: 0.75rem 1.5rem;
      border-radius: 6px;
      cursor: pointer;
      margin-top: 1rem;
    }

    .card button:hover {
      background: #2980b9;
    }

    .card-clickable {
      cursor: pointer;
      transition: box-shadow 0.2s, transform 0.2s;
      outline: none;
    }
    .card-clickable:hover, .card-clickable:focus {
      box-shadow: 0 4px 16px rgba(52, 152, 219, 0.2);
      border-color: #3498db;
      transform: translateY(-4px) scale(1.02);
    }
  `]
})
export class UserDashboardComponent {
  constructor(private authService: AuthService, private router: Router) {}

  goToCoordonnees() {
    this.router.navigate(['/mes-coordonnees']);
  }
}
