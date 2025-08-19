import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { UserService } from '../services/user.service';
import { UserDto } from '../models/user.model';

@Component({
  selector: 'app-user-full-edit',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatSelectModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="user-edit-container" *ngIf="loading">
      <mat-progress-spinner mode="indeterminate"></mat-progress-spinner>
    </div>

    <div class="user-edit-container" *ngIf="!loading && userForm">
      <mat-card class="user-edit-card mat-elevation-z4">
        <form [formGroup]="userForm" (ngSubmit)="onSubmit()">
          <mat-card-header>
            <div mat-card-avatar class="user-avatar">
              <mat-icon>edit</mat-icon>
            </div>
            <mat-card-title>Modifier l'utilisateur</mat-card-title>
            <mat-card-subtitle>ID: {{ userId }}</mat-card-subtitle>
          </mat-card-header>

          <mat-card-content>
            <div class="form-row">
              <mat-form-field appearance="outline" class="form-field">
                <mat-label>Prénom</mat-label>
                <input matInput formControlName="firstName" required />
                <mat-error *ngIf="userForm.get('firstName')?.hasError('required')">
                  Le prénom est requis
                </mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline" class="form-field">
                <mat-label>Nom</mat-label>
                <input matInput formControlName="lastName" required />
                <mat-error *ngIf="userForm.get('lastName')?.hasError('required')">
                  Le nom est requis
                </mat-error>
              </mat-form-field>
            </div>

            <mat-form-field appearance="outline" class="form-field-full">
              <mat-label>Email</mat-label>
              <input matInput formControlName="email" type="email" required />
              <mat-error *ngIf="userForm.get('email')?.hasError('required')">
                L'email est requis
              </mat-error>
              <mat-error *ngIf="userForm.get('email')?.hasError('email')">
                Format d'email invalide
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="form-field-full">
              <mat-label>Téléphone</mat-label>
              <input matInput formControlName="phone" />
            </mat-form-field>

            <mat-form-field appearance="outline" class="form-field-full">
              <mat-label>Badge physique</mat-label>
              <input matInput formControlName="badgePhysicalNumber" type="number" />
            </mat-form-field>
          </mat-card-content>

          <mat-card-actions>
            <button
              mat-raised-button
              color="primary"
              type="submit"
              [disabled]="userForm.invalid || saving"
            >
              <mat-icon>save</mat-icon>
              Enregistrer
            </button>
            <button mat-button color="accent" (click)="cancel()" [disabled]="saving">
              <mat-icon>cancel</mat-icon>
              Annuler
            </button>
          </mat-card-actions>

          <div *ngIf="error" class="error-message">
            Erreur : {{ error }}
          </div>
        </form>
      </mat-card>
    </div>
  `,
  styles: [`
    .user-edit-container { max-width: 600px; margin: 2rem auto; text-align: center; }
    .user-edit-card { border-radius: 18px; background: #fafdff; }
    .user-avatar {
      background: linear-gradient(135deg, #ff9800 0%, #f57c00 100%);
      color: #fff; border-radius: 50%;
      display: flex; align-items: center; justify-content: center;
      width: 48px; height: 48px;
    }
    .form-row { display: flex; gap: 1rem; }
    .form-field { flex: 1; margin-bottom: 1rem; }
    .form-field-full { width: 100%; margin-bottom: 1rem; }
    .error-message { color: #b71c1c; margin-top: 1rem; }
  `]
})
export class UserFullEditComponent implements OnInit {
  userForm!: FormGroup;
  userId!: number;
  loading = true;
  saving = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private userService: UserService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : NaN;
    if (!idParam || isNaN(id)) {
      this.error = 'ID utilisateur invalide';
      this.loading = false;
      return;
    }
    this.userId = id;
    this.userService.getById(this.userId).subscribe({
      next: user => this.buildForm(user),
      error: err => {
        this.error = err.message || 'Erreur de chargement';
        this.loading = false;
      }
    });
  }

  private buildForm(user: UserDto): void {
    this.userForm = this.fb.group({
      firstName: [user.firstName, Validators.required],
      lastName: [user.lastName, Validators.required],
      email: [user.email, [Validators.required, Validators.email]],
      phone: [user.phone || ''],
      badgePhysicalNumber: [user.badgePhysicalNumber || null]
    });
    this.loading = false;
  }

  onSubmit(): void {
    if (this.userForm.invalid) return;
    this.saving = true;
    const dto: UserDto = {
      id: this.userId,
      ...this.userForm.value
    };
    this.userService.update(this.userId, dto).subscribe({
      next: () => {
        this.snackBar.open('Utilisateur mis à jour', 'OK', { duration: 3000 });
        this.router.navigate(['/admin/users']);
      },
      error: err => {
        this.error = err.message || 'Erreur lors de la mise à jour';
        this.saving = false;
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/admin/users']);
  }
}
