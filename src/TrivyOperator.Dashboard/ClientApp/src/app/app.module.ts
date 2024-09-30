import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
//import { HttpClientModule } from '@angular/common/http';
import { provideHttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { VulnerabilityReportsComponent } from './vulnerability-reports/vulnerability-reports.component';
import { VulnerabilityReportsDetailedComponent } from './vulnerability-reports-detailed/vulnerability-reports-detailed.component';
//import { AlertsComponent } from './alerts/alerts.component';
import { AboutComponent } from './about/about.component';

import { ApiModule } from "../api/api.module";
import { environment } from "../environments/environment";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { BadgeModule } from 'primeng/badge';
import { ButtonModule } from 'primeng/button';
import { CarouselModule } from 'primeng/carousel';
import { ChartModule } from 'primeng/chart';
import { CheckboxModule } from 'primeng/checkbox';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { MenubarModule } from 'primeng/menubar';
import { MultiSelectModule } from 'primeng/multiselect';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { PanelModule } from 'primeng/panel';
import { SplitterModule } from 'primeng/splitter';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { TrivyTableComponent } from './trivy-table/trivy-table.component';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    VulnerabilityReportsComponent,
    VulnerabilityReportsDetailedComponent,
    AboutComponent,
  ],
  imports: [
    BrowserModule,
    //HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'vulnerability-reports', component: VulnerabilityReportsComponent },
      { path: 'vulnerability-reports-detailed', component: VulnerabilityReportsDetailedComponent },
      //{ path: 'alerts', component: AlertsComponent, },
      { path: 'alerts', loadComponent: () => import('./watcher-state/watcher-state.component').then(m => m.WatcherStateComponent) },
      { path: 'about', component: AboutComponent, },
    ]),
    ApiModule.forRoot({rootUrl: environment.baseUrl}),
    BrowserAnimationsModule,
    BadgeModule,
    ButtonModule,
    CarouselModule,
    ChartModule,
    CheckboxModule,
    DialogModule,
    DropdownModule,
    FloatLabelModule,
    InputTextModule,
    MenubarModule,
    MultiSelectModule,
    OverlayPanelModule,
    PanelModule,
    SplitterModule,
    TableModule,
    TagModule,

    TrivyTableComponent,
  ],
  providers: [provideHttpClient()],
  bootstrap: [AppComponent]
})
export class AppModule {
}
