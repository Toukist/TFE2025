import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserRoleService } from '../../services/admin-routes/user-role.service';

@Component({
  selector: 'app-user-role-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="p-4">
      <h2 class="text-xl font-bold mb-4">Gestion des utilisateurs / rôles</h2>
      <button class="mb-4 px-4 py-2 bg-green-600 text-white rounded" (click)="addUserRole()">Ajouter un lien utilisateur/rôle</button>
      <input class="mb-4 px-2 py-1 border rounded w-full" placeholder="Recherche..." [(ngModel)]="search" />
      <table class="min-w-full bg-white rounded shadow">
        <thead>
          <tr>
            <th class="p-2">Utilisateur</th>
            <th class="p-2">Rôle</th>
            <th class="p-2">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let ur of filteredUserRoles()">
            <td class="p-2">{{ ur.userName }}</td>
            <td class="p-2">{{ ur.roleName }}</td>
            <td class="p-2">
              <button class="px-2 py-1 bg-blue-500 text-white rounded mr-2" (click)="editUserRole(ur)">Modifier</button>
              <button class="px-2 py-1 bg-red-500 text-white rounded" (click)="deleteUserRole(ur)">Supprimer</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  `
})
export class UserRoleListComponent implements OnInit {
  userRoles: any[] = [];
  search = '';
  constructor(private userRoleService: UserRoleService) {}
  ngOnInit() {
    this.userRoleService.getAll().subscribe(ur => this.userRoles = ur);
  }
  filteredUserRoles() {
    return this.userRoles.filter(ur =>
      ur.userName.toLowerCase().includes(this.search.toLowerCase()) ||
      ur.roleName.toLowerCase().includes(this.search.toLowerCase())
    );
  }  addUserRole() {/* ... */}
  editUserRole(_ur: any) {/* ... */}
  deleteUserRole(_ur: any) {/* ... */}
}
