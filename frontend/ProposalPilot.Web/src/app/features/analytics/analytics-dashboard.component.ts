import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AnalyticsService } from '../../core/services/analytics.service';
import {
  DashboardOverview,
  ProposalTrends,
  EngagementAnalytics,
  AIUsageAnalytics,
  ClientAnalytics,
  FullAnalyticsReport
} from '../../core/models/analytics.model';

@Component({
  selector: 'app-analytics-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, DecimalPipe, DatePipe, CurrencyPipe],
  templateUrl: './analytics-dashboard.component.html'
})
export class AnalyticsDashboardComponent implements OnInit {
  private readonly analyticsService = inject(AnalyticsService);

  // State
  report = signal<FullAnalyticsReport | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  selectedPeriod = signal(30);

  // Tabs
  activeTab = signal<'overview' | 'trends' | 'engagement' | 'clients' | 'ai'>('overview');

  // Computed values
  overview = computed(() => this.report()?.overview);
  trends = computed(() => this.report()?.trends);
  engagement = computed(() => this.report()?.engagement);
  aiUsage = computed(() => this.report()?.aiUsage);
  clients = computed(() => this.report()?.clients);

  // Chart data computed
  statusChartData = computed(() => {
    const breakdown = this.trends()?.statusBreakdown || [];
    const maxCount = Math.max(...breakdown.map(s => s.count), 1);
    return breakdown.map(s => ({
      ...s,
      widthPercent: (s.count / maxCount) * 100,
      color: this.getStatusColor(s.status)
    }));
  });

  monthlyChartData = computed(() => {
    const monthly = this.trends()?.monthlyStats || [];
    const maxTotal = Math.max(...monthly.map(m => m.total), 1);
    return monthly.map(m => ({
      ...m,
      heightPercent: (m.total / maxTotal) * 100
    }));
  });

  industryChartData = computed(() => {
    const industries = this.clients()?.industryBreakdown || [];
    const maxCount = Math.max(...industries.map(i => i.proposalCount), 1);
    return industries.map(i => ({
      ...i,
      widthPercent: (i.proposalCount / maxCount) * 100
    }));
  });

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.error.set(null);

    this.analyticsService.getFullReport(this.selectedPeriod()).subscribe({
      next: (data) => {
        this.report.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading analytics:', err);
        this.error.set(err.error?.message || 'Failed to load analytics data');
        this.loading.set(false);
      }
    });
  }

  changePeriod(days: number): void {
    this.selectedPeriod.set(days);
    this.loadData();
  }

  setActiveTab(tab: 'overview' | 'trends' | 'engagement' | 'clients' | 'ai'): void {
    this.activeTab.set(tab);
  }

  getStatusColor(status: string): string {
    switch (status?.toLowerCase()) {
      case 'draft': return 'bg-gray-400';
      case 'sent': return 'bg-blue-500';
      case 'viewed': return 'bg-purple-500';
      case 'accepted': return 'bg-green-500';
      case 'rejected': return 'bg-red-500';
      case 'expired': return 'bg-yellow-500';
      default: return 'bg-gray-400';
    }
  }

  getEngagementLevelClass(level: string): string {
    switch (level?.toLowerCase()) {
      case 'hot': return 'bg-red-100 text-red-800';
      case 'warm': return 'bg-yellow-100 text-yellow-800';
      case 'cold': return 'bg-blue-100 text-blue-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }

  formatCurrency(value: number | undefined): string {
    if (value === undefined || value === null) return '$0';
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      maximumFractionDigits: 0
    }).format(value);
  }

  formatPercent(value: number | undefined): string {
    if (value === undefined || value === null) return '0%';
    return `${value.toFixed(1)}%`;
  }

  formatNumber(value: number | undefined): string {
    if (value === undefined || value === null) return '0';
    return new Intl.NumberFormat('en-US').format(value);
  }

  getGrowthClass(value: number | undefined): string {
    if (!value) return 'text-gray-600';
    return value > 0 ? 'text-green-600' : value < 0 ? 'text-red-600' : 'text-gray-600';
  }

  getGrowthIcon(value: number | undefined): string {
    if (!value) return '';
    return value > 0 ? '↑' : value < 0 ? '↓' : '';
  }
}
