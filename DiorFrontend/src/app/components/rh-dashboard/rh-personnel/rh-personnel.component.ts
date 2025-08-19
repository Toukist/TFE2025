import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';

import { UserService } from '../../../services/user.service';
import { TeamService } from '../../../services/team.service';
import { UserDto } from '../../../models/user.model';
import { TeamDto } from '../../../models/TeamDto';

@Component({
  selector: 'app-rh-personnel',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="rh-personnel-container">
      <header class="page-header">
        <h1>Gestion du Personnel</h1>
        <p>Vue d'ensemble des employés et équipes</p>
        <nav class="breadcrumb">
          <a routerLink="/rh">Tableau de Bord RH</a> > Personnel
        </nav>
      </header>

      <div class="content-grid">
        <!-- Section Employés -->
        <section class="employees-section">
          <h2>Employés ({{ (users$ | async)?.length || 0 }})</h2>
          <div class="filter-bar">
            <input type="text" placeholder="Rechercher un employé..." #searchInput (input)="filterUsers(searchInput.value)">
            <select (change)="filterByTeam($event)">
              <option value="">Toutes les équipes</option>
              <option *ngFor="let team of teams$ | async" [value]="team.id">{{ team.name }}</option>
            </select>
          </div>

          <div class="employees-grid" *ngIf="filteredUsers$ | async as users">
            <div *ngFor="let user of users" class="employee-card">              <div class="avatar">
                <span>{{ getInitials(user.firstName, user.lastName) }}</span>
              </div>
              <div class="employee-info">
                <h3>{{ user.firstName }} {{ user.lastName }}</h3>
                <p class="email">{{ user.email }}</p>
                <p class="team">{{ user.teamName || 'Aucune équipe' }}</p>
                <span class="status" [class.active]="user.isActive" [class.inactive]="!user.isActive">
                  {{ user.isActive ? 'Actif' : 'Inactif' }}
                </span>
              </div>
              <div class="actions">
                <button class="btn-edit">Modifier</button>
                <button class="btn-view">Voir</button>
              </div>
            </div>
          </div>
        </section>

        <!-- Section Équipes -->
        <section class="teams-section">
          <h2>Équipes ({{ (teams$ | async)?.length || 0 }})</h2>
          <div class="teams-list" *ngIf="teams$ | async as teams">
            <div *ngFor="let team of teams" class="team-card">
              <h3>{{ team.name }}</h3>
              <p>{{ team.description || 'Aucune description' }}</p>
              <div class="team-stats">
                <span class="member-count">{{ getTeamMemberCount(team) }} membres</span>
              </div>              <div class="team-members">
                <div *ngFor="let member of getTeamMembers(team) | slice:0:5" class="member-avatar" [title]="member.firstName + ' ' + member.lastName">
                  <span>{{ getInitials(member.firstName, member.lastName) }}</span>
                </div>
                <span *ngIf="getTeamMemberCount(team) > 5" class="more-members">+{{ getTeamMemberCount(team) - 5 }}</span>
              </div>
            </div>
          </div>
        </section>
      </div>
    </div>
  `,
  styles: [`
    .rh-personnel-container {
      padding: 2rem;
      max-width: 1200px;
      margin: 0 auto;
    }

    .page-header {
      margin-bottom: 2rem;
      h1 { color: #333; margin-bottom: 0.5rem; }
      p { color: #666; margin-bottom: 1rem; }
      .breadcrumb a { color: #ff9800; text-decoration: none; }
    }

    .content-grid {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 2rem;
    }

    .employees-section, .teams-section {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .filter-bar {
      display: flex;
      gap: 1rem;
      margin-bottom: 1.5rem;
      input, select {
        padding: 0.5rem;
        border: 1px solid #ddd;
        border-radius: 4px;
      }
      input { flex: 1; }
    }

    .employees-grid {
      display: grid;
      gap: 1rem;
    }

    .employee-card {
      display: flex;
      align-items: center;
      padding: 1rem;
      border: 1px solid #eee;
      border-radius: 8px;
      transition: box-shadow 0.2s;
    }
    .employee-card:hover {
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
    }

    .avatar {
      width: 50px;
      height: 50px;
      border-radius: 50%;
      background: #ff9800;
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
      margin-right: 1rem;
      font-weight: bold;
      img { width: 100%; height: 100%; border-radius: 50%; object-fit: cover; }
    }

    .employee-info {
      flex: 1;
      h3 { margin: 0 0 0.25rem 0; color: #333; }
      .email { color: #666; font-size: 0.9rem; margin: 0; }
      .team { color: #888; font-size: 0.85rem; margin: 0.25rem 0; }
      .status {
        font-size: 0.8rem;
        padding: 0.2rem 0.5rem;
        border-radius: 12px;
        &.active { background: #e8f5e8; color: #2e7d32; }
        &.inactive { background: #ffebee; color: #c62828; }
      }
    }

    .actions {
      display: flex;
      gap: 0.5rem;
      button {
        padding: 0.5rem 1rem;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-size: 0.85rem;
      }
      .btn-edit { background: #2196f3; color: white; }
      .btn-view { background: #f5f5f5; color: #333; }
    }

    .teams-list {
      display: grid;
      gap: 1rem;
    }

    .team-card {
      padding: 1.5rem;
      border: 1px solid #eee;
      border-radius: 8px;
      h3 { margin: 0 0 0.5rem 0; color: #333; }
      p { color: #666; margin: 0 0 1rem 0; font-size: 0.9rem; }
    }

    .team-stats {
      margin-bottom: 1rem;
      .member-count {
        background: #e3f2fd;
        color: #1976d2;
        padding: 0.25rem 0.5rem;
        border-radius: 12px;
        font-size: 0.8rem;
      }
    }

    .team-members {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      .member-avatar {
        width: 32px;
        height: 32px;
        border-radius: 50%;
        background: #009688;
        color: white;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 0.75rem;
        img { width: 100%; height: 100%; border-radius: 50%; object-fit: cover; }
      }
      .more-members {
        font-size: 0.8rem;
        color: #666;
      }
    }

    @media (max-width: 768px) {
      .content-grid {
        grid-template-columns: 1fr;
      }
      .filter-bar {
        flex-direction: column;
      }
    }
  `]
})
export class RhPersonnelComponent implements OnInit {
  private userService = inject(UserService);
  private teamService = inject(TeamService);

  users$: Observable<UserDto[]> = this.userService.getAll();
  teams$: Observable<TeamDto[]> = this.teamService.getAllTeams();
  filteredUsers$: Observable<UserDto[]> = this.users$; // même flux après enrichissement éventuel

  allUsers: UserDto[] = [];

  ngOnInit() {
    this.users$.subscribe(users => {
      this.allUsers = users.map(u => ({
        ...u,
        status: u.status || (u.isActive ? 'online' : 'offline')
      }));
      this.filteredUsers$ = this.users$;
    });
  }

  getInitials(firstName?: string, lastName?: string): string {
    const first = firstName ? firstName[0] : '';
    const last = lastName ? lastName[0] : '';
    return `${first}${last}`.toUpperCase();
  }

  filterUsers(searchTerm: string) {
    // Implémentation simple du filtre
    // Dans un vrai projet, vous pourriez utiliser des opérateurs RxJS pour une recherche plus sophistiquée
    console.log('Filtrage par:', searchTerm);
  }

  filterByTeam(event: any) {
    const teamId = event.target.value;
    console.log('Filtrage par équipe:', teamId);
  }  getTeamMembers(team: TeamDto): UserDto[] {
    return this.allUsers.filter(user => user.teamId === team.id);
  }

  getTeamMemberCount(team: TeamDto): number {
    return this.getTeamMembers(team).length;
  }
}
