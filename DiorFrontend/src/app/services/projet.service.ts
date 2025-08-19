import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { ProjetDto, CreateProjetRequest, ApiResponse } from '../models/projet.model';

@Injectable({
  providedIn: 'root'
})
export class ProjetService {
  private readonly apiUrl = '/api/Projet';

  constructor(private http: HttpClient) {}

  /**
   * Récupère tous les projets
   */
  getAll(): Observable<ProjetDto[]> {
    console.log('🔍 [ProjetService] Récupération de tous les projets');
    return this.http.get<ProjetDto[]>(this.apiUrl).pipe(
      tap(projets => console.log(`✅ [ProjetService] ${projets.length} projets récupérés`)),
      catchError(this.handleError)
    );
  }

  /**
   * Récupère un projet par son ID
   */
  getById(id: number): Observable<ProjetDto> {
    console.log(`🔍 [ProjetService] Récupération du projet ${id}`);
    return this.http.get<ProjetDto>(`${this.apiUrl}/${id}`).pipe(
      tap(projet => console.log(`✅ [ProjetService] Projet récupéré:`, projet.nom)),
      catchError(this.handleError)
    );
  }

  /**
   * Crée un nouveau projet
   */
  create(projet: CreateProjetRequest): Observable<ProjetDto> {
    console.log('🆕 [ProjetService] Création d\'un nouveau projet:', projet.nom);
    return this.http.post<ProjetDto>(this.apiUrl, projet).pipe(
      tap(createdProjet => console.log(`✅ [ProjetService] Projet créé avec l'ID:`, createdProjet.id)),
      catchError(this.handleError)
    );
  }

  /**
   * Met à jour un projet existant
   */
  update(id: number, projet: CreateProjetRequest): Observable<ProjetDto> {
    console.log(`🔄 [ProjetService] Mise à jour du projet ${id}:`, projet.nom);
    return this.http.put<ProjetDto>(`${this.apiUrl}/${id}`, projet).pipe(
      tap(updatedProjet => console.log(`✅ [ProjetService] Projet mis à jour:`, updatedProjet.nom)),
      catchError(this.handleError)
    );
  }

  /**
   * Supprime un projet
   */
  delete(id: number): Observable<void> {
    console.log(`🗑️ [ProjetService] Suppression du projet ${id}`);
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => console.log(`✅ [ProjetService] Projet ${id} supprimé avec succès`)),
      catchError(this.handleError)
    );
  }

  /**
   * Récupère les projets d'une équipe spécifique
   */
  getByTeamId(teamId: number): Observable<ProjetDto[]> {
    console.log(`🔍 [ProjetService] Récupération des projets de l'équipe ${teamId}`);
    return this.http.get<ProjetDto[]>(`${this.apiUrl}/team/${teamId}`).pipe(
      tap(projets => console.log(`✅ [ProjetService] ${projets.length} projets trouvés pour l'équipe ${teamId}`)),
      catchError(this.handleError)
    );
  }

  /**
   * Calcule la durée d'un projet en jours
   */
  calculerDureeProjet(projet: ProjetDto): number {
    if (!projet.dateDebut || !projet.dateFin) return 0;
    
    const debut = new Date(projet.dateDebut);
    const fin = new Date(projet.dateFin);
    const diffTime = Math.abs(fin.getTime() - debut.getTime());
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }

  /**
   * Vérifie si un projet est en retard
   */
  estEnRetard(projet: ProjetDto): boolean {
    if (!projet.dateFin) return false;
    
    const maintenant = new Date();
    const dateFin = new Date(projet.dateFin);
    return maintenant > dateFin;
  }

  /**
   * Calcule les jours restants pour un projet
   */
  joursRestants(projet: ProjetDto): number {
    if (!projet.dateFin) return 0;
    
    const maintenant = new Date();
    const dateFin = new Date(projet.dateFin);
    const diffTime = dateFin.getTime() - maintenant.getTime();
    const jours = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return Math.max(0, jours);
  }

  /**
   * Formate une date pour l'affichage
   */
  formaterDate(date: Date | string | undefined): string {
    if (!date) return 'Non définie';
    
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  /**
   * Validation côté client
   */
  validerProjet(projet: CreateProjetRequest): string[] {
    const erreurs: string[] = [];

    if (!projet.nom || projet.nom.trim().length === 0) {
      erreurs.push('Le nom du projet est obligatoire');
    }

    if (projet.nom && projet.nom.length > 100) {
      erreurs.push('Le nom du projet ne peut pas dépasser 100 caractères');
    }

    if (!projet.teamId || projet.teamId <= 0) {
      erreurs.push('Une équipe doit être assignée au projet');
    }

    if (projet.dateDebut && projet.dateFin) {
      const debut = new Date(projet.dateDebut);
      const fin = new Date(projet.dateFin);
      
      if (fin < debut) {
        erreurs.push('La date de fin ne peut pas être antérieure à la date de début');
      }
    }

    if (projet.description && projet.description.length > 1000) {
      erreurs.push('La description ne peut pas dépasser 1000 caractères');
    }

    return erreurs;
  }

  /**
   * Gestion centralisée des erreurs HTTP
   */
  private handleError = (error: HttpErrorResponse): Observable<never> => {
    let errorMessage = 'Une erreur inattendue s\'est produite';

    if (error.error instanceof ErrorEvent) {
      // Erreur côté client
      errorMessage = `Erreur client: ${error.error.message}`;
    } else {
      // Erreur côté serveur
      switch (error.status) {
        case 400:
          errorMessage = error.error?.message || 'Données invalides';
          break;
        case 404:
          errorMessage = 'Projet introuvable';
          break;
        case 409:
          errorMessage = 'Conflit de données';
          break;
        case 500:
          errorMessage = 'Erreur interne du serveur';
          break;
        default:
          errorMessage = `Erreur ${error.status}: ${error.error?.message || 'Erreur inconnue'}`;
      }
    }

    console.error('❌ [ProjetService] Erreur HTTP:', errorMessage, error);
    return throwError(() => new Error(errorMessage));
  }
}
