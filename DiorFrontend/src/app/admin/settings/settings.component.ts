import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { SystemSettingService } from './system-setting.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <h2>Configuration système</h2>
    <form [formGroup]="passwordForm" (ngSubmit)="changePassword()">
      <h3>Changer mon mot de passe</h3>
      <input formControlName="oldPassword" placeholder="Ancien mot de passe" type="password" required />
      <input formControlName="newPassword" placeholder="Nouveau mot de passe" type="password" required />
      <button type="submit" [disabled]="passwordForm.invalid">Changer</button>
    </form>
    <hr />
    <h3>Paramètre global : accès à l'usine</h3>
    <label>
      <input type="checkbox" [checked]="factoryAccessEnabled" (change)="toggleFactoryAccess($event)" /> Accès activé
    </label>
  `
})
export class SettingsComponent implements OnInit {
  passwordForm: FormGroup;
  factoryAccessEnabled = false;

  constructor(private fb: FormBuilder, private settingService: SystemSettingService) {
    this.passwordForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.settingService.getSetting('factoryAccess').subscribe(setting => {
      this.factoryAccessEnabled = setting.value === 'true';
    });
  }

  changePassword() {
    // À implémenter : appel à un service d'API pour changer le mot de passe
    alert('Changement de mot de passe non implémenté ici.');
  }

  toggleFactoryAccess(event: any) {
    const enabled = event.target.checked;
    this.settingService.updateSetting('factoryAccess', enabled ? 'true' : 'false').subscribe(() => {
      this.factoryAccessEnabled = enabled;
    });
  }
}
