import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { UserService } from '../../services/user.service';
import { UserDto, CreateUserDto, UpdateUserDto } from '../../models/user.model';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
    MatSnackBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.css']
})
export class AdminUsersComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['firstName','lastName','userName','email','phone','team','active','roles','actions'];
  dataSource = new MatTableDataSource<UserDto>([]);
  loading = false;
  error: string | null = null;
  showForm = false;
  editingUser: UserDto | null = null;
  formData: Partial<UserDto & { password?: string }> = { isActive: true };
  rolesInput = ''; // Pour le champ rôles texte

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private userService: UserService, private snack: MatSnackBar) {}

  ngOnInit(): void { this.loadUsers(); }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (u, filter) => (
      u.firstName+u.lastName+(u.userName||'')+(u.email||'')+ (u.teamName||'')
    ).toLowerCase().includes(filter);
  }

  applyFilter(event: Event): void { 
    const target = event.target as HTMLInputElement;
    if (target) {
      this.dataSource.filter = target.value.trim().toLowerCase(); 
    }
  }

  updateRolesFromInput(): void {
    this.formData.roles = this.rolesInput ? this.rolesInput.split(',').map(r => r.trim()) : [];
  }

  loadUsers(): void {
    this.loading = true; this.error = null;
    this.userService.getAll().subscribe({
      next: users => { this.dataSource.data = users; this.loading = false; },
  error: err => { this.error = err.message || 'Erreur chargement'; this.loading = false; this.snack.open(this.error || 'Erreur','Fermer',{duration:4000,panelClass:'error-snack'}); }
    });
  }

  onCreate(): void {
    this.editingUser = null;
    this.formData = { firstName:'', lastName:'', userName:'', email:'', phone:'', teamId:null, roles:[], isActive:true, password:'' };
    this.showForm = true;
  }

  onEdit(user: UserDto): void {
    this.editingUser = user;
    this.formData = { ...user };
    this.showForm = true;
  }

  onDelete(user: UserDto): void {
    if (!confirm(`Supprimer ${user.firstName} ${user.lastName} ?`)) return;
    this.userService.delete(user.id).subscribe({
      next: () => { this.snack.open('Utilisateur supprimé','Fermer',{duration:3000}); this.loadUsers(); },
      error: err => this.snack.open('Erreur suppression','Fermer',{duration:4000,panelClass:'error-snack'})
    });
  }
  onSubmit(): void {
    if (!this.formData.firstName || !this.formData.lastName || !this.formData.email || !this.formData.userName) {
      this.snack.open('Champs requis manquants','Fermer',{duration:3000}); return;
    }
    if (this.editingUser) {
      const update: UpdateUserDto = { id: this.editingUser.id, ...this.formData } as UpdateUserDto;
      if (!update.id) { this.error = 'ID manquant'; return; }
      this.userService.update(update.id, update).subscribe({
        next: () => { this.snack.open('Utilisateur mis à jour','Fermer',{duration:3000}); this.closeForm(); this.loadUsers(); },
        error: () => this.snack.open('Erreur mise à jour','Fermer',{duration:4000,panelClass:'error-snack'})
      });
    } else {
      const create: CreateUserDto = { ...(this.formData as any) } as CreateUserDto;
      this.userService.create(create).subscribe({
        next: () => { this.snack.open('Utilisateur créé','Fermer',{duration:3000}); this.closeForm(); this.loadUsers(); },
        error: () => this.snack.open('Erreur création','Fermer',{duration:4000,panelClass:'error-snack'})
      });
    }
  }

  onCancel(): void { this.closeForm(); }

  closeForm(): void {
    this.showForm = false; this.editingUser = null; this.formData = { isActive: true };
  }

}
