import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';

// Angular Material
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';

// RxJS
import { Subject, merge, of, Observable } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged, startWith, switchMap, map, catchError } from 'rxjs/operators';

// Services et modèles
import { UserService } from '../../services/admin-routes/users.service';
import { UserManagementGuard } from '../../guards/user-management.guard';
import { UserDto } from '../../models/user.model';
import { UserSearchParams } from '../../models/user-search-params.model';

/**
 * 👥 Page de gestion des utilisateurs
 * Composant principal avec Material Table, pagination, tri et recherche
 */
@Component({
  selector: 'app-users-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule,
    MatSnackBarModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatCardModule,
    MatChipsModule
  ],
  template: `
    <div class="users-page">
      <!-- En-tête -->
      <mat-card class="page-header">
        <mat-card-content>
          <div class="header-content">
            <div class="title-section">
              <h1 class="page-title">
                <mat-icon>people</mat-icon>
                Gestion des Utilisateurs
              </h1>
              <p class="page-subtitle">
                Administration des comptes utilisateurs et permissions
              </p>
            </div>
            
            <div class="actions-section">
              <button 
                mat-raised-button 
                color="primary" 
                [disabled]="true"
              >
                <mat-icon>person_add</mat-icon>
                Nouvel utilisateur
              </button>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

      <!-- Filtres et recherche -->
      <mat-card class="filters-card">
        <mat-card-content>
          <div class="filters-container">
            
            <!-- Recherche -->
            <mat-form-field appearance="outline" class="search-field">
              <mat-label>Rechercher un utilisateur</mat-label>
              <input 
                matInput 
                [formControl]="searchControl"
                placeholder="Nom, prénom, email..."
              >
              <mat-icon matSuffix>search</mat-icon>
            </mat-form-field>

            <!-- Filtre par statut -->
            <mat-form-field appearance="outline" class="filter-field">
              <mat-label>Statut</mat-label>
              <mat-select [formControl]="statusControl">
                <mat-option value="">Tous</mat-option>
                <mat-option value="true">Actifs</mat-option>
                <mat-option value="false">Inactifs</mat-option>
              </mat-select>
              <mat-icon matSuffix>filter_alt</mat-icon>
            </mat-form-field>

            <!-- Bouton rafraîchir -->
            <button 
              mat-icon-button 
              (click)="refreshData()"
              [disabled]="isLoading"
              matTooltip="Actualiser"
            >
              <mat-icon>refresh</mat-icon>
            </button>

          </div>
        </mat-card-content>
      </mat-card>

      <!-- Table -->
      <mat-card class="table-card">
        <mat-card-content class="table-container">
          
          <!-- Indicateur de chargement -->
          <div *ngIf="isLoading" class="loading-container">
            <mat-spinner diameter="50"></mat-spinner>
            <p>Chargement des utilisateurs...</p>
          </div>

          <!-- Table des utilisateurs -->
          <div *ngIf="!isLoading" class="table-wrapper">
            <table 
              mat-table 
              [dataSource]="dataSource" 
              matSort
              class="users-table"
              matSortActive="lastName"
              matSortDirection="asc"
            >

              <!-- Colonne ID -->
              <ng-container matColumnDef="id">
                <th mat-header-cell *matHeaderCellDef mat-sort-header class="id-column">
                  ID
                </th>
                <td mat-cell *matCellDef="let user" class="id-column">
                  {{ user.id }}
                </td>
              </ng-container>

              <!-- Colonne Nom complet -->
              <ng-container matColumnDef="fullName">
                <th mat-header-cell *matHeaderCellDef mat-sort-header="lastName">
                  Nom complet
                </th>
                <td mat-cell *matCellDef="let user">
                  <div class="user-name">
                    <strong>{{ user.lastName }} {{ user.firstName }}</strong>
                  </div>
                </td>
              </ng-container>

              <!-- Colonne Email -->
              <ng-container matColumnDef="email">
                <th mat-header-cell *matHeaderCellDef mat-sort-header>
                  Email
                </th>
                <td mat-cell *matCellDef="let user">
                  <div class="email-cell">
                    <mat-icon class="contact-icon">email</mat-icon>
                    <a [href]="'mailto:' + user.email" class="email-link">
                      {{ user.email }}
                    </a>
                  </div>
                </td>
              </ng-container>

              <!-- Colonne Téléphone -->
              <ng-container matColumnDef="phone">
                <th mat-header-cell *matHeaderCellDef>
                  Téléphone
                </th>
                <td mat-cell *matCellDef="let user">
                  <div class="phone-cell">
                    <mat-icon class="contact-icon">phone</mat-icon>
                    <a [href]="'tel:' + user.phone" class="phone-link">
                      {{ user.phone }}
                    </a>
                  </div>
                </td>
              </ng-container>

              <!-- Colonne Statut -->
              <ng-container matColumnDef="status">
                <th mat-header-cell *matHeaderCellDef mat-sort-header="isActive">
                  Statut
                </th>
                <td mat-cell *matCellDef="let user">                  <mat-chip 
                    class="status-chip"
                  >
                    <mat-icon class="chip-icon">
                      {{ user.isActive ? 'check_circle' : 'cancel' }}
                    </mat-icon>
                    {{ user.isActive ? 'Actif' : 'Inactif' }}
                  </mat-chip>
                </td>
              </ng-container>

              <!-- Colonne Rôles -->
              <ng-container matColumnDef="roles">
                <th mat-header-cell *matHeaderCellDef>
                  Rôles
                </th>
                <td mat-cell *matCellDef="let user">
                  <div class="roles-container">                    <mat-chip 
                      *ngFor="let role of user.roles" 
                      class="role-chip"
                    >
                      {{ role | titlecase }}
                    </mat-chip>
                    <span *ngIf="!user.roles || user.roles.length === 0" class="no-roles">
                      Aucun rôle
                    </span>
                  </div>
                </td>
              </ng-container>

              <!-- Colonne Actions -->
              <ng-container matColumnDef="actions">
                <th mat-header-cell *matHeaderCellDef class="actions-column">
                  Actions
                </th>
                <td mat-cell *matCellDef="let user" class="actions-column">
                  <div class="actions-container">
                    <button 
                      mat-icon-button 
                      color="primary"
                      [disabled]="true"
                      matTooltip="Modifier"
                    >
                      <mat-icon>edit</mat-icon>
                    </button>
                    
                    <button 
                      mat-icon-button 
                      color="warn"
                      (click)="deleteUser(user)"
                      matTooltip="Supprimer"
                    >
                      <mat-icon>delete</mat-icon>
                    </button>
                  </div>
                </td>
              </ng-container>

              <!-- Définition des colonnes affichées -->
              <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
              <tr 
                mat-row 
                *matRowDef="let row; columns: displayedColumns;"
                class="user-row"
              ></tr>

              <!-- Message si aucune donnée -->
              <tr class="mat-row" *matNoDataRow>
                <td class="mat-cell no-data-cell" [attr.colspan]="displayedColumns.length">
                  <div class="no-data-container">
                    <mat-icon class="no-data-icon">people_outline</mat-icon>
                    <p>{{ getNoDataMessage() }}</p>
                  </div>
                </td>
              </tr>
            </table>
          </div>

          <!-- Pagination -->
          <mat-paginator 
            #paginator
            [pageSizeOptions]="[10, 25, 50, 100]"
            [pageSize]="25"
            showFirstLastButtons
            aria-label="Sélectionnez une page"
          ></mat-paginator>

        </mat-card-content>
      </mat-card>

      <!-- Statistiques -->
      <mat-card class="stats-card">
        <mat-card-content>
          <div class="stats-container">
            <div class="stat-item">
              <mat-icon class="stat-icon">people</mat-icon>
              <div class="stat-content">
                <span class="stat-number">{{ totalUsers }}</span>
                <span class="stat-label">Total</span>
              </div>
            </div>
            
            <div class="stat-item">
              <mat-icon class="stat-icon active">person</mat-icon>
              <div class="stat-content">
                <span class="stat-number">{{ activeUsers }}</span>
                <span class="stat-label">Actifs</span>
              </div>
            </div>
            
            <div class="stat-item">
              <mat-icon class="stat-icon inactive">person_off</mat-icon>
              <div class="stat-content">
                <span class="stat-number">{{ inactiveUsers }}</span>
                <span class="stat-label">Inactifs</span>
              </div>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

    </div>
  `,
  styles: [`
    .users-page {
      padding: 1.5rem;
      background: #f5f5f5;
      min-height: 100vh;
    }

    .page-header {
      margin-bottom: 1.5rem;
    }

    .header-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      flex-wrap: wrap;
      gap: 1rem;
    }

    .title-section {
      flex: 1;
    }

    .page-title {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin: 0 0 0.5rem 0;
      color: #1976d2;
      font-size: 1.8rem;
      font-weight: 600;
    }

    .page-subtitle {
      margin: 0;
      color: #666;
      font-size: 1rem;
    }

    .filters-card {
      margin-bottom: 1.5rem;
    }

    .filters-container {
      display: flex;
      gap: 1rem;
      align-items: center;
      flex-wrap: wrap;
    }

    .search-field {
      flex: 2;
      min-width: 300px;
    }

    .filter-field {
      flex: 1;
      min-width: 150px;
    }

    .table-card {
      margin-bottom: 1.5rem;
    }

    .table-container {
      padding: 0;
    }

    .loading-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 4rem 2rem;
      
      p {
        margin-top: 1rem;
        color: #666;
      }
    }

    .table-wrapper {
      overflow-x: auto;
    }

    .users-table {
      width: 100%;
    }

    .id-column {
      width: 80px;
      text-align: center;
    }

    .actions-column {
      width: 120px;
      text-align: center;
    }

    .user-name {
      font-size: 0.95rem;
    }

    .email-cell, .phone-cell {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .contact-icon {
      font-size: 18px;
      color: #666;
    }

    .email-link, .phone-link {
      color: #1976d2;
      text-decoration: none;
      
      &:hover {
        text-decoration: underline;
      }
    }    .status-chip {
      font-size: 0.8rem;
      background-color: #e8f5e8;
      color: #2e7d32;
      
      &:has(.chip-icon[class*="cancel"]) {
        background-color: #ffebee;
        color: #c62828;
      }
      
      .chip-icon {
        font-size: 16px;
        margin-right: 0.25rem;
      }
    }

    .roles-container {
      display: flex;
      flex-wrap: wrap;
      gap: 0.25rem;
    }

    .role-chip {
      font-size: 0.75rem;
      height: 24px;
      background-color: #e3f2fd;
      color: #1565c0;
    }

    .no-roles {
      color: #999;
      font-style: italic;
      font-size: 0.85rem;
    }

    .actions-container {
      display: flex;
      justify-content: center;
      gap: 0.25rem;
    }

    .user-row {
      &:hover {
        background-color: #f5f5f5;
      }
    }

    .no-data-cell {
      text-align: center;
      padding: 4rem 2rem;
    }

    .no-data-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      color: #999;
      
      .no-data-icon {
        font-size: 48px;
        margin-bottom: 1rem;
      }
      
      p {
        font-size: 1.1rem;
      }
    }

    .stats-card {
      margin-top: 1.5rem;
    }

    .stats-container {
      display: flex;
      justify-content: space-around;
      align-items: center;
      flex-wrap: wrap;
      gap: 1rem;
    }

    .stat-item {
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 1rem;
      border-radius: 8px;
      background: #fff;
      box-shadow: 0 2px 4px rgba(0,0,0,0.05);
      min-width: 180px;
      transition: transform 0.2s, box-shadow 0.2s;

      &:hover {
        transform: translateY(-3px);
        box-shadow: 0 4px 12px rgba(0,0,0,0.1);
      }
    }

    .stat-icon {
      font-size: 32px;
      color: #1976d2;
      
      &.active {
        color: #2e7d32;
      }
      
      &.inactive {
        color: #c62828;
      }
    }

    .stat-content {
      display: flex;
      flex-direction: column;
    }

    .stat-number {
      font-size: 1.5rem;
      font-weight: 600;
      color: #333;
    }    .stat-label {
      font-size: 0.85rem;
      color: #777;
    }

    /* Améliorations pour l'accessibilité et l'UX des boutons */
    button[mat-icon-button] {
      transition: background-color 0.2s, transform 0.2s;
      &:hover {
        background-color: rgba(0,0,0,0.08);
        transform: scale(1.1);
      }
      &:focus {
        outline: 2px solid #1976d2;
        outline-offset: 2px;
      }
    }

    button[mat-raised-button] {
      transition: box-shadow 0.2s, background-color 0.2s;
      &:hover {
        box-shadow: 0 4px 12px rgba(0,0,0,0.2);
      }
      &:focus {
        outline: 2px solid #1976d2;
        outline-offset: 2px;
      }
    }
  `]
})
export class UsersPageComponent implements OnInit, OnDestroy, AfterViewInit {

  // Table configuration
  displayedColumns: string[] = ['id', 'fullName', 'email', 'phone', 'status', 'roles', 'actions'];
  dataSource = new MatTableDataSource<UserDto>([]);
  
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  // Form controls
  searchControl = new FormControl('');
  statusControl = new FormControl('');

  // État
  isLoading = false;
  totalUsers = 0;
  activeUsers = 0;
  inactiveUsers = 0;
  
  private destroy$ = new Subject<void>();
  private readonly availableRoles = ['admin', 'rh', 'manager', 'user'];

  constructor(
    private usersService: UserService,
    private userManagementGuard: UserManagementGuard,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.setupSearchAndFilters();
    this.loadInitialData();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    
    // Configuration du tri personnalisé
    this.dataSource.sortingDataAccessor = (user: UserDto, property: string) => {
      switch (property) {
        case 'fullName':
          return `${user.lastName} ${user.firstName}`.toLowerCase();
        case 'isActive':
          return user.isActive ? 1 : 0;
        default:
          return (user as any)[property];
      }
    };
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSearchAndFilters(): void {
    // Combinaison de la recherche et des filtres
    merge(
      this.searchControl.valueChanges.pipe(
        startWith(''),
        debounceTime(300),
        distinctUntilChanged()
      ),
      this.statusControl.valueChanges.pipe(
        startWith('')
      )
    ).pipe(
      takeUntil(this.destroy$),
      switchMap(() => this.loadUsersData())
    ).subscribe();
  }

  private loadInitialData(): void {
    this.loadUsersData().pipe(
      takeUntil(this.destroy$)
    ).subscribe();
  }

  private loadUsersData(): Observable<any> {
    this.isLoading = true;
    const searchParams: UserSearchParams = {
      search: this.searchControl.value || '',
      isActive: this.statusControl.value === '' ? undefined : this.statusControl.value === 'true',
      limit: 1000 // Charger tous pour la demo
    };
    return this.usersService.getUsers(searchParams).pipe(
      map(response => {
        this.handleUserResponse(response);
        this.isLoading = false;
        return response;
      }),
      catchError(error => {
        this.isLoading = false;
        this.showError('Erreur lors du chargement des utilisateurs: ' + error.message);
        return of(null);
      })
    );
  }

  handleUserResponse(response: any) {
    if (response && response.users) {
      this.dataSource.data = response.users;
      this.updateStats(response.users);
    }
  }

  private updateStats(users: UserDto[]): void {
    this.totalUsers = users.length;
    this.activeUsers = users.filter(user => user.isActive).length;
    this.inactiveUsers = users.filter(user => !user.isActive).length;
  }

  deleteUser(user: UserDto): void {
    if (confirm(`Êtes-vous sûr de vouloir supprimer l'utilisateur ${user.firstName} ${user.lastName} ?`)) {
      this.usersService.delete(user.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.showSuccess('Utilisateur supprimé avec succès');
            this.refreshData();
          },
          error: (error: any) => {
            this.showError('Erreur lors de la suppression: ' + error.message);
          }
        });
    }
  }

  refreshData(): void {
    this.loadUsersData().pipe(
      takeUntil(this.destroy$)
    ).subscribe();
  }

  getNoDataMessage(): string {
    if (this.searchControl.value || this.statusControl.value) {
      return 'Aucun utilisateur ne correspond aux critères de recherche';
    }
    return 'Aucun utilisateur trouvé';
  }

  private showSuccess(message: string): void {
    this.snackBar.open(message, 'Fermer', {
      duration: 4000,
      panelClass: ['success-snackbar']
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Fermer', {
      duration: 6000,
      panelClass: ['error-snackbar']
    });
  }
}
