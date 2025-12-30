import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TemplateService } from '../../../core/services/template.service';
import { TemplateListItem, TEMPLATE_CATEGORIES } from '../../../core/models/template.model';

@Component({
  selector: 'app-template-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="min-h-screen bg-gray-50 py-8">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <!-- Header -->
        <div class="mb-8">
          <h1 class="text-3xl font-bold text-gray-900 mb-2">Proposal Templates</h1>
          <p class="text-gray-600">Choose from professional templates or create your own</p>
        </div>

        <!-- Search & Filters -->
        <div class="bg-white rounded-lg shadow-md p-6 mb-8">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <!-- Search -->
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-gray-700 mb-2">Search Templates</label>
              <input
                type="text"
                [(ngModel)]="searchTerm"
                (ngModelChange)="onSearchChange()"
                placeholder="Search by name, description, or tags..."
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <!-- Category Filter -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Category</label>
              <select
                [(ngModel)]="selectedCategory"
                (ngModelChange)="onFilterChange()"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="">All Categories</option>
                @for (category of categories; track category) {
                  <option [value]="category">{{ category }}</option>
                }
              </select>
            </div>
          </div>

          <!-- Filter Tabs -->
          <div class="flex gap-4 mt-4 border-t pt-4">
            <button
              (click)="setFilter('all')"
              [class.bg-blue-600]="activeFilter === 'all'"
              [class.text-white]="activeFilter === 'all'"
              [class.bg-gray-100]="activeFilter !== 'all'"
              [class.text-gray-700]="activeFilter !== 'all'"
              class="px-4 py-2 rounded-lg font-medium transition-colors"
            >
              All Templates
            </button>
            <button
              (click)="setFilter('system')"
              [class.bg-blue-600]="activeFilter === 'system'"
              [class.text-white]="activeFilter === 'system'"
              [class.bg-gray-100]="activeFilter !== 'system'"
              [class.text-gray-700]="activeFilter !== 'system'"
              class="px-4 py-2 rounded-lg font-medium transition-colors"
            >
              System Templates
            </button>
            <button
              (click)="setFilter('my')"
              [class.bg-blue-600]="activeFilter === 'my'"
              [class.text-white]="activeFilter === 'my'"
              [class.bg-gray-100]="activeFilter !== 'my'"
              [class.text-gray-700]="activeFilter !== 'my'"
              class="px-4 py-2 rounded-lg font-medium transition-colors"
            >
              My Templates
            </button>
            <button
              (click)="setFilter('public')"
              [class.bg-blue-600]="activeFilter === 'public'"
              [class.text-white]="activeFilter === 'public'"
              [class.bg-gray-100]="activeFilter !== 'public'"
              [class.text-gray-700]="activeFilter !== 'public'"
              class="px-4 py-2 rounded-lg font-medium transition-colors"
            >
              Community
            </button>
          </div>
        </div>

        <!-- Create Template Button -->
        <div class="mb-6 flex justify-end">
          <a
            routerLink="/templates/new"
            class="inline-flex items-center px-6 py-3 bg-blue-600 text-white font-semibold rounded-lg shadow-md hover:bg-blue-700 transition-colors"
          >
            <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
            </svg>
            Create Template
          </a>
        </div>

        <!-- Loading State -->
        @if (loading) {
          <div class="flex items-center justify-center py-20">
            <div class="text-center">
              <svg class="animate-spin h-12 w-12 text-blue-600 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              <p class="text-gray-600">Loading templates...</p>
            </div>
          </div>
        }

        <!-- Error State -->
        @if (error) {
          <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-lg">
            {{ error }}
          </div>
        }

        <!-- Templates Grid -->
        @if (!loading && !error) {
          @if (templates.length === 0) {
            <div class="bg-white rounded-lg shadow-md p-12 text-center">
              <svg class="w-16 h-16 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
              <h3 class="text-xl font-semibold text-gray-900 mb-2">No templates found</h3>
              <p class="text-gray-600 mb-6">Try adjusting your filters or create a new template</p>
              <a
                routerLink="/templates/new"
                class="inline-flex items-center px-6 py-3 bg-blue-600 text-white font-semibold rounded-lg shadow-md hover:bg-blue-700 transition-colors"
              >
                Create Your First Template
              </a>
            </div>
          } @else {
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              @for (template of templates; track template.id) {
                <div class="bg-white rounded-lg shadow-md hover:shadow-xl transition-shadow overflow-hidden group cursor-pointer"
                     [routerLink]="['/templates', template.id]">
                  <!-- Template Header -->
                  <div class="p-6">
                    <div class="flex items-start justify-between mb-3">
                      <h3 class="text-lg font-bold text-gray-900 group-hover:text-blue-600 transition-colors line-clamp-2">
                        {{ template.name }}
                      </h3>
                      @if (template.isSystemTemplate) {
                        <span class="px-2 py-1 bg-blue-100 text-blue-800 text-xs font-semibold rounded-full whitespace-nowrap ml-2">
                          System
                        </span>
                      }
                    </div>

                    <p class="text-sm text-gray-600 mb-4 line-clamp-2">
                      {{ template.description || 'No description' }}
                    </p>

                    <!-- Category Badge -->
                    <div class="mb-4">
                      <span class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                        <svg class="w-3 h-3 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path d="M2 6a2 2 0 012-2h5l2 2h5a2 2 0 012 2v6a2 2 0 01-2 2H4a2 2 0 01-2-2V6z"></path>
                        </svg>
                        {{ template.category }}
                      </span>
                    </div>

                    <!-- Tags -->
                    @if (template.tags && template.tags.length > 0) {
                      <div class="flex flex-wrap gap-2 mb-4">
                        @for (tag of template.tags.slice(0, 3); track tag) {
                          <span class="px-2 py-1 bg-gray-100 text-gray-700 text-xs rounded">
                            {{ tag }}
                          </span>
                        }
                        @if (template.tags.length > 3) {
                          <span class="px-2 py-1 bg-gray-100 text-gray-700 text-xs rounded">
                            +{{ template.tags.length - 3 }}
                          </span>
                        }
                      </div>
                    }

                    <!-- Stats -->
                    <div class="flex items-center justify-between text-sm text-gray-600 pt-4 border-t">
                      <div class="flex items-center">
                        <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                          <path d="M9 2a1 1 0 000 2h2a1 1 0 100-2H9z"></path>
                          <path fill-rule="evenodd" d="M4 5a2 2 0 012-2 3 3 0 003 3h2a3 3 0 003-3 2 2 0 012 2v11a2 2 0 01-2 2H6a2 2 0 01-2-2V5zm3 4a1 1 0 000 2h.01a1 1 0 100-2H7zm3 0a1 1 0 000 2h3a1 1 0 100-2h-3zm-3 4a1 1 0 100 2h.01a1 1 0 100-2H7zm3 0a1 1 0 100 2h3a1 1 0 100-2h-3z" clip-rule="evenodd"></path>
                        </svg>
                        Used {{ template.usageCount }}x
                      </div>
                      @if (template.winRate) {
                        <div class="flex items-center text-green-600">
                          <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                          </svg>
                          {{ template.winRate }}% win rate
                        </div>
                      }
                      @if (template.estimatedTimeMinutes) {
                        <div class="flex items-center">
                          <svg class="w-4 h-4 mr-1" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"></path>
                          </svg>
                          ~{{ template.estimatedTimeMinutes }} min
                        </div>
                      }
                    </div>
                  </div>

                  <!-- Action Buttons -->
                  <div class="bg-gray-50 px-6 py-3 flex justify-between items-center">
                    <button
                      (click)="viewTemplate($event, template.id)"
                      class="text-blue-600 hover:text-blue-800 font-medium text-sm"
                    >
                      View Details
                    </button>
                    <button
                      (click)="useTemplate($event, template.id)"
                      class="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
                    >
                      Use Template
                    </button>
                  </div>
                </div>
              }
            </div>

            <!-- Pagination (if needed) -->
            @if (hasMore) {
              <div class="mt-8 text-center">
                <button
                  (click)="loadMore()"
                  class="px-6 py-3 bg-white text-gray-700 font-medium rounded-lg shadow-md hover:bg-gray-50 transition-colors"
                >
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
  templates: TemplateListItem[] = [];
  categories = TEMPLATE_CATEGORIES;
  loading = false;
  error = '';

  searchTerm = '';
  selectedCategory = '';
  activeFilter: 'all' | 'system' | 'my' | 'public' = 'all';
  currentPage = 1;
  pageSize = 20;
  hasMore = false;

  private searchTimeout: any;

  constructor(private templateService: TemplateService) {}

  ngOnInit(): void {
    this.loadTemplates();
  }

  loadTemplates(): void {
    this.loading = true;
    this.error = '';

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
        this.templates = templates;
        this.hasMore = templates.length === this.pageSize;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to load templates';
        this.loading = false;
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
    // Navigation handled by routerLink
  }

  useTemplate(event: Event, templateId: string): void {
    event.stopPropagation();
    // TODO: Open modal to select client and create proposal
    console.log('Use template:', templateId);
  }
}
