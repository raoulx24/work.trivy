import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { VulnerabilityReportsService } from "../../api/services/vulnerability-reports.service";
import { VulnerabilityReportSumaryDto } from "../../api/models/vulnerability-report-sumary-dto";
import { VrSeveritiesByNsSummaryDto } from '../../api/models/vr-severities-by-ns-summary-dto';
import { PrimeNgPieChartData, PrimeNgHorizontalBarChartData, PrimeNgChartUtils, SeveritiesSummary } from "../utils/primeng-chart.utils";
import { SeverityHelperService } from "../services/severity-helper.service"
import { SeverityDto } from "../../api/models/severity-dto"
import { UIChart } from 'primeng/chart';
import { timer } from 'rxjs';
import { VrSeveritiesByNsSummaryDetailDto } from '../../api/models/vr-severities-by-ns-summary-detail-dto';

import { ButtonModule } from 'primeng/button';
import { CarouselModule } from 'primeng/carousel';
import { ChartModule } from 'primeng/chart';
import { DialogModule } from 'primeng/dialog';
import { InputSwitchChangeEvent, InputSwitchModule } from 'primeng/inputswitch';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';

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
  distinctCount: number,
  isTotal: boolean,
}

export interface GenericNsTotalSortable {
  namespaceName: string,
  isTotal: boolean,
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, CarouselModule, ChartModule, DialogModule, InputSwitchModule, PanelModule, TableModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})

export class HomeComponent {
  public vulnerabilityReportSumaryDto?: VulnerabilityReportSumaryDto | null | undefined;

  public severityHelperService: SeverityHelperService;
  public get primeNgHelper(): PrimeNgChartUtils { return this._primeNgHelper; };
  private _primeNgHelper: PrimeNgChartUtils;

  //public pieChartData: PrimeNgPieChartData[] | null | undefined;
  public horizontalBarChartDataByNs: PrimeNgHorizontalBarChartData | null | undefined;
  public horizontalBarChartDataBySeverity: PrimeNgHorizontalBarChartData | null | undefined;
  public horizontalBarChartOption: any;
  public slides: string[] = ["barChartNS", "barChartSeverity"];

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
    this._primeNgHelper = new PrimeNgChartUtils(this.severityHelperService);
    severityHelperService.getSeverityDtos().then(result => {
      this.initComponents();
    });
  }

  private initComponents() {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--text-color');
    const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
    const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

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
    this.severityHelperService.getSeverityDtos().then(_ => {
      this.extractDataForCharts();
      this.extractDataForTables();
    });
  }

  private extractDataForCharts() {
    this._primeNgHelper.getDataForHorizontalBarChartByNamespace(
      this.vulnerabilityReportSumaryDto?.severitiesByNsSummaryDtos as SeveritiesSummary[], this.showDistinctValues)
      .then(x => this.horizontalBarChartDataByNs = x);
    this._primeNgHelper.getDataForHorizontalBarChartBySeverity(
      this.vulnerabilityReportSumaryDto?.severitiesByNsSummaryDtos as SeveritiesSummary[], this.showDistinctValues)
      .then(x => this.horizontalBarChartDataBySeverity = x);
  }

  private extractDataForTables() {
    if (this.vulnerabilityReportSumaryDto?.severitiesByNsSummaryDtos) {
      let severitesTotal = this.vulnerabilityReportSumaryDto.severitiesByNsSummaryDtos.find(x => x.isTotal);
      if (severitesTotal) {
        let tableValues: SeveritiySummary[] = [];
        severitesTotal.details?.sort((a, b) => a.id! - b.id!).forEach(x => {
          tableValues.push({
            severityName: this.severityHelperService.getCapitalizedName(x.id!),
            count: this.showDistinctValues ? x.distinctCount! : x.totalCount!,
            fixable: this.showDistinctValues ? x.fixableDistinctCount! : x.fixableTotalCount!,
          });
        });
        this.severitiesSummaryForTable = tableValues;
      }
    }
    this.othersSummaryForTable = [];
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

  getFooterData(fieldName: string): string {
    let dto: GenericByNsSummaryDto = this.genericByNsSummaryDtos.filter(x => x.isTotal)[0];
    if (!dto) {
      return "";
    }
    switch (fieldName) {
      case "namespaceName":
        return dto.namespaceName;
      case "totalCount":
        return dto.totalCount.toString();
      case "distinctCount":
        return dto.distinctCount.toString();
      default:
        return "";
    }
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

  onDistinctSwitch(event: any) {
    this.extractDataForCharts();
    this.extractDataForTables();
  }
}
