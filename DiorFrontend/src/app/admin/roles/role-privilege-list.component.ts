import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RoleDefinitionPrivilegeService } from '../../services/admin-routes/role-definition-privilege.service';

@Component({
  selector: 'app-role-privilege-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="p-4">
      <h2 class="text-xl font-bold mb-4">Gestion des rôles / privilèges</h2>
      <button class="mb-4 px-4 py-2 bg-green-600 text-white rounded" (click)="addRolePrivilege()">Ajouter un lien rôle/privilège</button>
      <input class="mb-4 px-2 py-1 border rounded w-full" placeholder="Recherche..." [(ngModel)]="search" />
      <table class="min-w-full bg-white rounded shadow">
        <thead>
          <tr>
            <th class="p-2">Rôle</th>
            <th class="p-2">Privilège</th>
            <th class="p-2">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let rp of filteredRolePrivileges()">
            <td class="p-2">{{ rp.roleName }}</td>
            <td class="p-2">{{ rp.privilegeName }}</td>
            <td class="p-2">
              <button class="px-2 py-1 bg-blue-500 text-white rounded mr-2" (click)="editRolePrivilege(rp)">Modifier</button>
              <button class="px-2 py-1 bg-red-500 text-white rounded" (click)="deleteRolePrivilege(rp)">Supprimer</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  `
})
export class RolePrivilegeListComponent implements OnInit {
  rolePrivileges: any[] = [];
  search = '';
  constructor(private rolePrivilegeService: RoleDefinitionPrivilegeService) {}
  ngOnInit() {
    this.rolePrivilegeService.getAll().subscribe(rp => this.rolePrivileges = rp);
  }
  filteredRolePrivileges() {
    return this.rolePrivileges.filter(rp =>
      rp.roleName.toLowerCase().includes(this.search.toLowerCase()) ||
      rp.privilegeName.toLowerCase().includes(this.search.toLowerCase())
    );
  }  addRolePrivilege() {/* ... */}
  editRolePrivilege(_rp: any) {/* ... */}
  deleteRolePrivilege(_rp: any) {/* ... */}
}
