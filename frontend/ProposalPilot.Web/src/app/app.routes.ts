import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: 'share/:token',
    loadComponent: () => import('./features/proposals/components/proposal-public-view.component').then(m => m.ProposalPublicViewComponent)
  },
  {
    path: 'auth',
    canActivate: [guestGuard],
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/auth/components/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () => import('./features/auth/components/register/register.component').then(m => m.RegisterComponent)
      }
    ]
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () => import('./features/profile/profile.component').then(m => m.ProfileComponent)
  },
  {
    path: 'settings',
    canActivate: [authGuard],
    loadComponent: () => import('./features/settings/settings.component').then(m => m.SettingsComponent)
  },
  {
    path: 'analytics',
    canActivate: [authGuard],
    loadComponent: () => import('./features/analytics/analytics-dashboard.component').then(m => m.AnalyticsDashboardComponent)
  },
  {
    path: 'briefs',
    canActivate: [authGuard],
    children: [
      {
        path: 'new',
        loadComponent: () => import('./features/briefs/components/brief-input.component').then(m => m.BriefInputComponent)
      },
      {
        path: ':id/analysis',
        loadComponent: () => import('./features/briefs/components/brief-analysis-result.component').then(m => m.BriefAnalysisResultComponent)
      }
    ]
  },
  {
    path: 'proposals',
    canActivate: [authGuard],
    children: [
      {
        path: ':id',
        loadComponent: () => import('./features/proposals/components/proposal-view.component').then(m => m.ProposalViewComponent)
      },
      {
        path: ':id/edit',
        loadComponent: () => import('./features/proposals/components/proposal-editor.component').then(m => m.ProposalEditorComponent)
      },
      {
        path: ':id/analytics',
        loadComponent: () => import('./features/proposals/components/proposal-analytics.component').then(m => m.ProposalAnalyticsComponent)
      }
    ]
  },
  { path: '**', redirectTo: '/dashboard' }
];
