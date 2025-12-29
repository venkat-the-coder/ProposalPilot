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
                        ${{ tier.price > 0 ? tier.price.toLocaleString() : 'TBD' }}
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
          <div class="mt-8 flex gap-4">
            <button routerLink="/briefs/new" class="px-6 py-3 bg-gray-200 text-gray-700 font-medium rounded-lg hover:bg-gray-300">
              ← New Brief
            </button>
            <button class="px-6 py-3 bg-green-600 text-white font-medium rounded-lg hover:bg-green-700">
              Send to Client
            </button>
            <button class="px-6 py-3 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700">
              Export PDF
            </button>
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
}
