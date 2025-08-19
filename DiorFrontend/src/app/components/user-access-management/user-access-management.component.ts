import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { UserService } from '../../services/user.service';
import { UserDto } from '../../models/user.model';
import { AccessCompetencyManagerComponent } from '../access-competency-manager/access-competency-manager.component'; // TODO: Vérifier le chemin si erreur

/**
 * 🔹 Composant pour gérer les accès et compétences des utilisateurs
 * Intègre la sélection d'utilisateur et la gestion des compétences
 */
@Component({
  selector: 'app-user-access-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AccessCompetencyManagerComponent],
  templateUrl: './user-access-management.component.html',
  styleUrls: ['./user-access-management.component.scss']
})
export class UserAccessManagementComponent implements OnInit, OnDestroy {
  // Données
  users: UserDto[] = [];
  filteredUsers: UserDto[] = [];
  selectedUser: UserDto | null = null;

  // FormControls
  searchControl = new FormControl('');
  userSelectControl = new FormControl<number | null>(null);

  // États
  isLoadingUsers = false;
  errorMessage = '';

  private destroy$ = new Subject<void>();

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.loadUsers();
    this.setupSearch();
    this.setupUserSelection();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Charge la liste des utilisateurs
   */
  loadUsers(): void {
    this.isLoadingUsers = true;
    this.errorMessage = '';

    this.userService.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (users: UserDto[]) => {
          this.users = users;
          this.filteredUsers = users;
          this.isLoadingUsers = false;
        },
        error: (error: any) => {
          console.error('Erreur lors du chargement des utilisateurs:', error);
          this.errorMessage = 'Erreur lors du chargement des utilisateurs';
          this.isLoadingUsers = false;
        }
      });
  }

  /**
   * Configure la recherche d'utilisateurs
   */
  setupSearch(): void {
    this.searchControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(searchTerm => {
        this.filterUsers(searchTerm || '');
      });
  }

  /**
   * Configure la sélection d'utilisateur
   */
  setupUserSelection(): void {
    this.userSelectControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(userId => {
        if (userId) {
          this.selectedUser = this.users.find(u => u.id === userId) || null;
        } else {
          this.selectedUser = null;
        }
      });
  }
  /**
   * Filtre les utilisateurs selon le terme de recherche
   */
  filterUsers(searchTerm: string): void {
    if (!searchTerm.trim()) {
      this.filteredUsers = this.users;
      return;
    }

    const term = searchTerm.toLowerCase();
    this.filteredUsers = this.users.filter(user =>
      (user.firstName && user.lastName && `${user.firstName} ${user.lastName}`.toLowerCase().includes(term)) ||
      (user.firstName && user.firstName.toLowerCase().includes(term)) ||
      (user.lastName && user.lastName.toLowerCase().includes(term))
    );
  }

  /**
   * Sélectionne un utilisateur depuis la liste
   */
  selectUserFromList(user: UserDto): void {
    this.userSelectControl.setValue(user.id);
    this.searchControl.setValue('');
  }

  /**
   * Désélectionne l'utilisateur actuel
   */
  clearSelection(): void {
    this.userSelectControl.setValue(null);
    this.selectedUser = null;
  }
  /**
   * Formate le nom complet de l'utilisateur
   */
  getFullUserName(user: UserDto): string {
    if (user.firstName && user.lastName) {
      return `${user.firstName} ${user.lastName}`;
    }
    return user.firstName || user.lastName || '';
  }

  /**
   * TrackBy function pour optimiser le rendu de la liste
   */
  trackByUser(index: number, user: UserDto): number {
    return user.id;
  }
}
