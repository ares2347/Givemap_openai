import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getLocations(): Observable<any> {
    return this.http.get(`${this.apiUrl}/location`);
  }
  
  getLocationDetails(id: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/location/${id}`);
  }

  getNeed(locationId: string) : Observable<any>{
    return this.http.get(`${this.apiUrl}/location/${locationId}/needs`);
  }

  addNeed(locationId: string, needData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/location/${locationId}/needs`, needData);
  }

  updateNeed(locationId: string, needId: string, needData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/location/${locationId}/needs/${needId}`, needData);
  }

  deleteNeed(locationId: string, needId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/location/${locationId}/needs/${needId}`);
  }

  addFeedback(locationId: string, comment: string, rating: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/location/${locationId}/feedback`, { comment, rating });
  }

  searchLocations(keyword: string): Observable<any> {
    const params = new HttpParams().set('keyword', keyword);
    return this.http.get(`${this.apiUrl}/location/search`, { params });
  }

  filterLocations(filters: any): Observable<any> {
    let params = new HttpParams();
    Object.keys(filters).forEach(key => {
      if (filters[key]) {
        params = params.append(key, filters[key]);
      }
    });
    return this.http.get(`${this.apiUrl}/location/filter`, { params });
  }

  getLocationFeedback(id: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/location/${id}/feedback`);
  }

  addDonation(needId: string, donationData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/location/needs/${needId}/donations`, donationData);
  }

  updateDonation(needId: string, donationId: string, donationData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/location/needs/${needId}/donations/${donationId}`, donationData);
  }

  deleteDonation(locationId: string, donationId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/location/needs/${locationId}/donations/${donationId}`);
  }

  getUserDonations(): Observable<any> {
    return this.http.get(`${this.apiUrl}/location/user/donations`);
  }
}
