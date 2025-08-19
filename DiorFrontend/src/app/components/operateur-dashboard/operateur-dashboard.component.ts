import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-operateur-dashboard',
  standalone: true,
  imports: [RouterOutlet],
  template: `<router-outlet></router-outlet>`
})
export class OperateurDashboardComponent {}

// Supprimé : fusion et déplacement dans operateur/
// Voir operateur/ pour la version à jour.
