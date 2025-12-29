import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="min-h-screen bg-gray-50 py-8">
      <div class="max-w-4xl mx-auto px-4">
        <h2 class="text-2xl font-bold text-gray-900 mb-6">Account Settings</h2>

        <!-- Tabs -->
        <div class="bg-white rounded-lg shadow-md">
          <div class="border-b border-gray-200">
            <nav class="flex -mb-px">
              <button
                (click)="activeTab = 'profile'"
                [class.border-blue-500]="activeTab === 'profile'"
                [class.text-blue-600]="activeTab === 'profile'"
                class="px-6 py-3 border-b-2 font-medium text-sm"
                [class.border-transparent]="activeTab !== 'profile'"
                [class.text-gray-500]="activeTab !== 'profile'"
              >
                Profile
              </button>
              <button
                (click)="activeTab = 'security'"
                [class.border-blue-500]="activeTab === 'security'"
                [class.text-blue-600]="activeTab === 'security'"
                class="px-6 py-3 border-b-2 font-medium text-sm"
                [class.border-transparent]="activeTab !== 'security'"
                [class.text-gray-500]="activeTab !== 'security'"
              >
                Security
              </button>
            </nav>
          </div>

          <div class="p-6">
            <!-- Profile Tab -->
            @if (activeTab === 'profile') {
              <div class="text-center py-8">
                <p class="text-gray-600">Profile settings are available on the <a routerLink="/profile" class="text-blue-600 hover:underline">Profile page</a>.</p>
              </div>
            }

            <!-- Security Tab -->
            @if (activeTab === 'security') {
              <div>
                <h3 class="text-lg font-medium text-gray-900 mb-4">Change Password</h3>

                @if (error) {
                  <div class="mb-4 p-4 bg-red-100 text-red-700 rounded">
                    {{ error }}
                  </div>
                }

                @if (successMessage) {
                  <div class="mb-4 p-4 bg-green-100 text-green-700 rounded">
                    {{ successMessage }}
                  </div>
                }

                <form [formGroup]="passwordForm" (ngSubmit)="onChangePassword()" class="max-w-md">
                  <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Current Password *
                    </label>
                    <input
                      type="password"
                      formControlName="currentPassword"
                      class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                    @if (passwordForm.get('currentPassword')?.touched && passwordForm.get('currentPassword')?.invalid) {
                      <p class="text-red-500 text-sm mt-1">Current password is required</p>
                    }
                  </div>

                  <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      New Password *
                    </label>
                    <input
                      type="password"
                      formControlName="newPassword"
                      class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                    @if (passwordForm.get('newPassword')?.touched && passwordForm.get('newPassword')?.invalid) {
                      <p class="text-red-500 text-sm mt-1">
                        @if (passwordForm.get('newPassword')?.hasError('required')) {
                          New password is required
                        }
                        @if (passwordForm.get('newPassword')?.hasError('minlength')) {
                          Password must be at least 8 characters
                        }
                        @if (passwordForm.get('newPassword')?.hasError('pattern')) {
                          Password must contain uppercase, lowercase, and number
                        }
                      </p>
                    }
                  </div>

                  <div class="mb-6">
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Confirm New Password *
                    </label>
                    <input
                      type="password"
                      formControlName="confirmPassword"
                      class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                    @if (passwordForm.get('confirmPassword')?.touched && passwordForm.get('confirmPassword')?.invalid) {
                      <p class="text-red-500 text-sm mt-1">Passwords do not match</p>
                    }
                  </div>

                  <div class="flex justify-end gap-3">
                    <button
                      type="button"
                      (click)="resetPasswordForm()"
                      class="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
                    >
                      Cancel
                    </button>
                    <button
                      type="submit"
                      [disabled]="saving || passwordForm.invalid"
                      class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                      {{ saving ? 'Changing...' : 'Change Password' }}
                    </button>
                  </div>
                </form>

                <div class="mt-8 p-4 bg-yellow-50 border border-yellow-200 rounded-md">
                  <h4 class="text-sm font-medium text-yellow-800 mb-2">Password Requirements</h4>
                  <ul class="text-sm text-yellow-700 list-disc list-inside space-y-1">
                    <li>At least 8 characters long</li>
                    <li>Contains at least one uppercase letter</li>
                    <li>Contains at least one lowercase letter</li>
                    <li>Contains at least one number</li>
                  </ul>
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
  activeTab: 'profile' | 'security' = 'security';
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
