import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TemplateService } from '../../../core/services/template.service';
import { Template } from '../../../core/models/template.model';

@Component({
  selector: 'app-template-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="min-h-screen bg-gray-50 py-8">
      <div class="max-w-5xl mx-auto px-4">
        @if (loading()) {
          <div class="flex items-center justify-center py-20">
            <div class="text-center">
              <svg class="animate-spin h-12 w-12 text-blue-600 mx-auto mb-4" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              <p class="text-gray-600">Loading template...</p>
            </div>
          </div>
        }

        @if (error()) {
          <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-lg mb-6">
            {{ error() }}
          </div>
        }

        @if (template()) {
          <!-- Header with Actions -->
          <div class="bg-white rounded-lg shadow-md p-8 mb-6">
            <div class="flex items-start justify-between mb-4">
              <div class="flex-1">
                <div class="flex items-center gap-3 mb-2">
                  <h1 class="text-3xl font-bold text-gray-900">{{ template()!.name }}</h1>
                  @if (template()!.isSystemTemplate) {
                    <span class="px-3 py-1 bg-blue-100 text-blue-800 text-sm font-semibold rounded-full">
                      System Template
                    </span>
                  }
                  @if (template()!.isPublic) {
                    <span class="px-3 py-1 bg-green-100 text-green-800 text-sm font-semibold rounded-full">
                      Public
                    </span>
                  }
                </div>
                @if (template()!.description) {
                  <p class="text-gray-600">{{ template()!.description }}</p>
                }
              </div>
            </div>

            <!-- Meta Info -->
            <div class="flex flex-wrap gap-6 text-sm text-gray-600 mb-6">
              <div class="flex items-center">
                <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M2 6a2 2 0 012-2h5l2 2h5a2 2 0 012 2v6a2 2 0 01-2 2H4a2 2 0 01-2-2V6z"></path>
                </svg>
                {{ template()!.category }}
              </div>
              <div class="flex items-center">
                <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M9 2a1 1 0 000 2h2a1 1 0 100-2H9z"></path>
                  <path fill-rule="evenodd" d="M4 5a2 2 0 012-2 3 3 0 003 3h2a3 3 0 003-3 2 2 0 012 2v11a2 2 0 01-2 2H6a2 2 0 01-2-2V5zm3 4a1 1 0 000 2h.01a1 1 0 100-2H7zm3 0a1 1 0 000 2h3a1 1 0 100-2h-3zm-3 4a1 1 0 100 2h.01a1 1 0 100-2H7zm3 0a1 1 0 100 2h3a1 1 0 100-2h-3z" clip-rule="evenodd"></path>
                </svg>
                Used {{ template()!.usageCount }} times
              </div>
              @if (template()!.winRate) {
                <div class="flex items-center text-green-600">
                  <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                  </svg>
                  {{ template()!.winRate }}% win rate
                </div>
              }
              @if (template()!.estimatedTimeMinutes) {
                <div class="flex items-center">
                  <svg class="w-5 h-5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"></path>
                  </svg>
                  ~{{ template()!.estimatedTimeMinutes }} minutes to complete
                </div>
              }
            </div>

            <!-- Tags -->
            @if (template()!.tags && template()!.tags.length > 0) {
              <div class="flex flex-wrap gap-2 mb-6">
                @for (tag of template()!.tags; track tag) {
                  <span class="px-3 py-1 bg-gray-100 text-gray-700 text-sm rounded-full">
                    {{ tag }}
                  </span>
                }
              </div>
            }

            <!-- Action Buttons -->
            <div class="flex flex-wrap gap-3">
              <button
                (click)="useTemplate()"
                class="inline-flex items-center px-6 py-3 bg-blue-600 text-white font-semibold rounded-lg shadow-md hover:bg-blue-700 transition-colors"
              >
                <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
                </svg>
                Use This Template
              </button>
              @if (!template()!.isSystemTemplate) {
                <button
                  (click)="editTemplate()"
                  class="inline-flex items-center px-6 py-3 bg-gray-600 text-white font-semibold rounded-lg shadow-md hover:bg-gray-700 transition-colors"
                >
                  <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
                  </svg>
                  Edit Template
                </button>
              }
              <button
                (click)="duplicateTemplate()"
                class="inline-flex items-center px-6 py-3 bg-white text-gray-700 font-semibold rounded-lg shadow-md hover:bg-gray-50 transition-colors border border-gray-300"
              >
                <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z"></path>
                </svg>
                Duplicate
              </button>
              <a
                routerLink="/templates"
                class="inline-flex items-center px-6 py-3 bg-white text-gray-700 font-semibold rounded-lg shadow-md hover:bg-gray-50 transition-colors border border-gray-300"
              >
                <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path>
                </svg>
                Back to Templates
              </a>
            </div>
          </div>

          <!-- Template Content Preview -->
          <div class="space-y-6">
            <div class="bg-white rounded-lg shadow-md p-8">
              <h2 class="text-2xl font-bold text-gray-900 mb-6 flex items-center">
                <svg class="w-6 h-6 mr-2" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M9 4.804A7.968 7.968 0 005.5 4c-1.255 0-2.443.29-3.5.804v10A7.969 7.969 0 015.5 14c1.669 0 3.218.51 4.5 1.385A7.962 7.962 0 0114.5 14c1.255 0 2.443.29 3.5.804v-10A7.968 7.968 0 0014.5 4c-1.255 0-2.443.29-3.5.804V12a1 1 0 11-2 0V4.804z"></path>
                </svg>
                Template Content
              </h2>

              @if (template()!.content.introduction) {
                <div class="mb-6">
                  <h3 class="text-lg font-semibold text-gray-800 mb-2">Introduction</h3>
                  <div class="prose max-w-none text-gray-600 whitespace-pre-line">{{ template()!.content.introduction }}</div>
                </div>
              }

              @if (template()!.content.problemStatement) {
                <div class="mb-6">
                  <h3 class="text-lg font-semibold text-gray-800 mb-2">Problem Statement</h3>
                  <div class="prose max-w-none text-gray-600 whitespace-pre-line">{{ template()!.content.problemStatement }}</div>
                </div>
              }

              @if (template()!.content.proposedSolution) {
                <div class="mb-6">
                  <h3 class="text-lg font-semibold text-gray-800 mb-2">Proposed Solution</h3>
                  <div class="prose max-w-none text-gray-600 whitespace-pre-line">{{ template()!.content.proposedSolution }}</div>
                </div>
              }

              @if (template()!.content.methodology) {
                <div class="mb-6">
                  <h3 class="text-lg font-semibold text-gray-800 mb-2">Methodology</h3>
                  <div class="prose max-w-none text-gray-600 whitespace-pre-line">{{ template()!.content.methodology }}</div>
                </div>
              }

              @if (template()!.content.deliverables) {
                <div class="mb-6">
                  <h3 class="text-lg font-semibold text-gray-800 mb-2">Deliverables</h3>
                  <div class="prose max-w-none text-gray-600 whitespace-pre-line">{{ template()!.content.deliverables }}</div>
                </div>
              }

              @if (template()!.content.timeline) {
                <div class="mb-6">
                  <h3 class="text-lg font-semibold text-gray-800 mb-2">Timeline</h3>
                  <div class="prose max-w-none text-gray-600 whitespace-pre-line">{{ template()!.content.timeline }}</div>
                </div>
              }
            </div>

            <!-- Default Pricing (if available) -->
            @if (template()!.defaultPricing) {
              <div class="bg-white rounded-lg shadow-md p-8">
                <h2 class="text-2xl font-bold text-gray-900 mb-6">Pricing Tiers</h2>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                  <!-- Basic Tier -->
                  <div class="border-2 border-gray-200 rounded-lg p-6 hover:border-blue-500 transition-colors">
                    <h3 class="text-xl font-bold text-gray-900 mb-2">{{ template()!.defaultPricing!.basic!.name }}</h3>
                    <div class="text-3xl font-bold text-blue-600 mb-2">
                      \${{ template()!.defaultPricing!.basic!.priceMin | number }} - \${{ template()!.defaultPricing!.basic!.priceMax | number }}
                    </div>
                    <p class="text-sm text-gray-600 mb-4">{{ template()!.defaultPricing!.basic!.timeline }}</p>
                    <p class="text-gray-700 mb-4">{{ template()!.defaultPricing!.basic!.description }}</p>
                    <ul class="space-y-2">
                      @for (feature of template()!.defaultPricing!.basic!.features; track feature) {
                        <li class="flex items-start">
                          <svg class="w-5 h-5 text-green-500 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                          </svg>
                          <span class="text-gray-700">{{ feature }}</span>
                        </li>
                      }
                    </ul>
                  </div>

                  <!-- Standard Tier -->
                  <div class="border-2 border-blue-500 rounded-lg p-6 relative">
                    <div class="absolute top-0 right-0 bg-blue-500 text-white px-3 py-1 text-xs font-bold rounded-bl-lg rounded-tr-lg">
                      POPULAR
                    </div>
                    <h3 class="text-xl font-bold text-gray-900 mb-2">{{ template()!.defaultPricing!.standard!.name }}</h3>
                    <div class="text-3xl font-bold text-blue-600 mb-2">
                      \${{ template()!.defaultPricing!.standard!.priceMin | number }} - \${{ template()!.defaultPricing!.standard!.priceMax | number }}
                    </div>
                    <p class="text-sm text-gray-600 mb-4">{{ template()!.defaultPricing!.standard!.timeline }}</p>
                    <p class="text-gray-700 mb-4">{{ template()!.defaultPricing!.standard!.description }}</p>
                    <ul class="space-y-2">
                      @for (feature of template()!.defaultPricing!.standard!.features; track feature) {
                        <li class="flex items-start">
                          <svg class="w-5 h-5 text-green-500 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                          </svg>
                          <span class="text-gray-700">{{ feature }}</span>
                        </li>
                      }
                    </ul>
                  </div>

                  <!-- Premium Tier -->
                  <div class="border-2 border-gray-200 rounded-lg p-6 hover:border-blue-500 transition-colors">
                    <h3 class="text-xl font-bold text-gray-900 mb-2">{{ template()!.defaultPricing!.premium!.name }}</h3>
                    <div class="text-3xl font-bold text-blue-600 mb-2">
                      \${{ template()!.defaultPricing!.premium!.priceMin | number }} - \${{ template()!.defaultPricing!.premium!.priceMax | number }}
                    </div>
                    <p class="text-sm text-gray-600 mb-4">{{ template()!.defaultPricing!.premium!.timeline }}</p>
                    <p class="text-gray-700 mb-4">{{ template()!.defaultPricing!.premium!.description }}</p>
                    <ul class="space-y-2">
                      @for (feature of template()!.defaultPricing!.premium!.features; track feature) {
                        <li class="flex items-start">
                          <svg class="w-5 h-5 text-green-500 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                          </svg>
                          <span class="text-gray-700">{{ feature }}</span>
                        </li>
                      }
                    </ul>
                  </div>
                </div>
              </div>
            }
          </div>
        }
      </div>
    </div>
  `
})
export class TemplateDetailComponent implements OnInit {
  template = signal<Template | null>(null);
  loading = signal(false);
  error = signal('');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private templateService: TemplateService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.params['id'];
    this.loadTemplate(id);
  }

  loadTemplate(id: string): void {
    this.loading.set(true);
    this.error.set('');

    this.templateService.getTemplateById(id).subscribe({
      next: (template) => {
        this.template.set(template);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to load template');
        this.loading.set(false);
      }
    });
  }

  useTemplate(): void {
    const template = this.template();
    if (template) {
      // Store selected template in localStorage for use when generating proposals
      localStorage.setItem('selectedTemplateId', template.id);
      localStorage.setItem('selectedTemplateName', template.name);

      // Navigate to briefs page to create a brief first
      this.router.navigate(['/briefs/new'], {
        queryParams: { template: template.id }
      });
    }
  }

  editTemplate(): void {
    const template = this.template();
    if (template) {
      this.router.navigate(['/templates/edit', template.id]);
    }
  }

  duplicateTemplate(): void {
    const template = this.template();
    if (!template) return;

    this.templateService.duplicateTemplate(template.id).subscribe({
      next: (newTemplate) => {
        this.router.navigate(['/templates', newTemplate.id]);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Failed to duplicate template');
      }
    });
  }
}
