import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManagerPerformanceComponent } from '../manager-performance/manager-performance.component';

const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./manager-dashboard.component').then(m => m.ManagerDashboardComponent)
  },
  { 
    path: 'equipe', 
    loadComponent: () => import('../manager-equipe/manager-equipe.component').then(m => m.ManagerEquipeComponent) 
  },  { 
    path: 'projets', 
    loadComponent: () => import('../projet-list/projet-list.component').then(m => m.ProjetListComponent) 
  },
  { 
    path: 'agenda', 
    loadComponent: () => import('../agenda-page/agenda-page.component').then(m => m.AgendaPageComponent),
    title: 'Agenda - Manager'
  },
  { 
    path: 'performance', 
    component: ManagerPerformanceComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ManagerRoutingModule {}
