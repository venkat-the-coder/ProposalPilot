export interface User {
  id: string;
  email: string;
  fullName?: string;
  companyName?: string;
}

export interface Proposal {
  id: string;
  title: string;
  description: string;
  status: string;
  clientId: string;
  userId: string;
  briefId?: string;
  originalBrief?: string;
  briefAnalysis?: string;
  basicTierJson: string;
  standardTierJson: string;
  premiumTierJson: string;
  deliverablesJson: string;
  timelineJson: string;
  termsAndConditions?: string;
  paymentTerms?: string;
  viewCount: number;
  firstViewedAt?: string;
  lastViewedAt?: string;
  sentAt?: string;
  acceptedAt?: string;
  rejectedAt?: string;
  expiresAt?: string;
  shareToken: string;
  isPublic: boolean;
  createdAt: string;
  updatedAt: string;
  client?: any; // Client details
  brief?: any; // Brief details
  user?: User; // User details
}

export interface GenerateProposalRequest {
  briefId: string;
  clientId: string;
  preferredTone?: string;
  proposalLength?: string;
  emphasis?: string;
}

export interface ProposalGenerationResult {
  title: string;
  sections: ProposalSections;
  metadata: ProposalMetadata;
}

export interface ProposalSections {
  openingHook: string;
  problemStatement: string;
  proposedSolution: string;
  methodology: string;
  timeline: string;
  investment: InvestmentSection;
  whyChooseUs: string;
  nextSteps: string;
}

export interface InvestmentSection {
  intro: string;
  tiers: PricingTier[];
}

export interface PricingTier {
  name: string;
  price: number;
  description: string;
  features: string[];
  timeline: string;
  highlighted?: boolean;
}

export interface ProposalMetadata {
  wordCount: number;
  estimatedReadTime: string;
  tone: string;
}

// Parsed proposal content from deliverablesJson
export interface ProposalContent {
  sections: {
    opening_hook: string;
    problem_statement: string;
    proposed_solution: string;
    methodology: string;
    timeline: string;
    why_choose_us: string;
    next_steps: string;
  };
  investment: InvestmentSection;
  metadata: ProposalMetadata;
}
