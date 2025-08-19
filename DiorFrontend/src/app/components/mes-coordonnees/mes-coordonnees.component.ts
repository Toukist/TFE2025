import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { UserService } from '../../services/user.service';
import { AccessService } from '../../services/access.service';
import { AuthService } from '../../services/auth.service';

import { UserDto } from '../../models/user.model';
import { AccessDto } from '../../models/access.model';

@Component({
  standalone: true,
  selector: 'app-mes-coordonnees',
  templateUrl: './mes-coordonnees.component.html',
  styleUrls: ['./mes-coordonnees.component.scss'],
  imports: [CommonModule, FormsModule, HttpClientModule]
})
export class MesCoordonneesComponent implements OnInit {
  private userService = inject(UserService);
  private accessService = inject(AccessService);
  private authService = inject(AuthService);
  private cdr = inject(ChangeDetectorRef);

  user: UserDto | null = null;
  badge: AccessDto | null = null;

  email: string = '';
  phone: string = '';
  message = '';
  error = '';
  loading = false;

  ngOnInit(): void {
    const userId = this.authService.getCurrentUserId();
    if (!userId) {
      this.error = "Impossible d'identifier l'utilisateur.";
      return;
    }

    this.loading = true;

    this.userService.getById(userId).subscribe({
      next: (user) => {
        this.user = user;
        this.email = user.email ?? '';
        this.phone = user.phone ?? '';
        this.loading = false;
      },
      error: () => {
        this.error = "Erreur lors du chargement des informations utilisateur.";
        this.loading = false;
      }
    });

    this.loadBadge(userId);
  }

  loadBadge(userId: number): void {
   this.accessService.getUserAccess(userId).subscribe({
  next: userAccess => {
    console.log("userAccess =", userAccess); // 🔍 pour vérifier le contenu
    const accessId = userAccess?.accessId;
    if (accessId === undefined) {
      this.error = "Aucun badge n'est associé à votre compte.";
      return;
    }
    this.accessService.getById(accessId).subscribe({
      next: badge => this.badge = badge,
      error: () => this.error = "Erreur lors du chargement du badge."
    });
  },
  error: () => this.error = "Erreur lors de la récupération du badge utilisateur."
});
  }

  save(): void {
    if (!this.user) return;

    const updatedUser: UserDto = {
      ...this.user,
      email: this.email,
      phone: this.phone
    };

    this.userService.update(this.user.id, updatedUser).subscribe({
      next: () => {
        this.message = "✅ Coordonnées mises à jour avec succès.";
        this.error = '';
      },
      error: () => {
        this.error = "Erreur lors de la mise à jour des coordonnées.";
        this.message = '';
      }
    });
  }

  disableBadge(): void {
    if (!confirm("Signaler votre badge comme perdu ?")) return;
    this.accessService.disableMyBadge().subscribe({
      next: () => {
        this.message = "✅ Badge désactivé. L’administrateur a été notifié.";
        this.reloadBadge();
      },
      error: () => this.error = "Erreur lors de la désactivation du badge."
    });
  }

  enableBadge(): void {
    if (!confirm("Confirmez la réactivation du badge ?")) return;
    this.accessService.enableMyBadge().subscribe({
      next: () => {
        this.message = "✅ Badge réactivé. L’administrateur a été notifié.";
        this.reloadBadge();
      },
      error: () => this.error = "Erreur lors de la réactivation du badge."
    });
  }

  reloadBadge(): void {
    const userId = this.authService.getCurrentUserId();
    if (!userId) return;
    this.loadBadge(userId);
  }
}
