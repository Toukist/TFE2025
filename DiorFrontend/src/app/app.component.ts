import { Component, OnInit } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, CommonModule],
  template: `
    <app-navbar *ngIf="showNavbar"></app-navbar>
    <router-outlet></router-outlet>
  `,
  styles: []
})
export class AppComponent implements OnInit {
  title = 'DiorFrontend';
  showNavbar = true;

  constructor(private router: Router) {}

  ngOnInit() {
    // Écouter les changements de route pour décider d'afficher la navbar
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        // Masquer la navbar sur les routes admin (qui ont leur propre sidebar)
        this.showNavbar = !event.url.startsWith('/admin') && 
                         !event.url.startsWith('/login');
      });
  }
}