import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class MapService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  addPin(latitude: number, longitude: number, name: string, description: string, category: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/location`, { latitude, longitude, name, description, category });
  }

  getPins(): Observable<any> {
    return this.http.get(`${this.apiUrl}/location`);
  }
}
