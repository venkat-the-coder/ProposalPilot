import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProposalService } from '../../../core/services/proposal.service';
import { Proposal, ProposalContent } from '../../../core/models/proposal.model';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-proposal-view',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="min-h-screen bg-gray-50 py-8">
      <div class="max-w-5xl mx-auto px-4">
        @if (loading) {
          <div class="flex items-center justify-center py-20">
            <div class="text-center">
              <svg class="animate-spin h-12 w-12 text-blue-600 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              <p class="text-gray-600">Loading proposal...</p>
            </div>
          </div>
        }

        @if (error) {
          <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
            {{ error }}
          </div>
        }

        @if (proposal && content) {
          <!-- Header -->
          <div class="bg-white rounded-lg shadow-md p-8 mb-6">
            <h1 class="text-4xl font-bold text-gray-900 mb-4">{{ proposal.title }}</h1>
            <div class="flex items-center gap-4 text-sm text-gray-600">
              <span class="px-3 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-800">
                {{ proposal.status }}
              </span>
              <span>Created: {{ proposal.createdAt | date:'medium' }}</span>
              @if (content.metadata) {
                <span>{{ content.metadata.estimatedReadTime }}</span>
                <span class="capitalize">Tone: {{ content.metadata.tone }}</span>
              }
            </div>
          </div>

          <!-- Proposal Content -->
          <div class="space-y-6">
            <!-- Opening Hook -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <div class="prose max-w-none" [innerHTML]="sanitize(content.sections.opening_hook)"></div>
            </div>

            <!-- Problem Statement -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-2xl font-bold text-gray-900 mb-4">The Challenge</h2>
              <div class="prose max-w-none" [innerHTML]="sanitize(content.sections.problem_statement)"></div>
            </div>

            <!-- Proposed Solution -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-2xl font-bold text-gray-900 mb-4">Our Solution</h2>
              <div class="prose max-w-none" [innerHTML]="sanitize(content.sections.proposed_solution)"></div>
            </div>

            <!-- Methodology -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-2xl font-bold text-gray-900 mb-4">How We'll Do It</h2>
              <div class="prose max-w-none" [innerHTML]="sanitize(content.sections.methodology)"></div>
            </div>

            <!-- Timeline -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-2xl font-bold text-gray-900 mb-4">Timeline</h2>
              <div class="prose max-w-none" [innerHTML]="sanitize(content.sections.timeline)"></div>
            </div>

            <!-- Investment -->
            @if (content.investment) {
              <div class="bg-white rounded-lg shadow-md p-8">
                <h2 class="text-2xl font-bold text-gray-900 mb-4">Investment</h2>
                <div class="prose max-w-none mb-6" [innerHTML]="sanitize(content.investment.intro)"></div>

                <div class="grid md:grid-cols-3 gap-6">
                  @for (tier of content.investment.tiers; track tier.name) {
                    <div class="border rounded-lg p-6"
                         [class.border-blue-600]="tier.highlighted"
                         [class.bg-blue-50]="tier.highlighted">
                      @if (tier.highlighted) {
                        <div class="text-xs font-semibold text-blue-600 mb-2">RECOMMENDED</div>
                      }
                      <h3 class="text-xl font-bold text-gray-900 mb-2">{{ tier.name }}</h3>
                      <div class="text-3xl font-bold text-gray-900 mb-2">
                        @if (tier.price > 0) {
                          $<span>{{ tier.price.toLocaleString() }}</span>
                        } @else {
                          TBD
                        }
                      </div>
                      <p class="text-sm text-gray-600 mb-4">{{ tier.description }}</p>
                      <ul class="space-y-2 mb-4">
                        @for (feature of tier.features; track feature) {
                          <li class="text-sm flex items-start">
                            <span class="text-green-600 mr-2">✓</span>
                            <span>{{ feature }}</span>
                          </li>
                        }
                      </ul>
                      <p class="text-sm text-gray-600">{{ tier.timeline }}</p>
                    </div>
                  }
                </div>
              </div>
            }

            <!-- Why Choose Us -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-2xl font-bold text-gray-900 mb-4">Why Choose Us</h2>
              <div class="prose max-w-none" [innerHTML]="sanitize(content.sections.why_choose_us)"></div>
            </div>

            <!-- Next Steps -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-2xl font-bold text-gray-900 mb-4">Next Steps</h2>
              <div class="prose max-w-none" [innerHTML]="sanitize(content.sections.next_steps)"></div>
            </div>
          </div>

          <!-- Actions -->
          <div class="mt-8 flex flex-wrap gap-4">
            <button routerLink="/briefs/new" class="px-6 py-3 bg-gray-200 text-gray-700 font-medium rounded-lg hover:bg-gray-300">
              ← New Brief
            </button>
            <button [routerLink]="['/proposals', proposal.id, 'edit']" class="px-6 py-3 bg-purple-600 text-white font-medium rounded-lg hover:bg-purple-700">
              Edit Proposal
            </button>
            <button (click)="openShareModal()" class="px-6 py-3 bg-indigo-600 text-white font-medium rounded-lg hover:bg-indigo-700">
              Share Proposal
            </button>
            <button [routerLink]="['/proposals', proposal.id, 'analytics']" class="px-6 py-3 bg-amber-600 text-white font-medium rounded-lg hover:bg-amber-700">
              View Analytics
            </button>
            <button class="px-6 py-3 bg-green-600 text-white font-medium rounded-lg hover:bg-green-700">
              Send to Client
            </button>
            <button class="px-6 py-3 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700">
              Export PDF
            </button>
          </div>
        }

        <!-- Share Modal -->
        @if (showShareModal) {
          <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div class="bg-white rounded-lg shadow-xl p-8 max-w-md w-full mx-4">
              <div class="flex justify-between items-center mb-6">
                <h2 class="text-2xl font-bold text-gray-900">Share Proposal</h2>
                <button (click)="closeShareModal()" class="text-gray-500 hover:text-gray-700">
                  <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                  </svg>
                </button>
              </div>

              <div class="mb-6">
                <div class="flex items-center justify-between mb-4">
                  <span class="text-sm font-medium text-gray-700">Public Sharing</span>
                  <button
                    (click)="togglePublicSharing()"
                    [class]="isPublic ? 'bg-indigo-600' : 'bg-gray-200'"
                    class="relative inline-flex h-6 w-11 items-center rounded-full transition-colors"
                  >
                    <span
                      [class]="isPublic ? 'translate-x-6' : 'translate-x-1'"
                      class="inline-block h-4 w-4 transform rounded-full bg-white transition-transform"
                    ></span>
                  </button>
                </div>
                <p class="text-sm text-gray-600">
                  @if (isPublic) {
                    Anyone with the link can view this proposal
                  } @else {
                    Enable public sharing to generate a shareable link
                  }
                </p>
              </div>

              @if (isPublic && shareUrl) {
                <div class="mb-6">
                  <label class="block text-sm font-medium text-gray-700 mb-2">Share Link</label>
                  <div class="flex gap-2">
                    <input
                      type="text"
                      [value]="shareUrl"
                      readonly
                      class="flex-1 px-3 py-2 border border-gray-300 rounded-lg bg-gray-50 text-sm"
                    />
                    <button
                      (click)="copyShareLink()"
                      class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 text-sm font-medium"
                    >
                      @if (copySuccess) {
                        Copied!
                      } @else {
                        Copy
                      }
                    </button>
                  </div>
                </div>

                <div class="bg-blue-50 border border-blue-200 rounded-lg p-4">
                  <div class="flex items-start">
                    <svg class="w-5 h-5 text-blue-600 mt-0.5 mr-3" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"></path>
                    </svg>
                    <div class="text-sm text-blue-800">
                      <p class="font-medium mb-1">Analytics Enabled</p>
                      <p>View counts and engagement metrics are being tracked for this shared proposal.</p>
                    </div>
                  </div>
                </div>
              }

              <div class="mt-6 flex justify-end">
                <button
                  (click)="closeShareModal()"
                  class="px-6 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 font-medium"
                >
                  Close
                </button>
              </div>
            </div>
          </div>
        }
      </div>
    </div>
  `
})
export class ProposalViewComponent implements OnInit {
  proposal: Proposal | null = null;
  content: ProposalContent | null = null;
  loading = true;
  error = '';
  showShareModal = false;
  shareUrl = '';
  isPublic = false;
  copySuccess = false;

  constructor(
    private route: ActivatedRoute,
    private proposalService: ProposalService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    const proposalId = this.route.snapshot.paramMap.get('id');
    if (proposalId) {
      this.loadProposal(proposalId);
    }
  }

  loadProposal(id: string): void {
    this.proposalService.getProposal(id).subscribe({
      next: (proposal) => {
        this.proposal = proposal;
        if (proposal.deliverablesJson) {
          try {
            this.content = JSON.parse(proposal.deliverablesJson);
          } catch (e) {
            this.error = 'Error parsing proposal content';
          }
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Error loading proposal';
        this.loading = false;
      }
    });
  }

  sanitize(html: string): SafeHtml {
    return this.sanitizer.sanitize(1, html) || '';
  }

  openShareModal(): void {
    if (!this.proposal) return;

    this.proposalService.toggleSharing(this.proposal.id).subscribe({
      next: (result) => {
        this.isPublic = result.isPublic;
        this.shareUrl = result.shareUrl;
        this.showShareModal = true;
      },
      error: (err) => {
        this.error = err.error?.message || 'Error toggling share';
      }
    });
  }

  closeShareModal(): void {
    this.showShareModal = false;
    this.copySuccess = false;
  }

  copyShareLink(): void {
    navigator.clipboard.writeText(this.shareUrl).then(() => {
      this.copySuccess = true;
      setTimeout(() => {
        this.copySuccess = false;
      }, 2000);
    });
  }

  togglePublicSharing(): void {
    if (!this.proposal) return;

    this.proposalService.toggleSharing(this.proposal.id).subscribe({
      next: (result) => {
        this.isPublic = result.isPublic;
        this.shareUrl = result.shareUrl;
      },
      error: (err) => {
        this.error = err.error?.message || 'Error toggling share';
      }
    });
  }
}
