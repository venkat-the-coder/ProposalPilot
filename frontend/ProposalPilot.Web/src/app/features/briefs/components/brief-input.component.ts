import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BriefService } from '../../../core/services/brief.service';
import { Brief } from '../../../core/models/brief.model';

@Component({
  selector: 'app-brief-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="min-h-screen bg-gray-50 py-8">
      <div class="max-w-4xl mx-auto px-4">
        <div class="bg-white rounded-lg shadow-md p-8">
          <h1 class="text-3xl font-bold text-gray-900 mb-2">New Client Brief</h1>
          <p class="text-gray-600 mb-8">Paste your client's brief below and our AI will analyze it to help you create a winning proposal.</p>

          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <!-- Title -->
            <div class="mb-6">
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Project Title *
              </label>
              <input
                type="text"
                formControlName="title"
                placeholder="e.g., E-commerce Website Redesign"
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
              @if (form.get('title')?.touched && form.get('title')?.invalid) {
                <p class="text-red-500 text-sm mt-1">Title is required</p>
              }
            </div>

            <!-- Raw Content -->
            <div class="mb-6">
              <label class="block text-sm font-medium text-gray-700 mb-2">
                Client Brief *
              </label>
              <textarea
                formControlName="rawContent"
                rows="12"
                placeholder="Paste the client's brief, RFP, or project description here..."
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 font-mono text-sm"
              ></textarea>
              @if (form.get('rawContent')?.touched && form.get('rawContent')?.invalid) {
                <p class="text-red-500 text-sm mt-1">Brief content is required (minimum 50 characters)</p>
              }
              <p class="text-gray-500 text-sm mt-1">
                {{ form.get('rawContent')?.value?.length || 0 }} characters
              </p>
            </div>

            <!-- Error Message -->
            @if (error) {
              <div class="mb-6 p-4 bg-red-100 text-red-700 rounded-lg">
                {{ error }}
              </div>
            }

            <!-- Submit Button -->
            <div class="flex gap-4">
              <button
                type="submit"
                [disabled]="loading || form.invalid"
                class="flex-1 py-3 px-6 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                @if (loading) {
                  <span class="flex items-center justify-center">
                    <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Creating & Analyzing...
                  </span>
                } @else {
                  Create Brief & Analyze with AI
                }
              </button>
            </div>
          </form>

          <!-- Info Box -->
          <div class="mt-8 p-4 bg-blue-50 border border-blue-200 rounded-lg">
            <h3 class="text-sm font-semibold text-blue-900 mb-2">💡 What happens next?</h3>
            <ol class="text-sm text-blue-800 space-y-1 list-decimal list-inside">
              <li>Your brief will be saved securely</li>
              <li>Our AI will analyze the requirements, pain points, and project signals</li>
              <li>You'll get a comprehensive analysis to guide your proposal</li>
              <li>The analysis typically takes 5-10 seconds</li>
            </ol>
          </div>
        </div>
      </div>
    </div>
  `
})
export class BriefInputComponent {
  form: FormGroup;
  loading = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private briefService: BriefService,
    private router: Router
  ) {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      rawContent: ['', [Validators.required, Validators.minLength(50)]]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    this.error = '';

    // First, create the brief
    this.briefService.createBrief(this.form.value).subscribe({
      next: (brief: Brief) => {
        // Then, trigger the analysis
        this.briefService.analyzeBrief(brief.id).subscribe({
          next: (analyzedBrief: Brief) => {
            this.loading = false;
            // Navigate to the analysis result page
            this.router.navigate(['/briefs', analyzedBrief.id, 'analysis']);
          },
          error: (err) => {
            this.loading = false;
            this.error = err.error?.message || 'Error analyzing brief. Please try again.';
          }
        });
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Error creating brief. Please try again.';
      }
    });
  }
}
