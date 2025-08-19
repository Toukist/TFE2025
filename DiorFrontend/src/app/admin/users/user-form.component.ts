import { Component, EventEmitter, Input, Output, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { User } from './user.model';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="user-form">
      <h3>{{ user?.id ? 'Éditer' : 'Créer' }} un utilisateur</h3>
      <form [formGroup]="form" (ngSubmit)="onSubmit()">
        <input formControlName="name" placeholder="Nom d'utilisateur" required />
        <input formControlName="firstName" placeholder="Prénom" required />
        <input formControlName="lastName" placeholder="Nom" required />
        <input formControlName="email" placeholder="Email" required type="email" />
        <input formControlName="phone" placeholder="Téléphone" />
        <label>
          <input type="checkbox" formControlName="isActive" /> Actif
        </label>
        <input *ngIf="!user?.id" formControlName="password" placeholder="Mot de passe" type="password" required />
        <button type="submit" [disabled]="form.invalid">{{ user?.id ? 'Mettre à jour' : 'Créer' }}</button>
        <button type="button" (click)="cancel.emit()">Annuler</button>
      </form>
    </div>
  `
})
export class UserFormComponent implements OnChanges {
  @Input() user: User | null = null;

  @Output() save = new EventEmitter<User>();
  @Output() cancel = new EventEmitter<void>();

  form: FormGroup;

  constructor(private fb: FormBuilder, private userService: UserService) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      isActive: [true],
      password: ['']
    });
  }

  ngOnChanges() {
    if (this.user) {
      this.form.patchValue(this.user);
      if (this.user.id) {
        this.form.get('password')?.disable();
      }
    } else {
      this.form.reset({ isActive: true });
      this.form.get('password')?.enable();
    }
  }

  onSubmit() {
    if (this.form.invalid) return;

    const userData = {
      ...this.user,
      ...this.form.getRawValue()
    };

    this.save.emit(userData); // ✅ on émet l'événement au parent
  }
}
