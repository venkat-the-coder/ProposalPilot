export interface QualityScoreResult {
  overallScore: number;
  grade: string;
  winProbability: string;
  scores: {
    [key: string]: CategoryScore;
  };
  strengths: string[];
  improvements: Improvement[];
  quickWins: string[];
  rewriteSuggestions?: {
    [key: string]: string;
  };
}

export interface CategoryScore {
  score: number;
  max: number;
  feedback: string;
}

export interface Improvement {
  priority: string;
  section: string;
  issue: string;
  suggestion: string;
}
