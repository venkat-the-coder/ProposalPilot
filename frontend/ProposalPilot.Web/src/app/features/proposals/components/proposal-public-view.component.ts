import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ProposalService } from '../../../core/services/proposal.service';
import { Proposal, ProposalContent } from '../../../core/models/proposal.model';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-proposal-public-view',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 py-12">
      <div class="max-w-5xl mx-auto px-4">
        @if (loading) {
          <div class="flex items-center justify-center py-20">
            <div class="text-center">
              <svg class="animate-spin h-12 w-12 text-indigo-600 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              <p class="text-gray-700 text-lg">Loading proposal...</p>
            </div>
          </div>
        }

        @if (error) {
          <div class="bg-white rounded-lg shadow-lg p-8 text-center">
            <svg class="w-16 h-16 text-red-500 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"></path>
            </svg>
            <h2 class="text-2xl font-bold text-gray-900 mb-2">Proposal Not Found</h2>
            <p class="text-gray-600">{{ error }}</p>
          </div>
        }

        @if (proposal && content) {
          <!-- Branding Header -->
          <div class="bg-white rounded-lg shadow-lg p-8 mb-8 border-t-4 border-indigo-600">
            <div class="flex items-center justify-between mb-6">
              <div>
                <h1 class="text-4xl font-bold text-gray-900 mb-2">{{ proposal.title }}</h1>
                @if (proposal.user) {
                  <p class="text-lg text-gray-600">
                    Prepared by <span class="font-semibold text-indigo-600">{{ proposal.user.fullName || proposal.user.email }}</span>
                  </p>
                }
              </div>
              <div class="text-right">
                <div class="px-4 py-2 rounded-full text-sm font-semibold bg-green-100 text-green-800 inline-block">
                  {{ proposal.status }}
                </div>
                <p class="text-sm text-gray-500 mt-2">{{ proposal.createdAt | date:'mediumDate' }}</p>
              </div>
            </div>

            @if (content.metadata) {
              <div class="flex items-center gap-6 text-sm text-gray-600 pt-4 border-t border-gray-200">
                <span class="flex items-center">
                  <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                  </svg>
                  {{ content.metadata.estimatedReadTime }}
                </span>
                <span class="flex items-center capitalize">
                  <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 8h10M7 12h4m1 8l-4-4H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-3l-4 4z"></path>
                  </svg>
                  Tone: {{ content.metadata.tone }}
                </span>
              </div>
            }
          </div>

          <!-- Proposal Content -->
          <div class="space-y-6">
            <!-- Opening Hook -->
            <div class="bg-white rounded-lg shadow-md p-8 border-l-4 border-indigo-500">
              <div class="prose prose-lg max-w-none" [innerHTML]="sanitize(content.sections.opening_hook)"></div>
            </div>

            <!-- Problem Statement -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-3xl font-bold text-gray-900 mb-4 flex items-center">
                <svg class="w-8 h-8 mr-3 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"></path>
                </svg>
                The Challenge
              </h2>
              <div class="prose prose-lg max-w-none" [innerHTML]="sanitize(content.sections.problem_statement)"></div>
            </div>

            <!-- Proposed Solution -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-3xl font-bold text-gray-900 mb-4 flex items-center">
                <svg class="w-8 h-8 mr-3 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                Our Solution
              </h2>
              <div class="prose prose-lg max-w-none" [innerHTML]="sanitize(content.sections.proposed_solution)"></div>
            </div>

            <!-- Methodology -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-3xl font-bold text-gray-900 mb-4 flex items-center">
                <svg class="w-8 h-8 mr-3 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01"></path>
                </svg>
                How We'll Do It
              </h2>
              <div class="prose prose-lg max-w-none" [innerHTML]="sanitize(content.sections.methodology)"></div>
            </div>

            <!-- Timeline -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-3xl font-bold text-gray-900 mb-4 flex items-center">
                <svg class="w-8 h-8 mr-3 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                </svg>
                Timeline
              </h2>
              <div class="prose prose-lg max-w-none" [innerHTML]="sanitize(content.sections.timeline)"></div>
            </div>

            <!-- Investment -->
            @if (content.investment) {
              <div class="bg-gradient-to-br from-indigo-50 to-purple-50 rounded-lg shadow-lg p-8">
                <h2 class="text-3xl font-bold text-gray-900 mb-2 text-center">Investment Options</h2>
                <div class="prose prose-lg max-w-none mb-8 text-center" [innerHTML]="sanitize(content.investment.intro)"></div>

                <div class="grid md:grid-cols-3 gap-6">
                  @for (tier of content.investment.tiers; track tier.name) {
                    <div class="bg-white border-2 rounded-xl p-6 transition-transform hover:scale-105"
                         [class.border-indigo-600]="tier.highlighted"
                         [class.shadow-xl]="tier.highlighted"
                         [class.border-gray-200]="!tier.highlighted">
                      @if (tier.highlighted) {
                        <div class="text-xs font-bold text-indigo-600 mb-3 uppercase tracking-wide">
                          ⭐ Recommended
                        </div>
                      }
                      <h3 class="text-2xl font-bold text-gray-900 mb-3">{{ tier.name }}</h3>
                      <div class="text-4xl font-bold text-indigo-600 mb-3">
                        @if (tier.price > 0) {
                          $<span>{{ tier.price.toLocaleString() }}</span>
                        } @else {
                          TBD
                        }
                      </div>
                      <p class="text-sm text-gray-600 mb-6">{{ tier.description }}</p>
                      <ul class="space-y-3 mb-6">
                        @for (feature of tier.features; track feature) {
                          <li class="text-sm flex items-start">
                            <svg class="w-5 h-5 text-green-500 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                            </svg>
                            <span>{{ feature }}</span>
                          </li>
                        }
                      </ul>
                      <div class="text-sm text-gray-600 font-medium">
                        <svg class="w-4 h-4 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                        </svg>
                        {{ tier.timeline }}
                      </div>
                    </div>
                  }
                </div>
              </div>
            }

            <!-- Why Choose Us -->
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-3xl font-bold text-gray-900 mb-4 flex items-center">
                <svg class="w-8 h-8 mr-3 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z"></path>
                </svg>
                Why Choose Us
              </h2>
              <div class="prose prose-lg max-w-none" [innerHTML]="sanitize(content.sections.why_choose_us)"></div>
            </div>

            <!-- Next Steps -->
            <div class="bg-indigo-600 text-white rounded-lg shadow-lg p-8">
              <h2 class="text-3xl font-bold mb-4 flex items-center">
                <svg class="w-8 h-8 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
                </svg>
                Next Steps
              </h2>
              <div class="prose prose-lg prose-invert max-w-none" [innerHTML]="sanitize(content.sections.next_steps)"></div>
            </div>
          </div>

          <!-- Footer CTA -->
          <div class="mt-12 bg-white rounded-lg shadow-xl p-8 text-center border-t-4 border-indigo-600">
            <h3 class="text-2xl font-bold text-gray-900 mb-3">Ready to Get Started?</h3>
            <p class="text-gray-600 mb-6">Let's discuss how we can bring this proposal to life.</p>
            @if (proposal.user) {
              <p class="text-lg text-gray-700 mb-2">
                Contact: <a href="mailto:{{ proposal.user.email }}" class="text-indigo-600 hover:text-indigo-700 font-semibold">
                  {{ proposal.user.email }}
                </a>
              </p>
            }
            <p class="text-sm text-gray-500 mt-6">
              Powered by ProposalMind AI
            </p>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    ::ng-deep .prose {
      color: #374151;
    }
    ::ng-deep .prose h1, ::ng-deep .prose h2, ::ng-deep .prose h3 {
      color: #1f2937;
    }
    ::ng-deep .prose-invert {
      color: #f3f4f6;
    }
    ::ng-deep .prose-invert h1, ::ng-deep .prose-invert h2, ::ng-deep .prose-invert h3 {
      color: #ffffff;
    }
  `]
})
export class ProposalPublicViewComponent implements OnInit {
  proposal: Proposal | null = null;
  content: ProposalContent | null = null;
  loading = true;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private proposalService: ProposalService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    const token = this.route.snapshot.paramMap.get('token');
    if (token) {
      this.loadSharedProposal(token);
    }
  }

  loadSharedProposal(token: string): void {
    this.proposalService.getSharedProposal(token).subscribe({
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
        this.error = err.error?.message || 'Proposal not found or not publicly shared';
        this.loading = false;
      }
    });
  }

  sanitize(html: string): SafeHtml {
    return this.sanitizer.sanitize(1, html) || '';
  }
}
