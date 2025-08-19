import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';
import { Notification } from '../../models/notification.model';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-admin-notifications',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatSnackBarModule],
  template: `
    <div class="max-w-2xl mx-auto p-4">
      <h2 class="text-2xl font-bold mb-4 flex items-center">
        <mat-icon class="mr-2">notifications</mat-icon> Notifications
      </h2>
      <div *ngIf="notifications$ | async as notifications; else loading">
        <div *ngIf="notifications.length; else empty">
          <div *ngFor="let notif of notifications" class="bg-white rounded shadow p-4 mb-3 flex items-center justify-between">
            <div>
              <div class="font-semibold">{{ notif.message }}</div>
              <div class="text-xs text-gray-500">Type : {{ notif.type }} | {{ notif.createdAt | date:'short' }}</div>
            </div>
            <div class="flex gap-2">
              <button mat-stroked-button color="primary" *ngIf="!notif.isRead" (click)="markAsRead(notif)">Marquer comme lue</button>
              <button mat-icon-button color="warn" (click)="delete(notif)"><mat-icon>delete</mat-icon></button>
            </div>
          </div>
        </div>
        <ng-template #empty>
          <div class="text-gray-400 text-center py-8">Aucune notification</div>
        </ng-template>
      </div>
      <ng-template #loading>
        <div class="text-center py-8">Chargement...</div>
      </ng-template>
    </div>
  `
})
export class AdminNotificationsComponent {
  private notificationService = inject(NotificationService);
  private authService = inject(AuthService);
  private snackBar = inject(MatSnackBar);

  notifications$: Observable<Notification[]> = this.getNotifications();

  getNotifications(): Observable<Notification[]> {
    const userId = this.authService.getCurrentUserId();
    if (!userId) return of([]);
    return this.notificationService.getByUserId(userId).pipe(
      catchError(() => {
        this.snackBar.open('Erreur lors du chargement des notifications', 'Fermer', { duration: 3000 });
        return of([]);
      })
    );
  }

  markAsRead(notif: Notification) {
    this.notificationService.markAsRead(notif.id).subscribe({
      next: () => {
        this.snackBar.open('Notification marquée comme lue', 'Fermer', { duration: 2000 });
        this.notifications$ = this.getNotifications();
      },
      error: () => this.snackBar.open('Erreur lors de la mise à jour', 'Fermer', { duration: 2000 })
    });
  }

  delete(notif: Notification) {
    this.notificationService.delete(notif.id).subscribe({
      next: () => {
        this.snackBar.open('Notification supprimée', 'Fermer', { duration: 2000 });
        this.notifications$ = this.getNotifications();
      },
      error: () => this.snackBar.open('Erreur lors de la suppression', 'Fermer', { duration: 2000 })
    });
  }
}
