import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TemplateService } from '../../../core/services/template.service';
import { TemplateListItem, TEMPLATE_CATEGORIES } from '../../../core/models/template.model';

@Component({
  selector: 'app-template-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50 to-purple-50 py-8">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <!-- Back Button -->
        <div class="mb-6 animate-fade-in">
          <a routerLink="/dashboard" class="btn-back">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
            </svg>
            Back to Dashboard
          </a>
        </div>

        <!-- Header Card -->
        <div class="card-gradient mb-8 animate-slide-in-up">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-4">
              <div class="w-16 h-16 rounded-xl bg-gradient-to-r from-purple-500 to-pink-500 flex items-center justify-center shadow-lg">
                <svg class="w-8 h-8 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
                </svg>
              </div>
              <div>
                <h1 class="text-3xl font-extrabold text-gray-900 mb-1">
                  Proposal Templates
                </h1>
                <p class="text-gray-600">
                  Choose from professional templates or create your own
                </p>
              </div>
            </div>
            <a
              routerLink="/templates/new"
              class="btn-primary hidden md:flex items-center gap-2">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
              </svg>
              Create Template
            </a>
          </div>
        </div>

        <!-- Search & Filters Card -->
        <div class="card-gradient mb-8 animate-scale-in">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
            <!-- Search -->
            <div class="md:col-span-2">
              <label class="block text-sm font-semibold text-gray-700 mb-2">Search Templates</label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <svg class="h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                  </svg>
                </div>
                <input
                  type="text"
                  [(ngModel)]="searchTerm"
                  (ngModelChange)="onSearchChange()"
                  placeholder="Search by name, description, or tags..."
                  class="input-field pl-10"
                />
              </div>
            </div>

            <!-- Category Filter -->
            <div>
              <label class="block text-sm font-semibold text-gray-700 mb-2">Category</label>
              <select
                [(ngModel)]="selectedCategory"
                (ngModelChange)="onFilterChange()"
                class="input-field"
              >
                <option value="">All Categories</option>
                @for (category of categories; track category) {
                  <option [value]="category">{{ category }}</option>
                }
              </select>
            </div>
          </div>

          <!-- Filter Tabs -->
          <div class="flex flex-wrap gap-3 pt-6 border-t border-gray-200">
            <button
              (click)="setFilter('all')"
              [class.badge-primary]="activeFilter === 'all'"
              [class.badge]="activeFilter !== 'all'"
              class="px-4 py-2 rounded-lg font-medium transition-all"
            >
              All Templates
            </button>
            <button
              (click)="setFilter('system')"
              [class.badge-primary]="activeFilter === 'system'"
              [class.badge]="activeFilter !== 'system'"
              class="px-4 py-2 rounded-lg font-medium transition-all"
            >
              System Templates
            </button>
            <button
              (click)="setFilter('my')"
              [class.badge-primary]="activeFilter === 'my'"
              [class.badge]="activeFilter !== 'my'"
              class="px-4 py-2 rounded-lg font-medium transition-all"
            >
              My Templates
            </button>
            <button
              (click)="setFilter('public')"
              [class.badge-primary]="activeFilter === 'public'"
              [class.badge]="activeFilter !== 'public'"
              class="px-4 py-2 rounded-lg font-medium transition-all"
            >
              Community
            </button>
          </div>
        </div>

        <!-- Create Template Button (Mobile) -->
        <div class="mb-6 md:hidden">
          <a
            routerLink="/templates/new"
            class="btn-primary w-full flex items-center justify-center gap-2">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
            </svg>
            Create Template
          </a>
        </div>

        <!-- Loading State -->
        @if (loading()) {
          <div class="card text-center py-20 animate-fade-in">
            <div class="inline-block animate-spin rounded-full h-16 w-16 border-b-4 border-blue-600 mb-4"></div>
            <p class="text-gray-600 font-medium text-lg">Loading templates...</p>
          </div>
        }

        <!-- Error State -->
        @if (error()) {
          <div class="card p-8 text-center animate-fade-in">
            <div class="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg class="w-8 h-8 text-red-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <h3 class="text-xl font-bold text-gray-900 mb-2">Failed to load templates</h3>
            <p class="text-gray-600 mb-6">{{ error() }}</p>
            <button (click)="loadTemplates()" class="btn-primary">
              <svg class="w-4 h-4 inline-block mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
              </svg>
              Retry
            </button>
          </div>
        }

        <!-- Templates Grid -->
        @if (!loading() && !error()) {
          @if (templates().length === 0) {
            <div class="card p-12 text-center animate-scale-in">
              <div class="w-20 h-20 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg class="w-10 h-10 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
                </svg>
              </div>
              <h3 class="text-2xl font-bold text-gray-900 mb-2">No templates found</h3>
              <p class="text-gray-600 mb-8">Try adjusting your filters or create a new template to get started</p>
              <a
                routerLink="/templates/new"
                class="btn-primary inline-flex items-center gap-2">
                <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                </svg>
                Create Your First Template
              </a>
            </div>
          } @else {
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 animate-fade-in">
              @for (template of templates(); track template.id) {
                <div class="card group cursor-pointer transition-all duration-300 hover:shadow-2xl hover:-translate-y-1 overflow-hidden"
                     [routerLink]="['/templates', template.id]">
                  <!-- Template Header -->
                  <div class="p-6">
                    <div class="flex items-start justify-between mb-3">
                      <h3 class="text-lg font-bold text-gray-900 group-hover:text-blue-600 transition-colors line-clamp-2 flex-1">
                        {{ template.name }}
                      </h3>
                      @if (template.isSystemTemplate) {
                        <span class="inline-flex items-center gap-1 px-2 py-0.5 bg-gradient-to-r from-blue-100 to-purple-100 text-blue-700 text-[10px] font-semibold rounded-full whitespace-nowrap ml-2 border border-blue-200">
                          <svg class="w-2.5 h-2.5" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M6.267 3.455a3.066 3.066 0 001.745-.723 3.066 3.066 0 013.976 0 3.066 3.066 0 001.745.723 3.066 3.066 0 012.812 2.812c.051.643.304 1.254.723 1.745a3.066 3.066 0 010 3.976 3.066 3.066 0 00-.723 1.745 3.066 3.066 0 01-2.812 2.812 3.066 3.066 0 00-1.745.723 3.066 3.066 0 01-3.976 0 3.066 3.066 0 00-1.745-.723 3.066 3.066 0 01-2.812-2.812 3.066 3.066 0 00-.723-1.745 3.066 3.066 0 010-3.976 3.066 3.066 0 00.723-1.745 3.066 3.066 0 012.812-2.812zm7.44 5.252a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                          </svg>
                          VERIFIED
                        </span>
                      }
                    </div>

                    <p class="text-sm text-gray-600 mb-4 line-clamp-2">
                      {{ template.description || 'No description available' }}
                    </p>

                    <!-- Category Badge -->
                    <div class="mb-4">
                      <span class="badge flex items-center gap-1 w-fit">
                        <svg class="w-3 h-3" fill="currentColor" viewBox="0 0 20 20">
                          <path d="M2 6a2 2 0 012-2h5l2 2h5a2 2 0 012 2v6a2 2 0 01-2 2H4a2 2 0 01-2-2V6z"></path>
                        </svg>
                        {{ template.category }}
                      </span>
                    </div>

                    <!-- Tags -->
                    @if (template.tags && template.tags.length > 0) {
                      <div class="flex flex-wrap gap-2 mb-4">
                        @for (tag of template.tags.slice(0, 3); track tag) {
                          <span class="badge text-xs">
                            {{ tag }}
                          </span>
                        }
                        @if (template.tags.length > 3) {
                          <span class="badge text-xs">
                            +{{ template.tags.length - 3 }}
                          </span>
                        }
                      </div>
                    }

                    <!-- Stats -->
                    <div class="flex items-center justify-between text-sm text-gray-600 pt-4 border-t border-gray-100">
                      <div class="flex items-center gap-1">
                        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                          <path d="M9 2a1 1 0 000 2h2a1 1 0 100-2H9z"></path>
                          <path fill-rule="evenodd" d="M4 5a2 2 0 012-2 3 3 0 003 3h2a3 3 0 003-3 2 2 0 012 2v11a2 2 0 01-2 2H6a2 2 0 01-2-2V5zm3 4a1 1 0 000 2h.01a1 1 0 100-2H7zm3 0a1 1 0 000 2h3a1 1 0 100-2h-3zm-3 4a1 1 0 100 2h.01a1 1 0 100-2H7zm3 0a1 1 0 100 2h3a1 1 0 100-2h-3z" clip-rule="evenodd"></path>
                        </svg>
                        {{ template.usageCount }}x
                      </div>
                      @if (template.winRate) {
                        <div class="flex items-center gap-1 text-green-600 font-medium">
                          <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                          </svg>
                          {{ template.winRate }}%
                        </div>
                      }
                      @if (template.estimatedTimeMinutes) {
                        <div class="flex items-center gap-1">
                          <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"></path>
                          </svg>
                          ~{{ template.estimatedTimeMinutes }}m
                        </div>
                      }
                    </div>
                  </div>

                  <!-- Action Buttons -->
                  <div class="bg-gradient-to-r from-gray-50 to-blue-50 px-6 py-4 flex justify-between items-center border-t border-gray-100">
                    <button
                      (click)="viewTemplate($event, template.id)"
                      class="text-blue-600 hover:text-blue-800 font-medium text-sm transition-colors"
                    >
                      View Details →
                    </button>
                    <button
                      (click)="useTemplate($event, template.id)"
                      class="btn-primary text-sm py-2 px-4"
                    >
                      Use Template
                    </button>
                  </div>
                </div>
              }
            </div>

            <!-- Pagination (if needed) -->
            @if (hasMore) {
              <div class="mt-8 text-center animate-fade-in">
                <button
                  (click)="loadMore()"
                  class="btn-secondary">
                  <svg class="w-4 h-4 inline-block mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                  </svg>
                  Load More Templates
                </button>
              </div>
            }
          }
        }
      </div>
    </div>
  `,
  styles: [`
    .line-clamp-2 {
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }
  `]
})
export class TemplateListComponent implements OnInit {
  templates = signal<TemplateListItem[]>([]);
  categories = TEMPLATE_CATEGORIES;
  loading = signal(false);
  error = signal('');

  searchTerm = '';
  selectedCategory = '';
  activeFilter: 'all' | 'system' | 'my' | 'public' = 'all';
  currentPage = 1;
  pageSize = 20;
  hasMore = false;

  private searchTimeout: any;

  constructor(
    private templateService: TemplateService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTemplates();
  }

  loadTemplates(): void {
    this.loading.set(true);
    this.error.set('');

    const request$ = this.activeFilter === 'system'
      ? this.templateService.getSystemTemplates()
      : this.activeFilter === 'my'
      ? this.templateService.getMyTemplates()
      : this.templateService.getTemplates({
          category: this.selectedCategory || undefined,
          searchTerm: this.searchTerm || undefined,
          includePublic: this.activeFilter === 'public',
          page: this.currentPage,
          pageSize: this.pageSize
        });

    request$.subscribe({
      next: (templates) => {
        this.templates.set(templates);
        this.hasMore = templates.length === this.pageSize;
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading templates:', err);
        this.error.set(err.error?.message || 'Failed to load templates. Please try again.');
        this.loading.set(false);
      }
    });
  }

  onSearchChange(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.currentPage = 1;
      this.loadTemplates();
    }, 300);
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadTemplates();
  }

  setFilter(filter: 'all' | 'system' | 'my' | 'public'): void {
    this.activeFilter = filter;
    this.currentPage = 1;
    this.loadTemplates();
  }

  loadMore(): void {
    this.currentPage++;
    this.loadTemplates();
  }

  viewTemplate(event: Event, templateId: string): void {
    event.stopPropagation();
    this.router.navigate(['/templates', templateId]);
  }

  useTemplate(event: Event, templateId: string): void {
    event.stopPropagation();
    // Navigate to create proposal with this template
    // For now, navigate to the template detail page where they can see full details
    this.router.navigate(['/templates', templateId], {
      queryParams: { action: 'use' }
    });
  }
}
