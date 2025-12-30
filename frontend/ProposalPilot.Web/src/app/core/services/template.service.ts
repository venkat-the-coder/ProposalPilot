import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Template,
  TemplateListItem,
  CreateTemplateRequest,
  UpdateTemplateRequest,
  SaveAsTemplateRequest,
  CreateFromTemplateRequest,
  TemplateFilters
} from '../models/template.model';

@Injectable({
  providedIn: 'root'
})
export class TemplateService {
  private readonly API_URL = `${environment.apiUrl}/templates`;

  constructor(private http: HttpClient) {}

  /**
   * Get all templates with optional filters
   */
  getTemplates(filters?: TemplateFilters): Observable<TemplateListItem[]> {
    let params = new HttpParams();

    if (filters?.category) {
      params = params.set('category', filters.category);
    }
    if (filters?.searchTerm) {
      params = params.set('searchTerm', filters.searchTerm);
    }
    if (filters?.includePublic !== undefined) {
      params = params.set('includePublic', filters.includePublic.toString());
    }
    if (filters?.page) {
      params = params.set('page', filters.page.toString());
    }
    if (filters?.pageSize) {
      params = params.set('pageSize', filters.pageSize.toString());
    }

    return this.http.get<TemplateListItem[]>(this.API_URL, { params });
  }

  /**
   * Get a specific template by ID
   */
  getTemplateById(id: string): Observable<Template> {
    return this.http.get<Template>(`${this.API_URL}/${id}`);
  }

  /**
   * Get system templates (cached)
   */
  getSystemTemplates(): Observable<TemplateListItem[]> {
    return this.http.get<TemplateListItem[]>(`${this.API_URL}/system`);
  }

  /**
   * Get current user's templates
   */
  getMyTemplates(): Observable<TemplateListItem[]> {
    return this.http.get<TemplateListItem[]>(`${this.API_URL}/my-templates`);
  }

  /**
   * Get template categories with counts
   */
  getCategories(): Observable<Record<string, number>> {
    return this.http.get<Record<string, number>>(`${this.API_URL}/categories`);
  }

  /**
   * Create a new template
   */
  createTemplate(request: CreateTemplateRequest): Observable<Template> {
    return this.http.post<Template>(this.API_URL, request);
  }

  /**
   * Update an existing template
   */
  updateTemplate(id: string, request: UpdateTemplateRequest): Observable<Template> {
    return this.http.put<Template>(`${this.API_URL}/${id}`, request);
  }

  /**
   * Delete a template
   */
  deleteTemplate(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }

  /**
   * Save an existing proposal as a template
   */
  saveProposalAsTemplate(request: SaveAsTemplateRequest): Observable<Template> {
    return this.http.post<Template>(`${this.API_URL}/save-from-proposal`, request);
  }

  /**
   * Create a proposal from a template
   */
  createProposalFromTemplate(request: CreateFromTemplateRequest): Observable<{ proposalId: string }> {
    return this.http.post<{ proposalId: string }>(`${this.API_URL}/create-proposal`, request);
  }

  /**
   * Duplicate/clone a template
   */
  duplicateTemplate(id: string): Observable<Template> {
    return this.http.post<Template>(`${this.API_URL}/${id}/duplicate`, {});
  }
}
