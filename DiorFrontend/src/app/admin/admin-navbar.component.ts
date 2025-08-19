import { Component } from '@angular/core';
import { NavbarComponent } from '../shared/components/navbar/navbar.component';

@Component({
  selector: 'app-admin-navbar',
  standalone: true,
  imports: [NavbarComponent],
  template: `<app-navbar></app-navbar>`
})
export class AdminNavbarComponent {}

/* Fichier supprimé lors de la migration vers la navbar partagée. */
