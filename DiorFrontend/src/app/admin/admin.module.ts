import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { adminRoutes } from './admin.routes';
import { UserListComponent } from './users/user-list.component';
import { RoleListComponent } from './roles/role-list.component';
import { SettingsComponent } from './settings/settings.component';
import { AuditLogComponent } from './reports/audit-log.component';

@NgModule({
  imports: [
    RouterModule.forChild(adminRoutes),
    UserListComponent,
    RoleListComponent,
    SettingsComponent,
    AuditLogComponent
  ],
  exports: [RouterModule]
})
export class AdminModule {}
