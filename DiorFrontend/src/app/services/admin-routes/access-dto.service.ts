import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccessDto } from '../../models/access.model';

@Injectable({
  providedIn: 'root'
})
export class AccessService {
  private readonly apiUrl = 'https://localhost:7201/api/Access';
  
  private readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    })
  };

  constructor(private http: HttpClient) {}

  getAll(): Observable<AccessDto[]> {
    return this.http.get<AccessDto[]>(this.apiUrl, this.httpOptions);
  }

  getById(id: number): Observable<AccessDto> {
    return this.http.get<AccessDto>(`${this.apiUrl}/${id}`, this.httpOptions);
  }
  create(access: AccessDto): Observable<AccessDto> {
    return this.http.post<AccessDto>(this.apiUrl, access, this.httpOptions);
  }

  update(id: number, access: AccessDto): Observable<AccessDto> {
    return this.http.put<AccessDto>(`${this.apiUrl}/${id}`, access, this.httpOptions);
  }
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, this.httpOptions);
  }
}
