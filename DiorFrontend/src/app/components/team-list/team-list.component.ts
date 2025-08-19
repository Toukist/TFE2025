import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TeamService } from '../../services/team.service';
import { TeamDto } from '../../models/TeamDto';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-team-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div *ngFor="let team of teams$ | async" class="bg-white rounded shadow p-4 flex flex-col">
        <div class="font-bold text-lg mb-2">{{ team.name }}</div>
        <div class="text-gray-600 mb-1">{{ team.description }}</div>
        <div class="text-xs text-gray-400 mb-4">Créée le {{ team.createdAt | date:'dd/MM/yyyy' }}</div>
        <a [routerLink]="['/team', team.id, 'members']" class="mt-auto bg-white hover:bg-gray-100 border border-gray-300 rounded px-3 py-1 text-sm text-blue-600 text-center transition">Voir les membres</a>
      </div>
    </div>
  `
})
export class TeamListComponent {
  teams$: Observable<TeamDto[]>;
  constructor(private teamService: TeamService) {
    this.teams$ = this.teamService.getAllTeams();
  }
}
