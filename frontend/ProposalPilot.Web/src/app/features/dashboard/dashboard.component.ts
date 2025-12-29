import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { User } from '../../core/models/auth.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="min-h-screen bg-gray-50">
      <nav class="bg-white shadow-sm">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="flex justify-between h-16">
            <div class="flex items-center">
              <h1 class="text-xl font-bold text-gray-900">ProposalPilot</h1>
            </div>
            <div class="flex items-center space-x-4">
              @if (currentUser) {
                <span class="text-sm text-gray-700">
                  {{ currentUser.firstName }} {{ currentUser.lastName }}
                </span>
              }
              <button
                (click)="logout()"
                class="px-4 py-2 text-sm font-medium text-gray-700 hover:text-gray-900 hover:bg-gray-100 rounded-md transition-colors">
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      <main class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div class="px-4 py-6 sm:px-0">
          <div class="bg-white rounded-lg shadow p-6">
            <h2 class="text-2xl font-bold text-gray-900 mb-4">Welcome to ProposalPilot!</h2>

            @if (currentUser) {
              <div class="space-y-4">
                <p class="text-gray-700">
                  Welcome back, <span class="font-semibold">{{ currentUser.firstName }}!</span>
                </p>

                <div class="bg-primary-50 border border-primary-200 rounded-md p-4">
                  <h3 class="font-semibold text-primary-900 mb-2">Your Account Details:</h3>
                  <dl class="space-y-2 text-sm">
                    <div>
                      <dt class="inline font-medium text-gray-700">Email:</dt>
                      <dd class="inline ml-2 text-gray-600">{{ currentUser.email }}</dd>
                    </div>
                    <div>
                      <dt class="inline font-medium text-gray-700">Name:</dt>
                      <dd class="inline ml-2 text-gray-600">{{ currentUser.firstName }} {{ currentUser.lastName }}</dd>
                    </div>
                    @if (currentUser.companyName) {
                      <div>
                        <dt class="inline font-medium text-gray-700">Company:</dt>
                        <dd class="inline ml-2 text-gray-600">{{ currentUser.companyName }}</dd>
                      </div>
                    }
                  </dl>
                </div>

                <div class="mt-6">
                  <p class="text-gray-600 mb-4">
                    This is your dashboard. The proposal creation features are coming soon!
                  </p>
                  <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <div class="card p-4 text-center">
                      <div class="text-3xl font-bold text-primary-600 mb-2">0</div>
                      <div class="text-sm text-gray-600">Active Proposals</div>
                    </div>
                    <div class="card p-4 text-center">
                      <div class="text-3xl font-bold text-green-600 mb-2">0</div>
                      <div class="text-sm text-gray-600">Accepted</div>
                    </div>
                    <div class="card p-4 text-center">
                      <div class="text-3xl font-bold text-blue-600 mb-2">0</div>
                      <div class="text-sm text-gray-600">Total Views</div>
                    </div>
                  </div>
                </div>
              </div>
            }
          </div>
        </div>
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
