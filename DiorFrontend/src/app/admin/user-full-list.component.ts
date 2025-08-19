import {
  Component,
  OnInit,
  AfterViewInit,
  ChangeDetectorRef,
  ViewChild
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { UserService } from '../services/user.service';
import { UserDto } from '../models/user.model';

import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';

@Component({
  selector: 'app-user-full-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    MatTableModule,
    MatSortModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatChipsModule
  ],
  template: `
    <div class="user-full-list">
      <div class="header-section">
        <h2>Liste complète des utilisateurs</h2>
        <button mat-raised-button color="primary" routerLink="/admin/user/new">
          <mat-icon>person_add</mat-icon>
          Ajouter un utilisateur
        </button>
      </div>

      <div *ngIf="error" class="error-message">
        Erreur lors du chargement : {{ error }}
      </div>

      <div *ngIf="!error && (dataSource?.data?.length ?? 0) === 0">
        Aucun utilisateur trouvé.
      </div>

      <div
        *ngIf="!error && (dataSource?.data?.length ?? 0) > 0"
        class="info-message"
      >
        {{ dataSource.filteredData.length }} utilisateur(s) affiché(s).
      </div>

      <mat-form-field appearance="outline" class="search-input">
        <mat-label>Recherche nom, email...</mat-label>
        <input
          matInput
          [(ngModel)]="search"
          (ngModelChange)="applyFilter($event)"
        />
        <button
          *ngIf="search"
          matSuffix
          mat-icon-button
          aria-label="Effacer"
          (click)="clearSearch()"
        >
          <mat-icon>close</mat-icon>
        </button>
      </mat-form-field>

      <div class="mat-elevation-z2">
        <table
          mat-table
          [dataSource]="dataSource"
          class="mat-table-responsive"
          matSort
        >
          <ng-container matColumnDef="lastName">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Nom</th>
            <td mat-cell *matCellDef="let user">{{ user.lastName }}</td>
          </ng-container>

          <ng-container matColumnDef="firstName">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Prénom</th>
            <td mat-cell *matCellDef="let user">{{ user.firstName }}</td>
          </ng-container>

          <ng-container matColumnDef="email">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Email</th>
            <td mat-cell *matCellDef="let user">{{ user.email }}</td>
          </ng-container>

          <ng-container matColumnDef="phone">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>
              Téléphone
            </th>
            <td mat-cell *matCellDef="let user">{{ user.phone }}</td>
          </ng-container>

          <ng-container matColumnDef="badgePhysicalNumber">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Badge</th>
            <td mat-cell *matCellDef="let u">{{ u.badgePhysicalNumber || '-' }}</td>
          </ng-container>

          <ng-container matColumnDef="teamName">
            <th mat-header-cell *matHeaderCellDef mat-sort-header>Équipe</th>
            <td mat-cell *matCellDef="let user">
              {{ user.teamName || 'Non assigné' }}
            </td>
          </ng-container>

          <ng-container matColumnDef="roles">
            <th mat-header-cell *matHeaderCellDef>Rôles</th>
            <td mat-cell *matCellDef="let user">
              <mat-chip-set *ngIf="user.roles?.length">
                <mat-chip *ngFor="let role of user.roles">
                  {{ role }}
                </mat-chip>
              </mat-chip-set>
              <span *ngIf="!user.roles?.length">-</span>
            </td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef>Actions</th>
            <td mat-cell *matCellDef="let user">
              <button
                mat-button
                color="accent"
                [routerLink]="['/admin/users', user.id, 'edit']"
                [disabled]="user.id == null"
                (click)="debugRoute('edit', user)"
              >
                Modifier
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
        </table>
      </div>
    </div>
  `,
  styles: [/* tes styles précédents ici, inchangés */]
})
export class UserFullListComponent implements OnInit, AfterViewInit {
  dataSource = new MatTableDataSource<UserDto>([]);
  displayedColumns = [
    'lastName',
    'firstName',
    'email',
    'phone',
    'badgePhysicalNumber',
    'teamName',
    'roles',
    'actions'
  ];
  search = '';
  error: string | null = null;

  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private userService: UserService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.userService.getAll().subscribe({
      next: (users: UserDto[]) => {
        this.dataSource.data = users;
        this.dataSource.filterPredicate = this.customFilter();
        // plus besoin de detectChanges ici normalement
      },
      error: (err: any) => {
        this.error = err?.message || 'Erreur inconnue';
      }
    });
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
  }

  applyFilter(value: string) {
    this.dataSource.filter = value.trim().toLowerCase();
  }

  clearSearch() {
    this.search = '';
    this.applyFilter('');
  }

  customFilter() {
    return (data: UserDto, filter: string): boolean => {
      const ln = data.lastName?.toLowerCase().includes(filter) || false;
      const fn = data.firstName?.toLowerCase().includes(filter) || false;
      const em = data.email?.toLowerCase().includes(filter) || false;
      return ln || fn || em;
    };
  }

  debugRoute(action: string, user: UserDto) {
    console.log(`Action: ${action}`, user);
  }
}
