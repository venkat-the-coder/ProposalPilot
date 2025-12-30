import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-settings',
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

        <!-- Header -->
        <div class="card-gradient mb-6 animate-slide-in-up">
          <div class="flex items-center gap-4">
            <div class="w-16 h-16 rounded-xl bg-gradient-to-r from-blue-600 to-purple-600 flex items-center justify-center shadow-lg">
              <svg class="w-8 h-8 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
            </div>
            <div>
              <h1 class="text-3xl font-extrabold text-gray-900 mb-1">
                Account Settings
              </h1>
              <p class="text-gray-600">
                Manage your account preferences and security
              </p>
            </div>
          </div>
        </div>

        <!-- Tabs -->
        <div class="card-gradient animate-scale-in">
          <div class="border-b border-gray-200">
            <nav class="flex -mb-px gap-2">
              <button
                (click)="activeTab = 'profile'"
                [class.border-blue-500]="activeTab === 'profile'"
                [class.text-blue-600]="activeTab === 'profile'"
                [class.bg-gradient-to-r]="activeTab === 'profile'"
                [class.from-blue-50]="activeTab === 'profile'"
                [class.to-purple-50]="activeTab === 'profile'"
                class="px-6 py-3 border-b-2 font-semibold text-sm rounded-t-lg transition-all duration-200"
                [class.border-transparent]="activeTab !== 'profile'"
                [class.text-gray-500]="activeTab !== 'profile'"
                [class.hover:bg-gray-50]="activeTab !== 'profile'"
              >
                <svg class="w-4 h-4 inline-block mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
                Profile
              </button>
              <button
                (click)="activeTab = 'security'"
                [class.border-blue-500]="activeTab === 'security'"
                [class.text-blue-600]="activeTab === 'security'"
                [class.bg-gradient-to-r]="activeTab === 'security'"
                [class.from-blue-50]="activeTab === 'security'"
                [class.to-purple-50]="activeTab === 'security'"
                class="px-6 py-3 border-b-2 font-semibold text-sm rounded-t-lg transition-all duration-200"
                [class.border-transparent]="activeTab !== 'security'"
                [class.text-gray-500]="activeTab !== 'security'"
                [class.hover:bg-gray-50]="activeTab !== 'security'"
              >
                <svg class="w-4 h-4 inline-block mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                </svg>
                Security
              </button>
              <button
                (click)="activeTab = 'notifications'"
                [class.border-blue-500]="activeTab === 'notifications'"
                [class.text-blue-600]="activeTab === 'notifications'"
                [class.bg-gradient-to-r]="activeTab === 'notifications'"
                [class.from-blue-50]="activeTab === 'notifications'"
                [class.to-purple-50]="activeTab === 'notifications'"
                class="px-6 py-3 border-b-2 font-semibold text-sm rounded-t-lg transition-all duration-200"
                [class.border-transparent]="activeTab !== 'notifications'"
                [class.text-gray-500]="activeTab !== 'notifications'"
                [class.hover:bg-gray-50]="activeTab !== 'notifications'"
              >
                <svg class="w-4 h-4 inline-block mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                </svg>
                Notifications
              </button>
            </nav>
          </div>

          <div class="p-6">
            <!-- Profile Tab -->
            @if (activeTab === 'profile') {
              <div class="text-center py-12 animate-fade-in">
                <div class="w-16 h-16 mx-auto mb-4 rounded-full bg-gradient-to-r from-blue-100 to-purple-100 flex items-center justify-center">
                  <svg class="w-8 h-8 text-blue-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                </div>
                <h3 class="text-xl font-bold text-gray-900 mb-2">Profile Settings</h3>
                <p class="text-gray-600 mb-6">
                  Profile settings are available on the dedicated profile page
                </p>
                <a routerLink="/profile" class="btn-primary inline-flex items-center gap-2">
                  <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6" />
                  </svg>
                  Go to Profile Page
                </a>
              </div>
            }

            <!-- Security Tab -->
            @if (activeTab === 'security') {
              <div class="animate-fade-in">
                <h3 class="text-xl font-bold text-gray-900 mb-6 flex items-center gap-2">
                  <svg class="w-6 h-6 text-blue-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                  </svg>
                  Change Password
                </h3>

                @if (error) {
                  <div class="mb-4 p-4 bg-gradient-to-r from-red-50 to-pink-50 border-l-4 border-red-500 rounded-lg animate-fade-in">
                    <div class="flex items-center">
                      <svg class="h-5 w-5 text-red-500 mr-2" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
                      </svg>
                      <p class="text-red-700 font-medium">{{ error }}</p>
                    </div>
                  </div>
                }

                @if (successMessage) {
                  <div class="mb-4 p-4 bg-gradient-to-r from-green-50 to-emerald-50 border-l-4 border-green-500 rounded-lg animate-fade-in">
                    <div class="flex items-center">
                      <svg class="h-5 w-5 text-green-500 mr-2" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
                      </svg>
                      <p class="text-green-700 font-medium">{{ successMessage }}</p>
                    </div>
                  </div>
                }

                <form [formGroup]="passwordForm" (ngSubmit)="onChangePassword()" class="max-w-md space-y-6">
                  <div>
                    <label class="block text-sm font-semibold text-gray-700 mb-2">
                      Current Password *
                    </label>
                    <div class="relative">
                      <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                        <svg class="h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                        </svg>
                      </div>
                      <input
                        type="password"
                        formControlName="currentPassword"
                        class="input-field pl-10"
                        placeholder="••••••••"
                      />
                    </div>
                    @if (passwordForm.get('currentPassword')?.touched && passwordForm.get('currentPassword')?.invalid) {
                      <p class="text-red-500 text-sm mt-1 flex items-center gap-1">
                        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
                        </svg>
                        Current password is required
                      </p>
                    }
                  </div>

                  <div>
                    <label class="block text-sm font-semibold text-gray-700 mb-2">
                      New Password *
                    </label>
                    <div class="relative">
                      <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                        <svg class="h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
                        </svg>
                      </div>
                      <input
                        type="password"
                        formControlName="newPassword"
                        class="input-field pl-10"
                        placeholder="••••••••"
                      />
                    </div>
                    @if (passwordForm.get('newPassword')?.touched && passwordForm.get('newPassword')?.invalid) {
                      <div class="text-red-500 text-sm mt-1 space-y-1">
                        @if (passwordForm.get('newPassword')?.hasError('required')) {
                          <p class="flex items-center gap-1">
                            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                              <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
                            </svg>
                            New password is required
                          </p>
                        }
                        @if (passwordForm.get('newPassword')?.hasError('minlength')) {
                          <p class="flex items-center gap-1">
                            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                              <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
                            </svg>
                            Password must be at least 8 characters
                          </p>
                        }
                        @if (passwordForm.get('newPassword')?.hasError('pattern')) {
                          <p class="flex items-center gap-1">
                            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                              <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
                            </svg>
                            Password must contain uppercase, lowercase, and number
                          </p>
                        }
                      </div>
                    }
                  </div>

                  <div>
                    <label class="block text-sm font-semibold text-gray-700 mb-2">
                      Confirm New Password *
                    </label>
                    <div class="relative">
                      <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                        <svg class="h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                      </div>
                      <input
                        type="password"
                        formControlName="confirmPassword"
                        class="input-field pl-10"
                        placeholder="••••••••"
                      />
                    </div>
                    @if (passwordForm.get('confirmPassword')?.touched && passwordForm.get('confirmPassword')?.invalid) {
                      <p class="text-red-500 text-sm mt-1 flex items-center gap-1">
                        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
                        </svg>
                        Passwords do not match
                      </p>
                    }
                  </div>

                  <div class="flex justify-end gap-3 pt-4">
                    <button
                      type="button"
                      (click)="resetPasswordForm()"
                      class="btn-secondary"
                    >
                      <svg class="w-4 h-4 inline-block mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                      </svg>
                      Cancel
                    </button>
                    <button
                      type="submit"
                      [disabled]="saving || passwordForm.invalid"
                      class="btn-primary disabled:opacity-50 disabled:cursor-not-allowed disabled:transform-none"
                    >
                      @if (saving) {
                        <span class="flex items-center gap-2">
                          <svg class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                          </svg>
                          Changing...
                        </span>
                      } @else {
                        <span class="flex items-center gap-2">
                          <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                          </svg>
                          Change Password
                        </span>
                      }
                    </button>
                  </div>
                </form>

                <div class="mt-8 p-6 bg-gradient-to-r from-yellow-50 to-orange-50 border border-yellow-200 rounded-xl">
                  <div class="flex items-start gap-3">
                    <div class="mt-1">
                      <svg class="w-6 h-6 text-yellow-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                      </svg>
                    </div>
                    <div>
                      <h4 class="text-sm font-bold text-yellow-800 mb-2">Password Requirements</h4>
                      <ul class="text-sm text-yellow-700 space-y-1">
                        <li class="flex items-center gap-2">
                          <svg class="w-4 h-4 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
                          </svg>
                          At least 8 characters long
                        </li>
                        <li class="flex items-center gap-2">
                          <svg class="w-4 h-4 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
                          </svg>
                          Contains at least one uppercase letter
                        </li>
                        <li class="flex items-center gap-2">
                          <svg class="w-4 h-4 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
                          </svg>
                          Contains at least one lowercase letter
                        </li>
                        <li class="flex items-center gap-2">
                          <svg class="w-4 h-4 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
                          </svg>
                          Contains at least one number
                        </li>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
            }

            <!-- Notifications Tab -->
            @if (activeTab === 'notifications') {
              <div class="text-center py-12 animate-fade-in">
                <div class="w-16 h-16 mx-auto mb-4 rounded-full bg-gradient-to-r from-blue-100 to-purple-100 flex items-center justify-center">
                  <svg class="w-8 h-8 text-blue-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                  </svg>
                </div>
                <h3 class="text-xl font-bold text-gray-900 mb-2">Notification Preferences</h3>
                <p class="text-gray-600 mb-6">
                  Notification settings will be available soon
                </p>
                <div class="inline-flex items-center gap-2 px-6 py-3 bg-gray-100 text-gray-600 rounded-xl font-medium">
                  <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  Coming Soon
                </div>
              </div>
            }
          </div>
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class SettingsComponent implements OnInit {
  activeTab: 'profile' | 'security' | 'notifications' = 'security';
  passwordForm: FormGroup;
  saving = false;
  error = '';
  successMessage = '';

  constructor(
    private fb: FormBuilder,
    private userService: UserService
  ) {
    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/)
      ]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {}

  passwordMatchValidator(group: FormGroup): { [key: string]: boolean } | null {
    const newPassword = group.get('newPassword')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;

    if (newPassword && confirmPassword && newPassword !== confirmPassword) {
      group.get('confirmPassword')?.setErrors({ mismatch: true });
      return { mismatch: true };
    }

    return null;
  }

  onChangePassword(): void {
    if (this.passwordForm.invalid) return;

    this.saving = true;
    this.error = '';
    this.successMessage = '';

    const { currentPassword, newPassword } = this.passwordForm.value;

    this.userService.changePassword({ currentPassword, newPassword }).subscribe({
      next: (response) => {
        this.successMessage = response.message || 'Password changed successfully!';
        this.saving = false;
        this.resetPasswordForm();

        // Clear success message after 5 seconds
        setTimeout(() => {
          this.successMessage = '';
        }, 5000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to change password';
        this.saving = false;
      }
    });
  }

  resetPasswordForm(): void {
    this.passwordForm.reset();
    this.error = '';
    this.successMessage = '';
  }
}
