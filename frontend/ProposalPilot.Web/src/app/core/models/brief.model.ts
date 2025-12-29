export interface Brief {
  id: string;
  title: string;
  rawContent: string;
  analyzedContent?: string;
  projectType?: string;
  industry?: string;
  estimatedBudget?: number;
  timeline?: string;
  keyRequirements?: string;
  technicalRequirements?: string;
  targetAudience?: string;
  status: string;
  analyzedAt?: string;
  tokensUsed?: number;
  analysisCost?: number;
  createdAt: string;
}

export interface CreateBriefRequest {
  title: string;
  rawContent: string;
}

export interface BriefAnalysisResult {
  projectOverview: ProjectOverview;
  requirements: Requirements;
  clientInsights: ClientInsights;
  projectSignals: ProjectSignals;
  riskAssessment: RiskAssessment;
  recommendedApproach: RecommendedApproach;
}

export interface ProjectOverview {
  type: string;
  industry: string;
  complexity: 'low' | 'medium' | 'high' | 'enterprise';
  confidenceScore: number;
}

export interface Requirements {
  explicit: string[];
  implicit: string[];
  technical: string[];
  deliverables: string[];
}

export interface ClientInsights {
  painPoints: string[];
  successCriteria: string[];
  decisionFactors: string[];
}

export interface ProjectSignals {
  timeline: TimelineInfo;
  budget: BudgetInfo;
}

export interface TimelineInfo {
  urgency: 'low' | 'medium' | 'high';
  durationEstimate: string;
  keyDates: string[];
}

export interface BudgetInfo {
  signals: string[];
  rangeEstimate: string;
  pricingSensitivity: 'low' | 'medium' | 'high';
}

export interface RiskAssessment {
  redFlags: string[];
  clarificationNeeded: string[];
  scopeCreepRisks: string[];
}

export interface RecommendedApproach {
  proposalTone: 'formal' | 'professional' | 'friendly' | 'consultative';
  keyThemes: string[];
  differentiators: string[];
  pricingStrategy: 'value_based' | 'competitive' | 'premium';
}
