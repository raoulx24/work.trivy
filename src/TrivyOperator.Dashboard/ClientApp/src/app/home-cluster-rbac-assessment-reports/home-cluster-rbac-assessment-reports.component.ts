import { Component, Input } from '@angular/core';

import { ClusterRbacAssessmentReportSummaryDto } from '../../api/models/cluster-rbac-assessment-report-summary-dto';
import { ClusterRbacAssessmentReportService } from '../../api/services/cluster-rbac-assessment-report.service';
import { SeverityUtils } from '../utils/severity.utils';

import { TableModule } from 'primeng/table';

@Component({
  selector: 'app-home-cluster-rbac-assessment-reports',
  standalone: true,
  imports: [TableModule],
  templateUrl: './home-cluster-rbac-assessment-reports.component.html',
  styleUrl: './home-cluster-rbac-assessment-reports.component.scss',
})
export class HomeClusterRbacAssessmentReportsComponent {
  @Input() set showDistinctValues(value: boolean) {
    this.localShowDistinctValues = value;
    this.onDistinctSwitch();
  }

  get showDistinctValues(): boolean {
    return this.localShowDistinctValues;
  }

  clusterRbacAssessmentReportSummaryDtos: ClusterRbacAssessmentReportSummaryDto[] = [];

  private localShowDistinctValues: boolean = true;

  constructor(private clusterRbacAssessmentReportService: ClusterRbacAssessmentReportService) {
    this.clusterRbacAssessmentReportService.getClusterRbacAssessmentReportSummaryDtos().subscribe({
      next: (res) => this.onDtos(res),
      error: (err) => console.error(err),
    });
  }

  private onDtos(dtos: ClusterRbacAssessmentReportSummaryDto[]) {
    this.clusterRbacAssessmentReportSummaryDtos = dtos;
  }

  onDistinctSwitch() {
    // TODO
  }

  severityWrapperGetCapitalizedName(severityId: number): string {
    return SeverityUtils.getCapitalizedName(severityId);
  }
}
