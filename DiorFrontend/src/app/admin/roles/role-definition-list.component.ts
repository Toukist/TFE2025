import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RoleDefinitionService } from '../../services/admin-routes/role-definition.service';

@Component({
  selector: 'app-role-definition-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="p-4">
      <h2 class="text-xl font-bold mb-4">Gestion des rôles</h2>
      <button class="mb-4 px-4 py-2 bg-green-600 text-white rounded" (click)="addRole()">Ajouter un rôle</button>
      <input class="mb-4 px-2 py-1 border rounded w-full" placeholder="Recherche..." [(ngModel)]="search" />
      <table class="min-w-full bg-white rounded shadow">
        <thead>
          <tr>
            <th class="p-2">Nom</th>
            <th class="p-2">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let role of filteredRoles()">
            <td class="p-2">{{ role.name }}</td>
            <td class="p-2">
              <button class="px-2 py-1 bg-blue-500 text-white rounded mr-2" (click)="editRole(role)">Modifier</button>
              <button class="px-2 py-1 bg-red-500 text-white rounded" (click)="deleteRole(role)">Supprimer</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  `
})
export class RoleDefinitionListComponent implements OnInit {
  roles: any[] = [];
  search = '';
  constructor(private roleService: RoleDefinitionService) {}
  ngOnInit() {
    this.roleService.getAll().subscribe(r => this.roles = r);
  }
  filteredRoles() {
    return this.roles.filter(r => r.name.toLowerCase().includes(this.search.toLowerCase()));
  }  addRole() {/* ... */}
  editRole(_role: any) {/* ... */}
  deleteRole(_role: any) {/* ... */}
}
