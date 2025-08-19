import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class RoleService {
  private readonly storageKey = 'activeRole';

  setActiveRole(role: string) {
    sessionStorage.setItem(this.storageKey, role);
  }

  getActiveRole(): string | null {
    return sessionStorage.getItem(this.storageKey);
  }

  clearActiveRole() {
    sessionStorage.removeItem(this.storageKey);
  }
}
