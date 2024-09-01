import { Component, ViewChild } from '@angular/core';
import { VulnerabilityReportsService } from "../../api/services/vulnerability-reports.service";
import { VulnerabilityReportSummaryDto } from "../../api/models/vulnerability-report-summary-dto";
import { PrimeNgPieChartData, PrimeNgHorizontalBarChartData, PrimeNgHelper, SeveritiesSummary } from "../../utils/severity-helper";
import { SeverityHelperService } from "../services/severity-helper.service"
import { SeverityDto } from "../../api/models/severity-dto"
import { UIChart } from 'primeng/chart';
import { timer } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public vulnerabilityReportSummaryDtos?: VulnerabilityReportSummaryDto[] | null | undefined;

  public severityHelperService: SeverityHelperService;
  public get primeNgHelper(): PrimeNgHelper { return this._primeNgHelper; };
  private _primeNgHelper: PrimeNgHelper;

  public pieChartData: PrimeNgPieChartData[] | null | undefined;
  public horizontalBarChartDataByNs: PrimeNgHorizontalBarChartData | null | undefined;
  public horizontalBarChartDataBySeverity: PrimeNgHorizontalBarChartData | null | undefined;
  public pieChartOptions: any;
  public horizontalBarChartOption: any;
  public slides: string[] = ["barChartNS", "barChartSeverity", "pieCharts"];

  public severityDtos: SeverityDto[] | null | undefined;
  public filterRefreshSeverities: SeverityDto[] = [];

  constructor(vulnerabilityReportsService: VulnerabilityReportsService, severityHelperService: SeverityHelperService) {
    vulnerabilityReportsService.getVulnerabilityReportSummaryDtos().subscribe(result => this.onVulnerabilityReportSummaryDtos(result), error => console.error(error));
    this.severityHelperService = severityHelperService;
    this._primeNgHelper = new PrimeNgHelper(this.severityHelperService);
    severityHelperService.getSeverityDtos().then(result => {
      this.filterRefreshSeverities = result;
      this.initComponents();
    });
  }

  private initComponents() {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--text-color');
    const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
    const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

    this.pieChartOptions = {
      plugins: {
        legend: {
          display: false,
          labels: {
            usePointStyle: true,
            color: textColor,
          },
          position: 'bottom',
        }
      }
    };

    this.horizontalBarChartOption = {
      indexAxis: 'y',
      responsive: true,
      maintainAspectRatio: false,
      aspectRatio: 0.6,
      plugins: {
        legend: {
          labels: {
            color: textColor
          },
          position: 'bottom',
        }
      },
      scales: {
        x: {
          ticks: {
            color: textColorSecondary,
            font: {
              weight: 500
            }
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false
          }
        },
        y: {
          ticks: {
            color: textColorSecondary
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false
          }
        }
      }
    };
  }

  onVulnerabilityReportSummaryDtos(vulnerabilityReportSummaryDtos?: VulnerabilityReportSummaryDto[]) {
    this.vulnerabilityReportSummaryDtos = vulnerabilityReportSummaryDtos;

    if (vulnerabilityReportSummaryDtos == null) {
      return;
    }
    this.severityHelperService.getSeverityDtos().then(x => {
      this.severityDtos = x;
      this.pieChartData = this._primeNgHelper.getDataForPieChart(vulnerabilityReportSummaryDtos as SeveritiesSummary[]);
      this._primeNgHelper.getDataForHorizontalBarChartByNamespace(vulnerabilityReportSummaryDtos as SeveritiesSummary[])
        .then(x => this.horizontalBarChartDataByNs = x);
      this._primeNgHelper.getDataForHorizontalBarChartBySeverity(vulnerabilityReportSummaryDtos as SeveritiesSummary[])
        .then(x => this.horizontalBarChartDataBySeverity = x);
    });
  }

  public onMamaClick(event: Event) {
    if (this.horizontalBarChartDataByNs == null) {
      console.log("ciudat");
      return;
    }
    console.log("mama");
    this.loading = !this.loading;
    this.horizontalBarChartDataByNs!.datasets[1].hidden = !this.horizontalBarChartDataByNs!.datasets[1].hidden;
    this.createChart = !this.createChart;
    console.log(this.createChart);
    timer(1).subscribe(x => { this.createChart = !this.createChart; })
    
    console.log(this.createChart);

    if (this.barChartByNs == null)
      return;

    console.log(this.horizontalBarChartDataByNs);
    console.log(this.horizontalBarChartOption);

    //const ci = this.barChartByNs.chart;
    //const meta = ci.getDatasetMeta(1);
    //console.log(meta);
    //console.log(ci.data.datasets[1]);
    //meta.hidden = meta.hidden != null ? !ci.data.datasets[1].hidden : false;
    //ci.update();

    //const dataset = this.barChartByNs.chart.data.datasets[2];
    //if (this.loading) {
    //  this.barChartByNs.chart.show(0);
    //  this.barChartByNs.chart.hide(1);
    //  this.barChartByNs.chart.hide(2);
    //}
    //else {
    //  this.barChartByNs.chart.show(0);
    //  this.barChartByNs.chart.show(1);
    //  this.barChartByNs.chart.show(2);
    //}
    //console.log(dataset);
    ////console.log(dataset.hidden);
    ////dataset.hidden = true;
    ////console.log(dataset.hidden);
    ////dataset.data = null;
    //console.log("here");
    //// Update the chart
    ////this.barChartByNs.chart = { ...this.barChartByNs.chart };
    //this.barChartByNs.chart.update();
  }

  public loading: boolean = false;
  @ViewChild('pieChart') pieChart?: UIChart;
  @ViewChild('barChartByNs') barChartByNs?: UIChart;
  hideNoTwo: boolean = false;
  public createChart: boolean = true;

}
