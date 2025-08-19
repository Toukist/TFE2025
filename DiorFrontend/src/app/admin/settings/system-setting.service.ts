import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SystemSetting } from './system-setting.model';

@Injectable({ providedIn: 'root' })
export class SystemSettingService {
  private apiUrl = 'https://localhost:7201/api/SystemSetting';

  constructor(private http: HttpClient) {}

  getSettings(): Observable<SystemSetting[]> {
    return this.http.get<SystemSetting[]>(this.apiUrl);
  }

  getSetting(key: string): Observable<SystemSetting> {
    return this.http.get<SystemSetting>(`${this.apiUrl}/${key}`);
  }

  updateSetting(key: string, value: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${key}`, { value });
  }
}
