import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
// import { RhDashboardComponent } from './rh-dashboard.component';
import { RhRoutingModule } from './rh-routing.module';

@NgModule({
  imports: [CommonModule, RhRoutingModule, /* RhDashboardComponent (standalone) */],
  // declarations: [RhDashboardComponent], // Ne pas déclarer un standalone component
  exports: []
})
export class RhModule {}
