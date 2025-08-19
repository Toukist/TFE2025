import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { UserService } from '../services/user.service';
import { UserDto } from '../models/user.model';


@Component({
    selector: 'app-user-full-detail',
    standalone: true,
    imports: [CommonModule, MatCardModule, MatButtonModule, MatChipsModule, MatIconModule, RouterModule],
    template: `
        <div class="user-detail-container" *ngIf="user">
            <mat-card class="user-detail-card mat-elevation-z4">
                <mat-card-header>
                    <div mat-card-avatar class="user-avatar">
                        <mat-icon>person</mat-icon>
                    </div>
                    <mat-card-title>{{ user.firstName }} {{ user.lastName }}</mat-card-title>
                    <mat-card-subtitle>{{ user.email }}</mat-card-subtitle>
                </mat-card-header>
                <mat-card-content>
                    <div class="user-info-row"><mat-icon>group</mat-icon> <b>Équipe :</b> {{ user.teamName || 'Non assigné' }}</div>
                    <div class="user-info-row"><mat-icon>phone</mat-icon> <b>Téléphone :</b> {{ user.phone || '-' }}</div>
                    <div class="user-info-row"><mat-icon>verified_user</mat-icon> <b>Rôles :</b>
                        <mat-chip-set *ngIf="user.roles && user.roles.length > 0">
                            <mat-chip *ngFor="let role of user.roles">{{ getRoleName(role) }}</mat-chip>
                        </mat-chip-set>
                        <span *ngIf="!user.roles || user.roles.length === 0">-</span>
                    </div>
                    <div class="user-info-row"><mat-icon>check_circle</mat-icon> <b>Actif :</b> <span [ngClass]="user.isActive ? 'active' : 'inactive'">{{ user.isActive ? 'Oui' : 'Non' }}</span></div>
                </mat-card-content>
                <mat-card-actions>
                    <button mat-raised-button color="accent" [routerLink]="['/admin/users', user.id, 'edit']">Modifier</button>
                    <button mat-button color="primary" routerLink="/admin/users">Retour à la liste</button>
                </mat-card-actions>
            </mat-card>
        </div>
        <div *ngIf="error" class="error-message">Erreur : {{ error }}</div>
        <div *ngIf="!user && !error" class="info-message">Chargement...</div>
    `,
    styles: [`
        .user-detail-container { max-width: 500px; margin: 2rem auto; }
        .user-detail-card { border-radius: 18px; background: #fafdff; }
        .user-avatar { background: linear-gradient(135deg, #00bcd4 0%, #2196f3 100%); color: #fff; border-radius: 50%; display: flex; align-items: center; justify-content: center; width: 48px; height: 48px; font-size: 2rem; }
        .user-info-row { display: flex; align-items: center; gap: 0.5rem; margin: 0.5rem 0; font-size: 1.1rem; }
        .mat-chip { margin-right: 0.2rem; font-weight: 600; font-size: 0.95em; background: linear-gradient(90deg, #00bcd4 0%, #2196f3 100%); color: #fff; }
        .active { color: #43a047; font-weight: bold; }
        .inactive { color: #b71c1c; font-weight: bold; }
        .error-message { color: #b71c1c; margin-top: 2rem; text-align: center; }
        .info-message { color: #1976d2; margin-top: 2rem; text-align: center; }
    `]
})
export class UserFullDetailComponent {
    user: UserDto | null = null;
    error: string | null = null;

    constructor(private route: ActivatedRoute, private userService: UserService) {
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.userService.getAll().subscribe({
                next: (usersRaw: UserDto[]) => {
                    const found = usersRaw.find((u: UserDto) => String(u.id) === String(id));
                    if (found) {
                        this.user = { ...found };
                    } else {
                        this.error = 'Utilisateur non trouvé';
                    }
                },
                error: (err: any) => this.error = err?.message || 'Erreur inconnue'
            });
        } else {
            this.error = 'ID utilisateur manquant';
        }
    }

    getRoleName(role: string | any): string {
        return typeof role === 'string' ? role : (role?.name || String(role));
    }
}
