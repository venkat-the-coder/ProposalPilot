import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProposalService } from '../../../core/services/proposal.service';
import { QualityScoreResult } from '../../../core/models/quality-score.model';

@Component({
  selector: 'app-quality-score-sidebar',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="bg-white rounded-lg shadow-md p-6">
      <div class="flex items-center justify-between mb-4">
        <h2 class="text-xl font-bold text-gray-900">Quality Score</h2>
        <button
          (click)="scoreProposal()"
          [disabled]="loading"
          class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:opacity-50 text-sm font-medium"
        >
          @if (loading) {
            <span class="flex items-center">
              <svg class="animate-spin h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Scoring...
            </span>
          } @else {
            {{ score ? 'Re-score' : 'Score Now' }}
          }
        </button>
      </div>

      @if (error) {
        <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4 text-sm">
          {{ error }}
        </div>
      }

      @if (score) {
        <!-- Overall Score Card -->
        <div class="mb-6 p-4 rounded-lg"
             [class.bg-green-50]="score.grade === 'A+' || score.grade === 'A'"
             [class.bg-blue-50]="score.grade === 'B'"
             [class.bg-yellow-50]="score.grade === 'C'"
             [class.bg-orange-50]="score.grade === 'D'"
             [class.bg-red-50]="score.grade === 'F'">
          <div class="text-center">
            <div class="text-5xl font-bold mb-2"
                 [class.text-green-700]="score.grade === 'A+' || score.grade === 'A'"
                 [class.text-blue-700]="score.grade === 'B'"
                 [class.text-yellow-700]="score.grade === 'C'"
                 [class.text-orange-700]="score.grade === 'D'"
                 [class.text-red-700]="score.grade === 'F'">
              {{ score.overallScore }}
            </div>
            <div class="text-2xl font-semibold mb-1"
                 [class.text-green-600]="score.grade === 'A+' || score.grade === 'A'"
                 [class.text-blue-600]="score.grade === 'B'"
                 [class.text-yellow-600]="score.grade === 'C'"
                 [class.text-orange-600]="score.grade === 'D'"
                 [class.text-red-600]="score.grade === 'F'">
              Grade {{ score.grade }}
            </div>
            <div class="text-sm font-medium capitalize"
                 [class.text-green-600]="score.grade === 'A+' || score.grade === 'A'"
                 [class.text-blue-600]="score.grade === 'B'"
                 [class.text-yellow-600]="score.grade === 'C'"
                 [class.text-orange-600]="score.grade === 'D'"
                 [class.text-red-600]="score.grade === 'F'">
              {{ score.winProbability.replace('_', ' ') }} Win Probability
            </div>
          </div>
        </div>

        <!-- Quick Wins -->
        @if (score.quickWins && score.quickWins.length > 0) {
          <div class="mb-6">
            <h3 class="text-sm font-semibold text-gray-900 mb-2 flex items-center">
              <svg class="w-4 h-4 mr-2 text-yellow-600" fill="currentColor" viewBox="0 0 20 20">
                <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"></path>
              </svg>
              Quick Wins
            </h3>
            <ul class="space-y-2">
              @for (win of score.quickWins; track win) {
                <li class="text-sm text-gray-700 pl-6 relative">
                  <span class="absolute left-0 top-1.5 w-2 h-2 bg-yellow-400 rounded-full"></span>
                  {{ win }}
                </li>
              }
            </ul>
          </div>
        }

        <!-- Category Scores -->
        <div class="mb-6">
          <h3 class="text-sm font-semibold text-gray-900 mb-3">Category Breakdown</h3>
          <div class="space-y-3">
            @for (category of getCategories(); track category) {
              <div>
                <div class="flex items-center justify-between mb-1">
                  <span class="text-sm font-medium text-gray-700 capitalize">{{ formatCategoryName(category) }}</span>
                  <span class="text-sm font-semibold text-gray-900">{{ score.scores[category].score }}/{{ score.scores[category].max }}</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2">
                  <div class="bg-indigo-600 h-2 rounded-full transition-all"
                       [style.width.%]="(score.scores[category].score / score.scores[category].max) * 100">
                  </div>
                </div>
              </div>
            }
          </div>
        </div>

        <!-- Improvements -->
        @if (score.improvements && score.improvements.length > 0) {
          <div class="mb-6">
            <h3 class="text-sm font-semibold text-gray-900 mb-3">Improvements</h3>
            <div class="space-y-3">
              @for (improvement of score.improvements; track $index) {
                <div class="border rounded-lg p-3"
                     [class.border-red-300]="improvement.priority === 'critical'"
                     [class.bg-red-50]="improvement.priority === 'critical'"
                     [class.border-orange-300]="improvement.priority === 'high'"
                     [class.bg-orange-50]="improvement.priority === 'high'"
                     [class.border-yellow-300]="improvement.priority === 'medium'"
                     [class.bg-yellow-50]="improvement.priority === 'medium'"
                     [class.border-gray-300]="improvement.priority === 'low'"
                     [class.bg-gray-50]="improvement.priority === 'low'">
                  <div class="flex items-start mb-1">
                    <span class="text-xs font-semibold px-2 py-0.5 rounded uppercase"
                          [class.bg-red-200]="improvement.priority === 'critical'"
                          [class.text-red-800]="improvement.priority === 'critical'"
                          [class.bg-orange-200]="improvement.priority === 'high'"
                          [class.text-orange-800]="improvement.priority === 'high'"
                          [class.bg-yellow-200]="improvement.priority === 'medium'"
                          [class.text-yellow-800]="improvement.priority === 'medium'"
                          [class.bg-gray-200]="improvement.priority === 'low'"
                          [class.text-gray-800]="improvement.priority === 'low'">
                      {{ improvement.priority }}
                    </span>
                    <span class="ml-2 text-xs font-medium text-gray-600">{{ improvement.section }}</span>
                  </div>
                  <p class="text-sm text-gray-800 font-medium mb-1">{{ improvement.issue }}</p>
                  <p class="text-sm text-gray-700">{{ improvement.suggestion }}</p>
                </div>
              }
            </div>
          </div>
        }

        <!-- Strengths -->
        @if (score.strengths && score.strengths.length > 0) {
          <div class="mb-6">
            <h3 class="text-sm font-semibold text-gray-900 mb-2 flex items-center">
              <svg class="w-4 h-4 mr-2 text-green-600" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              Strengths
            </h3>
            <ul class="space-y-1">
              @for (strength of score.strengths; track strength) {
                <li class="text-sm text-gray-700 pl-6 relative">
                  <span class="absolute left-0 top-1.5 w-2 h-2 bg-green-500 rounded-full"></span>
                  {{ strength }}
                </li>
              }
            </ul>
          </div>
        }
      } @else if (!loading) {
        <div class="text-center py-8 text-gray-500">
          <svg class="w-16 h-16 mx-auto mb-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
          </svg>
          <p class="text-sm font-medium">Click "Score Now" to analyze your proposal quality</p>
        </div>
      }
    </div>
  `
})
export class QualityScoreSidebarComponent {
  @Input() proposalId: string | null | undefined = null;

  score: QualityScoreResult | null = null;
  loading = false;
  error = '';

  constructor(private proposalService: ProposalService) {}

  scoreProposal(): void {
    if (!this.proposalId) return;

    this.loading = true;
    this.error = '';

    this.proposalService.scoreProposal(this.proposalId).subscribe({
      next: (result) => {
        this.score = result;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Error scoring proposal';
        this.loading = false;
      }
    });
  }

  getCategories(): string[] {
    if (!this.score?.scores) return [];
    return Object.keys(this.score.scores);
  }

  formatCategoryName(category: string): string {
    return category.replace(/_/g, ' ');
  }
}
