import { provideHttpClient } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ApiModule } from '../api/api.module';
import { environment } from '../environments/environment';

import { BadgeModule } from 'primeng/badge';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';
import { PanelMenuModule } from 'primeng/panelmenu';
import { SidebarModule } from 'primeng/sidebar';
import { TagModule } from 'primeng/tag';

import { initializeAppFactory, MainAppInitService } from './services/main-app-init.service';
import { TrivyTableComponent } from './trivy-table/trivy-table.component';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', loadComponent: () => import('./home/home.component').then((m) => m.HomeComponent) },
      {
        path: 'vulnerability-reports',
        loadComponent: () =>
          import('./vulnerability-reports/vulnerability-reports.component').then(
            (m) => m.VulnerabilityReportsComponent,
          ),
      },
      {
        path: 'vulnerability-reports-detailed',
        loadComponent: () =>
          import('./vulnerability-reports-detailed/vulnerability-reports-detailed.component').then(
            (m) => m.VulnerabilityReportsDetailedComponent,
          ),
      },
      {
        path: 'config-audit-reports',
        loadComponent: () =>
          import('./config-audit-reports/config-audit-reports.component').then((m) => m.ConfigAuditReportsComponent),
      },
      {
        path: 'config-audit-reports-detailed',
        loadComponent: () =>
          import('./config-audit-reports-detailed/config-audit-reports-detailed.component').then(
            (m) => m.ConfigAuditReportsDetailedComponent,
          ),
      },
      {
        path: 'cluster-rbac-assessment-reports',
        loadComponent: () =>
          import('./cluster-rbac-assessment-reports/cluster-rbac-assessment-reports.component').then(
            (m) => m.ClusterRbacAssessmentReportsComponent,
          ),
      },
      {
        path: 'cluster-rbac-assessment-reports-detailed',
        loadComponent: () =>
          import('./cluster-rbac-assessment-reports-detailed/cluster-rbac-assessment-reports-detailed.component').then(
            (m) => m.ClusterRbacAssessmentReportsDetailedComponent,
          ),
      },
      {
        path: 'exposed-secret-reports',
        loadComponent: () =>
          import('./exposed-secret-reports/exposed-secret-reports.component').then(
            (m) => m.ExposedSecretReportsComponent,
          ),
      },
      {
        path: 'exposed-secret-reports-detailed',
        loadComponent: () =>
          import('./exposed-secret-reports-detailed/exposed-secret-reports-detailed.component').then(
            (m) => m.ExposedSecretReportsDetailedComponent,
          ),
      },
      {
        path: 'cluster-compliance-reports',
        loadComponent: () =>
          import('./cluster-compliance-reports/cluster-compliance-reports.component').then(
            (m) => m.ClusterComplianceReportsComponent,
          ),
      },
      {
        path: 'cluster-compliance-reports-detailed',
        loadComponent: () =>
          import('./cluster-compliance-reports-detailed/cluster-compliance-reports-detailed.component').then(
            (m) => m.ClusterComplianceReportsDetailedComponent,
          ),
      },

      {
        path: 'cluster-vulnerability-reports',
        loadComponent: () =>
          import('./cluster-vulnerability-reports/cluster-vulnerability-reports.component').then(
            (m) => m.ClusterVulnerabilityReportsComponent,
          ),
      },
      {
        path: 'cluster-vulnerability-reports-detailed',
        loadComponent: () =>
          import('./cluster-vulnerability-reports-detailed/cluster-vulnerability-reports-detailed.component').then(
            (m) => m.ClusterVulnerabilityReportsDetailedComponent,
          ),
      },
      {
        path: 'rbac-assessment-reports',
        loadComponent: () =>
          import('./rbac-assessment-reports/rbac-assessment-reports.component').then(
            (m) => m.RbacAssessmentReportsComponent,
          ),
      },
      {
        path: 'rbac-assessment-reports-detailed',
        loadComponent: () =>
          import('./rbac-assessment-reports-detailed/rbac-assessment-reports-detailed.component').then(
            (m) => m.RbacAssessmentReportsDetailedComponent,
          ),
      },
      {
        path: 'watcher-states',
        loadComponent: () => import('./watcher-state/watcher-state.component').then((m) => m.WatcherStateComponent),
      },
      {
        path: 'settings',
        loadComponent: () => import('./settings/settings.component').then((m) => m.SettingsComponent),
      },
      { path: 'about', loadComponent: () => import('./about/about.component').then((m) => m.AboutComponent) },
      { path: 'fcose', loadComponent: () => import('./fcose/fcose.component').then((m) => m.FcoseComponent) },
    ]),
    ApiModule.forRoot({ rootUrl: environment.baseUrl }),
    BrowserAnimationsModule,
    BadgeModule,
    ButtonModule,
    MenubarModule,
    PanelMenuModule,
    SidebarModule,
    TagModule,
    TrivyTableComponent,
    FontAwesomeModule,
  ],
  providers: [
    provideHttpClient(),
    MainAppInitService,
    { provide: APP_INITIALIZER, useFactory: initializeAppFactory, deps: [MainAppInitService], multi: true },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
