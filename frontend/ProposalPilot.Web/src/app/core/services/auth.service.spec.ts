import { TestBed } from '@angular/core/testing';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { AuthService } from './auth.service';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';
import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest';
import { firstValueFrom } from 'rxjs';

describe('AuthService', () => {
  let service: AuthService;
  let httpClientSpy: { post: ReturnType<typeof vi.fn>; get: ReturnType<typeof vi.fn> };

  const mockAuthResponse: AuthResponse = {
    userId: 'test-user-id',
    email: 'test@example.com',
    firstName: 'Test',
    lastName: 'User',
    companyName: 'Test Company',
    accessToken: 'mock-access-token',
    refreshToken: 'mock-refresh-token',
    expiresAt: new Date(Date.now() + 3600000).toISOString()
  };

  beforeEach(() => {
    // Clear localStorage before each test
    localStorage.clear();

    httpClientSpy = {
      post: vi.fn(),
      get: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: HttpClient, useValue: httpClientSpy }
      ]
    });

    service = TestBed.inject(AuthService);
  });

  afterEach(() => {
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('login', () => {
    it('should call API and store tokens on successful login', async () => {
      const loginRequest: LoginRequest = {
        email: 'test@example.com',
        password: 'password123'
      };

      httpClientSpy.post.mockReturnValue(of(mockAuthResponse));

      const response = await firstValueFrom(service.login(loginRequest));

      expect(response).toEqual(mockAuthResponse);
      expect(localStorage.getItem('accessToken')).toBe('mock-access-token');
      expect(localStorage.getItem('refreshToken')).toBe('mock-refresh-token');
      expect(service.getCurrentUser()?.email).toBe('test@example.com');
    });
  });

  describe('register', () => {
    it('should call API and store tokens on successful registration', async () => {
      const registerRequest: RegisterRequest = {
        email: 'test@example.com',
        password: 'password123',
        firstName: 'Test',
        lastName: 'User'
      };

      httpClientSpy.post.mockReturnValue(of(mockAuthResponse));

      const response = await firstValueFrom(service.register(registerRequest));

      expect(response).toEqual(mockAuthResponse);
      expect(localStorage.getItem('accessToken')).toBe('mock-access-token');
      expect(service.isAuthenticated()).toBe(true);
    });
  });

  describe('logout', () => {
    it('should clear auth data when no token exists', async () => {
      await firstValueFrom(service.logout());

      expect(localStorage.getItem('accessToken')).toBeNull();
      expect(localStorage.getItem('refreshToken')).toBeNull();
      expect(service.getCurrentUser()).toBeNull();
    });

    it('should call API and clear auth data when token exists', async () => {
      localStorage.setItem('accessToken', 'test-token');
      httpClientSpy.post.mockReturnValue(of({}));

      await firstValueFrom(service.logout());

      expect(localStorage.getItem('accessToken')).toBeNull();
      expect(service.isAuthenticated()).toBe(false);
    });
  });

  describe('isAuthenticated', () => {
    it('should return false when no access token exists', () => {
      expect(service.isAuthenticated()).toBe(false);
    });

    it('should return true when access token exists', () => {
      localStorage.setItem('accessToken', 'test-token');
      expect(service.isAuthenticated()).toBe(true);
    });
  });

  describe('getAccessToken', () => {
    it('should return null when no token is stored', () => {
      expect(service.getAccessToken()).toBeNull();
    });

    it('should return the stored access token', () => {
      localStorage.setItem('accessToken', 'stored-token');
      expect(service.getAccessToken()).toBe('stored-token');
    });
  });
});
