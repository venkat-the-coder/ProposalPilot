import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { SubscriptionService, SubscriptionStatus } from '../../core/services/subscription.service';

interface PricingTier {
  name: string;
  price: number;
  priceLabel: string;
  description: string;
  features: string[];
  highlighted: boolean;
  planKey: string;
  buttonText: string;
  buttonClass: string;
}

@Component({
  selector: 'app-pricing',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 py-12 px-4 sm:px-6 lg:px-8">
      <div class="max-w-7xl mx-auto">
        <!-- Header -->
        <div class="text-center mb-12">
          <h1 class="text-4xl font-bold text-gray-900 mb-4">
            Choose Your Plan
          </h1>
          <p class="text-xl text-gray-600 max-w-2xl mx-auto">
            Start creating winning proposals today. Upgrade anytime as your business grows.
          </p>
        </div>

        <!-- Current Plan Banner -->
        @if (currentStatus) {
          <div class="mb-8 bg-blue-100 border border-blue-400 text-blue-700 px-6 py-4 rounded-lg text-center">
            <p class="font-medium">
              Current Plan: <span class="font-bold">{{ currentStatus.plan }}</span>
              @if (currentStatus.proposalsPerMonth > 0) {
                <span class="ml-4">
                  {{ currentStatus.proposalsUsedThisMonth }} /
                  {{ currentStatus.proposalsPerMonth === -1 ? '∞' : currentStatus.proposalsPerMonth }}
                  proposals used this month
                </span>
              }
            </p>
          </div>
        }

        <!-- Pricing Cards -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
          @for (tier of pricingTiers; track tier.name) {
            <div
              class="bg-white rounded-2xl shadow-lg overflow-hidden transition-transform hover:scale-105"
              [class.ring-4]="tier.highlighted"
              [class.ring-blue-500]="tier.highlighted"
            >
              @if (tier.highlighted) {
                <div class="bg-blue-600 text-white text-center py-2 text-sm font-bold">
                  MOST POPULAR
                </div>
              }

              <div class="p-8">
                <h3 class="text-2xl font-bold text-gray-900 mb-2">{{ tier.name }}</h3>
                <p class="text-gray-600 mb-6">{{ tier.description }}</p>

                <div class="mb-6">
                  <span class="text-4xl font-bold text-gray-900">{{ tier.priceLabel }}</span>
                  @if (tier.price > 0) {
                    <span class="text-gray-600">/month</span>
                  }
                </div>

                <ul class="space-y-3 mb-8">
                  @for (feature of tier.features; track feature) {
                    <li class="flex items-start">
                      <svg class="w-5 h-5 text-green-500 mr-2 mt-0.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
                      </svg>
                      <span class="text-gray-700">{{ feature }}</span>
                    </li>
                  }
                </ul>

                <button
                  (click)="selectPlan(tier.planKey)"
                  [disabled]="loading || (currentStatus?.plan === tier.name)"
                  [class]="tier.buttonClass + ' w-full py-3 px-6 rounded-lg font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed'"
                >
                  @if (currentStatus?.plan === tier.name) {
                    Current Plan
                  } @else {
                    {{ tier.buttonText }}
                  }
                </button>
              </div>
            </div>
          }
        </div>

        <!-- FAQ Section -->
        <div class="mt-16 max-w-3xl mx-auto">
          <h2 class="text-3xl font-bold text-center text-gray-900 mb-8">
            Frequently Asked Questions
          </h2>

          <div class="space-y-6">
            <div class="bg-white rounded-lg shadow p-6">
              <h3 class="font-bold text-gray-900 mb-2">Can I change plans anytime?</h3>
              <p class="text-gray-600">Yes! You can upgrade or downgrade your plan at any time. Changes take effect immediately.</p>
            </div>

            <div class="bg-white rounded-lg shadow p-6">
              <h3 class="font-bold text-gray-900 mb-2">What happens if I exceed my proposal limit?</h3>
              <p class="text-gray-600">You'll be prompted to upgrade to a higher tier or wait until your monthly quota resets.</p>
            </div>

            <div class="bg-white rounded-lg shadow p-6">
              <h3 class="font-bold text-gray-900 mb-2">Is there a free trial?</h3>
              <p class="text-gray-600">The Free plan gives you 3 proposals per month to try ProposalPilot risk-free!</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class PricingComponent implements OnInit {
  loading = false;
  currentStatus: SubscriptionStatus | null = null;

  pricingTiers: PricingTier[] = [
    {
      name: 'Free',
      price: 0,
      priceLabel: '$0',
      description: 'Perfect for trying out ProposalPilot',
      features: [
        '3 proposals per month',
        'Basic templates',
        'PDF export',
        'Email support'
      ],
      highlighted: false,
      planKey: 'Free',
      buttonText: 'Get Started',
      buttonClass: 'bg-gray-200 text-gray-700 hover:bg-gray-300'
    },
    {
      name: 'Starter',
      price: 29,
      priceLabel: '$29',
      description: 'For freelancers and small teams',
      features: [
        '10 proposals per month',
        'AI-powered analysis',
        'Advanced templates',
        'PDF & DOCX export',
        'Priority email support'
      ],
      highlighted: false,
      planKey: 'Starter',
      buttonText: 'Start Free Trial',
      buttonClass: 'bg-blue-600 text-white hover:bg-blue-700'
    },
    {
      name: 'Professional',
      price: 99,
      priceLabel: '$99',
      description: 'For growing businesses',
      features: [
        '50 proposals per month',
        'AI-powered analysis',
        'Advanced templates',
        'PDF & DOCX export',
        'Quality scoring',
        'Analytics dashboard',
        'Priority support',
        'Custom branding'
      ],
      highlighted: true,
      planKey: 'Professional',
      buttonText: 'Start Free Trial',
      buttonClass: 'bg-blue-600 text-white hover:bg-blue-700'
    },
    {
      name: 'Enterprise',
      price: 299,
      priceLabel: '$299',
      description: 'For large teams and agencies',
      features: [
        'Unlimited proposals',
        'Everything in Professional',
        'White-labeling',
        'Dedicated account manager',
        'Custom integrations',
        'SLA guarantee',
        'Team collaboration',
        'Advanced analytics'
      ],
      highlighted: false,
      planKey: 'Enterprise',
      buttonText: 'Contact Sales',
      buttonClass: 'bg-indigo-600 text-white hover:bg-indigo-700'
    }
  ];

  constructor(
    private subscriptionService: SubscriptionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCurrentStatus();
  }

  loadCurrentStatus(): void {
    this.subscriptionService.getSubscriptionStatus().subscribe({
      next: (status) => {
        this.currentStatus = status;
      },
      error: (error) => {
        console.error('Error loading subscription status:', error);
      }
    });
  }

  selectPlan(planKey: string): void {
    if (planKey === 'Free') {
      // Free plan - just navigate to dashboard
      this.router.navigate(['/dashboard']);
      return;
    }

    if (planKey === 'Enterprise') {
      // Enterprise - mailto for sales
      window.location.href = 'mailto:sales@proposalpilot.com?subject=Enterprise Plan Inquiry';
      return;
    }

    this.loading = true;
    this.subscriptionService.redirectToCheckout(planKey);
  }
}
