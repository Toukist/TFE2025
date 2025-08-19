import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Subject, takeUntil, forkJoin, switchMap } from 'rxjs';
import { AccessCompetencyService } from '../../services/access-competency.service';
import { AccessCompetency } from '../../models/access-competency.model';

interface UserAccessCompetencyDisplay {
  id: number;
  competencyName: string;
  // Ajoutez ici d’autres propriétés si elles existent dans le modèle fusionné
}

/**
 * 🔹 Composant pour gérer les AccessCompetencies d'un utilisateur
 * Permet d'afficher, ajouter et supprimer les compétences d'accès
 */
@Component({
  selector: 'app-access-competency-manager',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './access-competency-manager.component.html',
  styleUrls: ['./access-competency-manager.component.scss']
})
export class AccessCompetencyManagerComponent implements OnInit, OnDestroy {
  @Input() userId!: number;
  @Input() userName = '';

  // Données
  userAccessCompetencies: UserAccessCompetencyDisplay[] = [];
  allAccessCompetencies: AccessCompetency[] = [];
  availableCompetencies: AccessCompetency[] = [];

  // FormControl pour la sélection
  selectedCompetencyControl = new FormControl<number | null>(null);

  // États
  isLoading = false;
  isAdding = false;
  errorMessage = '';
  successMessage = '';

  private destroy$ = new Subject<void>();

  constructor(private accessCompetencyService: AccessCompetencyService) {}

  ngOnInit(): void {
    if (!this.userId) {
      this.errorMessage = 'ID utilisateur requis';
      return;
    }
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Charge toutes les données nécessaires
   */
  loadData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    // Charger toutes les compétences ET les compétences de l'utilisateur en parallèle
    forkJoin({
      allCompetencies: this.accessCompetencyService.getAllAccessCompetencies(),
      userCompetencies: this.accessCompetencyService.getUserAccessCompetencies(this.userId)
    })
    .pipe(
      takeUntil(this.destroy$),
      switchMap(({ allCompetencies, userCompetencies }) => {
        this.allAccessCompetencies = allCompetencies.filter(c => c.isActive);
        
        // Pour chaque UserAccessCompetency, récupérer les détails de la compétence
        const competencyDetailsRequests = userCompetencies.map(uac =>
          this.accessCompetencyService.getAccessCompetencyById(uac.id)
            .pipe(
              takeUntil(this.destroy$)
            )
        );

        if (competencyDetailsRequests.length === 0) {
          this.userAccessCompetencies = [];
          this.updateAvailableCompetencies();
          this.isLoading = false;
          return [];
        }

        return forkJoin(competencyDetailsRequests).pipe(
          takeUntil(this.destroy$)
        );
      })
    )
    .subscribe({
      next: (competencyDetails: AccessCompetency[]) => {
        // Construire la liste des compétences utilisateur avec détails
        this.accessCompetencyService.getUserAccessCompetencies(this.userId)
          .pipe(takeUntil(this.destroy$))
          .subscribe(userCompetencies => {
            this.userAccessCompetencies = userCompetencies.map((uac, index) => ({
              id: uac.id,
              competencyName: competencyDetails[index]?.name || 'Inconnu',
            }));

            this.updateAvailableCompetencies();
            this.isLoading = false;
          });
      },
      error: (error) => {
        console.error('Erreur lors du chargement des données:', error);
        this.errorMessage = 'Erreur lors du chargement des données';
        this.isLoading = false;
      }
    });
  }

  /**
   * Met à jour la liste des compétences disponibles (non encore attribuées)
   */
  updateAvailableCompetencies(): void {
    const assignedCompetencyIds = this.userAccessCompetencies.map(uac => uac.id); // Utiliser id comme clé unique
    this.availableCompetencies = this.allAccessCompetencies.filter(
      comp => !assignedCompetencyIds.includes(comp.id)
    );
  }

  /**
   * Ajoute une compétence d'accès à l'utilisateur
   */
  addCompetency(): void {
    const competencyId = this.selectedCompetencyControl.value;
    if (!competencyId) {
      this.errorMessage = 'Veuillez sélectionner une compétence';
      return;
    }

    this.isAdding = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.accessCompetencyService.addUserAccessCompetency(this.userId, competencyId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Compétence ajoutée avec succès';
          this.selectedCompetencyControl.reset();
          this.loadData(); // Recharger les données
          this.isAdding = false;
        },
        error: (error) => {
          console.error('Erreur lors de l\'ajout:', error);
          this.errorMessage = 'Erreur lors de l\'ajout de la compétence';
          this.isAdding = false;
        }
      });
  }

  /**
   * Supprime une compétence d'accès de l'utilisateur
   */
  removeCompetency(userAccessCompetency: UserAccessCompetencyDisplay): void {
    if (!confirm(`Êtes-vous sûr de vouloir retirer la compétence "${userAccessCompetency.competencyName}" ?`)) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.accessCompetencyService.deleteUserAccessCompetency(userAccessCompetency.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.successMessage = 'Compétence supprimée avec succès';
          this.loadData(); // Recharger les données
        },
        error: (error) => {
          console.error('Erreur lors de la suppression:', error);
          this.errorMessage = 'Erreur lors de la suppression de la compétence';
        }
      });
  }

  /**
   * Formate la date d'attribution
   */
  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR') + ' à ' + date.toLocaleTimeString('fr-FR', { 
      hour: '2-digit', 
      minute: '2-digit' 
    });
  }

  /**
   * Efface les messages
   */
  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  /**
   * TrackBy function pour optimiser le rendu de la liste
   */
  trackByCompetency(index: number, item: UserAccessCompetencyDisplay): number {
    return item.id;
  }

  /**
   * TrackBy function pour optimiser le rendu de la liste des compétences disponibles
   */
  trackByAvailableCompetency(index: number, item: AccessCompetency): number {
    return item.id;
  }
}
