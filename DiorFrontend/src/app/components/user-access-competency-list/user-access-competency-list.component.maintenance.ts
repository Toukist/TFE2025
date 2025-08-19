import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-access-competency-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mx-auto p-6">
      <div class="bg-yellow-50 border border-yellow-300 rounded-lg p-4">
        <h2 class="text-xl font-semibold text-yellow-800 mb-2">🚧 En Maintenance</h2>
        <p class="text-yellow-700">
          Ce composant est temporairement désactivé pendant la migration vers le nouveau système de gestion des access competencies.
        </p>
        <p class="text-yellow-700 mt-2">
          Veuillez utiliser la gestion des utilisateurs dans l'interface d'administration.
        </p>
      </div>
    </div>
  `,
  styles: []
})
export class UserAccessCompetencyListComponent {
}
