import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { QuillModule } from 'ngx-quill';
import { ProposalService } from '../../../core/services/proposal.service';
import { Proposal, ProposalContent } from '../../../core/models/proposal.model';
import { QualityScoreSidebarComponent } from './quality-score-sidebar.component';

@Component({
  selector: 'app-proposal-editor',
  standalone: true,
  imports: [CommonModule, FormsModule, QuillModule, RouterLink, QualityScoreSidebarComponent],
  template: `
    <div class="min-h-screen bg-gray-50 py-8">
      <div class="max-w-7xl mx-auto px-4 grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Main Editor Column -->
        <div class="lg:col-span-2">
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
          <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {{ error }}
          </div>
        }

        @if (proposal && sections) {
          <!-- Header -->
          <div class="bg-white rounded-lg shadow-md p-6 mb-6">
            <div class="flex items-center justify-between mb-4">
              <h1 class="text-3xl font-bold text-gray-900">Edit Proposal</h1>
              <div class="flex gap-3">
                <!-- Export Buttons -->
                <div class="flex gap-2 mr-2 border-r pr-3">
                  <button
                    (click)="exportPdf()"
                    class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 flex items-center gap-2"
                    title="Export as PDF"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M9 19l3 3m0 0l3-3m-3 3V10"></path>
                    </svg>
                    PDF
                  </button>
                  <button
                    (click)="exportDocx()"
                    class="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 flex items-center gap-2"
                    title="Export as DOCX"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M9 19l3 3m0 0l3-3m-3 3V10"></path>
                    </svg>
                    DOCX
                  </button>
                </div>
                <button
                  [routerLink]="['/proposals', proposal.id]"
                  class="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300"
                >
                  Cancel
                </button>
                <button
                  (click)="saveProposal()"
                  [disabled]="saving"
                  class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
                >
                  @if (saving) {
                    Saving...
                  } @else {
                    Save Changes
                  }
                </button>
              </div>
            </div>
          </div>

          <!-- Section Editors -->
          <div class="space-y-6">
            <!-- Opening Hook -->
            <div class="bg-white rounded-lg shadow-md p-6">
              <h2 class="text-xl font-bold text-gray-900 mb-4">Opening Hook</h2>
              <quill-editor
                [(ngModel)]="sections.opening_hook"
                [modules]="quillModules"
                class="bg-white"
              ></quill-editor>
            </div>

            <!-- Problem Statement -->
            <div class="bg-white rounded-lg shadow-md p-6">
              <h2 class="text-xl font-bold text-gray-900 mb-4">Problem Statement</h2>
              <quill-editor
                [(ngModel)]="sections.problem_statement"
                [modules]="quillModules"
              ></quill-editor>
            </div>

            <!-- Proposed Solution -->
            <div class="bg-white rounded-lg shadow-md p-6">
              <h2 class="text-xl font-bold text-gray-900 mb-4">Proposed Solution</h2>
              <quill-editor
                [(ngModel)]="sections.proposed_solution"
                [modules]="quillModules"
              ></quill-editor>
            </div>

            <!-- Methodology -->
            <div class="bg-white rounded-lg shadow-md p-6">
              <h2 class="text-xl font-bold text-gray-900 mb-4">Methodology</h2>
              <quill-editor
                [(ngModel)]="sections.methodology"
                [modules]="quillModules"
              ></quill-editor>
            </div>

            <!-- Timeline -->
            <div class="bg-white rounded-lg shadow-md p-6">
              <h2 class="text-xl font-bold text-gray-900 mb-4">Timeline</h2>
              <quill-editor
                [(ngModel)]="sections.timeline"
                [modules]="quillModules"
              ></quill-editor>
            </div>

            <!-- Why Choose Us -->
            <div class="bg-white rounded-lg shadow-md p-6">
              <h2 class="text-xl font-bold text-gray-900 mb-4">Why Choose Us</h2>
              <quill-editor
                [(ngModel)]="sections.why_choose_us"
                [modules]="quillModules"
              ></quill-editor>
            </div>

            <!-- Next Steps -->
            <div class="bg-white rounded-lg shadow-md p-6">
              <h2 class="text-xl font-bold text-gray-900 mb-4">Next Steps</h2>
              <quill-editor
                [(ngModel)]="sections.next_steps"
                [modules]="quillModules"
              ></quill-editor>
            </div>
          </div>

          <!-- Save Button (Bottom) -->
          <div class="mt-8 flex justify-end gap-3">
            <button
              [routerLink]="['/proposals', proposal.id]"
              class="px-6 py-3 bg-gray-200 text-gray-700 font-medium rounded-lg hover:bg-gray-300"
            >
              Cancel
            </button>
            <button
              (click)="saveProposal()"
              [disabled]="saving"
              class="px-8 py-3 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 disabled:opacity-50"
            >
              @if (saving) {
                <span class="flex items-center">
                  <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Saving...
                </span>
              } @else {
                Save Changes
              }
            </button>
          </div>
        }
        </div>

        <!-- Quality Score Sidebar -->
        <div class="lg:col-span-1">
          <div class="sticky top-8">
            <app-quality-score-sidebar [proposalId]="proposal?.id"></app-quality-score-sidebar>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    ::ng-deep .ql-container {
      min-height: 200px;
      font-size: 16px;
    }
    ::ng-deep .ql-editor {
      min-height: 200px;
    }
  `]
})
export class ProposalEditorComponent implements OnInit {
  proposal: Proposal | null = null;
  sections: any = {};
  loading = true;
  saving = false;
  error = '';

  quillModules = {
    toolbar: [
      ['bold', 'italic', 'underline', 'strike'],
      ['blockquote', 'code-block'],
      [{ 'list': 'ordered'}, { 'list': 'bullet' }],
      [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
      ['link'],
      ['clean']
    ]
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private proposalService: ProposalService
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
            const content: ProposalContent = JSON.parse(proposal.deliverablesJson);
            this.sections = { ...content.sections };
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

  saveProposal(): void {
    if (!this.proposal) return;

    this.saving = true;
    this.error = '';

    this.proposalService.updateProposal(this.proposal.id, { sections: this.sections }).subscribe({
      next: () => {
        this.saving = false;
        // Navigate back to view mode
        this.router.navigate(['/proposals', this.proposal!.id]);
      },
      error: (err) => {
        this.saving = false;
        this.error = err.error?.message || 'Error saving proposal';
        alert(this.error);
      }
    });
  }

  exportPdf(): void {
    if (!this.proposal) return;
    this.proposalService.exportPdf(this.proposal.id);
  }

  exportDocx(): void {
    if (!this.proposal) return;
    this.proposalService.exportDocx(this.proposal.id);
  }
}
