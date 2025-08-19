import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { ProjetService } from '../../services/projet.service';
import { TeamService } from '../../services/team.service';
import { ProjetDto, CreateProjetRequest } from '../../models/projet.model';
import { TeamDto } from '../../models/TeamDto';

@Component({
  selector: 'app-projet-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="projet-manager">
      <!-- En-tête -->
      <div class="page-header">
        <h1>Gestion des Projets</h1>
        <p>Créez et gérez les projets de vos équipes</p>
        <nav class="breadcrumb">
          <a routerLink="/manager">Dashboard</a>
          <span> > </span>
          <span>Projets</span>
        </nav>
      </div>

      <!-- Statistiques rapides -->
      <div class="stats-cards">
        <div class="stat-card">
          <h3>{{ projets().length }}</h3>
          <p>Projets totaux</p>
        </div>
        <div class="stat-card">
          <h3>{{ getProjetsEnCours() }}</h3>
          <p>En cours</p>
        </div>
        <div class="stat-card">
          <h3>{{ getProjetsEnRetard() }}</h3>
          <p>En retard</p>
        </div>
        <div class="stat-card">
          <h3>{{ (teams() || []).length }}</h3>
          <p>Équipes actives</p>
        </div>
      </div>

      <!-- Formulaire de création/édition -->
      <div class="form-section">
        <div class="section-header">
          <h2>{{ modeEdition() ? 'Modifier le projet' : 'Nouveau projet' }}</h2>
          <button 
            *ngIf="modeEdition()" 
            (click)="annulerEdition()" 
            class="btn btn--secondary">
            Annuler l'édition
          </button>
        </div>

        <form [formGroup]="projetForm" (ngSubmit)="onSubmit()" class="projet-form">
          <div class="form-row">
            <div class="form-group">
              <label for="nom">Nom du projet *</label>
              <input 
                id="nom" 
                type="text" 
                formControlName="nom" 
                placeholder="Nom du projet"
                [class.error]="isFieldInvalid('nom')">
              <div *ngIf="isFieldInvalid('nom')" class="error-text">
                {{ getErrorMessage('nom') }}
              </div>
            </div>

            <div class="form-group">
              <label for="teamId">Équipe assignée *</label>
              <select id="teamId" formControlName="teamId" [class.error]="isFieldInvalid('teamId')">
                <option value="">Sélectionner une équipe</option>
                <option *ngFor="let team of teams()" [value]="team.id">
                  {{ team.name }} ({{ getTeamMemberCount(team) }} membres)
                </option>
              </select>
              <div *ngIf="isFieldInvalid('teamId')" class="error-text">
                {{ getErrorMessage('teamId') }}
              </div>
            </div>
          </div>

          <div class="form-group">
            <label for="description">Description</label>
            <textarea 
              id="description" 
              formControlName="description" 
              placeholder="Description du projet"
              rows="3">
            </textarea>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="dateDebut">Date de début</label>
              <input 
                id="dateDebut" 
                type="date" 
                formControlName="dateDebut">
            </div>

            <div class="form-group">
              <label for="dateFin">Date de fin</label>
              <input 
                id="dateFin" 
                type="date" 
                formControlName="dateFin"
                [class.error]="isFieldInvalid('dateFin')">
              <div *ngIf="isFieldInvalid('dateFin')" class="error-text">
                {{ getErrorMessage('dateFin') }}
              </div>
            </div>
          </div>

          <div class="form-actions">
            <button type="button" (click)="resetForm()" class="btn btn--secondary">
              Réinitialiser
            </button>
            <button 
              type="submit" 
              [disabled]="projetForm.invalid || loading()" 
              class="btn btn--primary">
              <span *ngIf="loading()">⏳</span>
              {{ modeEdition() ? 'Mettre à jour' : 'Créer le projet' }}
            </button>
          </div>
        </form>
      </div>

      <!-- Liste des projets -->
      <div class="projects-section">
        <div class="section-header">
          <h2>Projets existants ({{ projets().length }})</h2>
          <div class="filters">
            <input 
              type="text" 
              placeholder="Rechercher..." 
              [(ngModel)]="searchTerm"
              (input)="filtrerProjets()"
              class="search-input">
            <select [(ngModel)]="filtreEquipe" (change)="filtrerProjets()" class="filter-select">
              <option value="">Toutes les équipes</option>
              <option *ngFor="let team of teams()" [value]="team.id">{{ team.name }}</option>
            </select>
          </div>
        </div>

        <div *ngIf="loading()" class="loading">
          Chargement des projets...
        </div>

        <div *ngIf="!loading() && projets().length === 0" class="empty-state">
          <p>Aucun projet trouvé</p>
        </div>

        <div *ngIf="!loading() && projets().length > 0" class="projects-grid">
          <div *ngFor="let projet of projetsFiltres()" class="project-card">
            <div class="project-header">
              <h3>{{ projet.nom }}</h3>
              <div class="project-status" [class]="getStatusClass(projet)">
                {{ getStatusLabel(projet) }}
              </div>
            </div>

            <div class="project-content">
              <p *ngIf="projet.description" class="description">
                {{ projet.description }}
              </p>
              
              <div class="project-meta">
                <div class="meta-item">
                  <span class="label">Équipe:</span>
                  <span class="value">{{ projet.teamName || 'Non assignée' }}</span>
                </div>
                <div class="meta-item" *ngIf="projet.dateDebut">
                  <span class="label">Début:</span>
                  <span class="value">{{ projetService.formaterDate(projet.dateDebut) }}</span>
                </div>
                <div class="meta-item" *ngIf="projet.dateFin">
                  <span class="label">Fin:</span>
                  <span class="value">{{ projetService.formaterDate(projet.dateFin) }}</span>
                </div>
                <div class="meta-item" *ngIf="projet.dateFin">
                  <span class="label">Jours restants:</span>
                  <span class="value" [class]="getJoursRestantsClass(projet)">
                    {{ projetService.joursRestants(projet) }}
                  </span>
                </div>
              </div>
            </div>

            <div class="project-actions">
              <button (click)="editerProjet(projet)" class="btn btn--small btn--secondary">
                Modifier
              </button>
              <button (click)="supprimerProjet(projet)" class="btn btn--small btn--danger">
                Supprimer
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./projet-list.component.scss']
})
export class ProjetListComponent implements OnInit {
  // Injection des services
  projetService = inject(ProjetService);
  private teamService = inject(TeamService);
  private fb = inject(FormBuilder);

  // État du composant avec signals
  projets = signal<ProjetDto[]>([]);
  teams = signal<TeamDto[]>([]);
  projetsFiltres = signal<ProjetDto[]>([]);
  loading = signal<boolean>(false);
  modeEdition = signal<boolean>(false);
  projetEnEdition = signal<ProjetDto | null>(null);

  // Filtres
  searchTerm = '';
  filtreEquipe = '';

  // Formulaire réactif
  projetForm: FormGroup = this.fb.group({
    nom: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', [Validators.maxLength(1000)]],
    dateDebut: [''],
    dateFin: [''],
    teamId: ['', [Validators.required]]
  });

  ngOnInit(): void {
    console.log('🚀 [ProjetListComponent] Initialisation');
    this.chargerDonnees();
    this.setupValidationPersonnalisee();
  }

  /**
   * Charge les données initiales
   */
  private async chargerDonnees(): Promise<void> {
    this.loading.set(true);
    
    try {
      // Charger les équipes en premier
      const teams = await this.teamService.getAllTeams().toPromise();
      this.teams.set(teams || []);

      // Puis charger les projets
      const projets = await this.projetService.getAll().toPromise();
      this.projets.set(projets || []);
      this.projetsFiltres.set(projets || []);

      console.log('✅ [ProjetListComponent] Données chargées:', {
        projets: projets?.length || 0,
        teams: teams?.length || 0
      });
    } catch (error) {
      console.error('❌ [ProjetListComponent] Erreur de chargement:', error);
    } finally {
      this.loading.set(false);
    }
  }

  /**
   * Configuration de la validation personnalisée
   */
  private setupValidationPersonnalisee(): void {
    // Validation des dates (date fin >= date début)
    this.projetForm.get('dateFin')?.valueChanges.subscribe(() => {
      this.validerDates();
    });

    this.projetForm.get('dateDebut')?.valueChanges.subscribe(() => {
      this.validerDates();
    });
  }

  /**
   * Validation des dates
   */
  private validerDates(): void {
    const dateDebut = this.projetForm.get('dateDebut')?.value;
    const dateFin = this.projetForm.get('dateFin')?.value;

    if (dateDebut && dateFin && new Date(dateFin) < new Date(dateDebut)) {
      this.projetForm.get('dateFin')?.setErrors({ dateInvalide: true });
    } else {
      const erreurs = this.projetForm.get('dateFin')?.errors;
      if (erreurs?.['dateInvalide']) {
        delete erreurs['dateInvalide'];
        this.projetForm.get('dateFin')?.setErrors(
          Object.keys(erreurs).length === 0 ? null : erreurs
        );
      }
    }
  }

  /**
   * Soumission du formulaire
   */
  async onSubmit(): Promise<void> {
    if (this.projetForm.invalid) {
      this.marquerTousLesChamps();
      return;
    }

    this.loading.set(true);
    const formData: CreateProjetRequest = this.projetForm.value;

    try {
      if (this.modeEdition()) {
        // Mise à jour
        const projetEdite = this.projetEnEdition();
        if (projetEdite) {
          await this.projetService.update(projetEdite.id, formData).toPromise();
          console.log('✅ [ProjetListComponent] Projet mis à jour');
        }
      } else {
        // Création
        await this.projetService.create(formData).toPromise();
        console.log('✅ [ProjetListComponent] Projet créé');
      }

      // Recharger les données et réinitialiser
      await this.chargerDonnees();
      this.resetForm();
      this.annulerEdition();

    } catch (error) {
      console.error('❌ [ProjetListComponent] Erreur de soumission:', error);
      alert('Erreur lors de la sauvegarde du projet');
    } finally {
      this.loading.set(false);
    }
  }

  /**
   * Édition d'un projet
   */
  editerProjet(projet: ProjetDto): void {
    console.log('✏️ [ProjetListComponent] Édition du projet:', projet.nom);
    
    this.modeEdition.set(true);
    this.projetEnEdition.set(projet);

    // Remplir le formulaire avec les données du projet
    this.projetForm.patchValue({
      nom: projet.nom,
      description: projet.description || '',
      dateDebut: projet.dateDebut ? this.formatDateForInput(projet.dateDebut) : '',
      dateFin: projet.dateFin ? this.formatDateForInput(projet.dateFin) : '',
      teamId: projet.teamId
    });

    // Scroll vers le formulaire
    document.querySelector('.form-section')?.scrollIntoView({ behavior: 'smooth' });
  }

  /**
   * Annulation de l'édition
   */
  annulerEdition(): void {
    this.modeEdition.set(false);
    this.projetEnEdition.set(null);
    this.resetForm();
  }

  /**
   * Suppression d'un projet
   */
  async supprimerProjet(projet: ProjetDto): Promise<void> {
    const confirmer = confirm(`Êtes-vous sûr de vouloir supprimer le projet "${projet.nom}" ?`);
    
    if (confirmer) {
      this.loading.set(true);

      try {
        await this.projetService.delete(projet.id).toPromise();
        console.log('✅ [ProjetListComponent] Projet supprimé:', projet.nom);
        
        // Recharger les données
        await this.chargerDonnees();

      } catch (error) {
        console.error('❌ [ProjetListComponent] Erreur de suppression:', error);
        alert('Erreur lors de la suppression du projet');
      } finally {
        this.loading.set(false);
      }
    }
  }

  /**
   * Filtrage des projets
   */
  filtrerProjets(): void {
    let filtres = this.projets();

    // Filtre par terme de recherche
    if (this.searchTerm.trim()) {
      const terme = this.searchTerm.toLowerCase();
      filtres = filtres.filter(projet =>
        projet.nom.toLowerCase().includes(terme) ||
        (projet.description || '').toLowerCase().includes(terme) ||
        (projet.teamName || '').toLowerCase().includes(terme)
      );
    }

    // Filtre par équipe
    if (this.filtreEquipe) {
      filtres = filtres.filter(projet => projet.teamId === +this.filtreEquipe);
    }

    this.projetsFiltres.set(filtres);
  }

  /**
   * Réinitialisation du formulaire
   */
  resetForm(): void {
    this.projetForm.reset();
    this.projetForm.markAsUntouched();
  }

  /**
   * Méthodes utilitaires pour le template
   */
  isFieldInvalid(fieldName: string): boolean {
    const field = this.projetForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getErrorMessage(fieldName: string): string {
    const field = this.projetForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return `Le champ ${fieldName} est obligatoire`;
      if (field.errors['maxlength']) return `Trop de caractères`;
      if (field.errors['dateInvalide']) return 'La date de fin doit être postérieure à la date de début';
    }
    return '';
  }

  private marquerTousLesChamps(): void {
    Object.keys(this.projetForm.controls).forEach(key => {
      this.projetForm.get(key)?.markAsTouched();
    });
  }

  getProjetsEnCours(): number {
    return this.projets().filter(p => 
      p.dateDebut && new Date(p.dateDebut) <= new Date() &&
      (!p.dateFin || new Date(p.dateFin) >= new Date())
    ).length;
  }

  getProjetsEnRetard(): number {
    return this.projets().filter(p => this.projetService.estEnRetard(p)).length;
  }

  getTeamMemberCount(team: TeamDto): number {
    // Placeholder - à remplacer par la vraie logique si disponible
    return 0;
  }

  getStatusClass(projet: ProjetDto): string {
    if (this.projetService.estEnRetard(projet)) return 'status-retard';
    if (projet.dateDebut && new Date(projet.dateDebut) <= new Date()) return 'status-en-cours';
    return 'status-planifie';
  }

  getStatusLabel(projet: ProjetDto): string {
    if (this.projetService.estEnRetard(projet)) return 'En retard';
    if (projet.dateDebut && new Date(projet.dateDebut) <= new Date()) return 'En cours';
    return 'Planifié';
  }

  getJoursRestantsClass(projet: ProjetDto): string {
    const jours = this.projetService.joursRestants(projet);
    if (jours <= 0) return 'text-danger';
    if (jours <= 7) return 'text-warning';
    return 'text-success';
  }

  private formatDateForInput(date: Date | string): string {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toISOString().split('T')[0];
  }
}
