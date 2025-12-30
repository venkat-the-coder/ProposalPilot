import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TemplateService } from '../../../core/services/template.service';
import { TemplateListItem } from '../../../core/models/template.model';

@Component({
  selector: 'app-template-select-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    @if (isOpen) {
      <!-- Modal Overlay -->
      <div class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4" (click)="close()">
        <!-- Modal Content -->
        <div class="bg-white rounded-lg shadow-xl max-w-5xl w-full max-h-[90vh] overflow-hidden" (click)="$event.stopPropagation()">
          <!-- Modal Header -->
          <div class="bg-blue-600 text-white px-6 py-4 flex items-center justify-between">
            <h2 class="text-2xl font-bold">Select a Template</h2>
            <button (click)="close()" class="text-white hover:text-gray-200 transition-colors">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
              </svg>
            </button>
          </div>

          <!-- Search & Filter -->
          <div class="p-6 border-b">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <input
                type="text"
                [(ngModel)]="searchTerm"
                (ngModelChange)="onSearchChange()"
                placeholder="Search templates..."
                class="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
              <select
                [(ngModel)]="selectedCategory"
                (ngModelChange)="onFilterChange()"
                class="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="">All Categories</option>
                <option value="Web Development">Web Development</option>
                <option value="Marketing">Marketing</option>
                <option value="Design">Design</option>
                <option value="Consulting">Consulting</option>
                <option value="Writing">Writing</option>
                <option value="SEO">SEO</option>
                <option value="Mobile Development">Mobile Development</option>
                <option value="Social Media">Social Media</option>
              </select>
            </div>
          </div>

          <!-- Templates List -->
          <div class="p-6 overflow-y-auto max-h-[500px]">
            @if (loading) {
              <div class="flex items-center justify-center py-12">
                <div class="text-center">
                  <svg class="animate-spin h-10 w-10 text-blue-600 mx-auto mb-4" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  <p class="text-gray-600">Loading templates...</p>
                </div>
              </div>
            }

            @if (error) {
              <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-lg">
                {{ error }}
              </div>
            }

            @if (!loading && !error) {
              @if (filteredTemplates.length === 0) {
                <div class="text-center py-12">
                  <svg class="w-16 h-16 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
                  </svg>
                  <p class="text-gray-600">No templates found</p>
                  <button
                    (click)="startFromScratch()"
                    class="mt-4 px-6 py-2 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 transition-colors"
                  >
                    Start from Scratch
                  </button>
                </div>
              } @else {
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  @for (template of filteredTemplates; track template.id) {
                    <div
                      (click)="selectTemplate(template)"
                      class="border-2 rounded-lg p-4 cursor-pointer transition-all hover:border-blue-500 hover:shadow-md"
                      [class.border-blue-500]="selectedTemplateId === template.id"
                      [class.bg-blue-50]="selectedTemplateId === template.id"
                    >
                      <div class="flex items-start justify-between mb-2">
                        <h3 class="font-bold text-gray-900 flex-1">{{ template.name }}</h3>
                        @if (template.isSystemTemplate) {
                          <span class="px-2 py-1 bg-blue-100 text-blue-800 text-xs font-semibold rounded-full ml-2">
                            System
                          </span>
                        }
                      </div>
                      <p class="text-sm text-gray-600 mb-3 line-clamp-2">
                        {{ template.description || 'No description' }}
                      </p>
                      <div class="flex items-center justify-between text-xs text-gray-500">
                        <span class="px-2 py-1 bg-gray-100 rounded">{{ template.category }}</span>
                        <div class="flex items-center gap-3">
                          <span>{{ template.usageCount }}x used</span>
                          @if (template.estimatedTimeMinutes) {
                            <span>~{{ template.estimatedTimeMinutes }}min</span>
                          }
                        </div>
                      </div>
                    </div>
                  }
                </div>
              }
            }
          </div>

          <!-- Modal Footer -->
          <div class="bg-gray-50 px-6 py-4 flex items-center justify-between border-t">
            <button
              (click)="startFromScratch()"
              class="px-6 py-2 bg-gray-200 text-gray-700 font-medium rounded-lg hover:bg-gray-300 transition-colors"
            >
              Start from Scratch
            </button>
            <div class="flex gap-3">
              <button
                (click)="close()"
                class="px-6 py-2 bg-gray-200 text-gray-700 font-medium rounded-lg hover:bg-gray-300 transition-colors"
              >
                Cancel
              </button>
              <button
                (click)="confirmSelection()"
                [disabled]="!selectedTemplateId"
                class="px-6 py-2 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 transition-colors disabled:bg-gray-300 disabled:cursor-not-allowed"
              >
                Use Template
              </button>
            </div>
          </div>
        </div>
      </div>
    }
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
export class TemplateSelectModalComponent implements OnInit {
  @Input() isOpen = false;
  @Output() templateSelected = new EventEmitter<string>();
  @Output() startFromScratchClicked = new EventEmitter<void>();
  @Output() modalClosed = new EventEmitter<void>();

  templates: TemplateListItem[] = [];
  filteredTemplates: TemplateListItem[] = [];
  loading = false;
  error = '';

  searchTerm = '';
  selectedCategory = '';
  selectedTemplateId: string | null = null;

  private searchTimeout: any;

  constructor(private templateService: TemplateService) {}

  ngOnInit(): void {
    if (this.isOpen) {
      this.loadTemplates();
    }
  }

  ngOnChanges(): void {
    if (this.isOpen && this.templates.length === 0) {
      this.loadTemplates();
    }
  }

  loadTemplates(): void {
    this.loading = true;
    this.error = '';

    this.templateService.getTemplates().subscribe({
      next: (templates) => {
        this.templates = templates;
        this.filteredTemplates = templates;
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
      this.applyFilters();
    }, 300);
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    this.filteredTemplates = this.templates.filter(template => {
      const matchesSearch = !this.searchTerm ||
        template.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        template.description?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        template.tags.some(tag => tag.toLowerCase().includes(this.searchTerm.toLowerCase()));

      const matchesCategory = !this.selectedCategory || template.category === this.selectedCategory;

      return matchesSearch && matchesCategory;
    });
  }

  selectTemplate(template: TemplateListItem): void {
    this.selectedTemplateId = template.id;
  }

  confirmSelection(): void {
    if (this.selectedTemplateId) {
      this.templateSelected.emit(this.selectedTemplateId);
      this.close();
    }
  }

  startFromScratch(): void {
    this.startFromScratchClicked.emit();
    this.close();
  }

  close(): void {
    this.isOpen = false;
    this.selectedTemplateId = null;
    this.searchTerm = '';
    this.selectedCategory = '';
    this.modalClosed.emit();
  }
}
