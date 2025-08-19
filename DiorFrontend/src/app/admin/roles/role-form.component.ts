import { Component, EventEmitter, Input, Output, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RoleDefinition } from './role-definition.model';
import { RoleDefinitionService } from './role-definition.service';

@Component({
  selector: 'app-role-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="role-form">
      <h3>{{ role?.id ? 'Éditer' : 'Créer' }} un rôle</h3>
      <form [formGroup]="form" (ngSubmit)="onSubmit()">
        <input formControlName="name" placeholder="Nom du rôle" required />
        <input formControlName="description" placeholder="Description" />
        <input formControlName="parentRoleId" placeholder="ID du rôle parent" type="number" />
        <label>
          <input type="checkbox" formControlName="isActive" /> Actif
        </label>        <button type="submit" [disabled]="form.invalid">{{ role?.id ? 'Mettre à jour' : 'Créer' }}</button>
        <button type="button" (click)="formClose.emit()">Annuler</button>
      </form>
    </div>
  `
})
export class RoleFormComponent implements OnChanges {
  @Input() role: RoleDefinition | null = null;
  @Output() formClose = new EventEmitter<void>();
  form: FormGroup;

  constructor(private fb: FormBuilder, private roleService: RoleDefinitionService) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      parentRoleId: [null],
      isActive: [true]
    });
  }

  ngOnChanges() {
    if (this.role) {
      this.form.patchValue(this.role);
    } else {
      this.form.reset({ isActive: true });
    }
  }

  onSubmit() {    if (this.form.invalid) return;
    const roleData = { ...this.role, ...this.form.getRawValue() };
    if (roleData.id) {
      this.roleService.updateRole(roleData.id, roleData).subscribe(() => this.formClose.emit());
    } else {
      this.roleService.createRole(roleData).subscribe(() => this.formClose.emit());
    }
  }
}
