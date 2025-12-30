import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class EmailService {
  private readonly API_URL = `${environment.apiUrl}/email`;

  constructor(private http: HttpClient) {}

  sendVerificationEmail(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.API_URL}/send-verification`, {});
  }

  verifyEmail(userId: string, token: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.API_URL}/verify`, { userId, token });
  }
}
