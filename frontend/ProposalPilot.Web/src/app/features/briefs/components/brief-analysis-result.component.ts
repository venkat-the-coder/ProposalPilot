import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { BriefService } from '../../../core/services/brief.service';
import { Brief, BriefAnalysisResult } from '../../../core/models/brief.model';

@Component({
  selector: 'app-brief-analysis-result',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="min-h-screen bg-gray-50 py-8">
      <div class="max-w-6xl mx-auto px-4">
        @if (loading) {
          <div class="flex items-center justify-center py-20">
            <div class="text-center">
              <svg class="animate-spin h-12 w-12 text-blue-600 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              <p class="text-gray-600">Loading brief analysis...</p>
            </div>
          </div>
        }

        @if (error) {
          <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
            {{ error }}
          </div>
        }

        @if (brief && analysis) {
          <div class="bg-white rounded-lg shadow-md p-8 mb-6">
            <div class="flex items-start justify-between mb-6">
              <div>
                <h1 class="text-3xl font-bold text-gray-900 mb-2">{{ brief.title }}</h1>
                <div class="flex items-center gap-4 text-sm text-gray-600">
                  <span class="flex items-center">
                    <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                    {{ brief.createdAt | date:'short' }}
                  </span>
                  <span class="px-3 py-1 rounded-full text-xs font-semibold"
                        [class]="brief.status === 'Analyzed' ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'">
                    {{ brief.status }}
                  </span>
                  @if (brief.tokensUsed) {
                    <span class="text-xs">{{ brief.tokensUsed }} tokens</span>
                  }
                </div>
              </div>
              <button
                (click)="generateProposal()"
                class="px-6 py-3 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 transition-colors"
              >
                Generate Proposal →
              </button>
            </div>
          </div>

          <!-- Project Overview -->
          <div class="bg-white rounded-lg shadow-md p-6 mb-6">
            <h2 class="text-xl font-bold text-gray-900 mb-4">📊 Project Overview</h2>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div class="p-4 bg-blue-50 rounded-lg">
                <p class="text-sm text-gray-600 mb-1">Type</p>
                <p class="font-semibold text-gray-900">{{ analysis.projectOverview.type }}</p>
              </div>
              <div class="p-4 bg-blue-50 rounded-lg">
                <p class="text-sm text-gray-600 mb-1">Industry</p>
                <p class="font-semibold text-gray-900">{{ analysis.projectOverview.industry }}</p>
              </div>
              <div class="p-4 bg-blue-50 rounded-lg">
                <p class="text-sm text-gray-600 mb-1">Complexity</p>
                <p class="font-semibold text-gray-900 capitalize">{{ analysis.projectOverview.complexity }}</p>
              </div>
              <div class="p-4 bg-blue-50 rounded-lg">
                <p class="text-sm text-gray-600 mb-1">Confidence</p>
                <p class="font-semibold text-gray-900">{{ analysis.projectOverview.confidenceScore }}%</p>
              </div>
            </div>
          </div>

          <div class="grid md:grid-cols-2 gap-6">
            <!-- Requirements -->
            <div class="bg-white rounded-lg shadow-md p-6">
              <h2 class="text-xl font-bold text-gray-900 mb-4">✅ Requirements</h2>

              <div class="mb-4">
                <h3 class="font-semibold text-gray-700 mb-2">Explicit Requirements</h3>
                <ul class="space-y-1">
                  @for (req of analysis.requirements.explicit; track req) {
                    <li class="text-sm text-gray-600 flex items-start">
                      <span class="text-blue-600 mr-2">•</span>
                      <span>{{ req }}</span>
                    </li>
                  }
                </ul>
              </div>

              <div class="mb-4">
                <h3 class="font-semibold text-gray-700 mb-2">Implicit Requirements</h3>
                <ul class="space-y-1">
                  @for (req of analysis.requirements.implicit; track req) {
                    <li class="text-sm text-gray-600 flex items-start">
                      <span class="text-gray-400 mr-2">•</span>
                      <span>{{ req }}</span>
                    </li>
                  }
                </ul>
              </div>

              @if (analysis.requirements.technical.length > 0) {
                <div>
                  <h3 class="font-semibold text-gray-700 mb-2">Technical Specs</h3>
                  <ul class="space-y-1">
                    @for (req of analysis.requirements.technical; track req) {
                      <li class="text-sm text-gray-600 flex items-start">
                        <span class="text-purple-600 mr-2">•</span>
                        <span>{{ req }}</span>
                      </li>
                    }
                  </ul>
                </div>
              }
            </div>

            <!-- Client Insights -->
            <div class="bg-white rounded-lg shadow-md p-6">
              <h2 class="text-xl font-bold text-gray-900 mb-4">💡 Client Insights</h2>

              <div class="mb-4">
                <h3 class="font-semibold text-gray-700 mb-2">Pain Points</h3>
                <ul class="space-y-1">
                  @for (point of analysis.clientInsights.painPoints; track point) {
                    <li class="text-sm text-gray-600 flex items-start">
                      <span class="text-red-500 mr-2">⚠</span>
                      <span>{{ point }}</span>
                    </li>
                  }
                </ul>
              </div>

              <div class="mb-4">
                <h3 class="font-semibold text-gray-700 mb-2">Success Criteria</h3>
                <ul class="space-y-1">
                  @for (criteria of analysis.clientInsights.successCriteria; track criteria) {
                    <li class="text-sm text-gray-600 flex items-start">
                      <span class="text-green-600 mr-2">✓</span>
                      <span>{{ criteria }}</span>
                    </li>
                  }
                </ul>
              </div>

              <div>
                <h3 class="font-semibold text-gray-700 mb-2">Decision Factors</h3>
                <ul class="space-y-1">
                  @for (factor of analysis.clientInsights.decisionFactors; track factor) {
                    <li class="text-sm text-gray-600 flex items-start">
                      <span class="text-blue-600 mr-2">→</span>
                      <span>{{ factor }}</span>
                    </li>
                  }
                </ul>
              </div>
            </div>

            <!-- Project Signals -->
            <div class="bg-white rounded-lg shadow-md p-6">
              <h2 class="text-xl font-bold text-gray-900 mb-4">📅 Project Signals</h2>

              <div class="mb-4">
                <h3 class="font-semibold text-gray-700 mb-2">Timeline</h3>
                <div class="space-y-2">
                  <p class="text-sm">
                    <span class="font-medium">Urgency:</span>
                    <span class="ml-2 px-2 py-1 rounded text-xs font-semibold"
                          [class]="analysis.projectSignals.timeline.urgency === 'high' ? 'bg-red-100 text-red-800' :
                                   analysis.projectSignals.timeline.urgency === 'medium' ? 'bg-yellow-100 text-yellow-800' :
                                   'bg-green-100 text-green-800'">
                      {{ analysis.projectSignals.timeline.urgency }}
                    </span>
                  </p>
                  <p class="text-sm">
                    <span class="font-medium">Duration:</span>
                    <span class="text-gray-600 ml-2">{{ analysis.projectSignals.timeline.durationEstimate }}</span>
                  </p>
                  @if (analysis.projectSignals.timeline.keyDates.length > 0) {
                    <div>
                      <span class="font-medium text-sm">Key Dates:</span>
                      <ul class="mt-1 space-y-1">
                        @for (date of analysis.projectSignals.timeline.keyDates; track date) {
                          <li class="text-sm text-gray-600 ml-4">• {{ date }}</li>
                        }
                      </ul>
                    </div>
                  }
                </div>
              </div>

              <div>
                <h3 class="font-semibold text-gray-700 mb-2">Budget</h3>
                <div class="space-y-2">
                  <p class="text-sm">
                    <span class="font-medium">Range:</span>
                    <span class="text-gray-600 ml-2">{{ analysis.projectSignals.budget.rangeEstimate }}</span>
                  </p>
                  <p class="text-sm">
                    <span class="font-medium">Price Sensitivity:</span>
                    <span class="ml-2 px-2 py-1 rounded text-xs font-semibold capitalize"
                          [class]="analysis.projectSignals.budget.pricingSensitivity === 'high' ? 'bg-yellow-100 text-yellow-800' : 'bg-green-100 text-green-800'">
                      {{ analysis.projectSignals.budget.pricingSensitivity }}
                    </span>
                  </p>
                </div>
              </div>
            </div>

            <!-- Risk Assessment -->
            <div class="bg-white rounded-lg shadow-md p-6">
              <h2 class="text-xl font-bold text-gray-900 mb-4">⚠ Risk Assessment</h2>

              @if (analysis.riskAssessment.redFlags.length > 0) {
                <div class="mb-4">
                  <h3 class="font-semibold text-red-700 mb-2">Red Flags</h3>
                  <ul class="space-y-1">
                    @for (flag of analysis.riskAssessment.redFlags; track flag) {
                      <li class="text-sm text-gray-600 flex items-start">
                        <span class="text-red-600 mr-2">🚩</span>
                        <span>{{ flag }}</span>
                      </li>
                    }
                  </ul>
                </div>
              }

              @if (analysis.riskAssessment.clarificationNeeded.length > 0) {
                <div class="mb-4">
                  <h3 class="font-semibold text-gray-700 mb-2">Questions to Ask</h3>
                  <ul class="space-y-1">
                    @for (question of analysis.riskAssessment.clarificationNeeded; track question) {
                      <li class="text-sm text-gray-600 flex items-start">
                        <span class="text-blue-600 mr-2">?</span>
                        <span>{{ question }}</span>
                      </li>
                    }
                  </ul>
                </div>
              }

              @if (analysis.riskAssessment.scopeCreepRisks.length > 0) {
                <div>
                  <h3 class="font-semibold text-gray-700 mb-2">Scope Creep Risks</h3>
                  <ul class="space-y-1">
                    @for (risk of analysis.riskAssessment.scopeCreepRisks; track risk) {
                      <li class="text-sm text-gray-600 flex items-start">
                        <span class="text-yellow-600 mr-2">⚡</span>
                        <span>{{ risk }}</span>
                      </li>
                    }
                  </ul>
                </div>
              }
            </div>
          </div>

          <!-- Recommended Approach -->
          <div class="bg-white rounded-lg shadow-md p-6 mt-6">
            <h2 class="text-xl font-bold text-gray-900 mb-4">🎯 Recommended Approach</h2>

            <div class="grid md:grid-cols-2 gap-6">
              <div>
                <p class="text-sm mb-4">
                  <span class="font-medium">Proposal Tone:</span>
                  <span class="ml-2 px-3 py-1 rounded-full text-xs font-semibold bg-purple-100 text-purple-800 capitalize">
                    {{ analysis.recommendedApproach.proposalTone }}
                  </span>
                </p>
                <p class="text-sm">
                  <span class="font-medium">Pricing Strategy:</span>
                  <span class="ml-2 px-3 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800 capitalize">
                    {{ analysis.recommendedApproach.pricingStrategy.replace('_', ' ') }}
                  </span>
                </p>
              </div>

              <div>
                <h3 class="font-semibold text-gray-700 mb-2">Key Themes to Emphasize</h3>
                <div class="flex flex-wrap gap-2">
                  @for (theme of analysis.recommendedApproach.keyThemes; track theme) {
                    <span class="px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-xs font-medium">
                      {{ theme }}
                    </span>
                  }
                </div>
              </div>
            </div>

            @if (analysis.recommendedApproach.differentiators.length > 0) {
              <div class="mt-4">
                <h3 class="font-semibold text-gray-700 mb-2">How to Stand Out</h3>
                <ul class="space-y-1">
                  @for (diff of analysis.recommendedApproach.differentiators; track diff) {
                    <li class="text-sm text-gray-600 flex items-start">
                      <span class="text-green-600 mr-2">★</span>
                      <span>{{ diff }}</span>
                    </li>
                  }
                </ul>
              </div>
            }
          </div>

          <!-- Actions -->
          <div class="mt-8 flex gap-4">
            <button
              routerLink="/briefs/new"
              class="px-6 py-3 bg-gray-200 text-gray-700 font-medium rounded-lg hover:bg-gray-300 transition-colors"
            >
              ← New Brief
            </button>
            <button
              (click)="generateProposal()"
              class="flex-1 px-6 py-3 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 transition-colors"
            >
              Generate Proposal from This Analysis →
            </button>
          </div>
        }
      </div>
    </div>
  `
})
export class BriefAnalysisResultComponent implements OnInit {
  brief: Brief | null = null;
  analysis: BriefAnalysisResult | null = null;
  loading = true;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private briefService: BriefService
  ) {}

  ngOnInit(): void {
    const briefId = this.route.snapshot.paramMap.get('id');
    if (briefId) {
      this.loadBrief(briefId);
    }
  }

  loadBrief(id: string): void {
    this.briefService.getBrief(id).subscribe({
      next: (brief) => {
        this.brief = brief;
        if (brief.analyzedContent) {
          try {
            this.analysis = JSON.parse(brief.analyzedContent);
          } catch (e) {
            this.error = 'Error parsing analysis results';
          }
        }
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Error loading brief';
        this.loading = false;
      }
    });
  }

  generateProposal(): void {
    // TODO: Navigate to proposal generation with this brief
    alert('Proposal generation coming in Day 16-18!');
  }
}
