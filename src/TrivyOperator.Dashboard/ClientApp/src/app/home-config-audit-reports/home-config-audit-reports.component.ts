import { Component } from '@angular/core';
import { ConfigAuditReportSummaryDto } from '../../api/models/config-audit-report-summary-dto'

import { ConfigAuditReportService } from '../../api/services/config-audit-report.service'
import { SeverityHelperService } from '../services/severity-helper.service'

@Component({
  selector: 'app-home-config-audit-reports',
  standalone: true,
  imports: [],
  templateUrl: './home-config-audit-reports.component.html',
  styleUrl: './home-config-audit-reports.component.scss'
})

export class HomeConfigAuditReportsComponent {
  configAuditReportSummaryDtos: ConfigAuditReportSummaryDto[] | null = null;
  namespaceNames: string[] = [];
  kinds: string[] = [];
  severities: number[] = [];

  public showDistinctValues: boolean = true;

  constructor(private configAuditReportService: ConfigAuditReportService, severityHelperService: SeverityHelperService) {
    console.log("constructor - car");
    this.configAuditReportService.getConfigAuditReportSumaryDtos()
      .subscribe({
        next: (res) => this.onDtos(res),
        error: (err) => console.error(err)
      });
  }

  private onDtos(dtos: ConfigAuditReportSummaryDto[]) {
    this.configAuditReportSummaryDtos = dtos;

    const result = dtos.reduce((acc, item) => {
      if (item.namespaceName && !acc.namespaceNames.includes(item.namespaceName)) {
        acc.namespaceNames.push(item.namespaceName);
      }
      if (item.kind && !acc.kinds.includes(item.kind)) {
        acc.kinds.push(item.kind);
      }
      if (!acc.severities.includes(item.severityId!)) {
        acc.severities.push(item.severityId!);
      }
      return acc;
    }, { namespaceNames: [] as string[], kinds: [] as string[], severities: [] as number[] });

    const { namespaceNames, kinds, severities } = result;
    this.namespaceNames = namespaceNames;
    this.kinds = kinds;
    this.severities = severities;

    console.log(namespaceNames);
    console.log(kinds);
    console.log(severities);
  }
}
