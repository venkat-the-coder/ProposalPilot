import { Component, Input, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  EngagementService,
  ProposalEngagementMetrics,
  FollowUpRecommendation,
  FollowUpDto
} from '../../../../core/services/engagement.service';

@Component({
  selector: 'app-engagement-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './engagement-dashboard.component.html'
})
export class EngagementDashboardComponent implements OnInit {
  @Input() proposalId!: string;

  private readonly engagementService = inject(EngagementService);

  // State
  metrics = signal<ProposalEngagementMetrics | null>(null);
  recommendation = signal<FollowUpRecommendation | null>(null);
  followUps = signal<FollowUpDto[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  // Schedule follow-up modal
  showScheduleModal = signal(false);
  scheduledDate = signal('');
  scheduledTime = signal('10:00');
  customMessage = signal('');
  scheduling = signal(false);

  // Computed values
  engagementLevelClass = computed(() => {
    const level = this.metrics()?.engagementLevel || '';
    return this.engagementService.getEngagementLevelClass(level);
  });

  engagementLevelIcon = computed(() => {
    const level = this.metrics()?.engagementLevel || '';
    return this.engagementService.getEngagementLevelIcon(level);
  });

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.error.set(null);

    // Load all data in parallel
    this.engagementService.getEngagementMetrics(this.proposalId).subscribe({
      next: (data) => {
        this.metrics.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading metrics:', err);
        this.error.set('Failed to load engagement metrics');
        this.loading.set(false);
      }
    });

    this.engagementService.getFollowUpRecommendation(this.proposalId).subscribe({
      next: (data) => this.recommendation.set(data)
    });

    this.engagementService.getFollowUps(this.proposalId).subscribe({
      next: (data) => this.followUps.set(data)
    });
  }

  openScheduleModal(): void {
    // Set default date to tomorrow
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.scheduledDate.set(tomorrow.toISOString().split('T')[0]);
    this.scheduledTime.set('10:00');
    this.customMessage.set(this.recommendation()?.suggestedMessage || '');
    this.showScheduleModal.set(true);
  }

  closeScheduleModal(): void {
    this.showScheduleModal.set(false);
  }

  scheduleFollowUp(): void {
    const date = this.scheduledDate();
    const time = this.scheduledTime();
    if (!date || !time) return;

    const scheduledFor = new Date(`${date}T${time}:00Z`).toISOString();

    this.scheduling.set(true);
    this.engagementService.scheduleFollowUp({
      proposalId: this.proposalId,
      scheduledFor,
      customMessage: this.customMessage() || undefined
    }).subscribe({
      next: (result) => {
        if (result.success) {
          this.closeScheduleModal();
          this.loadData();
        } else {
          this.error.set(result.message);
        }
        this.scheduling.set(false);
      },
      error: (err) => {
        console.error('Error scheduling follow-up:', err);
        this.error.set('Failed to schedule follow-up');
        this.scheduling.set(false);
      }
    });
  }

  cancelFollowUp(followUpId: string): void {
    if (!confirm('Are you sure you want to cancel this follow-up?')) return;

    this.engagementService.cancelFollowUp(followUpId).subscribe({
      next: () => this.loadData(),
      error: (err) => {
        console.error('Error cancelling follow-up:', err);
        this.error.set('Failed to cancel follow-up');
      }
    });
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'scheduled':
        return 'bg-blue-100 text-blue-800';
      case 'sent':
        return 'bg-green-100 text-green-800';
      case 'cancelled':
        return 'bg-gray-100 text-gray-800';
      case 'skipped':
        return 'bg-yellow-100 text-yellow-800';
      case 'failed':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  formatDuration(duration: string | null): string {
    if (!duration) return 'N/A';
    // Parse TimeSpan format (e.g., "1.02:30:00" for 1 day 2 hours 30 min)
    const parts = duration.split('.');
    if (parts.length === 2) {
      const days = parseInt(parts[0], 10);
      if (days > 0) return `${days}d ago`;
    }
    const timeParts = (parts[parts.length - 1] || duration).split(':');
    const hours = parseInt(timeParts[0], 10);
    const minutes = parseInt(timeParts[1], 10);
    if (hours > 0) return `${hours}h ago`;
    if (minutes > 0) return `${minutes}m ago`;
    return 'Just now';
  }
}
