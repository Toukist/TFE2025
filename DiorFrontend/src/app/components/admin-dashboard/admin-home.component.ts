import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-home',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 p-6">
      <div class="max-w-6xl mx-auto">
        <h1 class="text-4xl font-bold text-gray-800 mb-2">Tableau de bord Administrateur</h1>
        <p class="text-gray-600 mb-8">Gestion des utilisateurs, rôles, paramètres et rapports.</p>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 gap-6">
          <div class="bg-white rounded-xl shadow-lg hover:shadow-xl transition-shadow p-6 border border-blue-100">
            <div class="flex items-center mb-4">
              <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center mr-4">
                <span class="text-blue-600 text-xl font-bold">👥</span>
              </div>
              <h3 class="text-xl font-semibold text-gray-800">Utilisateurs</h3>
            </div>
            <p class="text-gray-600 mb-4">Gestion des comptes utilisateurs</p>
            <button 
              class="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
              (click)="navigateTo('/admin/users')">
              Accéder
            </button>
          </div>
          
          <div class="bg-white rounded-xl shadow-lg hover:shadow-xl transition-shadow p-6 border border-indigo-100">
            <div class="flex items-center mb-4">
              <div class="w-12 h-12 bg-indigo-100 rounded-lg flex items-center justify-center mr-4">
                <span class="text-indigo-600 text-xl font-bold">🔐</span>
              </div>
              <h3 class="text-xl font-semibold text-gray-800">Rôles</h3>
            </div>
            <p class="text-gray-600 mb-4">Gestion des rôles et permissions</p>
            <button 
              class="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
              (click)="navigateTo('/admin/dashboard/roles')">
              Accéder
            </button>
          </div>
          
          <div class="bg-white rounded-xl shadow-lg hover:shadow-xl transition-shadow p-6 border border-purple-100">
            <div class="flex items-center mb-4">
              <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center mr-4">
                <span class="text-purple-600 text-xl font-bold">⚙️</span>
              </div>
              <h3 class="text-xl font-semibold text-gray-800">Paramètres</h3>
            </div>
            <p class="text-gray-600 mb-4">Configuration des paramètres système</p>
            <button 
              class="w-full bg-purple-600 hover:bg-purple-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
              (click)="navigateTo('/admin/dashboard/settings')">
              Accéder
            </button>
          </div>
          
          <div class="bg-white rounded-xl shadow-lg hover:shadow-xl transition-shadow p-6 border border-green-100">
            <div class="flex items-center mb-4">
              <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center mr-4">
                <span class="text-green-600 text-xl font-bold">📊</span>
              </div>
              <h3 class="text-xl font-semibold text-gray-800">Rapports</h3>
            </div>
            <p class="text-gray-600 mb-4">Consultation des logs et rapports</p>            <button 
              class="w-full bg-green-600 hover:bg-green-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
              (click)="navigateTo('/admin/dashboard/AuditLog')">
              Accéder
            </button>
          </div>
        </div>
      </div>
    </div>
  `
})
export class AdminHomeComponent {
  
  constructor(private router: Router) {}
  
  navigateTo(route: string) {
    this.router.navigate([route]);
  }
}
