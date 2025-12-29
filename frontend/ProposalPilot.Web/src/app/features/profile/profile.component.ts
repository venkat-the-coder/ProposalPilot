import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { User } from '../../core/models/user.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="min-h-screen bg-gray-50 py-8">
      <div class="max-w-3xl mx-auto px-4">
        <div class="bg-white rounded-lg shadow-md p-6">
          <h2 class="text-2xl font-bold text-gray-900 mb-6">My Profile</h2>

          @if (loading) {
            <div class="text-center py-8">
              <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
              <p class="mt-2 text-gray-600">Loading profile...</p>
            </div>
          }

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

          @if (profile && !loading) {
            <form [formGroup]="profileForm" (ngSubmit)="onSubmit()">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    First Name *
                  </label>
                  <input
                    type="text"
                    formControlName="firstName"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                  @if (profileForm.get('firstName')?.touched && profileForm.get('firstName')?.invalid) {
                    <p class="text-red-500 text-sm mt-1">First name is required</p>
                  }
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Last Name *
                  </label>
                  <input
                    type="text"
                    formControlName="lastName"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                  @if (profileForm.get('lastName')?.touched && profileForm.get('lastName')?.invalid) {
                    <p class="text-red-500 text-sm mt-1">Last name is required</p>
                  }
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Company Name
                  </label>
                  <input
                    type="text"
                    formControlName="companyName"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Job Title
                  </label>
                  <input
                    type="text"
                    formControlName="jobTitle"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Phone Number
                  </label>
                  <input
                    type="tel"
                    formControlName="phoneNumber"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Email
                  </label>
                  <input
                    type="email"
                    [value]="profile.email"
                    disabled
                    class="w-full px-3 py-2 border border-gray-300 rounded-md bg-gray-100 cursor-not-allowed"
                  />
                  <p class="text-xs text-gray-500 mt-1">Email cannot be changed</p>
                </div>
              </div>

              <div class="mb-6 p-4 bg-gray-50 rounded-md">
                <h3 class="text-sm font-medium text-gray-900 mb-2">Account Information</h3>
                <div class="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <span class="text-gray-600">Email Verified:</span>
                    <span class="ml-2" [class.text-green-600]="profile.emailConfirmed" [class.text-red-600]="!profile.emailConfirmed">
                      {{ profile.emailConfirmed ? 'Yes' : 'No' }}
                    </span>
                  </div>
                  <div>
                    <span class="text-gray-600">Member Since:</span>
                    <span class="ml-2 text-gray-900">{{ profile.createdAt | date:'mediumDate' }}</span>
                  </div>
                </div>
              </div>

              <div class="flex justify-end gap-3">
                <button
                  type="button"
                  (click)="loadProfile()"
                  class="px-4 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  [disabled]="saving || profileForm.invalid"
                  class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {{ saving ? 'Saving...' : 'Save Changes' }}
                </button>
              </div>
            </form>
          }
        </div>
      </div>
    </div>
  `,
  styles: []
})
export class ProfileComponent implements OnInit {
  profile: User | null = null;
  profileForm: FormGroup;
  loading = false;
  saving = false;
  error = '';
  successMessage = '';

  constructor(
    private fb: FormBuilder,
    private userService: UserService
  ) {
    this.profileForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      companyName: [''],
      jobTitle: [''],
      phoneNumber: ['']
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.loading = true;
    this.error = '';
    this.successMessage = '';

    this.userService.getProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.profileForm.patchValue({
          firstName: profile.firstName,
          lastName: profile.lastName,
          companyName: profile.companyName || '',
          jobTitle: profile.jobTitle || '',
          phoneNumber: profile.phoneNumber || ''
        });
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to load profile';
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.profileForm.invalid) return;

    this.saving = true;
    this.error = '';
    this.successMessage = '';

    this.userService.updateProfile(this.profileForm.value).subscribe({
      next: (updatedProfile) => {
        this.profile = updatedProfile;
        this.successMessage = 'Profile updated successfully!';
        this.saving = false;

        // Clear success message after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to update profile';
        this.saving = false;
      }
    });
  }
}
