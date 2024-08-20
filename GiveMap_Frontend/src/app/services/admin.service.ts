import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUsers(page: number = 1, limit: number = 10): Observable<any> {
    return this.http.get(`${this.apiUrl}/admin/users?page=${page}&limit=${limit}`);
  }

  updateUser(userId: string, userData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/users/${userId}`, userData);
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/users/${userId}`);
  }

  getLocations(page: number = 1, limit: number = 10): Observable<any> {
    return this.http.get(`${this.apiUrl}/admin/locations?page=${page}&limit=${limit}`);
  }

  updateLocation(locationId: string, locationData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/locations/${locationId}`, locationData);
  }

  deleteLocation(locationId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/locations/${locationId}`);
  }
}
