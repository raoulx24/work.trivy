import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { provideHttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

import { ApiModule } from "../api/api.module";
import { environment } from "../environments/environment";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { BadgeModule } from 'primeng/badge';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';
import { TagModule } from 'primeng/tag';

import { TrivyTableComponent } from './trivy-table/trivy-table.component';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', loadComponent: () => import('./home/home.component').then(m => m.HomeComponent) },
      { path: 'vulnerability-reports', loadComponent: () => import('./vulnerability-reports/vulnerability-reports.component').then(m => m.VulnerabilityReportsComponent) },
      { path: 'vulnerability-reports-detailed', loadComponent: () => import('./vulnerability-reports-detailed/vulnerability-reports-detailed.component').then(m => m.VulnerabilityReportsDetailedComponent) },
      { path: 'config-audit-reports', loadComponent: () => import('./config-audit-reports/config-audit-reports.component').then(m => m.ConfigAuditReportsComponent) },
      { path: 'config-audit-reports-detailed', loadComponent: () => import('./config-audit-reports-detailed/config-audit-reports-detailed.component').then(m => m.ConfigAuditReportsDetailedComponent) },
      { path: 'watcher-states', loadComponent: () => import('./watcher-state/watcher-state.component').then(m => m.WatcherStateComponent) },
      { path: 'settings', loadComponent: () => import('./settings/settings.component').then(m => m.SettingsComponent) },
      { path: 'about', loadComponent: () => import('./about/about.component').then(m => m.AboutComponent) },
    ]),
    ApiModule.forRoot({rootUrl: environment.baseUrl}),
    BrowserAnimationsModule,
    BadgeModule,
    ButtonModule,
    MenubarModule,
    TagModule,

    TrivyTableComponent,
  ],
  providers: [provideHttpClient()],
  bootstrap: [AppComponent]
})
export class AppModule {
}
