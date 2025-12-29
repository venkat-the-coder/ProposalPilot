export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  companyName?: string;
  jobTitle?: string;
  phoneNumber?: string;
  profileImageUrl?: string;
  emailConfirmed: boolean;
  lastLoginAt?: string;
  createdAt: string;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  companyName?: string;
  jobTitle?: string;
  phoneNumber?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}
