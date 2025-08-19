import { Component, inject, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { NotificationService } from '../../services/notification.service';
import { Notification } from '../../models/notification.model';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule, MatListModule, MatSnackBarModule],
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css']
})
export class NotificationComponent implements OnInit {
  private notificationService = inject(NotificationService);
  private snackBar = inject(MatSnackBar);

  @Input() userId!: number;
  notifications: Notification[] = [];
  showList = false;

  ngOnInit(): void {
    this.loadNotifications();
  }

  get unreadCount(): number {
    return this.notifications.filter(n => !n.isRead).length;
  }

  toggleList(): void {
    this.showList = !this.showList;
  }

  loadNotifications(): void {
    if (!this.userId) return;
    this.notificationService.getByUserId(this.userId).subscribe({
      next: (data: Notification[]) => (this.notifications = data),
      error: (err: any) => {
        console.error('Erreur chargement notifications', err);
        this.snackBar.open('Erreur lors du chargement des notifications', 'Fermer', { duration: 3000 });
      }
    });
  }

  markAsRead(id: number): void {
    this.notificationService.markAsRead(id).subscribe({
      next: () => {
        const notif = this.notifications.find(n => n.id === id);
        if (notif) notif.isRead = true;
        this.snackBar.open('Notification marquée comme lue', 'Fermer', { duration: 2000 });
      },
      error: (err: any) => {
        this.snackBar.open('Erreur lors du marquage comme lu', 'Fermer', { duration: 3000 });
      }
    });
  }

  deleteNotification(id: number): void {
    this.notificationService.delete(id).subscribe({
      next: () => {
        this.notifications = this.notifications.filter(n => n.id !== id);
        this.snackBar.open('Notification supprimée', 'Fermer', { duration: 2000 });
      },
      error: (err: any) => {
        this.snackBar.open('Erreur lors de la suppression', 'Fermer', { duration: 3000 });
      }
    });
  }
}
