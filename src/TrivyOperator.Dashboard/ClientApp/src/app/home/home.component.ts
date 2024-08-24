import { Component } from '@angular/core';
import { VulnerabilityReportsService } from "../../api/services/vulnerability-reports.service";
import { VulnerabilityReportSummaryDto } from "../../api/models/vulnerability-report-summary-dto";
import { PrimeNgChartData, PrimeNgHelper } from "../../utils/severity-helper";
import { SeverityHelperService } from "../services/severity-helper.service"

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public vulnerabilityReportSummaryDtos?: VulnerabilityReportSummaryDto[] | null | undefined;

  public get primeNgHelper(): PrimeNgHelper { return this._primeNgHelper; };
  private _primeNgHelper: PrimeNgHelper;
  private _severityHelperService: SeverityHelperService;

  public primeNgChartData: PrimeNgChartData[] | null | undefined;
  public primeNgChartOptions: any;

  constructor(vulnerabilityReportsService: VulnerabilityReportsService, severityHelperService: SeverityHelperService) {
    vulnerabilityReportsService.getVulnerabilityReportSummaryDtos().subscribe(result => this.onVulnerabilityReportSummaryDtos(result), error => console.error(error));
    this._severityHelperService = severityHelperService;
    this._primeNgHelper = new PrimeNgHelper(severityHelperService);

    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--text-color');
    this.primeNgChartOptions = {
      plugins: {
        legend: {
          labels: {
            usePointStyle: true,
            color: textColor,
          },
          position: 'bottom',
        }
      }
    };
  }

  onVulnerabilityReportSummaryDtos(vulnerabilityReportSummaryDtos?: VulnerabilityReportSummaryDto[]) {
    this.vulnerabilityReportSummaryDtos = vulnerabilityReportSummaryDtos;

    if (vulnerabilityReportSummaryDtos == null) {
      return;
    }

    let primeNgChartData: PrimeNgChartData[] = [];

    this._severityHelperService.getSeverityDtos().then(x => {

      for (var vulnerabilityReportSummaryDto of vulnerabilityReportSummaryDtos) {
        let chartData: PrimeNgChartData = this.primeNgHelper.GetDataForPrimeNgChart(vulnerabilityReportSummaryDto.values,
          vulnerabilityReportSummaryDto.namespaceName!,
          x);
        primeNgChartData.push(chartData);
      }

      this.primeNgChartData = primeNgChartData;
    });
  }
}
