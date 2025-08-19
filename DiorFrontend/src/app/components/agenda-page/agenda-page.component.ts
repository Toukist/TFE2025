import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

// Interface pour les événements du calendrier
interface CalendarEvent {
  id: string;
  text: string;
  start: string;
  end: string;
  resource?: string;
  backColor?: string;
  borderColor?: string;
  fontColor?: string;
  toolTip?: string;
}

@Component({
  selector: 'app-agenda-page',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="agenda-container">
      <!-- En-tête avec navigation -->
      <div class="agenda-header">
        <h1>📅 Mon Agenda</h1>        <div class="agenda-controls">
          <button (click)="naviguerSemainePrecedente()" class="btn btn-secondary">
            ← Semaine précédente
          </button>
          <button (click)="naviguerAujourdhui()" class="btn btn-primary">
            Aujourd'hui
          </button>
          <button (click)="naviguerSemaineSuivante()" class="btn btn-secondary">
            Semaine suivante →
          </button>
          <button (click)="rafraichir()" class="btn btn-outline">
            🔄 Rafraîchir
          </button>
        </div>
      </div>

      <!-- Indicateur de chargement -->
      <div *ngIf="loading" class="loading-indicator">
        <div class="spinner"></div>
        Chargement de votre agenda...
      </div>      <!-- Affichage des erreurs -->
      <div *ngIf="error" class="error-message">
        ⚠️ {{ error }}
      </div>

      <!-- Vue calendrier simplifiée -->
      <div *ngIf="!loading" class="calendar-view">
        <div class="calendar-header">
          <h2>{{ getSemaineCourante() }}</h2>
        </div>
        
        <div class="calendar-grid">
          <div class="day-header" *ngFor="let jour of joursAffichage">
            <strong>{{ jour.nom }}</strong>
            <span class="date">{{ jour.date }}</span>
          </div>
          
          <div class="day-column" *ngFor="let jour of joursAffichage">
            <div class="events-container">
              <div 
                *ngFor="let event of getEvenementsJour(jour.dateComplete)" 
                class="event-card"
                [style.background-color]="event.backColor || '#3788d8'"
                (click)="modifierEvenement(event)">
                <div class="event-title">{{ event.text }}</div>
                <div class="event-time">{{ formatHeureEvent(event.start) }} - {{ formatHeureEvent(event.end) }}</div>
              </div>
              
              <button 
                (click)="nouvelEvenement(jour.dateComplete)" 
                class="add-event-btn">
                ➕ Ajouter
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Liste des événements -->
      <div *ngIf="!loading && events.length > 0" class="events-list">
        <h3>📋 Tous mes événements</h3>
        <div class="event-item" *ngFor="let event of events">
          <div class="event-info">
            <strong>{{ event.text }}</strong>
            <span class="event-date">{{ formatDateEvent(event.start) }}</span>
            <span class="event-duration">{{ formatDureeEvent(event.start, event.end) }}</span>
          </div>
          <div class="event-actions">
            <button (click)="modifierEvenement(event)" class="btn btn-small btn-secondary">
              ✏️ Modifier
            </button>
            <button (click)="supprimerEvenement(event)" class="btn btn-small btn-danger">
              🗑️ Supprimer
            </button>
          </div>
        </div>
      </div>

      <!-- Message si pas d'événements -->
      <div *ngIf="!loading && events.length === 0" class="empty-state">
        <h3>📭 Aucun événement</h3>
        <p>Votre agenda est vide. Créez votre premier événement !</p>
        <button (click)="nouvelEvenementAujourdhui()" class="btn btn-primary">
          ➕ Créer un événement
        </button>
      </div>

      <!-- Instructions -->
      <div class="instructions">
        <h3>💡 Instructions :</h3>
        <ul>
          <li>Cliquez sur "Ajouter" dans un jour pour créer un nouvel événement</li>
          <li>Cliquez sur un événement pour le modifier</li>
          <li>Utilisez les boutons de navigation pour changer de semaine</li>
          <li>Le bouton "Rafraîchir" recharge les données de démonstration</li>
        </ul>
      </div>
    </div>
  `,
  styles: [`
    .agenda-container {
      padding: 20px;
      max-width: 1400px;
      margin: 0 auto;
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    }

    .agenda-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
      padding: 25px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      border-radius: 15px;
      color: white;
      box-shadow: 0 8px 25px rgba(0,0,0,0.15);
    }

    .agenda-header h1 {
      margin: 0;
      font-size: 2.2rem;
      font-weight: 700;
    }

    .agenda-controls {
      display: flex;
      gap: 12px;
    }

    .btn {
      padding: 10px 18px;
      border: none;
      border-radius: 8px;
      cursor: pointer;
      font-size: 14px;
      font-weight: 500;
      transition: all 0.3s ease;
      text-decoration: none;
      display: inline-flex;
      align-items: center;
      gap: 5px;
    }

    .btn-primary {
      background: #007bff;
      color: white;
    }

    .btn-primary:hover {
      background: #0056b3;
      transform: translateY(-1px);
    }

    .btn-secondary {
      background: rgba(255,255,255,0.2);
      color: white;
      border: 1px solid rgba(255,255,255,0.3);
    }

    .btn-secondary:hover {
      background: rgba(255,255,255,0.3);
    }

    .btn-outline {
      background: transparent;
      border: 1px solid rgba(255,255,255,0.5);
      color: white;
    }

    .btn-outline:hover {
      background: rgba(255,255,255,0.2);
    }

    .btn-small {
      padding: 6px 12px;
      font-size: 12px;
    }

    .btn-danger {
      background: #dc3545;
      color: white;
    }

    .btn-danger:hover {
      background: #c82333;
    }

    .loading-indicator {
      text-align: center;
      padding: 60px;
      font-size: 1.2rem;
      color: #666;
    }

    .spinner {
      width: 40px;
      height: 40px;
      border: 4px solid #f3f3f3;
      border-top: 4px solid #3498db;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      margin: 0 auto 20px;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .error-message {
      background: #f8d7da;
      color: #721c24;
      padding: 20px;
      border-radius: 8px;
      margin-bottom: 20px;
      border: 1px solid #f5c6cb;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .calendar-view {
      background: white;
      border-radius: 15px;
      box-shadow: 0 5px 20px rgba(0,0,0,0.1);
      overflow: hidden;
      margin-bottom: 30px;
    }

    .calendar-header {
      background: #f8f9fa;
      padding: 20px;
      text-align: center;
      border-bottom: 1px solid #dee2e6;
    }

    .calendar-header h2 {
      margin: 0;
      color: #495057;
      font-size: 1.5rem;
    }

    .calendar-grid {
      display: grid;
      grid-template-columns: repeat(7, 1fr);
      min-height: 400px;
    }

    .day-header {
      background: #343a40;
      color: white;
      padding: 15px 10px;
      text-align: center;
      font-weight: 600;
    }

    .day-header .date {
      display: block;
      font-size: 0.9rem;
      opacity: 0.8;
      margin-top: 5px;
    }

    .day-column {
      border-right: 1px solid #dee2e6;
      border-bottom: 1px solid #dee2e6;
      min-height: 200px;
      position: relative;
    }

    .events-container {
      padding: 10px;
      height: 100%;
    }

    .event-card {
      background: #3788d8;
      color: white;
      padding: 8px;
      border-radius: 6px;
      margin-bottom: 8px;
      cursor: pointer;
      font-size: 0.85rem;
      transition: transform 0.2s ease;
    }

    .event-card:hover {
      transform: scale(1.02);
    }

    .event-title {
      font-weight: 600;
      margin-bottom: 2px;
    }

    .event-time {
      font-size: 0.75rem;
      opacity: 0.9;
    }

    .add-event-btn {
      background: transparent;
      border: 2px dashed #ccc;
      color: #666;
      padding: 8px;
      border-radius: 6px;
      cursor: pointer;
      width: 100%;
      font-size: 0.8rem;
      transition: all 0.3s ease;
    }

    .add-event-btn:hover {
      border-color: #007bff;
      color: #007bff;
      background: rgba(0,123,255,0.1);
    }

    .events-list {
      background: white;
      border-radius: 15px;
      padding: 25px;
      box-shadow: 0 5px 20px rgba(0,0,0,0.1);
      margin-bottom: 30px;
    }

    .events-list h3 {
      margin-top: 0;
      color: #333;
      border-bottom: 2px solid #007bff;
      padding-bottom: 10px;
    }

    .event-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 15px;
      border: 1px solid #e9ecef;
      border-radius: 8px;
      margin-bottom: 10px;
      transition: all 0.3s ease;
    }

    .event-item:hover {
      box-shadow: 0 3px 10px rgba(0,0,0,0.1);
      transform: translateY(-1px);
    }

    .event-info strong {
      display: block;
      color: #333;
      margin-bottom: 5px;
    }

    .event-date, .event-duration {
      font-size: 0.9rem;
      color: #666;
      margin-right: 15px;
    }

    .event-actions {
      display: flex;
      gap: 8px;
    }

    .empty-state {
      text-align: center;
      padding: 60px;
      background: white;
      border-radius: 15px;
      box-shadow: 0 5px 20px rgba(0,0,0,0.1);
      margin-bottom: 30px;
    }

    .empty-state h3 {
      margin-top: 0;
      color: #666;
    }

    .instructions {
      background: #f8f9fa;
      border-radius: 15px;
      padding: 25px;
      border-left: 4px solid #007bff;
    }

    .instructions h3 {
      margin-top: 0;
      color: #333;
    }

    .instructions ul {
      margin: 15px 0 0 0;
      padding-left: 20px;
    }

    .instructions li {
      margin: 8px 0;
      color: #666;
      line-height: 1.5;
    }

    @media (max-width: 768px) {
      .agenda-header {
        flex-direction: column;
        gap: 20px;
        text-align: center;
      }

      .agenda-controls {
        flex-wrap: wrap;
        justify-content: center;
      }

      .calendar-grid {
        grid-template-columns: 1fr;
      }

      .day-header {
        display: none;
      }

      .day-column {
        border: 1px solid #dee2e6;
        margin-bottom: 10px;
        border-radius: 8px;
      }

      .day-column::before {
        content: attr(data-day);
        display: block;
        background: #343a40;
        color: white;
        padding: 10px;
        font-weight: 600;
        text-align: center;
      }

      .event-item {
        flex-direction: column;
        align-items: flex-start;
        gap: 10px;
      }

      .event-actions {
        width: 100%;
        justify-content: flex-end;
      }
    }
  `]
})
export class AgendaPageComponent implements OnInit {
  private auth = inject(AuthService);

  events: CalendarEvent[] = [];
  loading = true;
  error: string | null = null;
  semaineCourante = new Date();
  joursAffichage: any[] = [];

  ngOnInit(): void {
    console.log('🚀 [AgendaPageComponent] Initialisation');
    this.initialiserCalendrier();
    this.chargerEvenements();
  }

  /**
   * Initialise l'affichage du calendrier
   */
  private initialiserCalendrier(): void {
    this.joursAffichage = this.genererJoursSemaine(this.semaineCourante);
  }

  /**
   * Génère les jours de la semaine à afficher
   */
  private genererJoursSemaine(date: Date): any[] {
    const jours = [];
    const lundiSemaine = this.getLundiSemaine(date);
    
    const nomsJours = ['Lundi', 'Mardi', 'Mercredi', 'Jeudi', 'Vendredi', 'Samedi', 'Dimanche'];
    
    for (let i = 0; i < 7; i++) {
      const jour = new Date(lundiSemaine);
      jour.setDate(lundiSemaine.getDate() + i);
      
      jours.push({
        nom: nomsJours[i],
        date: jour.getDate().toString().padStart(2, '0'),
        dateComplete: jour.toISOString().split('T')[0]
      });
    }
    
    return jours;
  }

  /**
   * Obtient le lundi de la semaine d'une date donnée
   */
  private getLundiSemaine(date: Date): Date {
    const jour = new Date(date);
    const jourSemaine = jour.getDay();
    const diff = jourSemaine === 0 ? -6 : 1 - jourSemaine; // Ajustement pour que lundi = 0
    jour.setDate(jour.getDate() + diff);
    return jour;
  }  /**
   * Charge les événements du calendrier pour l'utilisateur connecté
   * Version démonstration avec données statiques
   */
  private async chargerEvenements(): Promise<void> {
    this.loading = true;
    this.error = null;

    try {
      console.log('📅 [AgendaPageComponent] Chargement des événements (mode démonstration)');
      
      // Simuler un petit délai de chargement pour l'UX
      await new Promise(resolve => setTimeout(resolve, 500));
      
      // Charger directement les données de démonstration
      this.chargerDonneesDemo();
      
      console.log(`✅ [AgendaPageComponent] ${this.events.length} événements de démonstration chargés`);

    } catch (error) {
      console.error('❌ [AgendaPageComponent] Erreur lors du chargement des événements:', error);
      this.error = "Erreur lors du chargement des événements de démonstration";
      this.events = [];
    } finally {
      this.loading = false;
    }
  }
  /**
   * Charge des données de démonstration réalistes
   */
  chargerDonneesDemo(): void {
    console.log('🎭 [AgendaPageComponent] Chargement des données de démonstration');
    this.error = null;
    
    const aujourd_hui = new Date();
    const hier = new Date(aujourd_hui);
    hier.setDate(aujourd_hui.getDate() - 1);
    const demain = new Date(aujourd_hui);
    demain.setDate(aujourd_hui.getDate() + 1);
    const aprèsDemain = new Date(aujourd_hui);
    aprèsDemain.setDate(aujourd_hui.getDate() + 2);
    const dansTroisJours = new Date(aujourd_hui);
    dansTroisJours.setDate(aujourd_hui.getDate() + 3);
    
    this.events = [
      // Aujourd'hui
      {
        id: "demo1",
        text: "Réunion équipe projet",
        start: `${aujourd_hui.toISOString().split('T')[0]}T09:00:00`,
        end: `${aujourd_hui.toISOString().split('T')[0]}T10:30:00`,
        backColor: "#3788d8",
        toolTip: "Réunion hebdomadaire de l'équipe projet TFE"
      },
      {
        id: "demo2",
        text: "Point client",
        start: `${aujourd_hui.toISOString().split('T')[0]}T14:00:00`,
        end: `${aujourd_hui.toISOString().split('T')[0]}T15:00:00`,
        backColor: "#28a745",
        toolTip: "Point d'avancement avec le client"
      },
      
      // Hier
      {
        id: "demo3",
        text: "Formation Angular 20",
        start: `${hier.toISOString().split('T')[0]}T13:30:00`,
        end: `${hier.toISOString().split('T')[0]}T17:00:00`,
        backColor: "#dd5500",
        toolTip: "Formation sur les nouveautés Angular 20"
      },
      
      // Demain
      {
        id: "demo4",
        text: "Code Review",
        start: `${demain.toISOString().split('T')[0]}T10:00:00`,
        end: `${demain.toISOString().split('T')[0]}T11:00:00`,
        backColor: "#6f42c1",
        toolTip: "Revue de code du module CRUD Projet"
      },
      {
        id: "demo5",
        text: "Démo stakeholders",
        start: `${demain.toISOString().split('T')[0]}T16:00:00`,
        end: `${demain.toISOString().split('T')[0]}T17:30:00`,
        backColor: "#fd7e14",
        toolTip: "Démonstration de l'avancement aux parties prenantes"
      },
      
      // Après-demain
      {
        id: "demo6",
        text: "Présentation TFE",
        start: `${aprèsDemain.toISOString().split('T')[0]}T11:00:00`,
        end: `${aprèsDemain.toISOString().split('T')[0]}T12:00:00`,
        backColor: "#dc3545",
        toolTip: "Présentation du projet TFE devant le jury"
      },
      
      // Dans 3 jours
      {
        id: "demo7",
        text: "Planification sprint",
        start: `${dansTroisJours.toISOString().split('T')[0]}T09:30:00`,
        end: `${dansTroisJours.toISOString().split('T')[0]}T11:00:00`,
        backColor: "#20c997",
        toolTip: "Planification du prochain sprint de développement"
      },
      {
        id: "demo8",
        text: "Entretien candidat",
        start: `${dansTroisJours.toISOString().split('T')[0]}T15:00:00`,
        end: `${dansTroisJours.toISOString().split('T')[0]}T16:00:00`,
        backColor: "#6c757d",
        toolTip: "Entretien d'embauche pour développeur Angular"
      }
    ];
    
    console.log('✅ [AgendaPageComponent] Données de démonstration chargées');
  }

  /**
   * Gestion des événements utilisateur
   */  modifierEvenement(event: CalendarEvent): void {
    console.log('✏️ [AgendaPageComponent] Modification événement:', event.text);
    
    const nouveauTitre = prompt(`Modifier l'événement:`, event.text);
    if (nouveauTitre && nouveauTitre !== event.text) {
      const index = this.events.findIndex(e => e.id === event.id);
      if (index !== -1) {
        this.events[index].text = nouveauTitre;
        console.log('✅ [AgendaPageComponent] Événement modifié (mode démonstration)');
      }
    }
  }

  supprimerEvenement(event: CalendarEvent): void {
    console.log('🗑️ [AgendaPageComponent] Suppression événement:', event.text);
    
    if (confirm(`Supprimer l'événement "${event.text}" ?`)) {
      this.events = this.events.filter(e => e.id !== event.id);
      console.log('✅ [AgendaPageComponent] Événement supprimé (mode démonstration)');
    }
  }
  nouvelEvenement(date: string): void {
    console.log('➕ [AgendaPageComponent] Nouvel événement pour:', date);
    
    const titre = prompt('Titre de l\'événement:');
    if (titre) {
      const heure = prompt('Heure de début (HH:MM):', '09:00') || '09:00';
      const duree = prompt('Durée en heures:', '1') || '1';
      
      const heureDebut = `${date}T${heure}:00`;
      const heureFin = new Date(`${date}T${heure}:00`);
      heureFin.setHours(heureFin.getHours() + parseInt(duree));
      
      const nouvelEvent: CalendarEvent = {
        id: `new_${Date.now()}`,
        text: titre,
        start: heureDebut,
        end: heureFin.toISOString(),
        backColor: "#3788d8"
      };
      
      this.events = [...this.events, nouvelEvent];
      console.log('✅ [AgendaPageComponent] Événement créé:', titre);
    }
  }

  nouvelEvenementAujourdhui(): void {
    const aujourdhui = new Date().toISOString().split('T')[0];
    this.nouvelEvenement(aujourdhui);
  }

  /**
   * Actions de navigation du calendrier
   */
  naviguerSemainePrecedente(): void {
    this.semaineCourante.setDate(this.semaineCourante.getDate() - 7);
    this.semaineCourante = new Date(this.semaineCourante);
    this.initialiserCalendrier();
  }

  naviguerSemaineSuivante(): void {
    this.semaineCourante.setDate(this.semaineCourante.getDate() + 7);
    this.semaineCourante = new Date(this.semaineCourante);
    this.initialiserCalendrier();
  }

  naviguerAujourdhui(): void {
    this.semaineCourante = new Date();
    this.initialiserCalendrier();
  }
  /**
   * Rafraîchir les données
   */
  rafraichir(): void {
    console.log('🔄 [AgendaPageComponent] Rafraîchissement des données');
    this.chargerEvenements();
  }

  /**
   * Méthodes utilitaires pour le template
   */
  getSemaineCourante(): string {
    const premier = this.joursAffichage[0];
    const dernier = this.joursAffichage[6];
    const mois = this.semaineCourante.toLocaleDateString('fr-FR', { month: 'long', year: 'numeric' });
    return `${premier?.date} - ${dernier?.date} ${mois}`;
  }

  getEvenementsJour(date: string): CalendarEvent[] {
    return this.events.filter(event => event.start.startsWith(date));
  }

  formatHeureEvent(dateTime: string): string {
    return new Date(dateTime).toLocaleTimeString('fr-FR', { 
      hour: '2-digit', 
      minute: '2-digit' 
    });
  }

  formatDateEvent(dateTime: string): string {
    return new Date(dateTime).toLocaleDateString('fr-FR', {
      weekday: 'long',
      day: '2-digit',
      month: 'long',
      year: 'numeric'
    });
  }

  formatDureeEvent(start: string, end: string): string {
    const debut = new Date(start);
    const fin = new Date(end);
    const dureeMs = fin.getTime() - debut.getTime();
    const dureeHeures = Math.round(dureeMs / (1000 * 60 * 60) * 10) / 10;
    return `${dureeHeures}h`;
  }
}
