import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Brief, CreateBriefRequest } from '../models/brief.model';

@Injectable({
  providedIn: 'root'
})
export class BriefService {
  private readonly API_URL = `${environment.apiUrl}/briefs`;

  constructor(private http: HttpClient) {}

  createBrief(request: CreateBriefRequest): Observable<Brief> {
    return this.http.post<Brief>(this.API_URL, request);
  }

  getBrief(id: string): Observable<Brief> {
    return this.http.get<Brief>(`${this.API_URL}/${id}`);
  }

  analyzeBrief(id: string): Observable<Brief> {
    return this.http.post<Brief>(`${this.API_URL}/${id}/analyze`, {});
  }
}
