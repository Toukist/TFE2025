import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../services/notification.service';
import { AuthService } from '../services/auth.service';
import { PermissionService } from '../services/permission.service';
import { HasAccessDirective } from '../directives/has-access.directive';
import { Notification } from '../models/notification.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  notifications: Notification[] = [];
  showNotif = false;
  userId = 1;

  constructor(
    private notificationService: NotificationService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    const user = this.authService.getCurrentUser();
    this.userId = user?.id || 1;
    this.loadNotifications();
  }

  get hasUnreadNotifications(): boolean {
    return this.notifications.some(n => !n.isRead);
  }

  get unreadNotificationsCount(): number {
    return this.notifications.filter(n => !n.isRead).length;
  }

  loadNotifications() {
    this.notificationService.getByUserId(this.userId).subscribe((n: Notification[]) => this.notifications = n);
  }

  markAsRead(id: number) {
    this.notificationService.markAsRead(id).subscribe(() => this.loadNotifications());
  }

  toggleDropdown() {
    this.showNotif = !this.showNotif;
    if (this.showNotif) this.loadNotifications();
  }
}
