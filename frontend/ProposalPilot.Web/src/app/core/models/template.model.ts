// Template Models

export interface Template {
  id: string;
  name: string;
  description?: string;
  category: string;
  tags: string[];
  content: TemplateContent;
  defaultPricing?: PricingTemplate;
  isSystemTemplate: boolean;
  isPublic: boolean;
  userId?: string;
  userName?: string;
  usageCount: number;
  winRate?: number;
  thumbnailUrl?: string;
  estimatedTimeMinutes?: number;
  createdAt: string;
  updatedAt?: string;
}

export interface TemplateListItem {
  id: string;
  name: string;
  description?: string;
  category: string;
  tags: string[];
  isSystemTemplate: boolean;
  isPublic: boolean;
  userName?: string;
  usageCount: number;
  winRate?: number;
  thumbnailUrl?: string;
  estimatedTimeMinutes?: number;
  createdAt: string;
}

export interface TemplateContent {
  introduction: string;
  problemStatement: string;
  proposedSolution: string;
  methodology: string;
  deliverables: string;
  timeline: string;
  teamAndExperience: string;
  termsAndConditions: string;
  callToAction: string;
}

export interface PricingTemplate {
  basic: PricingTier;
  standard: PricingTier;
  premium: PricingTier;
}

export interface PricingTier {
  name: string;
  description: string;
  priceMin: number;
  priceMax: number;
  features: string[];
  timeline: string;
}

// Request DTOs
export interface CreateTemplateRequest {
  name: string;
  description?: string;
  category: string;
  tags: string[];
  content: TemplateContent;
  defaultPricing?: PricingTemplate;
  isPublic: boolean;
  thumbnailUrl?: string;
  estimatedTimeMinutes?: number;
}

export interface UpdateTemplateRequest {
  name: string;
  description?: string;
  category: string;
  tags: string[];
  content: TemplateContent;
  defaultPricing?: PricingTemplate;
  isPublic: boolean;
  thumbnailUrl?: string;
  estimatedTimeMinutes?: number;
}

export interface SaveAsTemplateRequest {
  proposalId: string;
  templateName: string;
  description?: string;
  category: string;
  tags: string[];
  isPublic: boolean;
}

export interface CreateFromTemplateRequest {
  templateId: string;
  clientId: string;
  proposalTitle: string;
  customizations?: Record<string, string>;
}

// Template categories
export const TEMPLATE_CATEGORIES = [
  'Web Development',
  'Marketing',
  'Design',
  'Consulting',
  'Writing',
  'Video Production',
  'Mobile Development',
  'SEO',
  'Social Media',
  'Other'
] as const;

export type TemplateCategory = typeof TEMPLATE_CATEGORIES[number];

// Template filters
export interface TemplateFilters {
  category?: string;
  searchTerm?: string;
  includePublic?: boolean;
  page?: number;
  pageSize?: number;
}
