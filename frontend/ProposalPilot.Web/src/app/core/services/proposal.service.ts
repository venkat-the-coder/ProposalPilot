import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Proposal, GenerateProposalRequest } from '../models/proposal.model';

@Injectable({
  providedIn: 'root'
})
export class ProposalService {
  private readonly API_URL = `${environment.apiUrl}/proposals`;

  constructor(private http: HttpClient) {}

  generateProposal(request: GenerateProposalRequest): Observable<{ proposalId: string }> {
    return this.http.post<{ proposalId: string }>(`${this.API_URL}/generate`, request);
  }

  getProposal(id: string): Observable<Proposal> {
    return this.http.get<Proposal>(`${this.API_URL}/${id}`);
  }

  getProposals(): Observable<Proposal[]> {
    return this.http.get<Proposal[]>(this.API_URL);
  }

  updateProposal(id: string, updates: any): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/${id}`, updates);
  }
}
