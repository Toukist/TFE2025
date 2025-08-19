import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';

import { UserService } from '../../../services/user.service';
import { UserDto } from '../../../models/user.model';

// Interfaces pour les données de formation
interface Formation {
  id: number;
  title: string;
  description: string;
  instructor: string;
  category: 'technique' | 'management' | 'soft-skills' | 'securite';
  duration: number; // en heures
  startDate: string;
  endDate: string;
  maxParticipants: number;
  currentParticipants: number;
  status: 'upcoming' | 'ongoing' | 'completed';
  progress: number; // 0-100
}

interface Certification {
  id: number;
  name: string;
  issuer: string;
  validityPeriod: number; // en mois
  difficulty: 'beginner' | 'intermediate' | 'advanced';
  requiredFormations: number[];
}

interface UserFormationProgress {
  userId: number;
  userName: string;
  completedFormations: number;
  ongoingFormations: number;
  certifications: number;
  totalHours: number;
  avgScore: number;
}

@Component({
  selector: 'app-rh-formation',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="rh-formation-container">
      <header class="page-header">
        <h1>Formation & Développement</h1>
        <p>Gestion des formations et certifications</p>
        <nav class="breadcrumb">
          <a routerLink="/rh">Tableau de Bord RH</a> > Formation
        </nav>
      </header>

      <div class="stats-overview">
        <div class="stat-card active-trainings">
          <div class="stat-icon">📚</div>
          <div class="stat-info">
            <span class="stat-value">{{ activeFormations.length }}</span>
            <span class="stat-label">Formations Actives</span>
          </div>
        </div>
        <div class="stat-card total-participants">
          <div class="stat-icon">👥</div>
          <div class="stat-info">
            <span class="stat-value">{{ getTotalParticipants() }}</span>
            <span class="stat-label">Participants</span>
          </div>
        </div>
        <div class="stat-card certifications">
          <div class="stat-icon">🏆</div>
          <div class="stat-info">
            <span class="stat-value">{{ availableCertifications.length }}</span>
            <span class="stat-label">Certifications</span>
          </div>
        </div>
        <div class="stat-card completion-rate">
          <div class="stat-icon">📈</div>
          <div class="stat-info">
            <span class="stat-value">{{ getCompletionRate() }}%</span>
            <span class="stat-label">Taux Réussite</span>
          </div>
        </div>
      </div>

      <div class="content-layout">
        <!-- Section Formations -->
        <section class="formations-section">
          <div class="section-header">
            <h2>Formations</h2>
            <div class="controls">
              <select (change)="filterByCategory($event)">
                <option value="">Toutes les catégories</option>
                <option value="technique">Technique</option>
                <option value="management">Management</option>
                <option value="soft-skills">Soft Skills</option>
                <option value="securite">Sécurité</option>
              </select>
              <select (change)="filterByStatus($event)">
                <option value="">Tous les statuts</option>
                <option value="upcoming">À venir</option>
                <option value="ongoing">En cours</option>
                <option value="completed">Terminée</option>
              </select>
              <button class="btn-add">+ Nouvelle Formation</button>
            </div>
          </div>

          <div class="formations-grid">
            <div *ngFor="let formation of filteredFormations" class="formation-card" [class]="'status-' + formation.status">
              <div class="formation-header">
                <div class="title-section">
                  <h3>{{ formation.title }}</h3>
                  <span class="category" [class]="'category-' + formation.category">
                    {{ getCategoryLabel(formation.category) }}
                  </span>
                </div>
                <div class="status-section">
                  <span class="status-badge" [class]="'status-' + formation.status">
                    {{ getStatusLabel(formation.status) }}
                  </span>
                </div>
              </div>

              <p class="formation-description">{{ formation.description }}</p>

              <div class="formation-details">
                <div class="detail-item">
                  <span class="label">👨‍🏫 Formateur:</span>
                  <span class="value">{{ formation.instructor }}</span>
                </div>
                <div class="detail-item">
                  <span class="label">⏱️ Durée:</span>
                  <span class="value">{{ formation.duration }}h</span>
                </div>
                <div class="detail-item">
                  <span class="label">📅 Période:</span>
                  <span class="value">{{ formatDateRange(formation.startDate, formation.endDate) }}</span>
                </div>
                <div class="detail-item">
                  <span class="label">👥 Participants:</span>
                  <span class="value">{{ formation.currentParticipants }}/{{ formation.maxParticipants }}</span>
                </div>
              </div>

              <div class="participation-bar" *ngIf="formation.status !== 'upcoming'">
                <div class="progress-container">
                  <div class="progress-bar" [style.width.%]="formation.progress"></div>
                </div>
                <span class="progress-text">{{ formation.progress }}% complété</span>
              </div>

              <div class="formation-actions">
                <button class="btn-details">Détails</button>
                <button class="btn-participants">Participants</button>
                <button *ngIf="formation.status === 'upcoming'" class="btn-edit">Modifier</button>
              </div>
            </div>
          </div>
        </section>

        <!-- Sidebar Droite -->
        <aside class="sidebar">
          <!-- Section Certifications -->
          <div class="certifications-section">
            <h3>Certifications Disponibles</h3>
            <div class="certifications-list">
              <div *ngFor="let cert of availableCertifications" class="certification-item">
                <div class="cert-header">
                  <h4>{{ cert.name }}</h4>
                  <span class="difficulty" [class]="'diff-' + cert.difficulty">
                    {{ getDifficultyLabel(cert.difficulty) }}
                  </span>
                </div>
                <p class="cert-issuer">{{ cert.issuer }}</p>
                <div class="cert-details">
                  <span class="validity">Validité: {{ cert.validityPeriod }} mois</span>
                  <span class="requirements" *ngIf="cert.requiredFormations.length">
                    {{ cert.requiredFormations.length }} formation(s) requise(s)
                  </span>
                </div>
              </div>
            </div>
          </div>

          <!-- Section Top Performers -->
          <div class="top-performers-section">
            <h3>Top Performers</h3>
            <div class="performers-list">
              <div *ngFor="let performer of topPerformers; let i = index" class="performer-item">
                <div class="rank">#{{ i + 1 }}</div>
                <div class="performer-info">
                  <span class="name">{{ performer.userName }}</span>
                  <div class="stats">
                    <span class="formations">{{ performer.completedFormations }} formations</span>
                    <span class="hours">{{ performer.totalHours }}h</span>
                    <span class="score">{{ performer.avgScore }}/10</span>
                  </div>
                </div>
                <div class="badges">
                  <span class="cert-badge" *ngIf="performer.certifications > 0">
                    🏆 {{ performer.certifications }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <!-- Section Prochaines Échéances -->
          <div class="deadlines-section">
            <h3>Prochaines Échéances</h3>
            <div class="deadlines-list">
              <div *ngFor="let deadline of upcomingDeadlines" class="deadline-item">
                <div class="deadline-info">
                  <span class="formation-title">{{ deadline.title }}</span>
                  <span class="deadline-date" [class]="getDeadlineClass(deadline.daysLeft)">
                    {{ deadline.daysLeft > 0 ? deadline.daysLeft + ' jours' : 'Aujourd\'hui' }}
                  </span>
                </div>
                <div class="deadline-type">{{ deadline.type }}</div>
              </div>
            </div>
          </div>
        </aside>
      </div>
    </div>
  `,
  styles: [`
    .rh-formation-container {
      padding: 2rem;
      max-width: 1400px;
      margin: 0 auto;
    }

    .page-header {
      margin-bottom: 2rem;
      h1 { color: #333; margin-bottom: 0.5rem; }
      p { color: #666; margin-bottom: 1rem; }
      .breadcrumb a { color: #ffeb3b; text-decoration: none; }
    }

    .stats-overview {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1.5rem;
      margin-bottom: 2rem;
    }

    .stat-card {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      display: flex;
      align-items: center;
      border-left: 4px solid;

      &.active-trainings { border-left-color: #ff9800; }
      &.total-participants { border-left-color: #2196f3; }
      &.certifications { border-left-color: #ffeb3b; }
      &.completion-rate { border-left-color: #4caf50; }

      .stat-icon {
        font-size: 2rem;
        margin-right: 1rem;
      }

      .stat-info {
        display: flex;
        flex-direction: column;
        .stat-value {
          font-size: 2rem;
          font-weight: bold;
          color: #333;
        }
        .stat-label {
          color: #666;
          font-size: 0.9rem;
        }
      }
    }

    .content-layout {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 2rem;
    }

    .formations-section {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .section-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
      flex-wrap: wrap;
      gap: 1rem;

      h2 { margin: 0; color: #333; }

      .controls {
        display: flex;
        gap: 1rem;
        align-items: center;

        select {
          padding: 0.5rem;
          border: 1px solid #ddd;
          border-radius: 4px;
          background: white;
        }

        .btn-add {
          background: #ffeb3b;
          color: #333;
          border: none;
          padding: 0.5rem 1rem;
          border-radius: 4px;
          cursor: pointer;
          font-weight: 500;
          transition: background 0.2s;

          &:hover { background: #fdd835; }
        }
      }
    }

    .formations-grid {
      display: grid;
      gap: 1.5rem;
    }

    .formation-card {
      border: 1px solid #eee;
      border-radius: 8px;
      padding: 1.5rem;
      transition: all 0.2s;
      border-left: 4px solid;

      &.status-upcoming { border-left-color: #2196f3; }
      &.status-ongoing { border-left-color: #ff9800; }
      &.status-completed { border-left-color: #4caf50; }

      &:hover {
        box-shadow: 0 4px 12px rgba(0,0,0,0.1);
        transform: translateY(-2px);
      }
    }

    .formation-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 1rem;

      .title-section {
        flex: 1;
        
        h3 {
          margin: 0 0 0.5rem 0;
          color: #333;
          font-size: 1.2rem;
        }

        .category {
          padding: 0.25rem 0.5rem;
          border-radius: 12px;
          font-size: 0.75rem;
          font-weight: bold;

          &.category-technique { background: #e3f2fd; color: #1976d2; }
          &.category-management { background: #f3e5f5; color: #7b1fa2; }
          &.category-soft-skills { background: #e8f5e8; color: #2e7d32; }
          &.category-securite { background: #ffebee; color: #c62828; }
        }
      }

      .status-badge {
        padding: 0.25rem 0.75rem;
        border-radius: 12px;
        font-size: 0.8rem;
        font-weight: 500;

        &.status-upcoming { background: #e3f2fd; color: #1976d2; }
        &.status-ongoing { background: #fff3e0; color: #f57c00; }
        &.status-completed { background: #e8f5e8; color: #2e7d32; }
      }
    }

    .formation-description {
      color: #666;
      margin-bottom: 1rem;
      font-size: 0.9rem;
      line-height: 1.4;
    }

    .formation-details {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 0.75rem;
      margin-bottom: 1rem;

      .detail-item {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        font-size: 0.85rem;

        .label {
          color: #777;
          min-width: fit-content;
        }

        .value {
          color: #333;
          font-weight: 500;
        }
      }
    }

    .participation-bar {
      display: flex;
      align-items: center;
      gap: 1rem;
      margin-bottom: 1rem;

      .progress-container {
        flex: 1;
        height: 8px;
        background: #e0e0e0;
        border-radius: 4px;
        overflow: hidden;

        .progress-bar {
          height: 100%;
          background: #ffeb3b;
          transition: width 0.3s ease;
        }
      }

      .progress-text {
        font-size: 0.85rem;
        color: #666;
        min-width: 80px;
      }
    }

    .formation-actions {
      display: flex;
      gap: 0.5rem;

      button {
        padding: 0.5rem 1rem;
        border: 1px solid #ddd;
        border-radius: 4px;
        background: white;
        cursor: pointer;
        font-size: 0.85rem;
        transition: all 0.2s;

        &:hover {
          background: #f5f5f5;
        }

        &.btn-details { border-color: #2196f3; color: #2196f3; }
        &.btn-participants { border-color: #ff9800; color: #ff9800; }
        &.btn-edit { border-color: #4caf50; color: #4caf50; }
      }
    }

    .sidebar {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .certifications-section,
    .top-performers-section,
    .deadlines-section {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);

      h3 {
        margin: 0 0 1rem 0;
        color: #333;
        font-size: 1.1rem;
      }
    }

    .certification-item {
      margin-bottom: 1.5rem;
      padding: 1rem;
      border: 1px solid #eee;
      border-radius: 6px;

      .cert-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 0.5rem;

        h4 {
          margin: 0;
          color: #333;
          font-size: 0.95rem;
        }

        .difficulty {
          padding: 0.2rem 0.4rem;
          border-radius: 8px;
          font-size: 0.7rem;
          font-weight: bold;

          &.diff-beginner { background: #e8f5e8; color: #2e7d32; }
          &.diff-intermediate { background: #fff3e0; color: #f57c00; }
          &.diff-advanced { background: #ffebee; color: #c62828; }
        }
      }

      .cert-issuer {
        color: #666;
        font-size: 0.85rem;
        margin: 0 0 0.5rem 0;
      }

      .cert-details {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
        font-size: 0.8rem;
        color: #777;
      }
    }

    .performer-item {
      display: flex;
      align-items: center;
      padding: 0.75rem;
      border: 1px solid #eee;
      border-radius: 6px;
      margin-bottom: 0.75rem;

      .rank {
        font-weight: bold;
        color: #ffeb3b;
        margin-right: 0.75rem;
        font-size: 1.1rem;
      }

      .performer-info {
        flex: 1;

        .name {
          display: block;
          font-weight: 500;
          color: #333;
          margin-bottom: 0.25rem;
        }

        .stats {
          display: flex;
          gap: 0.5rem;
          font-size: 0.75rem;
          color: #666;
        }
      }

      .badges {
        .cert-badge {
          font-size: 0.8rem;
        }
      }
    }

    .deadline-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.75rem;
      border: 1px solid #eee;
      border-radius: 6px;
      margin-bottom: 0.5rem;

      .deadline-info {
        flex: 1;

        .formation-title {
          display: block;
          font-weight: 500;
          color: #333;
          margin-bottom: 0.25rem;
        }

        .deadline-date {
          font-size: 0.8rem;

          &.urgent { color: #f44336; font-weight: bold; }
          &.warning { color: #ff9800; }
          &.normal { color: #666; }
        }
      }

      .deadline-type {
        font-size: 0.8rem;
        color: #777;
      }
    }

    @media (max-width: 1024px) {
      .content-layout {
        grid-template-columns: 1fr;
      }

      .section-header {
        flex-direction: column;
        align-items: stretch;

        .controls {
          flex-wrap: wrap;
        }
      }

      .formation-details {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class RhFormationComponent implements OnInit {
  private userService = inject(UserService);

  // Données simulées pour les formations
  allFormations: Formation[] = [
    {
      id: 1,
      title: 'Angular Avancé',
      description: 'Formation approfondie sur Angular 18+ avec les dernières fonctionnalités et bonnes pratiques.',
      instructor: 'Marie Techno',
      category: 'technique',
      duration: 40,
      startDate: '2025-07-01',
      endDate: '2025-07-15',
      maxParticipants: 12,
      currentParticipants: 8,
      status: 'upcoming',
      progress: 0
    },
    {
      id: 2,
      title: 'Leadership et Management',
      description: 'Développer ses compétences en leadership et gestion d\'équipe.',
      instructor: 'Pierre Manager',
      category: 'management',
      duration: 24,
      startDate: '2025-06-01',
      endDate: '2025-06-30',
      maxParticipants: 15,
      currentParticipants: 15,
      status: 'ongoing',
      progress: 65
    },
    {
      id: 3,
      title: 'Sécurité RGPD',
      description: 'Formation complète sur la conformité RGPD et la protection des données.',
      instructor: 'Sophie Sécurité',
      category: 'securite',
      duration: 16,
      startDate: '2025-05-01',
      endDate: '2025-05-10',
      maxParticipants: 20,
      currentParticipants: 18,
      status: 'completed',
      progress: 100
    },
    {
      id: 4,
      title: 'Communication Efficace',
      description: 'Améliorer ses compétences en communication orale et écrite.',
      instructor: 'Claire Communication',
      category: 'soft-skills',
      duration: 20,
      startDate: '2025-07-10',
      endDate: '2025-07-25',
      maxParticipants: 10,
      currentParticipants: 6,
      status: 'upcoming',
      progress: 0
    },
    {
      id: 5,
      title: 'DevOps et CI/CD',
      description: 'Mise en place de pipelines DevOps et intégration continue.',
      instructor: 'Jean DevOps',
      category: 'technique',
      duration: 32,
      startDate: '2025-06-15',
      endDate: '2025-07-05',
      maxParticipants: 8,
      currentParticipants: 7,
      status: 'ongoing',
      progress: 40
    }
  ];

  availableCertifications: Certification[] = [
    {
      id: 1,
      name: 'Angular Expert',
      issuer: 'Angular Institute',
      validityPeriod: 24,
      difficulty: 'advanced',
      requiredFormations: [1, 5]
    },
    {
      id: 2,
      name: 'Manager Certifié',
      issuer: 'Management Academy',
      validityPeriod: 36,
      difficulty: 'intermediate',
      requiredFormations: [2, 4]
    },
    {
      id: 3,
      name: 'RGPD Officer',
      issuer: 'Data Protection Board',
      validityPeriod: 12,
      difficulty: 'intermediate',
      requiredFormations: [3]
    }
  ];

  topPerformers: UserFormationProgress[] = [
    {
      userId: 1,
      userName: 'Marie Dubois',
      completedFormations: 8,
      ongoingFormations: 2,
      certifications: 3,
      totalHours: 160,
      avgScore: 9.2
    },
    {
      userId: 2,
      userName: 'Pierre Martin',
      completedFormations: 6,
      ongoingFormations: 1,
      certifications: 2,
      totalHours: 120,
      avgScore: 8.8
    },
    {
      userId: 3,
      userName: 'Sophie Laurent',
      completedFormations: 5,
      ongoingFormations: 3,
      certifications: 1,
      totalHours: 140,
      avgScore: 8.5
    }
  ];

  upcomingDeadlines = [
    { title: 'Angular Avancé', type: 'Début', daysLeft: 15 },
    { title: 'Leadership', type: 'Fin', daysLeft: 5 },
    { title: 'Communication', type: 'Début', daysLeft: 25 }
  ];

  filteredFormations: Formation[] = this.allFormations;
  activeFormations = this.allFormations.filter(f => f.status !== 'completed');

  ngOnInit() {
    // Initialisation
  }

  filterByCategory(event: any) {
    const category = event.target.value;
    this.applyFilters(category, null);
  }

  filterByStatus(event: any) {
    const status = event.target.value;
    this.applyFilters(null, status);
  }

  private applyFilters(category: string | null, status: string | null) {
    this.filteredFormations = this.allFormations.filter(formation => {
      const categoryMatch = !category || formation.category === category;
      const statusMatch = !status || formation.status === status;
      return categoryMatch && statusMatch;
    });
  }

  getTotalParticipants(): number {
    return this.allFormations.reduce((total, f) => total + f.currentParticipants, 0);
  }

  getCompletionRate(): number {
    const completedFormations = this.allFormations.filter(f => f.status === 'completed').length;
    const totalFormations = this.allFormations.length;
    return totalFormations > 0 ? Math.round((completedFormations / totalFormations) * 100) : 0;
  }

  getCategoryLabel(category: string): string {
    const labels: {[key: string]: string} = {
      'technique': 'Technique',
      'management': 'Management',
      'soft-skills': 'Soft Skills',
      'securite': 'Sécurité'
    };
    return labels[category] || category;
  }

  getStatusLabel(status: string): string {
    const labels: {[key: string]: string} = {
      'upcoming': 'À venir',
      'ongoing': 'En cours',
      'completed': 'Terminée'
    };
    return labels[status] || status;
  }

  getDifficultyLabel(difficulty: string): string {
    const labels: {[key: string]: string} = {
      'beginner': 'Débutant',
      'intermediate': 'Intermédiaire',
      'advanced': 'Avancé'
    };
    return labels[difficulty] || difficulty;
  }

  formatDateRange(startDate: string, endDate: string): string {
    const start = new Date(startDate).toLocaleDateString('fr-FR', { 
      day: '2-digit', 
      month: '2-digit' 
    });
    const end = new Date(endDate).toLocaleDateString('fr-FR', { 
      day: '2-digit', 
      month: '2-digit' 
    });
    return `${start} - ${end}`;
  }

  getDeadlineClass(daysLeft: number): string {
    if (daysLeft <= 3) return 'urgent';
    if (daysLeft <= 7) return 'warning';
    return 'normal';
  }
}
