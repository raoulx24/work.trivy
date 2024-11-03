import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { MainAppInitService } from '../services/main-app-init.service';

import { HomeClusterRbacAssessmentReportsComponent } from '../home-cluster-rbac-assessment-reports/home-cluster-rbac-assessment-reports.component';
import { HomeConfigAuditReportsComponent } from '../home-config-audit-reports/home-config-audit-reports.component';
import { HomeExposedSecretReportsComponent } from '../home-exposed-secret-reports/home-exposed-secret-reports.component';
import { HomeVulnerabilityReportsComponent } from '../home-vulnerability-reports/home-vulnerability-reports.component';

import { InputSwitchModule } from 'primeng/inputswitch';
import { TabViewModule } from 'primeng/tabview';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    HomeVulnerabilityReportsComponent,
    HomeConfigAuditReportsComponent,
    HomeClusterRbacAssessmentReportsComponent,
    HomeExposedSecretReportsComponent,
    InputSwitchModule,
    TabViewModule,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
  showDistinctValues: boolean = true;
  enabledTrivyReports: string[] = ['crar', 'car', 'esr', 'vr'];

  constructor(private mainAppInitService: MainAppInitService) {}

  ngOnInit() {
    this.mainAppInitService.backendSettingsDto$.subscribe((updatedBackendSettingsDto) => {
      this.enabledTrivyReports =
        updatedBackendSettingsDto.trivyReportConfigDtos?.filter((x) => x.enabled).map((x) => x.id ?? '') ??
        this.enabledTrivyReports;
    });
  }
}
