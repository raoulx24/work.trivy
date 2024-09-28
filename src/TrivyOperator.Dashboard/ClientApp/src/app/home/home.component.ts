import { Component, ViewChild } from '@angular/core';
import { VulnerabilityReportsService } from "../../api/services/vulnerability-reports.service";
import { VulnerabilityReportSumaryDto } from "../../api/models/vulnerability-report-sumary-dto";
import { VrSeveritiesByNsSummaryDto } from '../../api/models/vr-severities-by-ns-summary-dto';
import { PrimeNgPieChartData, PrimeNgHorizontalBarChartData, PrimeNgHelper, SeveritiesSummary } from "../../utils/severity-helper";
import { SeverityHelperService } from "../services/severity-helper.service"
import { SeverityDto } from "../../api/models/severity-dto"
import { UIChart } from 'primeng/chart';
import { timer } from 'rxjs';
import { VrSeveritiesByNsSummaryDetailDto } from '../../api/models/vr-severities-by-ns-summary-detail-dto';

export interface SeveritiySummary {
  severityName: string;
  count: number;
  fixable: number;
}

export interface OtherSummaryMainStatistics {
  description: "Images" | "Images OSes" | "End of Service Life";
  count: number;
}

export interface GenericSummaryDto {
  name: string;
  count: number;
}

export interface GenericByNsSummaryDto {
  namespaceName: string,
  totalCount: number,
  uniqueCount: number,
  isTotal: boolean,
}

export interface GenericNsTotalSortable {
  namespaceName: string,
  isTotal: boolean,
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
  public vulnerabilityReportSumaryDto?: VulnerabilityReportSumaryDto | null | undefined;

  public severityHelperService: SeverityHelperService;
  public get primeNgHelper(): PrimeNgHelper { return this._primeNgHelper; };
  private _primeNgHelper: PrimeNgHelper;

  public pieChartData: PrimeNgPieChartData[] | null | undefined;
  public horizontalBarChartDataByNs: PrimeNgHorizontalBarChartData | null | undefined;
  public horizontalBarChartDataBySeverity: PrimeNgHorizontalBarChartData | null | undefined;
  public pieChartOptions: any;
  public horizontalBarChartOption: any;
  public slides: string[] = ["barChartNS", "barChartSeverity"];

  public severityDtos: SeverityDto[] | null | undefined;
  public filterRefreshSeverities: SeverityDto[] = [];

  public severitiesSummaryForTable: SeveritiySummary[] = [];
  public othersSummaryForTable: OtherSummaryMainStatistics[] = [];

  public isMoreVRDetailsModalVisible: boolean = false;
  public moreOthersModalTitle: string = "";
  public isMoreOthersModalVisible: boolean = false;
  public genericSummaryDtos: GenericSummaryDto[] = [];
  public genericByNsSummaryDtos: GenericByNsSummaryDto[] = [];

  public showDistinctValues: boolean = true;

  constructor(vulnerabilityReportsService: VulnerabilityReportsService, severityHelperService: SeverityHelperService) {
    vulnerabilityReportsService.getVulnerabilityReportSumaryDto()
      .subscribe({
        next: (res) => this.onVulnerabilityReportSummaryDtos(res),
        error: (err) => console.error(err)
      });
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

  onVulnerabilityReportSummaryDtos(vulnerabilityReportSumaryDto?: VulnerabilityReportSumaryDto) {
    this.vulnerabilityReportSumaryDto = vulnerabilityReportSumaryDto;

    if (vulnerabilityReportSumaryDto == null) {
      return;
    }
    this.severityHelperService.getSeverityDtos().then(x => {
      this.severityDtos = x;
      this.pieChartData = this._primeNgHelper.getDataForPieChart(vulnerabilityReportSumaryDto.severitiesByNsSummaryDtos as SeveritiesSummary[]);
      this._primeNgHelper.getDataForHorizontalBarChartByNamespace(vulnerabilityReportSumaryDto.severitiesByNsSummaryDtos as SeveritiesSummary[])
        .then(x => this.horizontalBarChartDataByNs = x);
      this._primeNgHelper.getDataForHorizontalBarChartBySeverity(vulnerabilityReportSumaryDto.severitiesByNsSummaryDtos as SeveritiesSummary[])
        .then(x => this.horizontalBarChartDataBySeverity = x);
      this.extractDataForTables();
    });
  }

  private extractDataForTables() {
    if (this.vulnerabilityReportSumaryDto?.severitiesByNsSummaryDtos) {
      let severitesTotal = this.vulnerabilityReportSumaryDto.severitiesByNsSummaryDtos.find(x => x.isTotal);
      if (severitesTotal) {
        let tableValues: SeveritiySummary[] = [];
        severitesTotal.details?.sort((a, b) => a.id! - b.id!).forEach(x => {
          tableValues.push({
            severityName: this.severityHelperService.getName(x.id!),
            count: this.showDistinctValues ? x.distinctCount! : x.totalCount!,
            fixable: this.showDistinctValues ? x.fixableDistinctCount! : x.fixableTotalCount!,
          });
        });
        this.severitiesSummaryForTable = tableValues;
      }
    }
    if (this.vulnerabilityReportSumaryDto?.imagesByNSSummaryDtos) {
      let totalData = this.vulnerabilityReportSumaryDto.imagesByNSSummaryDtos.find(x => x.isTotal);
      if (totalData) {
        this.othersSummaryForTable.push({
          description: "Images",
          count: this.showDistinctValues ? totalData.distinctCount! : totalData.totalCount!,
        })
      }
    }
    if (this.vulnerabilityReportSumaryDto?.imageOSesByNSSummaryDtos) {
      let totalData = this.vulnerabilityReportSumaryDto.imageOSesByNSSummaryDtos.find(x => x.isTotal);
      if (totalData) {
        this.othersSummaryForTable.push({
          description: "Images OSes",
          count: this.showDistinctValues ? totalData.distinctCount! : totalData.totalCount!,
        })
      }
    }
    if (this.vulnerabilityReportSumaryDto?.imageEOSLByNsSummaryDtos) {
      let totalData = this.vulnerabilityReportSumaryDto.imageEOSLByNsSummaryDtos.find(x => x.isTotal);
      if (totalData) {
        this.othersSummaryForTable.push({
          description: "End of Service Life",
          count: this.showDistinctValues ? totalData.distinctCount! : totalData.totalCount!,
        })
      }
    }

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

  public onOthersMore(element: OtherSummaryMainStatistics) {
    

    this.moreOthersModalTitle = "More Info for " + element.description;
    let tempByNsSummary: GenericByNsSummaryDto[] = [];
    let tempSummary: GenericSummaryDto[] = [];
    switch (element.description) {
      case 'Images':
        tempByNsSummary = this.vulnerabilityReportSumaryDto?.imagesByNSSummaryDtos! as GenericByNsSummaryDto[]
        this.genericByNsSummaryDtos = tempByNsSummary.sort(this.sortOthersByNsSummary);
        this.genericSummaryDtos = this.vulnerabilityReportSumaryDto?.imagesSummaryDtos!.sort((a, b) => a.name! > b.name! ? 1 : -1) as GenericSummaryDto[];
        break;
      case 'Images OSes':
        tempByNsSummary = this.vulnerabilityReportSumaryDto?.imageOSesByNSSummaryDtos! as GenericByNsSummaryDto[];
        this.genericByNsSummaryDtos = tempByNsSummary.sort(this.sortOthersByNsSummary);
        this.genericSummaryDtos = this.vulnerabilityReportSumaryDto?.imageOSSummaryDtos!.sort((a, b) => a.name! > b.name! ? 1 : -1) as GenericSummaryDto[];
        break;
      case 'End of Service Life':
        tempByNsSummary = this.vulnerabilityReportSumaryDto?.imageEOSLByNsSummaryDtos! as GenericByNsSummaryDto[];
        this.genericByNsSummaryDtos = tempByNsSummary.sort(this.sortOthersByNsSummary);
        this.genericSummaryDtos = this.vulnerabilityReportSumaryDto?.imageEOSLSummaryDtos!.sort((a, b) => a.name! > b.name! ? 1 : -1) as GenericSummaryDto[];
        break;
    }
    this.isMoreOthersModalVisible = true;
  }

  public onVrsMore(event: MouseEvent) {
    this.isMoreVRDetailsModalVisible = true;
    console.log("mama");
  }

  sortOthersByNsSummary = (a: GenericNsTotalSortable, b: GenericNsTotalSortable): number => {
    if (a.isTotal === b.isTotal) {
      return a.namespaceName > b.namespaceName ? 1 : -1;
    }
    return a.isTotal ? 1 : -1;
  };

  getRowStyle(rowData: GenericNsTotalSortable) {
    return rowData.isTotal ? { 'font-weight' : 'bold' } : {};
  }

  getSeveritiesDistinctCount(details: VrSeveritiesByNsSummaryDetailDto[], severityId: number): number  {
    let detail = details.find(x => x.id === severityId);
    if (detail == null) {
      return 0;
    }

    return this.showDistinctValues ? detail.distinctCount! : detail.totalCount!;
  }

  getSeveritiesFixableCount(details: VrSeveritiesByNsSummaryDetailDto[], severityId: number): number {
    let detail = details?.find(d => d.id === severityId);
    if (detail == null) {
      return 0;
    }

    return this.showDistinctValues ? detail.fixableDistinctCount! : detail.fixableTotalCount!;
  }

}
