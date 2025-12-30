import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { BriefService } from '../../../core/services/brief.service';
import { Brief } from '../../../core/models/brief.model';

@Component({
  selector: 'app-brief-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50 to-purple-50 py-8">
      <div class="max-w-4xl mx-auto px-4">
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
        <div class="card-gradient mb-6 animate-slide-in-up">
          <div class="flex items-center gap-4">
            <div class="w-16 h-16 rounded-xl bg-gradient-to-r from-blue-500 to-purple-500 flex items-center justify-center shadow-lg">
              <svg class="w-8 h-8 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
            </div>
            <div>
              <h1 class="text-3xl font-extrabold text-gray-900 mb-1">
                New Client Brief
              </h1>
              <p class="text-gray-600">
                Paste your client's brief and let AI analyze it for you
              </p>
            </div>
          </div>
        </div>

        <!-- Selected Template Info -->
        @if (selectedTemplateId() && selectedTemplateName()) {
          <div class="card-gradient mb-6 animate-fade-in">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-12 h-12 rounded-lg bg-gradient-to-r from-purple-100 to-blue-100 flex items-center justify-center">
                  <svg class="w-6 h-6 text-purple-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
                  </svg>
                </div>
                <div>
                  <p class="text-sm text-gray-600 font-medium">Using Template</p>
                  <p class="text-lg font-bold text-gray-900">{{ selectedTemplateName() }}</p>
                </div>
              </div>
              <button
                type="button"
                (click)="removeTemplate()"
                class="text-red-600 hover:text-red-800 font-medium text-sm flex items-center gap-1 transition-colors"
              >
                <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
                Remove Template
              </button>
            </div>
          </div>
        }

        <!-- Main Form Card -->
        <div class="card-gradient animate-scale-in">
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <!-- Error Message -->
            @if (error()) {
              <div class="mb-6 p-4 bg-gradient-to-r from-red-50 to-pink-50 border-l-4 border-red-500 rounded-lg animate-fade-in">
                <div class="flex items-center">
                  <svg class="h-5 w-5 text-red-500 mr-2" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
                  </svg>
                  <p class="text-red-700 font-medium">{{ error() }}</p>
                </div>
              </div>
            }

            <!-- Project Title -->
            <div class="mb-6">
              <label for="title" class="block text-sm font-semibold text-gray-700 mb-2">
                Project Title *
              </label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <svg class="h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
                  </svg>
                </div>
                <input
                  id="title"
                  type="text"
                  formControlName="title"
                  placeholder="e.g., E-commerce Website Redesign"
                  class="input-field pl-10"
                >
              </div>
              @if (form.get('title')?.touched && form.get('title')?.invalid) {
                <p class="text-red-500 text-sm mt-1 flex items-center gap-1">
                  <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
                  </svg>
                  Title is required (minimum 3 characters)
                </p>
              }
            </div>

            <!-- Brief Content -->
            <div class="mb-6">
              <div class="flex items-center justify-between mb-2">
                <label for="rawContent" class="block text-sm font-semibold text-gray-700">
                  Client Brief *
                </label>
                <span class="text-sm text-gray-500">
                  {{ form.get('rawContent')?.value?.length || 0 }} / 50 characters minimum
                </span>
              </div>
              <textarea
                id="rawContent"
                formControlName="rawContent"
                rows="14"
                placeholder="Paste the client's brief, RFP, or project description here...

Examples of what to include:
• Project goals and objectives
• Target audience and requirements
• Budget and timeline
• Technical specifications
• Pain points and challenges"
                class="input-field font-mono text-sm resize-none"
              ></textarea>
              @if (form.get('rawContent')?.touched && form.get('rawContent')?.invalid) {
                <p class="text-red-500 text-sm mt-1 flex items-center gap-1">
                  <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                    <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
                  </svg>
                  Brief content is required (minimum 50 characters)
                </p>
              }
            </div>

            <!-- Info Box -->
            <div class="mb-6 p-4 bg-gradient-to-r from-blue-50 to-cyan-50 border-l-4 border-blue-500 rounded-lg">
              <div class="flex items-start gap-3">
                <svg class="w-6 h-6 text-blue-600 mt-0.5 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                </svg>
                <div>
                  <h3 class="text-sm font-semibold text-blue-900 mb-2">What happens next?</h3>
                  <ol class="text-sm text-blue-800 space-y-1 list-decimal list-inside">
                    <li>Your brief will be saved securely</li>
                    <li>AI will analyze requirements, pain points, and key signals</li>
                    <li>You'll receive a comprehensive analysis to guide your proposal</li>
                    <li>Analysis typically takes 5-10 seconds</li>
                  </ol>
                </div>
              </div>
            </div>

            <!-- Submit Button -->
            <div class="flex gap-4">
              <button
                type="button"
                routerLink="/dashboard"
                class="btn-secondary flex-1">
                <svg class="w-4 h-4 inline-block mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
                Cancel
              </button>
              <button
                type="submit"
                [disabled]="loading() || form.invalid"
                class="btn-primary flex-1 disabled:opacity-50 disabled:cursor-not-allowed disabled:transform-none">
                @if (loading()) {
                  <span class="flex items-center justify-center">
                    <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Analyzing with AI...
                  </span>
                } @else {
                  <span class="flex items-center justify-center gap-2">
                    <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                    </svg>
                    Analyze Brief with AI
                  </span>
                }
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `
})
export class BriefInputComponent implements OnInit {
  form: FormGroup;
  loading = signal(false);
  error = signal('');
  selectedTemplateId = signal<string | null>(null);
  selectedTemplateName = signal<string | null>(null);

  constructor(
    private fb: FormBuilder,
    private briefService: BriefService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      rawContent: ['', [Validators.required, Validators.minLength(50)]]
    });
  }

  ngOnInit(): void {
    // Check for template in query params or localStorage
    const templateId = this.route.snapshot.queryParams['template'] || localStorage.getItem('selectedTemplateId');
    const templateName = localStorage.getItem('selectedTemplateName');

    if (templateId && templateName) {
      this.selectedTemplateId.set(templateId);
      this.selectedTemplateName.set(templateName);
    }
  }

  removeTemplate(): void {
    this.selectedTemplateId.set(null);
    this.selectedTemplateName.set(null);
    localStorage.removeItem('selectedTemplateId');
    localStorage.removeItem('selectedTemplateName');
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.error.set('');

    // First, create the brief
    this.briefService.createBrief(this.form.value).subscribe({
      next: (brief: Brief) => {
        // Then, trigger the analysis
        this.briefService.analyzeBrief(brief.id).subscribe({
          next: (analyzedBrief: Brief) => {
            this.loading.set(false);

            // Keep the template selection in localStorage for proposal generation
            // It's already stored from the template detail page

            // Navigate to the analysis result page
            // Template selection will be picked up there
            this.router.navigate(['/briefs', analyzedBrief.id, 'analysis']);
          },
          error: (err) => {
            this.loading.set(false);
            this.error.set(err.error?.message || 'Error analyzing brief. Please try again.');
          }
        });
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message || 'Error creating brief. Please try again.');
      }
    });
  }
}
