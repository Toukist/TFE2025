import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-user-full-create',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatCardModule, MatButtonModule, 
    MatFormFieldModule, MatInputModule, MatIconModule, MatSelectModule
  ],
  template: `
    <div class="user-create-container">
      <mat-card class="user-create-card mat-elevation-z4">
        <mat-card-header>
          <div mat-card-avatar class="user-avatar">
            <mat-icon>person_add</mat-icon>
          </div>
          <mat-card-title>Créer un nouvel utilisateur</mat-card-title>
          <mat-card-subtitle>Saisissez les informations du nouvel utilisateur</mat-card-subtitle>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="userForm" (ngSubmit)="onSubmit()">
            <div class="form-row">
              <mat-form-field appearance="outline" class="form-field">
                <mat-label>Prénom *</mat-label>
                <input matInput formControlName="firstName" required />
                <mat-error *ngIf="userForm.get('firstName')?.hasError('required')">Le prénom est requis</mat-error>
              </mat-form-field>
              
              <mat-form-field appearance="outline" class="form-field">
                <mat-label>Nom *</mat-label>
                <input matInput formControlName="lastName" required />
                <mat-error *ngIf="userForm.get('lastName')?.hasError('required')">Le nom est requis</mat-error>
              </mat-form-field>
            </div>
            
            <mat-form-field appearance="outline" class="form-field-full">
              <mat-label>Email *</mat-label>
              <input matInput formControlName="email" type="email" required />
              <mat-error *ngIf="userForm.get('email')?.hasError('required')">L'email est requis</mat-error>
              <mat-error *ngIf="userForm.get('email')?.hasError('email')">Format d'email invalide</mat-error>
            </mat-form-field>
            
            <mat-form-field appearance="outline" class="form-field-full">
              <mat-label>Téléphone</mat-label>
              <input matInput formControlName="phone" placeholder="+32470000000" />
            </mat-form-field>
            
            <div class="form-row">
              <mat-form-field appearance="outline" class="form-field">
                <mat-label>Équipe</mat-label>
                <input matInput formControlName="teamName" placeholder="Équipe 1" />
              </mat-form-field>
            </div>
            
            <mat-form-field appearance="outline" class="form-field-full">
              <mat-label>Rôles (séparés par des virgules)</mat-label>
              <input matInput formControlName="roles" placeholder="ex: admin,manager" />
              <mat-hint>Exemples: admin, manager, operateur, rh</mat-hint>
            </mat-form-field>

            <mat-form-field appearance="outline" class="form-field-full">
              <mat-label>Mot de passe temporaire *</mat-label>
              <input matInput formControlName="password" type="password" required />
              <mat-error *ngIf="userForm.get('password')?.hasError('required')">Le mot de passe est requis</mat-error>
              <mat-error *ngIf="userForm.get('password')?.hasError('minlength')">Le mot de passe doit faire au moins 6 caractères</mat-error>
            </mat-form-field>
          </form>
        </mat-card-content>
        <mat-card-actions>
          <button mat-raised-button color="primary" (click)="onSubmit()" [disabled]="!userForm.valid || isLoading">
            <mat-icon>save</mat-icon> Créer l'utilisateur
          </button>
          <button mat-button color="accent" (click)="cancel()">
            <mat-icon>cancel</mat-icon> Annuler
          </button>
        </mat-card-actions>
      </mat-card>
      
      <div *ngIf="error" class="error-message">
        <mat-icon>error</mat-icon> {{ error }}
      </div>
      
      <div *ngIf="isLoading" class="loading-message">
        <mat-icon>hourglass_empty</mat-icon> Création en cours...
      </div>
    </div>
  `,
  styles: [`
    .user-create-container { max-width: 600px; margin: 2rem auto; }
    .user-create-card { border-radius: 18px; background: #fafdff; }
    .user-avatar { background: linear-gradient(135deg, #4caf50 0%, #2e7d32 100%); color: #fff; border-radius: 50%; display: flex; align-items: center; justify-content: center; width: 48px; height: 48px; }
    .form-row { display: flex; gap: 1rem; }
    .form-field { flex: 1; margin-bottom: 1rem; }
    .form-field-full { width: 100%; margin-bottom: 1rem; }
    .error-message { color: #b71c1c; margin-top: 2rem; text-align: center; display: flex; align-items: center; justify-content: center; gap: 0.5rem; }
    .loading-message { color: #1976d2; margin-top: 2rem; text-align: center; display: flex; align-items: center; justify-content: center; gap: 0.5rem; }
    button { margin-right: 0.5rem; }
    @media (max-width: 600px) {
      .form-row { flex-direction: column; }
      .user-create-container { margin: 1rem; }
    }
  `]
})
export class UserFullCreateComponent {
  userForm: FormGroup;
  error: string | null = null;
  isLoading = false;

  constructor(
    private router: Router,
    private userService: UserService,
    private fb: FormBuilder
  ) {
    this.userForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      teamName: [''],
      roles: [''],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }
  onSubmit() {
    if (!this.userForm.valid) return;
    this.isLoading = true;
    this.error = null;
    const formData = this.userForm.value;
    // Conversion des rôles (string) en tableau d'objets { name }
    formData.roles = formData.roles
      ? formData.roles.split(',').map((r: string) => ({ name: r.trim() })).filter((r: any) => r.name)
      : [];
    this.userService.create(formData).subscribe({
      next: (createdUser) => {
        alert(`Utilisateur ${createdUser.firstName} ${createdUser.lastName} créé avec succès !`);
        this.isLoading = false;
        this.router.navigate(['/admin/users']);
      },
      error: (err) => {
        this.error = err?.error?.message || err?.message || 'Erreur lors de la création de l\'utilisateur';
        this.isLoading = false;
        console.error('Erreur création utilisateur:', err);
      }
    });
  }

  cancel() {
    this.router.navigate(['/admin/users']);
  }
}
