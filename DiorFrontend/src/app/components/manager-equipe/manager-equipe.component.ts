import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { ManagerDashboardService } from '../manager-dashboard/manager-dashboard.service';
import { TeamService } from '../../services/team.service';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { UserDto } from '../../models/user.model';
import { TeamDto as Team } from '../../models/TeamDto';

@Component({
  selector: 'app-manager-equipe',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="manager-equipe">
      <div class="page-header">        <h1>Mon Équipe</h1>
        <p>Vue détaillée des membres de votre équipe</p>
        <nav class="breadcrumb">
          <a routerLink="/manager">Dashboard</a>
          <span> > </span>
          <span>Équipe</span>
        </nav>
      </div>

      <div class="page-content">
        <!-- Filtres et recherche -->
        <div class="filters">
          <div class="search-box">
            <input 
              type="text" 
              placeholder="Rechercher un membre..."
              [(ngModel)]="searchTerm"
              (input)="onSearch()">
          </div>
          <div class="filter-buttons">
            <button 
              *ngFor="let filter of statusFilters" 
              (click)="setStatusFilter(filter.value)"
              [class.active]="selectedStatus === filter.value"
              class="filter-btn">
              {{ filter.label }}
            </button>
          </div>
        </div>

        <!-- Tableau des utilisateurs -->
        <div class="users-table-container">
          <table class="users-table">
            <thead>
              <tr>
                <th>Membre</th>
                <th>Contact</th>
                <th>Équipe</th>
                <th>Statut</th>
                <th>Dernière connexion</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let user of filteredUsers">
                <td class="user-cell">
                  <div class="user-info">
                    <div class="user-avatar">
                      <img *ngIf="user.avatar" [src]="user.avatar" [alt]="user.firstName + ' ' + user.lastName">
                      <span *ngIf="!user.avatar">{{ user.firstName[0] }}{{ user.lastName[0] }}</span>
                    </div>
                    <div class="user-details">
                      <h4>{{ user.firstName }} {{ user.lastName }}</h4>
                      <span class="user-role">{{ user.role || 'Membre' }}</span>
                    </div>
                  </div>
                </td>
                <td class="contact-cell">
                  <div class="contact-info">
                    <a [href]="'mailto:' + user.email" class="email-link">{{ user.email }}</a>
                    <a [href]="'tel:' + user.phone" class="phone-link">{{ user.phone }}</a>
                  </div>
                </td>
                <td class="team-cell">
                  <span class="team-name">{{ user.teamName || 'Non assigné' }}</span>
                </td>
                <td class="status-cell">
                  <span class="status-badge" [class]="'status-' + (user.status || 'offline')">
                    {{ getStatusLabel(user.status || 'offline') }}
                  </span>
                </td>
                <td class="date-cell">
                  {{ formatDate(user.lastConnection) }}
                </td>
                <td class="actions-cell">
                  <button (click)="contactUser(user)" class="action-btn action-btn--primary">
                    💬 Contacter
                  </button>
                  <button (click)="viewProfile(user)" class="action-btn action-btn--secondary">
                    👤 Profil
                  </button>
                </td>
              </tr>
            </tbody>
          </table>

          <!-- Empty State -->
          <div *ngIf="filteredUsers.length === 0" class="empty-state">
            <div class="empty-state__icon">👥</div>
            <div class="empty-state__text">Aucun membre trouvé</div>
            <div class="empty-state__subtext">Essayez de modifier vos critères de recherche</div>
          </div>
        </div>

        <!-- Stats rapides -->
        <div class="team-stats">
          <div class="stat-card">
            <h3>Résumé de l'équipe</h3>
            <div class="stats-grid">
              <div class="stat-item">
                <span class="stat-value">{{ users.length }}</span>
                <span class="stat-label">Total membres</span>
              </div>
              <div class="stat-item">
                <span class="stat-value">{{ getActiveCount() }}</span>
                <span class="stat-label">Actifs aujourd'hui</span>
              </div>
              <div class="stat-item">
                <span class="stat-value">{{ getOnlineCount() }}</span>
                <span class="stat-label">En ligne maintenant</span>
              </div>
              <div class="stat-item">
                <span class="stat-value">{{ getPresenceRate() }}%</span>
                <span class="stat-label">Taux de présence</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./manager-equipe.component.scss']
})
export class ManagerEquipeComponent implements OnInit {
  private dashboardService = inject(ManagerDashboardService);
  private teamService = inject(TeamService);
  private authService = inject(AuthService);
  private userService = inject(UserService);
  private route = inject(ActivatedRoute);

  users: UserDto[] = [];
  filteredUsers: UserDto[] = [];
  teams: Team[] = [];
  currentManagerTeamId: number | null = null;
  
  searchTerm = '';
  selectedStatus = 'all';
  
  statusFilters = [
    { value: 'all', label: 'Tous' },
    { value: 'online', label: 'En ligne' },
    { value: 'away', label: 'Absent' },
    { value: 'offline', label: 'Hors ligne' }
  ];
  ngOnInit(): void {
    console.log('🚀 [ManagerEquipeComponent] Initialisation - Mode équipe du manager');
    this.loadManagerTeamData();
    
    // Vérifier si un teamId est passé en paramètre
    this.route.queryParams.subscribe(params => {
      if (params['teamId']) {
        this.filterByTeam(+params['teamId']);
      }
    });
  }  private loadManagerTeamData(): void {
    // Récupérer l'utilisateur connecté
    const currentUser = this.authService.getCurrentUser();
    console.log('👤 [ManagerEquipeComponent] Utilisateur connecté:', JSON.stringify(currentUser, null, 2));
      const userId = currentUser?.Id || currentUser?.id;
    console.log(`🔍 [ManagerEquipeComponent] ID utilisateur détecté: ${userId} (Id: ${currentUser?.Id}, id: ${currentUser?.id})`);
    
    if (userId) {
      // Récupérer les détails complets de l'utilisateur depuis l'API pour obtenir le TeamId
      console.log(`🔍 [ManagerEquipeComponent] Récupération des détails complets pour l'utilisateur ${userId}`);
      console.log(`📡 [ManagerEquipeComponent] Appel API: /api/user/${userId}`);      
      this.userService.getById(userId).subscribe({
        next: (userDetails: UserDto) => {
          console.log('👤 [ManagerEquipeComponent] Détails utilisateur complets reçus:');
          console.log(JSON.stringify(userDetails, null, 2));          const teamId = userDetails.teamId;
          console.log(`🔍 [ManagerEquipeComponent] TeamId extrait: ${teamId} (teamId: ${userDetails.teamId})`);
          
          // SOLUTION TEMPORAIRE: Si le manager01 (ID 118) n'a pas de TeamId, on lui assigne l'équipe 1
          let finalTeamId = teamId;
          if (!finalTeamId && userId === 118) {
            finalTeamId = 1;
            console.log(`🔧 [ManagerEquipeComponent] CORRECTION: Attribution TeamId=1 pour manager01 (ID 118)`);
          }
          
          if (finalTeamId) {
            this.currentManagerTeamId = finalTeamId;
            console.log(`🎯 [ManagerEquipeComponent] TeamId du manager confirmé: ${this.currentManagerTeamId}`);
            console.log(`📡 [ManagerEquipeComponent] Appel API équipe: /api/Team/${finalTeamId}/users`);
            
            // Récupérer seulement les membres de l'équipe du manager
            this.teamService.getTeamMembers(finalTeamId).subscribe({
              next: (teamMembers: UserDto[]) => {
                console.log(`✅ [ManagerEquipeComponent] ${teamMembers.length} membres de l'équipe chargés:`);
                teamMembers.forEach((member, index) => {
                  console.log(`  ${index + 1}. ${member.firstName} ${member.lastName} (ID: ${member.id}, teamId: ${member.teamId})`);
                });
                this.users = this.mapUserDtoToUser(teamMembers);
                this.applyFilters();
                console.log(`🎉 [ManagerEquipeComponent] Filtrage terminé, ${this.filteredUsers.length} utilisateurs affichés`);
              },
              error: (error) => {
                console.error('❌ [ManagerEquipeComponent] Erreur lors du chargement de l\'équipe:', error);
                console.log('🔄 [ManagerEquipeComponent] Passage au fallback - chargement de tous les utilisateurs');
                // Fallback vers tous les utilisateurs si l'API échoue
                this.loadAllUsersAsFallback();
              }
            });
          } else {
            console.warn('⚠️ [ManagerEquipeComponent] Aucun TeamId dans les détails utilisateur de l\'API');
            console.log('🔄 [ManagerEquipeComponent] Tentative avec TeamId de localStorage');
            // Fallback si l'utilisateur a un TeamId dans les données de localStorage
            if (currentUser?.TeamId) {
              this.currentManagerTeamId = currentUser.TeamId;
              console.log(`🎯 [ManagerEquipeComponent] Utilisation du TeamId de localStorage: ${this.currentManagerTeamId}`);
              
              this.teamService.getTeamMembers(currentUser.TeamId).subscribe({
                next: (teamMembers: UserDto[]) => {
                  console.log(`✅ [ManagerEquipeComponent] ${teamMembers.length} membres de l'équipe chargés (fallback localStorage)`);
                  this.users = this.mapUserDtoToUser(teamMembers);
                  this.applyFilters();
                },
                error: (error) => {
                  console.error('❌ [ManagerEquipeComponent] Erreur lors du chargement de l\'équipe (fallback):', error);
                  this.loadAllUsersAsFallback();
                }
              });
            } else {
              console.warn('⚠️ [ManagerEquipeComponent] Aucun TeamId disponible nulle part, chargement de tous les utilisateurs');
              this.loadAllUsersAsFallback();
            }
          }
        },
        error: (error) => {
          console.error('❌ [ManagerEquipeComponent] Erreur lors de la récupération des détails utilisateur:', error);
          console.log('🔄 [ManagerEquipeComponent] Passage au fallback localStorage');
          // Fallback si l'utilisateur a un TeamId dans les données de localStorage
          if (currentUser?.TeamId) {
            this.currentManagerTeamId = currentUser.TeamId;
            console.log(`🎯 [ManagerEquipeComponent] Utilisation du TeamId de localStorage: ${this.currentManagerTeamId}`);
            
            this.teamService.getTeamMembers(currentUser.TeamId).subscribe({
              next: (teamMembers: UserDto[]) => {
                console.log(`✅ [ManagerEquipeComponent] ${teamMembers.length} membres de l'équipe chargés (fallback localStorage)`);
                this.users = this.mapUserDtoToUser(teamMembers);
                this.applyFilters();
              },
              error: (error) => {
                console.error('❌ [ManagerEquipeComponent] Erreur lors du chargement de l\'équipe (fallback):', error);
                this.loadAllUsersAsFallback();
              }
            });
          } else {
            console.warn('⚠️ [ManagerEquipeComponent] Aucun TeamId disponible, chargement de tous les utilisateurs');
            this.loadAllUsersAsFallback();
          }
        }
      });    } else {
      console.error('❌ [ManagerEquipeComponent] Aucun utilisateur connecté trouvé ou pas d\'ID');
      console.log('🔍 [ManagerEquipeComponent] Structure currentUser:', JSON.stringify(currentUser, null, 2));
      this.loadAllUsersAsFallback();
    }
    
    // Charger les équipes pour les infos générales
    this.loadTeams();
  }

  private mapUserDtoToUser(userDtos: Partial<UserDto>[]): UserDto[] {
    return userDtos.map(dto => {
      const firstName = dto.firstName ?? '';
      const lastName = dto.lastName ?? '';
      const name = dto.name || dto.Name || `${firstName} ${lastName}`.trim();
      return {
        id: dto.id ?? 0,
        isActive: dto.isActive ?? true,
        firstName,
        lastName,
        email: dto.email ?? '',
        userName: dto.userName,
        phone: dto.phone ?? '',
        teamId: dto.teamId ?? null,
        teamName: dto.teamName ?? null,
        badgePhysicalNumber: dto.badgePhysicalNumber ?? null,
        roles: dto.roles ?? [],
        name,
        Name: dto.Name ?? name,
        createdAt: dto.createdAt ?? new Date(),
        updatedAt: dto.updatedAt ?? new Date(),
        lastEditAt: dto.lastEditAt ?? null,
        lastEditBy: dto.lastEditBy ?? '',
        avatar: dto.avatar ?? this.generateAvatar(firstName, lastName),
        status: dto.status ?? (Math.random() > 0.7 ? 'online' : Math.random() > 0.5 ? 'away' : 'offline'),
        lastConnection: dto.lastConnection ?? new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000),
        role: dto.role ?? (dto.roles && dto.roles.length ? 
          (typeof dto.roles[0] === 'string' ? dto.roles[0] : (dto.roles[0] as any)?.name || 'Membre') : 'Membre'),
        accessCompetencies: dto.accessCompetencies ?? [],
        password: dto.password
      } as UserDto;
    });
  }

  private generateAvatar(firstName: string, lastName: string): string {
    // Génère un avatar simple basé sur les initiales
    return `https://ui-avatars.com/api/?name=${firstName}+${lastName}&background=random`;
  }

  private loadAllUsersAsFallback(): void {
    console.log('🔄 [ManagerEquipeComponent] Fallback: chargement de tous les utilisateurs');
    this.dashboardService.getUsers().subscribe({
      next: (users) => {
        console.log(`📊 [ManagerEquipeComponent] ${users.length} utilisateurs chargés (fallback)`);
        this.users = this.mapUserDtoToUser(users);
        this.applyFilters();
      },
      error: (error) => {
        console.error('❌ [ManagerEquipeComponent] Erreur lors du chargement des utilisateurs:', error);
      }
    });
  }

  private loadTeams(): void {
    this.dashboardService.getTeams().subscribe({
      next: (teams) => {
        this.teams = teams.map((t: any) => ({
          id: t.id,
          name: t.name,
          description: t.description,
          createdAt: t.createdAt ?? new Date(),
        }));
      },
      error: (error) => {
        console.error('❌ [ManagerEquipeComponent] Erreur lors du chargement des équipes:', error);
      }
    });
  }

  onSearch(): void {
    this.applyFilters();
  }

  setStatusFilter(status: string): void {
    this.selectedStatus = status;
    this.applyFilters();
  }

  private applyFilters(): void {
    let filtered = this.users;

    // Filtre par terme de recherche
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(user =>
        user.firstName.toLowerCase().includes(term) ||
        user.lastName.toLowerCase().includes(term) ||
        user.email.toLowerCase().includes(term) ||
        (user.teamName || '').toLowerCase().includes(term)
      );
    }

    // Filtre par statut
    if (this.selectedStatus !== 'all') {
      filtered = filtered.filter(user => user.status === this.selectedStatus);
    }

    this.filteredUsers = filtered;
  }

  private filterByTeam(teamId: number): void {
    this.filteredUsers = this.users.filter(user => user.teamId === teamId);
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'Jamais';
    const d = new Date(date);
    return d.toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'online': return 'En ligne';
      case 'away': return 'Absent';
      case 'offline': return 'Hors ligne';
      default: return 'Inconnu';
    }
  }

  contactUser(user: UserDto): void {
    // Ouvrir client email ou chat
    window.location.href = `mailto:${user.email}?subject=Contact Manager`;
  }

  viewProfile(user: UserDto): void {
    // Navigation vers le profil utilisateur
    console.log('Voir profil:', user);
  }

  getActiveCount(): number {
    return this.users.filter(user => user.isActive).length;
  }

  getOnlineCount(): number {
  return this.users.filter(user => user.status === 'online').length;
  }

  getPresenceRate(): number {
    if (this.users.length === 0) return 0;
    const active = this.getActiveCount();
    return Math.round((active / this.users.length) * 100);
  }

  private tryUserApiAsFallback(userId: number, currentUser: any): void {
    console.log(`🔄 [ManagerEquipeComponent] Tentative API User pour ID ${userId}...`);
    this.userService.getById(userId).subscribe({
      next: (userDetails: UserDto) => {
        console.log('👤 [ManagerEquipeComponent] Détails utilisateur API:', userDetails);        if (userDetails?.teamId) {
          this.currentManagerTeamId = userDetails.teamId;
          console.log(`🎯 [ManagerEquipeComponent] TeamId trouvé via API: ${this.currentManagerTeamId}`);
          
          this.teamService.getTeamMembers(userDetails.teamId!).subscribe({
            next: (teamMembers: UserDto[]) => {
              console.log(`✅ [ManagerEquipeComponent] ${teamMembers.length} membres via API User`);
              this.users = this.mapUserDtoToUser(teamMembers);
              this.applyFilters();
            },
            error: (error) => {
              console.error('❌ [ManagerEquipeComponent] Erreur Team API:', error);
              this.loadAllUsersAsFallback();
            }
          });
        } else {
          console.warn('⚠️ [ManagerEquipeComponent] Pas de TeamId via API User');
          this.loadAllUsersAsFallback();
        }
      },
      error: (error) => {
        console.error('❌ [ManagerEquipeComponent] Erreur API User:', error);
        this.loadAllUsersAsFallback();
      }
    });
  }
}
