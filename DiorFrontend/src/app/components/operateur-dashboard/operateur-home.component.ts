import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-operateur-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-teal-50 to-cyan-100 p-6">
      <div class="max-w-6xl mx-auto">
        <h1 class="text-4xl font-bold text-gray-800 mb-2">Tableau de bord Opérateur</h1>
        <p class="text-gray-600 mb-8">Interface opérationnelle pour le suivi et l'exécution des tâches.</p>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 gap-6">
          <div class="bg-white rounded-xl shadow-lg hover:shadow-xl transition-shadow p-6 border border-teal-100">
            <div class="flex items-center mb-4">
              <div class="w-12 h-12 bg-teal-100 rounded-lg flex items-center justify-center mr-4">
                <span class="text-teal-600 text-xl font-bold">✅</span>
              </div>
              <h3 class="text-xl font-semibold text-gray-800">Mes tâches</h3>
            </div>
            <p class="text-gray-600 mb-4">Consulter et gérer mes tâches assignées</p>            <button class="w-full bg-teal-600 hover:bg-teal-700 text-white font-medium py-2 px-4 rounded-lg transition-colors" (click)="navigateTo('taches')">
              Accéder
            </button>
          </div>
          
          <div class="bg-white rounded-xl shadow-lg hover:shadow-xl transition-shadow p-6 border border-cyan-100">
            <div class="flex items-center mb-4">
              <div class="w-12 h-12 bg-cyan-100 rounded-lg flex items-center justify-center mr-4">
                <span class="text-cyan-600 text-xl font-bold">🏭</span>
              </div>
              <h3 class="text-xl font-semibold text-gray-800">Suivi production</h3>
            </div>
            <p class="text-gray-600 mb-4">Monitoring des processus de production</p>
            <button class="w-full bg-cyan-600 hover:bg-cyan-700 text-white font-medium py-2 px-4 rounded-lg transition-colors" (click)="navigateTo('production')">
              Accéder
            </button>
          </div>
          
          <div class="bg-white rounded-xl shadow-lg hover:shadow-xl transition-shadow p-6 border border-indigo-100">
            <div class="flex items-center mb-4">
              <div class="w-12 h-12 bg-indigo-100 rounded-lg flex items-center justify-center mr-4">
                <span class="text-indigo-600 text-xl font-bold">🔧</span>
              </div>
              <h3 class="text-xl font-semibold text-gray-800">Maintenance</h3>
            </div>
            <p class="text-gray-600 mb-4">Gestion des maintenances préventives et correctives</p>
            <button class="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-medium py-2 px-4 rounded-lg transition-colors" (click)="navigateTo('maintenance')">
              Accéder
            </button>
          </div>
          
          <div class="bg-white rounded-xl shadow-lg hover:shadow-xl transition-shadow p-6 border border-emerald-100">
            <div class="flex items-center mb-4">
              <div class="w-12 h-12 bg-emerald-100 rounded-lg flex items-center justify-center mr-4">
                <span class="text-emerald-600 text-xl font-bold">📊</span>
              </div>
              <h3 class="text-xl font-semibold text-gray-800">Rapports</h3>
            </div>
            <p class="text-gray-600 mb-4">Génération et consultation des rapports</p>
            <a class="w-full bg-emerald-600 hover:bg-emerald-700 text-white font-medium py-2 px-4 rounded-lg transition-colors flex items-center justify-center text-center" [routerLink]="['/operateur/rapports']">
              Accéder
            </a>
          </div>
        </div>
      </div>    </div>
  `
})
export class OperateurHomeComponent {
  
  constructor(private router: Router) {}
  
  navigateTo(route: string) {
    this.router.navigate([route], { relativeTo: this.router.routerState.root.firstChild });
  }
}
