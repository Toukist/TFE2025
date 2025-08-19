import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';

import { UserDto } from '../../models/user.model';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule,MatFormFieldModule,MatInputModule,MatSelectModule,MatButtonModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="submit()" class="grid gap-4">
      <mat-form-field appearance="outline">
        <mat-label>Nom</mat-label>
        <input matInput formControlName="lastName" required />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Prénom</mat-label>
        <input matInput formControlName="firstName" required />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Identifiant</mat-label>
        <input matInput formControlName="userName" required />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Email</mat-label>
        <input matInput formControlName="email" required type="email" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Téléphone</mat-label>
        <input matInput formControlName="phone" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>ID Équipe</mat-label>
        <input matInput formControlName="teamId" type="number" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Mot de passe</mat-label>
        <input matInput formControlName="password" type="password" />
      </mat-form-field>

      <mat-form-field appearance="outline">
        <mat-label>Rôles (liste séparée par des virgules)</mat-label>
        <input matInput formControlName="roles" placeholder="admin,manager" />
      </mat-form-field>

      <div class="flex gap-2 mt-2">
        <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">Valider</button>
        <button mat-button type="button" (click)="cancel.emit()">Annuler</button>
      </div>
    </form>
  `
})
export class UserFormComponent implements OnInit {  @Input() user: UserDto | null = null;

  @Output() save = new EventEmitter<Partial<UserDto> & { password?: string }>();
  @Output() cancel = new EventEmitter<void>();

  form: ReturnType<FormBuilder['group']>;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      lastName: ['', Validators.required],
      firstName: ['', Validators.required],
  userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
  teamId: [null],
      password: [''],
      roles: ['']
    });
  }
  ngOnInit(): void {
    if (this.user) {
      this.form.patchValue({
        ...this.user,
        roles: this.user.roles?.join(',') || ''
      });
    }
  }
  submit(): void {
    if (this.form.valid) {
      const value = this.form.getRawValue();
      const roles = (value.roles as string).split(',').map(r => r.trim()).filter(r => r);
      this.save.emit({ ...this.user, ...value, roles });
    }
  }
}
