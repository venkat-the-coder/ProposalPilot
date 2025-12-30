// Dashboard overview with key metrics
export interface DashboardOverview {
  totalProposals: number;
  proposalsSent: number;
  proposalsAccepted: number;
  proposalsRejected: number;
  proposalsDraft: number;
  winRate: number;
  totalValue: number;
  averageProposalValue: number;
  proposalsThisMonth: number;
  proposalsLastMonth: number;
  monthOverMonthGrowth: number;
  activeFollowUps: number;
  pendingResponses: number;
}

// Proposal trends over time
export interface ProposalTrends {
  dailyStats: DailyProposalStats[];
  monthlyStats: MonthlyProposalStats[];
  statusBreakdown: StatusBreakdown[];
  averageDaysToResponse: number;
  fastestResponse: number;
  slowestResponse: number;
}

export interface DailyProposalStats {
  date: string;
  created: number;
  sent: number;
  accepted: number;
  rejected: number;
  views: number;
}

export interface MonthlyProposalStats {
  year: number;
  month: number;
  monthName: string;
  total: number;
  accepted: number;
  winRate: number;
  totalValue: number;
}

export interface StatusBreakdown {
  status: string;
  count: number;
  percentage: number;
}

// Engagement analytics across proposals
export interface EngagementAnalytics {
  totalViews: number;
  totalUniqueViewers: number;
  totalEmailOpens: number;
  totalEmailClicks: number;
  averageEngagementScore: number;
  hotProposals: number;
  warmProposals: number;
  coldProposals: number;
  topEngagedProposals: TopEngagedProposal[];
  engagementByDay: EngagementByDay[];
}

export interface TopEngagedProposal {
  proposalId: string;
  title: string;
  clientName: string;
  engagementScore: number;
  engagementLevel: string;
  views: number;
  emailOpens: number;
  lastActivity: string | null;
}

export interface EngagementByDay {
  date: string;
  views: number;
  emailOpens: number;
  emailClicks: number;
}

// AI usage and cost analytics
export interface AIUsageAnalytics {
  totalRequests: number;
  totalInputTokens: number;
  totalOutputTokens: number;
  totalCost: number;
  averageCostPerProposal: number;
  averageResponseTimeMs: number;
  successRate: number;
  usageByOperation: AIUsageByOperation[];
  usageByDay: AIUsageByDay[];
  modelUsage: AIModelUsage[];
}

export interface AIUsageByOperation {
  operation: string;
  requestCount: number;
  totalTokens: number;
  totalCost: number;
  averageResponseTimeMs: number;
}

export interface AIUsageByDay {
  date: string;
  requests: number;
  tokens: number;
  cost: number;
}

export interface AIModelUsage {
  model: string;
  requestCount: number;
  totalCost: number;
  percentage: number;
}

// Client and industry analytics
export interface ClientAnalytics {
  totalClients: number;
  repeatClients: number;
  repeatClientRate: number;
  topClients: TopClient[];
  industryBreakdown: IndustryBreakdown[];
}

export interface TopClient {
  clientId: string;
  clientName: string;
  companyName: string | null;
  proposalCount: number;
  acceptedCount: number;
  totalValue: number;
  winRate: number;
}

export interface IndustryBreakdown {
  industry: string;
  proposalCount: number;
  acceptedCount: number;
  winRate: number;
  averageValue: number;
}

// Complete analytics report combining all metrics
export interface FullAnalyticsReport {
  overview: DashboardOverview;
  trends: ProposalTrends;
  engagement: EngagementAnalytics;
  aiUsage: AIUsageAnalytics;
  clients: ClientAnalytics;
  generatedAt: string;
}
