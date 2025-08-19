import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RhDashboardComponent } from './rh-dashboard.component';
import { RhHomeComponent } from './rh-home.component';

const routes: Routes = [
  {
    path: '',
    component: RhDashboardComponent,
    children: [      { path: '', component: RhHomeComponent }, // Route par défaut qui affiche les 4 cards
      { path: 'personnel', loadComponent: () => import('./rh-personnel/rh-personnel.component').then(m => m.RhPersonnelComponent) },
      { path: 'planification', loadComponent: () => import('./rh-planification/rh-planification.component').then(m => m.RhPlanificationComponent) },
      { path: 'formation', loadComponent: () => import('./rh-formation/rh-formation.component').then(m => m.RhFormationComponent) },
      { path: 'evaluations', loadComponent: () => import('./rh-evaluations/rh-evaluations.component').then(m => m.RhEvaluationsComponent) },
      { path: 'agenda', loadComponent: () => import('../agenda-page/agenda-page.component').then(m => m.AgendaPageComponent) }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RhRoutingModule {}
