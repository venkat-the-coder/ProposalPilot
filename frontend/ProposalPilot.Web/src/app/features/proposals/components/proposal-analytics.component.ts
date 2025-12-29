import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProposalService } from '../../../core/services/proposal.service';
import { EngagementDashboardComponent } from './engagement-dashboard/engagement-dashboard.component';

interface ProposalAnalytics {
  totalViews: number;
  firstViewedAt: string | null;
  lastViewedAt: string | null;
  isPublic: boolean;
  shareToken: string;
  viewHistory: ViewEvent[];
}

interface ViewEvent {
  id: string;
  proposalId: string;
  viewedAt: string;
  viewerIpAddress: string | null;
  viewerUserAgent: string | null;
  viewerCountry: string | null;
  viewerCity: string | null;
}

@Component({
  selector: 'app-proposal-analytics',
  standalone: true,
  imports: [CommonModule, RouterLink, EngagementDashboardComponent],
  template: `
    <div class="min-h-screen bg-gray-50 py-8">
      <div class="max-w-6xl mx-auto px-4">
        <!-- Header -->
        <div class="mb-8">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-3xl font-bold text-gray-900">Proposal Analytics</h1>
              <p class="text-gray-600 mt-1">Track engagement and views for your shared proposal</p>
            </div>
            <button [routerLink]="['/proposals', proposalId]" class="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300">
              Back to Proposal
            </button>
          </div>
        </div>

        <!-- Tabs -->
        <div class="mb-6">
          <nav class="flex space-x-4 border-b border-gray-200">
            <button
              (click)="activeTab = 'engagement'"
              [class]="activeTab === 'engagement'
                ? 'border-b-2 border-blue-600 text-blue-600 pb-4 px-1 font-medium'
                : 'text-gray-500 hover:text-gray-700 pb-4 px-1'"
            >
              Engagement
            </button>
            <button
              (click)="activeTab = 'views'"
              [class]="activeTab === 'views'
                ? 'border-b-2 border-blue-600 text-blue-600 pb-4 px-1 font-medium'
                : 'text-gray-500 hover:text-gray-700 pb-4 px-1'"
            >
              View History
            </button>
          </nav>
        </div>

        <!-- Engagement Tab -->
        @if (activeTab === 'engagement' && proposalId) {
          <app-engagement-dashboard [proposalId]="proposalId"></app-engagement-dashboard>
        }

        <!-- Views Tab -->
        @if (activeTab === 'views') {
          @if (loading) {
            <div class="flex items-center justify-center py-20">
              <div class="text-center">
                <svg class="animate-spin h-12 w-12 text-blue-600 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                <p class="text-gray-600">Loading analytics...</p>
              </div>
            </div>
          }

          @if (error) {
            <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
              {{ error }}
            </div>
          }

          @if (analytics) {
            <!-- Summary Cards -->
            <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
              <!-- Total Views -->
              <div class="bg-white rounded-lg shadow-md p-6">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-sm font-medium text-gray-600">Total Views</p>
                    <p class="text-3xl font-bold text-gray-900 mt-2">{{ analytics.totalViews }}</p>
                  </div>
                  <div class="p-3 bg-blue-100 rounded-full">
                    <svg class="w-8 h-8 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path>
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"></path>
                    </svg>
                  </div>
                </div>
              </div>

              <!-- First View -->
              <div class="bg-white rounded-lg shadow-md p-6">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-sm font-medium text-gray-600">First Viewed</p>
                    <p class="text-lg font-semibold text-gray-900 mt-2">
                      @if (analytics.firstViewedAt) {
                        {{ analytics.firstViewedAt | date:'short' }}
                      } @else {
                        Not viewed yet
                      }
                    </p>
                  </div>
                  <div class="p-3 bg-green-100 rounded-full">
                    <svg class="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                    </svg>
                  </div>
                </div>
              </div>

              <!-- Last View -->
              <div class="bg-white rounded-lg shadow-md p-6">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-sm font-medium text-gray-600">Last Viewed</p>
                    <p class="text-lg font-semibold text-gray-900 mt-2">
                      @if (analytics.lastViewedAt) {
                        {{ analytics.lastViewedAt | date:'short' }}
                      } @else {
                        Not viewed yet
                      }
                    </p>
                  </div>
                  <div class="p-3 bg-purple-100 rounded-full">
                    <svg class="w-8 h-8 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                  </div>
                </div>
              </div>
            </div>

            <!-- Sharing Status -->
            <div class="bg-white rounded-lg shadow-md p-6 mb-8">
              <div class="flex items-center justify-between">
                <div>
                  <h2 class="text-lg font-bold text-gray-900 mb-2">Sharing Status</h2>
                  <div class="flex items-center gap-3">
                    @if (analytics.isPublic) {
                      <span class="px-3 py-1 rounded-full text-sm font-semibold bg-green-100 text-green-800">
                        Public
                      </span>
                      <span class="text-sm text-gray-600">Anyone with the link can view</span>
                    } @else {
                      <span class="px-3 py-1 rounded-full text-sm font-semibold bg-gray-200 text-gray-700">
                        Private
                      </span>
                      <span class="text-sm text-gray-600">Only you can view this proposal</span>
                    }
                  </div>
                </div>
                @if (analytics.isPublic) {
                  <div class="text-right">
                    <p class="text-sm text-gray-600 mb-2">Share Link</p>
                    <div class="flex gap-2">
                      <input
                        type="text"
                        [value]="getShareUrl()"
                        readonly
                        class="px-3 py-2 border border-gray-300 rounded-lg bg-gray-50 text-sm w-96"
                      />
                      <button
                        (click)="copyShareLink()"
                        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm font-medium"
                      >
                        @if (copySuccess) {
                          Copied!
                        } @else {
                          Copy
                        }
                      </button>
                    </div>
                  </div>
                }
              </div>
            </div>

            <!-- View History -->
            <div class="bg-white rounded-lg shadow-md overflow-hidden">
              <div class="px-6 py-4 border-b border-gray-200">
                <h2 class="text-lg font-bold text-gray-900">View History</h2>
                <p class="text-sm text-gray-600 mt-1">Detailed log of all proposal views</p>
              </div>

              @if (analytics.viewHistory && analytics.viewHistory.length > 0) {
                <div class="overflow-x-auto">
                  <table class="min-w-full divide-y divide-gray-200">
                    <thead class="bg-gray-50">
                      <tr>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Date & Time
                        </th>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Location
                        </th>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          IP Address
                        </th>
                        <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Device
                        </th>
                      </tr>
                    </thead>
                    <tbody class="bg-white divide-y divide-gray-200">
                      @for (view of analytics.viewHistory; track view.id) {
                        <tr class="hover:bg-gray-50">
                          <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            {{ view.viewedAt | date:'medium' }}
                          </td>
                          <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                            @if (view.viewerCity && view.viewerCountry) {
                              {{ view.viewerCity }}, {{ view.viewerCountry }}
                            } @else {
                              Unknown
                            }
                          </td>
                          <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600 font-mono">
                            {{ view.viewerIpAddress || 'Unknown' }}
                          </td>
                          <td class="px-6 py-4 text-sm text-gray-600 max-w-xs truncate">
                            {{ getDeviceInfo(view.viewerUserAgent) }}
                          </td>
                        </tr>
                      }
                    </tbody>
                  </table>
                </div>
              } @else {
                <div class="px-6 py-12 text-center">
                  <svg class="w-16 h-16 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
                  </svg>
                  <p class="text-gray-600 font-medium">No views yet</p>
                  <p class="text-sm text-gray-500 mt-1">Share your proposal to start tracking views</p>
                </div>
              }
            </div>
          }
        }
      </div>
    </div>
  `
})
export class ProposalAnalyticsComponent implements OnInit {
  proposalId: string | null = null;
  analytics: ProposalAnalytics | null = null;
  loading = true;
  error = '';
  copySuccess = false;
  activeTab: 'engagement' | 'views' = 'engagement';

  constructor(
    private route: ActivatedRoute,
    private proposalService: ProposalService
  ) {}

  ngOnInit(): void {
    this.proposalId = this.route.snapshot.paramMap.get('id');
    if (this.proposalId) {
      this.loadAnalytics(this.proposalId);
    }
  }

  loadAnalytics(id: string): void {
    this.proposalService.getProposalAnalytics(id).subscribe({
      next: (analytics) => {
        this.analytics = analytics;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Error loading analytics';
        this.loading = false;
      }
    });
  }

  getShareUrl(): string {
    if (!this.analytics) return '';
    return `${window.location.origin}/share/${this.analytics.shareToken}`;
  }

  copyShareLink(): void {
    navigator.clipboard.writeText(this.getShareUrl()).then(() => {
      this.copySuccess = true;
      setTimeout(() => {
        this.copySuccess = false;
      }, 2000);
    });
  }

  getDeviceInfo(userAgent: string | null): string {
    if (!userAgent) return 'Unknown';

    // Simple device detection
    if (userAgent.includes('Mobile') || userAgent.includes('Android') || userAgent.includes('iPhone')) {
      if (userAgent.includes('Chrome')) return 'Mobile - Chrome';
      if (userAgent.includes('Safari')) return 'Mobile - Safari';
      if (userAgent.includes('Firefox')) return 'Mobile - Firefox';
      return 'Mobile Device';
    }

    if (userAgent.includes('Chrome')) return 'Desktop - Chrome';
    if (userAgent.includes('Safari')) return 'Desktop - Safari';
    if (userAgent.includes('Firefox')) return 'Desktop - Firefox';
    if (userAgent.includes('Edge')) return 'Desktop - Edge';

    return 'Desktop';
  }
}
