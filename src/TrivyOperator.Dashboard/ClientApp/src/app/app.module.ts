import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { SideMenuComponent } from './side-menu/side-menu.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { VulnerabilityReportsComponent } from './vulnerability-reports/vulnerability-reports.component';
import { VulnerabilityReportsDetailedComponent } from './vulnerability-reports-detailed/vulnerability-reports-detailed.component';
import { AlertsComponent } from './alerts/alerts.component';

import { AgGridModule } from "ag-grid-angular";
import { ApiModule } from "../api/api.module";
import { environment } from "../environments/environment";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AccordionModule } from 'primeng/accordion';
import { BadgeModule } from 'primeng/badge';
import { ButtonModule } from 'primeng/button';
import { ChartModule } from 'primeng/chart';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { MenubarModule } from 'primeng/menubar';
import { MultiSelectModule } from 'primeng/multiselect';
import { SplitButtonModule } from 'primeng/splitbutton';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

@NgModule({
  declarations: [
    AppComponent,
    SideMenuComponent,
    NavMenuComponent,
    HomeComponent,
    VulnerabilityReportsComponent,
    VulnerabilityReportsDetailedComponent,
    AlertsComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'vulnerability-reports', component: VulnerabilityReportsComponent },
      { path: 'vulnerability-reports-detailed', component: VulnerabilityReportsDetailedComponent },
      { path: 'alerts', component: AlertsComponent ,}
    ]),
    AgGridModule,
    ApiModule.forRoot({rootUrl: environment.baseUrl}),
    BrowserAnimationsModule,
    AccordionModule,
    BadgeModule,
    ButtonModule,
    ChartModule,
    FloatLabelModule,
    InputTextModule,
    MenubarModule,
    MultiSelectModule,
    SplitButtonModule,
    TableModule,
    TagModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
}
