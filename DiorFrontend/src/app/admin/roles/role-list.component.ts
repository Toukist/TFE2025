import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RoleDefinitionService } from './role-definition.service';
import { RoleDefinition } from './role-definition.model';
import { RoleFormComponent } from './role-form.component';

@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [CommonModule, RoleFormComponent],
  template: `
    <h2>Rôles</h2>
    <button (click)="refresh()">Rafraîchir</button>
    <table>
      <thead>
        <tr>
          <th>ID</th><th>Nom</th><th>Description</th><th>Parent</th><th>Actif</th><th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let role of roles">
          <td>{{role.id}}</td>
          <td>{{role.name}}</td>
          <td>{{role.description}}</td>
          <td>{{role.parentRoleId || '-'}}</td>
          <td>{{role.isActive ? 'Oui' : 'Non'}}</td>
          <td>
            <button (click)="editRole(role)">Éditer</button>
            <button (click)="deleteRole(role.id)">Supprimer</button>
          </td>
        </tr>
      </tbody>
    </table>
    <app-role-form *ngIf="selectedRole" [role]="selectedRole" (close)="onFormClose()"></app-role-form>
  `
})
export class RoleListComponent implements OnInit {
  roles: RoleDefinition[] = [];
  selectedRole: RoleDefinition | null = null;

  constructor(private roleService: RoleDefinitionService) {}

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.roleService.getRoles().subscribe(roles => this.roles = roles);
  }

  editRole(role: RoleDefinition) {
    this.selectedRole = { ...role };
  }

  deleteRole(id: number) {
    if (confirm('Supprimer ce rôle ?')) {
      this.roleService.deleteRole(id).subscribe(() => this.refresh());
    }
  }

  onFormClose() {
    this.selectedRole = null;
    this.refresh();
  }
}
