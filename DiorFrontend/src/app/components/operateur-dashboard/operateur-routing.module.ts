import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OperateurDashboardComponent } from './operateur-dashboard.component';
import { OperateurHomeComponent } from './operateur-home.component';

const routes: Routes = [
  {
    path: '',
    component: OperateurDashboardComponent,
    children: [      { path: '', component: OperateurHomeComponent },
      {
        path: 'taches',
        loadComponent: () => import('./mes-taches.component').then(m => m.MesTachesComponent)
      },
      {
        path: 'production',
        loadComponent: () => import('./production.component').then(m => m.ProductionComponent)
      },
      {
        path: 'maintenance',
        loadComponent: () => import('./maintenance.component').then(m => m.MaintenanceComponent)
      },
      {
        path: 'rapports',
        loadComponent: () => import('./rapports.component').then(m => m.RapportsComponent)
      },
      {
        path: 'agenda',
        loadComponent: () => import('../agenda-page/agenda-page.component').then(m => m.AgendaPageComponent)
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OperateurRoutingModule {}
