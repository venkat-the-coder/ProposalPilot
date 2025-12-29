import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface SubscriptionStatus {
  plan: string;
  isActive: boolean;
  startDate?: Date;
  endDate?: Date;
  cancelledAt?: Date;
  autoRenew: boolean;
  proposalsPerMonth: number;
  proposalsUsedThisMonth: number;
  usageResetDate?: Date;
  hasAIAnalysis: boolean;
  hasAdvancedTemplates: boolean;
  hasPrioritySupport: boolean;
  hasWhiteLabeling: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  private readonly API_URL = `${environment.apiUrl}/subscription`;

  constructor(private http: HttpClient) {}

  createCheckoutSession(plan: string): Observable<{ sessionId: string; url: string }> {
    return this.http.post<{ sessionId: string; url: string }>(
      `${this.API_URL}/checkout`,
      { plan }
    );
  }

  createPortalSession(): Observable<{ url: string }> {
    return this.http.post<{ url: string }>(`${this.API_URL}/portal`, {});
  }

  getSubscriptionStatus(): Observable<SubscriptionStatus> {
    return this.http.get<SubscriptionStatus>(`${this.API_URL}/status`);
  }

  redirectToCheckout(plan: string): void {
    this.createCheckoutSession(plan).subscribe({
      next: (response) => {
        window.location.href = response.url;
      },
      error: (error) => {
        console.error('Error creating checkout session:', error);
        alert('Failed to start checkout. Please try again.');
      }
    });
  }

  redirectToPortal(): void {
    this.createPortalSession().subscribe({
      next: (response) => {
        window.location.href = response.url;
      },
      error: (error) => {
        console.error('Error creating portal session:', error);
        alert('Failed to open customer portal. Please try again.');
      }
    });
  }
}
