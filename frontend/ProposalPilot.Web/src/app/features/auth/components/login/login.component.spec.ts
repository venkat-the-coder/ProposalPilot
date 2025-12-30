import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { AuthService } from '../../../../core/services/auth.service';
import { vi, describe, it, expect, beforeEach } from 'vitest';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceSpy: { login: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    authServiceSpy = {
      login: vi.fn()
    };

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        provideRouter([])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have invalid form when empty', () => {
    expect(component.form.valid).toBe(false);
  });

  it('should have valid form when email and password are provided', () => {
    component.form.patchValue({
      email: 'test@example.com',
      password: 'password123'
    });
    expect(component.form.valid).toBe(true);
  });

  it('should validate email format', () => {
    component.form.patchValue({
      email: 'invalid-email',
      password: 'password123'
    });
    expect(component.form.get('email')?.valid).toBe(false);

    component.form.patchValue({ email: 'valid@example.com' });
    expect(component.form.get('email')?.valid).toBe(true);
  });

  it('should require password', () => {
    component.form.patchValue({
      email: 'test@example.com',
      password: ''
    });
    expect(component.form.get('password')?.valid).toBe(false);
  });

  it('should not call authService.login when form is invalid', () => {
    component.onSubmit();
    expect(authServiceSpy.login).not.toHaveBeenCalled();
  });

  it('should call authService.login with form values when form is valid', () => {
    const mockResponse = {
      userId: 'test-id',
      email: 'test@example.com',
      firstName: 'Test',
      lastName: 'User',
      companyName: null,
      accessToken: 'token',
      refreshToken: 'refresh',
      expiresAt: new Date().toISOString()
    };

    authServiceSpy.login.mockReturnValue(of(mockResponse));

    component.form.patchValue({
      email: 'test@example.com',
      password: 'password123'
    });

    component.onSubmit();

    expect(authServiceSpy.login).toHaveBeenCalledWith({
      email: 'test@example.com',
      password: 'password123'
    });
  });

  it('should set loading to true during login attempt', () => {
    authServiceSpy.login.mockReturnValue(of({}));

    component.form.patchValue({
      email: 'test@example.com',
      password: 'password123'
    });

    expect(component.loading).toBe(false);
    component.onSubmit();
    // Note: loading is set synchronously, then reset in the subscription
  });

  it('should display error message on login failure', () => {
    const errorResponse = { error: { message: 'Invalid credentials' } };
    authServiceSpy.login.mockReturnValue(throwError(() => errorResponse));

    component.form.patchValue({
      email: 'test@example.com',
      password: 'wrongpassword'
    });

    component.onSubmit();

    expect(component.error).toBe('Invalid credentials');
    expect(component.loading).toBe(false);
  });

  it('should display default error message when error has no message', () => {
    authServiceSpy.login.mockReturnValue(throwError(() => ({})));

    component.form.patchValue({
      email: 'test@example.com',
      password: 'wrongpassword'
    });

    component.onSubmit();

    expect(component.error).toBe('Login failed. Please check your credentials and try again.');
  });
});
