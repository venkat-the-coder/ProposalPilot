import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  DashboardOverview,
  ProposalTrends,
  EngagementAnalytics,
  AIUsageAnalytics,
  ClientAnalytics,
  FullAnalyticsReport
} from '../models/analytics.model';

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {
  private readonly API_URL = `${environment.apiUrl}/analytics`;

  constructor(private http: HttpClient) {}

  getOverview(): Observable<DashboardOverview> {
    return this.http.get<DashboardOverview>(`${this.API_URL}/overview`);
  }

  getTrends(days: number = 30): Observable<ProposalTrends> {
    const params = new HttpParams().set('days', days.toString());
    return this.http.get<ProposalTrends>(`${this.API_URL}/trends`, { params });
  }

  getEngagement(): Observable<EngagementAnalytics> {
    return this.http.get<EngagementAnalytics>(`${this.API_URL}/engagement`);
  }

  getAIUsage(days: number = 30): Observable<AIUsageAnalytics> {
    const params = new HttpParams().set('days', days.toString());
    return this.http.get<AIUsageAnalytics>(`${this.API_URL}/ai-usage`, { params });
  }

  getClients(): Observable<ClientAnalytics> {
    return this.http.get<ClientAnalytics>(`${this.API_URL}/clients`);
  }

  getFullReport(days: number = 30): Observable<FullAnalyticsReport> {
    const params = new HttpParams().set('days', days.toString());
    return this.http.get<FullAnalyticsReport>(`${this.API_URL}/report`, { params });
  }
}
