import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { HomeVulnerabilityReportsComponent } from '../home-vulnerability-reports/home-vulnerability-reports.component'
import { HomeConfigAuditReportsComponent } from '../home-config-audit-reports/home-config-audit-reports.component'
import { HomeClusterRbacAssessmentReportsComponent } from '../home-cluster-rbac-assessment-reports/home-cluster-rbac-assessment-reports.component'
import { HomeExposedSecretReportsComponent } from '../home-exposed-secret-reports/home-exposed-secret-reports.component'

import { InputSwitchModule } from 'primeng/inputswitch';

import { TabViewModule } from 'primeng/tabview';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule,
    HomeVulnerabilityReportsComponent,
    HomeConfigAuditReportsComponent,
    HomeClusterRbacAssessmentReportsComponent,
    HomeExposedSecretReportsComponent,
    InputSwitchModule, TabViewModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})

export class HomeComponent {
  showDistinctValues: boolean = true;
}
