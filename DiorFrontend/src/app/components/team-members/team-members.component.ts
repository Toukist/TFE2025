import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { TeamService } from '../../services/team.service';
import { UserDto } from '../../models/user.model';
import { Observable, switchMap } from 'rxjs';

@Component({
  selector: 'app-team-members',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="mb-4 text-xl font-bold">Membres de l'équipe</div>
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div *ngFor="let user of members$ | async" class="bg-white rounded shadow p-4">
        <div class="font-semibold">{{ user.firstName }} {{ user.lastName }}</div>
        <div class="text-gray-600 text-sm mb-1">Identifiant : {{ user.firstName }} {{ user.lastName }}</div>
        <div class="text-xs text-gray-400 mb-1">Téléphone : {{ user.phone }}</div>
        <div class="text-xs text-gray-400">Email : {{ user.email }}</div>
      </div>
    </div>
  `
})
export class TeamMembersComponent {
  members$: Observable<UserDto[]>;
  constructor(private route: ActivatedRoute, private teamService: TeamService) {
    this.members$ = this.route.paramMap.pipe(
      switchMap(params => {
        const id = params.get('id');
        if (id !== null && id !== undefined) {
          return this.teamService.getTeamMembers(Number(id));
        }
        return new Observable<UserDto[]>(observer => {
          observer.next([]);
          observer.complete();
        });
      })
    );
  }
}
