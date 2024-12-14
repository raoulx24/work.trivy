import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { MainAppInitService } from '../services/main-app-init.service';
import { LocalStorageUtils } from '../utils/local-storage.utils';

import { HomeClusterRbacAssessmentReportsComponent } from '../home-cluster-rbac-assessment-reports/home-cluster-rbac-assessment-reports.component';
import { HomeConfigAuditReportsComponent } from '../home-config-audit-reports/home-config-audit-reports.component';
import { HomeExposedSecretReportsComponent } from '../home-exposed-secret-reports/home-exposed-secret-reports.component';
import { HomeVulnerabilityReportsComponent } from '../home-vulnerability-reports/home-vulnerability-reports.component';

import { InputSwitchModule } from 'primeng/inputswitch';
import { TabViewChangeEvent, TabViewModule } from 'primeng/tabview';

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
  enabledTrivyReports: string[] = ['crar', 'car', 'esr', 'vr'];
  tabPageActiveIndex: number = 0;

  constructor(private mainAppInitService: MainAppInitService) {}

  private _showDistinctValues: boolean = true;

  get showDistinctValues() {
    return this._showDistinctValues;
  }

  set showDistinctValues(value: boolean) {
    this._showDistinctValues = value;
    localStorage.setItem('home.showDistinctValues', value.toString());
  }

  ngOnInit() {
    this.mainAppInitService.backendSettingsDto$.subscribe((updatedBackendSettingsDto) => {
      this.enabledTrivyReports =
        updatedBackendSettingsDto.trivyReportConfigDtos?.filter((x) => x.enabled).map((x) => x.id ?? '') ??
        this.enabledTrivyReports;
    });

    this.showDistinctValues = LocalStorageUtils.getBoolKeyValue('home.showDistinctValues') ?? true;
    this.tabPageActiveIndex = LocalStorageUtils.getNumberKeyValue('home.tabPageActiveIndex') ?? 0;
  }

  onTabPageChange(event: TabViewChangeEvent) {
    localStorage.setItem('home.tabPageActiveIndex', event.index.toString());
  }
}
