import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { User } from '../../core/models/auth.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-gray-50 via-blue-50 to-purple-50">
      <!-- Modern Navigation -->
      <nav class="bg-white/80 backdrop-blur-lg border-b border-gray-200/50 shadow-lg sticky top-0 z-50">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="flex justify-between h-16">
            <div class="flex items-center space-x-8">
              <a routerLink="/dashboard" class="text-2xl font-extrabold text-gradient">
                ProposalPilot
              </a>
              <div class="hidden md:flex space-x-2">
                <a routerLink="/dashboard" class="nav-link nav-link-active">
                  <svg class="w-4 h-4 inline-block mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                  </svg>
                  Dashboard
                </a>
                <a routerLink="/briefs/new" class="nav-link">
                  <svg class="w-4 h-4 inline-block mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                  </svg>
                  New Brief
                </a>
                <a routerLink="/analytics" class="nav-link">
                  <svg class="w-4 h-4 inline-block mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                  Analytics
                </a>
                <a routerLink="/profile" class="nav-link">
                  <svg class="w-4 h-4 inline-block mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                  Profile
                </a>
                <a routerLink="/settings" class="nav-link">
                  <svg class="w-4 h-4 inline-block mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  </svg>
                  Settings
                </a>
              </div>
            </div>
            <div class="flex items-center space-x-4">
              @if (currentUser) {
                <div class="hidden md:flex items-center gap-3 px-4 py-2 rounded-xl bg-gradient-to-r from-blue-50 to-purple-50 border border-blue-100">
                  <div class="w-8 h-8 rounded-full bg-gradient-to-r from-blue-600 to-purple-600 flex items-center justify-center text-white font-bold text-sm">
                    {{ currentUser.firstName.charAt(0) }}{{ currentUser.lastName.charAt(0) }}
                  </div>
                  <span class="text-sm font-semibold text-gray-700">
                    {{ currentUser.firstName }} {{ currentUser.lastName }}
                  </span>
                </div>
              }
              <button
                (click)="logout()"
                class="btn-secondary px-4 py-2 text-sm">
                <svg class="w-4 h-4 inline-block mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                </svg>
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      <!-- Main Content -->
      <main class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
        @if (currentUser) {
          <!-- Welcome Header -->
          <div class="mb-8 animate-slide-in-up">
            <div class="card-gradient p-8">
              <div class="flex items-center justify-between">
                <div>
                  <h1 class="text-4xl font-extrabold text-gray-900 mb-2">
                    Welcome back, <span class="text-gradient">{{ currentUser.firstName }}!</span>
                  </h1>
                  <p class="text-lg text-gray-600">
                    Ready to create winning proposals with AI?
                  </p>
                </div>
                <div class="hidden md:block">
                  <svg class="w-24 h-24 text-blue-600 opacity-20" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M10.394 2.08a1 1 0 00-.788 0l-7 3a1 1 0 000 1.84L5.25 8.051a.999.999 0 01.356-.257l4-1.714a1 1 0 11.788 1.838L7.667 9.088l1.94.831a1 1 0 00.787 0l7-3a1 1 0 000-1.838l-7-3zM3.31 9.397L5 10.12v4.102a8.969 8.969 0 00-1.05-.174 1 1 0 01-.89-.89 11.115 11.115 0 01.25-3.762zM9.3 16.573A9.026 9.026 0 007 14.935v-3.957l1.818.78a3 3 0 002.364 0l5.508-2.361a11.026 11.026 0 01.25 3.762 1 1 0 01-.89.89 8.968 8.968 0 00-5.35 2.524 1 1 0 01-1.4 0zM6 18a1 1 0 001-1v-2.065a8.935 8.935 0 00-2-.712V17a1 1 0 001 1z"/>
                  </svg>
                </div>
              </div>
            </div>
          </div>

          <!-- Statistics Cards -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8 animate-fade-in">
            <div class="stat-card">
              <div class="flex items-center justify-between mb-3">
                <div class="w-12 h-12 rounded-xl bg-gradient-to-r from-blue-500 to-purple-500 flex items-center justify-center">
                  <svg class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                  </svg>
                </div>
                <span class="badge badge-primary">Active</span>
              </div>
              <div class="text-4xl font-extrabold text-gray-900 mb-1">0</div>
              <div class="text-sm font-medium text-gray-600">Active Proposals</div>
              <div class="mt-3 pt-3 border-t border-blue-100">
                <p class="text-xs text-gray-500">Start creating your first proposal!</p>
              </div>
            </div>

            <div class="stat-card">
              <div class="flex items-center justify-between mb-3">
                <div class="w-12 h-12 rounded-xl bg-gradient-to-r from-green-500 to-emerald-500 flex items-center justify-center">
                  <svg class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <span class="badge badge-success">Won</span>
              </div>
              <div class="text-4xl font-extrabold text-gray-900 mb-1">0</div>
              <div class="text-sm font-medium text-gray-600">Accepted Proposals</div>
              <div class="mt-3 pt-3 border-t border-green-100">
                <p class="text-xs text-gray-500">Your success rate: N/A</p>
              </div>
            </div>

            <div class="stat-card">
              <div class="flex items-center justify-between mb-3">
                <div class="w-12 h-12 rounded-xl bg-gradient-to-r from-purple-500 to-pink-500 flex items-center justify-center">
                  <svg class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                </div>
                <span class="badge badge-primary">Views</span>
              </div>
              <div class="text-4xl font-extrabold text-gray-900 mb-1">0</div>
              <div class="text-sm font-medium text-gray-600">Total Views</div>
              <div class="mt-3 pt-3 border-t border-purple-100">
                <p class="text-xs text-gray-500">Engagement: N/A</p>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="mb-8">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Quick Actions</h2>
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
              <button class="card hover:shadow-2xl transition-all duration-300 text-left group">
                <div class="w-12 h-12 rounded-xl bg-gradient-to-r from-blue-500 to-purple-500 flex items-center justify-center mb-3 group-hover:scale-110 transition-transform">
                  <svg class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                  </svg>
                </div>
                <h3 class="font-semibold text-gray-900 mb-1">Create Proposal</h3>
                <p class="text-sm text-gray-600">Start a new AI-powered proposal</p>
              </button>

              <button class="card hover:shadow-2xl transition-all duration-300 text-left group">
                <div class="w-12 h-12 rounded-xl bg-gradient-to-r from-green-500 to-emerald-500 flex items-center justify-center mb-3 group-hover:scale-110 transition-transform">
                  <svg class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
                  </svg>
                </div>
                <h3 class="font-semibold text-gray-900 mb-1">View Templates</h3>
                <p class="text-sm text-gray-600">Browse proposal templates</p>
              </button>

              <button class="card hover:shadow-2xl transition-all duration-300 text-left group">
                <div class="w-12 h-12 rounded-xl bg-gradient-to-r from-orange-500 to-red-500 flex items-center justify-center mb-3 group-hover:scale-110 transition-transform">
                  <svg class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                </div>
                <h3 class="font-semibold text-gray-900 mb-1">View Analytics</h3>
                <p class="text-sm text-gray-600">Check your performance</p>
              </button>

              <a routerLink="/settings" class="card hover:shadow-2xl transition-all duration-300 text-left group">
                <div class="w-12 h-12 rounded-xl bg-gradient-to-r from-gray-500 to-gray-700 flex items-center justify-center mb-3 group-hover:scale-110 transition-transform">
                  <svg class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  </svg>
                </div>
                <h3 class="font-semibold text-gray-900 mb-1">Settings</h3>
                <p class="text-sm text-gray-600">Manage your account</p>
              </a>
            </div>
          </div>

          <!-- Account Information -->
          <div class="card-gradient animate-fade-in">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Account Information</h2>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div class="space-y-3">
                <div class="flex items-center gap-3 p-3 bg-white rounded-lg">
                  <div class="w-10 h-10 rounded-lg bg-gradient-to-r from-blue-500 to-purple-500 flex items-center justify-center">
                    <svg class="w-5 h-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 12a4 4 0 10-8 0 4 4 0 008 0zm0 0v1.5a2.5 2.5 0 005 0V12a9 9 0 10-9 9m4.5-1.206a8.959 8.959 0 01-4.5 1.207" />
                    </svg>
                  </div>
                  <div>
                    <p class="text-xs font-medium text-gray-500">Email</p>
                    <p class="text-sm font-semibold text-gray-900">{{ currentUser.email }}</p>
                  </div>
                </div>

                <div class="flex items-center gap-3 p-3 bg-white rounded-lg">
                  <div class="w-10 h-10 rounded-lg bg-gradient-to-r from-green-500 to-emerald-500 flex items-center justify-center">
                    <svg class="w-5 h-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                    </svg>
                  </div>
                  <div>
                    <p class="text-xs font-medium text-gray-500">Full Name</p>
                    <p class="text-sm font-semibold text-gray-900">{{ currentUser.firstName }} {{ currentUser.lastName }}</p>
                  </div>
                </div>
              </div>

              <div class="space-y-3">
                @if (currentUser.companyName) {
                  <div class="flex items-center gap-3 p-3 bg-white rounded-lg">
                    <div class="w-10 h-10 rounded-lg bg-gradient-to-r from-purple-500 to-pink-500 flex items-center justify-center">
                      <svg class="w-5 h-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                      </svg>
                    </div>
                    <div>
                      <p class="text-xs font-medium text-gray-500">Company</p>
                      <p class="text-sm font-semibold text-gray-900">{{ currentUser.companyName }}</p>
                    </div>
                  </div>
                }

                <div class="flex items-center gap-3 p-3 bg-white rounded-lg">
                  <div class="w-10 h-10 rounded-lg bg-gradient-to-r from-orange-500 to-red-500 flex items-center justify-center">
                    <svg class="w-5 h-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </div>
                  <div>
                    <p class="text-xs font-medium text-gray-500">Member Since</p>
                    <p class="text-sm font-semibold text-gray-900">Just joined!</p>
                  </div>
                </div>
              </div>
            </div>

            <div class="mt-6 p-4 bg-gradient-to-r from-blue-50 to-purple-50 rounded-xl border border-blue-100">
              <div class="flex items-start gap-3">
                <div class="mt-1">
                  <svg class="w-6 h-6 text-blue-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <div>
                  <h3 class="font-semibold text-blue-900 mb-1">Getting Started</h3>
                  <p class="text-sm text-blue-700">
                    The proposal creation features are coming soon! You can update your profile and settings in the meantime.
                  </p>
                </div>
              </div>
            </div>
          </div>
        }
      </main>
    </div>
  `,
  styles: []
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  logout(): void {
    this.authService.logout().subscribe({
      complete: () => {
        this.router.navigate(['/auth/login']);
      }
    });
  }
}
