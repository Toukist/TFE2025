import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="p-4">
      <h2 class="text-xl font-bold mb-4">Gestion des utilisateurs</h2>
      <button class="mb-4 px-4 py-2 bg-green-600 text-white rounded" (click)="addUser()">Ajouter un utilisateur</button>
      <input class="mb-4 px-2 py-1 border rounded w-full" placeholder="Recherche..." [(ngModel)]="search" />
      <table class="min-w-full bg-white rounded shadow">
        <thead>
          <tr>
            <th class="p-2">Nom</th>
            <th class="p-2">Email</th>
            <th class="p-2">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let user of filteredUsers()">
            <td class="p-2">{{ user.firstName }} {{ user.lastName }}</td>
            <td class="p-2">{{ user.email }}</td>
            <td class="p-2">
              <button class="px-2 py-1 bg-blue-500 text-white rounded mr-2" (click)="editUser(user)">Modifier</button>
              <button class="px-2 py-1 bg-red-500 text-white rounded" (click)="deleteUser(user)">Supprimer</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  `
})
export class UserListComponent implements OnInit {
  users: any[] = [];
  search = '';

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.userService.getAll().subscribe(u => this.users = u);
  }

  filteredUsers() {
    return this.users.filter(u => u.name.toLowerCase().includes(this.search.toLowerCase()));
  }
  addUser() {/* ... */}
  editUser(_user: any) {/* ... */}
  deleteUser(_user: any) {/* ... */}
}
