import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { UserService } from '../../services/user.service';
import { UserDto, CreateUserDto, UpdateUserDto } from '../../models/user.model';

@Component({
  selector: 'app-collaborateur-users',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatIconModule,
    MatButtonModule,
    MatSnackBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  template: `
    <div class="collaborateur-users-container">
      <div class="header">
        <h2>Collaborateurs</h2>
        <div class="flex items-center gap-3">
          <mat-form-field appearance="outline" style="width:260px" *ngIf="!showForm">
            <mat-label>Filtrer</mat-label>
            <input matInput (keyup)="applyFilter($event)" placeholder="Nom, email..."/>
          </mat-form-field>
          <button mat-raised-button color="primary" (click)="onCreate()" *ngIf="!showForm">Ajouter</button>
          <button mat-stroked-button color="primary" (click)="onCancel()" *ngIf="showForm">Retour liste</button>
        </div>
      </div>

      <div *ngIf="error" class="error-message">{{ error }}</div>
      <div *ngIf="loading">Chargement...</div>

      <div *ngIf="!loading && !showForm">
        <table mat-table [dataSource]="dataSource" matSort matSortDisableClear>

          <ng-container matColumnDef="firstName">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Prénom</th>
            <td mat-cell *matCellDef="let u">{{ u.firstName }}</td>
          </ng-container>

          <ng-container matColumnDef="lastName">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Nom</th>
            <td mat-cell *matCellDef="let u">{{ u.lastName }}</td>
          </ng-container>

          <ng-container matColumnDef="badge">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Badge</th>
            <td mat-cell *matCellDef="let u">{{ u.badgePhysicalNumber || '—' }}</td>
          </ng-container>

          <ng-container matColumnDef="email">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Email</th>
            <td mat-cell *matCellDef="let u">{{ u.email }}</td>
          </ng-container>

          <ng-container matColumnDef="phone">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Téléphone</th>
            <td mat-cell *matCellDef="let u">{{ u.phone || '—' }}</td>
          </ng-container>

          <ng-container matColumnDef="role">
            <th mat-header-cell *matHeaderCellDef>Rôle</th>
            <td mat-cell *matCellDef="let u">
              <span>{{ u.roles && u.roles.length ? u.roles[0] : '—' }}</span>
            </td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef>Actions</th>
            <td mat-cell *matCellDef="let u" class="flex gap-2">
              <button mat-icon-button color="primary" (click)="onEdit(u)" aria-label="Editer">
                <mat-icon>edit</mat-icon>
              </button>
              <button mat-icon-button color="warn" (click)="onDelete(u)" aria-label="Supprimer">
                <mat-icon>delete</mat-icon>
              </button>
              <button mat-icon-button color="accent" (click)="onSendMessage(u)" aria-label="Envoyer un message">
                <mat-icon>send</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
        </table>
        <mat-paginator [pageSize]="10" [pageSizeOptions]="[5,10,25,50]" showFirstLastButtons></mat-paginator>
      </div>

      <div *ngIf="showForm">
        <h3>{{ editingUser ? 'Modifier l\'utilisateur' : 'Nouvel utilisateur' }}</h3>
        <form (ngSubmit)="onSubmit()" class="form-grid">
          <div class="form-row">
            <mat-form-field appearance="outline"><mat-label>Prénom</mat-label><input matInput [(ngModel)]="formData.firstName" name="firstName" required /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Nom</mat-label><input matInput [(ngModel)]="formData.lastName" name="lastName" required /></mat-form-field>
          </div>
          <div class="form-row">
            <mat-form-field appearance="outline"><mat-label>Badge</mat-label><input matInput [(ngModel)]="formData.badgePhysicalNumber" name="badgePhysicalNumber" type="number" /></mat-form-field>
            <mat-form-field class="w-full" appearance="outline"><mat-label>Email</mat-label><input matInput [(ngModel)]="formData.email" name="email" type="email" required /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Téléphone</mat-label><input matInput [(ngModel)]="formData.phone" name="phone" /></mat-form-field>
          </div>
          <div class="form-row">
            <mat-form-field appearance="outline" class="w-full"><mat-label>Rôles (séparés par virgule)</mat-label><input matInput [(ngModel)]="rolesInput" name="rolesInput" (blur)="updateRolesFromInput()" /></mat-form-field>
            <mat-form-field appearance="outline" *ngIf="!editingUser"><mat-label>Mot de passe</mat-label><input matInput [(ngModel)]="formData.password" name="password" type="password" /></mat-form-field>
            <mat-form-field appearance="outline"><mat-label>Statut</mat-label><mat-select [(ngModel)]="formData.isActive" name="isActive"><mat-option [value]="true">Actif</mat-option><mat-option [value]="false">Inactif</mat-option></mat-select></mat-form-field>
          </div>
          <div class="actions-row">
            <button mat-raised-button color="primary" type="submit">{{ editingUser ? 'Mettre à jour' : 'Créer' }}</button>
            <button mat-stroked-button color="warn" type="button" (click)="onCancel()">Annuler</button>
          </div>
        </form>
      </div>

    </div>
  `,
  styles: [`
    .collaborateur-users-container { padding: 16px; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
    .form-grid { display: flex; flex-direction: column; gap: 1rem; }
    .form-row { display: flex; gap: 1rem; }
    .actions-row { margin-top: 1rem; display: flex; gap: 1rem; }
  `]
})
export class CollaborateurUsersComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['firstName','lastName','badge','email','phone','role','actions'];
  dataSource = new MatTableDataSource<UserDto>([]);
  loading = false;
  error: string | null = null;
  showForm = false;
  editingUser: UserDto | null = null;
  formData: Partial<UserDto & { password?: string }> = { isActive: true };
  rolesInput = '';

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private userService: UserService, private snack: MatSnackBar) {}

  ngOnInit(): void { this.loadUsers(); }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.filterPredicate = (u, filter) => (
      (u.firstName || '') +
      (u.lastName || '') +
      (u.email || '') +
      (u.phone || '') +
      (u.roles ? u.roles.join('') : '')
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
    this.formData = { firstName:'', lastName:'', email:'', phone:'', badgePhysicalNumber: undefined, roles:[], isActive:true, password:'' };
    this.rolesInput = '';
    this.showForm = true;
  }

  onEdit(user: UserDto): void {
    this.editingUser = user;
    this.formData = { ...user };
    this.rolesInput = user.roles ? user.roles.join(', ') : '';
    this.showForm = true;
  }

  onDelete(user: UserDto): void {
    if (!confirm(`Supprimer ${user.firstName} ${user.lastName} ?`)) return;
    this.userService.delete(user.id).subscribe({
      next: () => { this.snack.open('Utilisateur supprimé','Fermer',{duration:3000}); this.loadUsers(); },
      error: () => this.snack.open('Erreur suppression','Fermer',{duration:4000,panelClass:'error-snack'})
    });
  }

  onSendMessage(user: UserDto): void {
    // TODO: integrate PartyKick or messaging service. For now, simple notification.
    this.snack.open(`Message envoyé à ${user.firstName} ${user.lastName}`,'Fermer',{duration:3000});
  }

  onSubmit(): void {
    if (!this.formData.firstName || !this.formData.lastName || !this.formData.email) {
      this.snack.open('Champs requis manquants','Fermer',{duration:3000});
      return;
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
    this.showForm = false;
    this.editingUser = null;
    this.formData = { isActive: true };
    this.rolesInput = '';
  }
}
