import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface EngagementFactor {
  name: string;
  points: number;
  description: string;
}

export interface EngagementScoreResult {
  score: number;
  level: string;
  description: string;
  factors: EngagementFactor[];
}

export interface EmailEngagement {
  emailLogId: string;
  emailType: string;
  status: string;
  sentAt: string;
  openedAt: string | null;
  clickedAt: string | null;
  openCount: number;
  clickCount: number;
}

export interface ProposalEngagementMetrics {
  proposalId: string;
  totalViews: number;
  uniqueViews: number;
  emailOpens: number;
  emailClicks: number;
  engagementScore: number;
  engagementLevel: string;
  firstViewedAt: string | null;
  lastViewedAt: string | null;
  lastEmailOpenedAt: string | null;
  timeSinceLastActivity: string | null;
  daysSinceSent: number;
  emailHistory: EmailEngagement[];
}

export interface FollowUpRecommendation {
  shouldFollowUp: boolean;
  reason: string;
  suggestedTone: string;
  recommendedDelayDays: number;
  suggestedMessage: string;
}

export interface FollowUpDto {
  id: string;
  proposalId: string;
  sequenceNumber: number;
  scheduledFor: string;
  sentAt: string | null;
  status: string;
  subject: string;
  triggerReason: string;
  canCancel: boolean;
}

export interface ScheduleFollowUpRequest {
  proposalId: string;
  scheduledFor: string;
  customMessage?: string;
}

export interface ScheduleFollowUpResult {
  success: boolean;
  followUpId: string | null;
  message: string;
  scheduledFor: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class EngagementService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  /**
   * Get engagement metrics for a proposal
   */
  getEngagementMetrics(proposalId: string): Observable<ProposalEngagementMetrics> {
    return this.http.get<ProposalEngagementMetrics>(
      `${this.apiUrl}/proposals/${proposalId}/engagement`
    );
  }

  /**
   * Get follow-up recommendation for a proposal
   */
  getFollowUpRecommendation(proposalId: string): Observable<FollowUpRecommendation> {
    return this.http.get<FollowUpRecommendation>(
      `${this.apiUrl}/proposals/${proposalId}/followup-recommendation`
    );
  }

  /**
   * Get all follow-ups for a proposal
   */
  getFollowUps(proposalId: string): Observable<FollowUpDto[]> {
    return this.http.get<FollowUpDto[]>(
      `${this.apiUrl}/followups/proposal/${proposalId}`
    );
  }

  /**
   * Schedule a new follow-up
   */
  scheduleFollowUp(request: ScheduleFollowUpRequest): Observable<ScheduleFollowUpResult> {
    return this.http.post<ScheduleFollowUpResult>(
      `${this.apiUrl}/followups`,
      request
    );
  }

  /**
   * Cancel a scheduled follow-up
   */
  cancelFollowUp(followUpId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/followups/${followUpId}`);
  }

  /**
   * Get engagement level color class
   */
  getEngagementLevelClass(level: string): string {
    switch (level?.toLowerCase()) {
      case 'hot':
        return 'text-red-600 bg-red-100';
      case 'warm':
        return 'text-orange-600 bg-orange-100';
      case 'cold':
        return 'text-blue-600 bg-blue-100';
      default:
        return 'text-gray-600 bg-gray-100';
    }
  }

  /**
   * Get engagement level icon
   */
  getEngagementLevelIcon(level: string): string {
    switch (level?.toLowerCase()) {
      case 'hot':
        return 'local_fire_department';
      case 'warm':
        return 'wb_sunny';
      case 'cold':
        return 'ac_unit';
      default:
        return 'remove_circle_outline';
    }
  }
}
