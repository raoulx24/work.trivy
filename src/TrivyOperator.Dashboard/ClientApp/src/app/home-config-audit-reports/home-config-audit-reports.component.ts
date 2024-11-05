import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

import { ConfigAuditReportSummaryDto } from '../../api/models/config-audit-report-summary-dto';
import { ConfigAuditReportService } from '../../api/services/config-audit-report.service';
import { PrimeNgChartUtils, PrimeNgHorizontalBarChartData, SeveritiesSummary } from '../utils/primeng-chart.utils';
import { CarSeveritySummary } from './home-config-audit-reports.types';

import { ButtonModule } from 'primeng/button';
import { CarouselModule } from 'primeng/carousel';
import { ChartModule } from 'primeng/chart';
import { DialogModule } from 'primeng/dialog';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { SeverityUtils } from '../utils/severity.utils';

@Component({
  selector: 'app-home-config-audit-reports',
  standalone: true,
  imports: [CommonModule, ButtonModule, CarouselModule, ChartModule, DialogModule, TableModule, TagModule],
  templateUrl: './home-config-audit-reports.component.html',
  styleUrl: './home-config-audit-reports.component.scss',
})
export class HomeConfigAuditReportsComponent {
  configAuditReportSummaryDtos: ConfigAuditReportSummaryDto[] | null = null;
  namespaceNames: string[] = [];
  kinds: string[] = [];
  severities: number[] = [];
  carSeveritySummaries: CarSeveritySummary[] = [];
  public slides: string[] = ['nsByNs', 'nsBySev', 'kindByNs', 'kindBySev'];
  severitesSummariesNamespace: SeveritiesSummary[] = [];
  severitesSummariesKind: SeveritiesSummary[] = [];
  barchartDataNsByNs: PrimeNgHorizontalBarChartData | null = null;
  barchartDataNsBySev: PrimeNgHorizontalBarChartData | null = null;
  barchartDataKindByNs: PrimeNgHorizontalBarChartData | null = null;
  barchartDataKindBySev: PrimeNgHorizontalBarChartData | null = null;
  public horizontalBarChartOption: any;
  isCarDetailsDialogVisible: boolean = false;
  private localShowDistinctValues: boolean = true;

  constructor(private configAuditReportService: ConfigAuditReportService) {
    this.configAuditReportService.getConfigAuditReportSumaryDtos().subscribe({
      next: (res) => this.onDtos(res),
      error: (err) => console.error(err),
    });
  }

  get showDistinctValues(): boolean {
    return this.localShowDistinctValues;
  }

  @Input() set showDistinctValues(value: boolean) {
    this.localShowDistinctValues = value;
    this.onDistinctSwitch();
  }

  onCarsMore(_event: MouseEvent) {
    this.isCarDetailsDialogVisible = true;
  }

  onDistinctSwitch() {
    if (this.configAuditReportSummaryDtos) {
      this.computeStatisticsByNs();
      this.computeStatisticsByKind();
    }
  }

  getCountFromConfigAuditReportSummaryDtos(namespaceName: string, kind: string, severityId: number): string {
    if (!this.configAuditReportSummaryDtos) {
      return '';
    }
    const result = this.configAuditReportSummaryDtos
      .filter((dto) => dto.namespaceName === namespaceName)
      .filter((dto) => dto.kind === kind)
      .find((dto) => dto.severityId === severityId);
    if (result) {
      return (this.showDistinctValues ? (result.distinctCount ?? 0) : (result.totalCount ?? 0)).toString();
    } else {
      return '0';
    }
  }

  severityWrapperGetCapitalizedName(severityId: number): string {
    return SeverityUtils.getCapitalizedName(severityId);
  }

  severityWrapperGetGetCssColor(severityId: number): string {
    return SeverityUtils.getCssColor(severityId);
  }

  private onDtos(dtos: ConfigAuditReportSummaryDto[]) {
    this.configAuditReportSummaryDtos = dtos;

    this.getArraysFromDtos();
    this.computeStatisticsByNs();
    this.computeStatisticsByKind();
    this.horizontalBarChartOption = PrimeNgChartUtils.getHorizontalBarChartOption();
  }

  private getArraysFromDtos() {
    if (!this.configAuditReportSummaryDtos) {
      return;
    }
    const result = this.configAuditReportSummaryDtos.reduce(
      (acc, item) => {
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
      },
      { namespaceNames: [] as string[], kinds: [] as string[], severities: [] as number[] },
    );

    const { namespaceNames, kinds, severities } = result;
    this.namespaceNames = namespaceNames.sort();
    this.kinds = kinds.sort();
    this.severities = severities.sort((a, b) => a - b);
  }

  private computeStatisticsByNs() {
    if (!this.configAuditReportSummaryDtos) {
      return;
    }
    const groupedSumForCarSeverities = this.configAuditReportSummaryDtos
      .filter((dto) => dto.namespaceName === '')
      .reduce(
        (acc, item) => {
          const severityName: string = SeverityUtils.getCapitalizedName(item.severityId!);
          if (!acc[severityName]) {
            acc[severityName] = 0;
          }
          acc[severityName] += this.showDistinctValues ? (item.distinctCount ?? 0) : (item.totalCount ?? 0);
          return acc;
        },
        {} as Record<string, number>,
      );

    this.carSeveritySummaries = Object.keys(groupedSumForCarSeverities).map((key) => ({
      severityName: key,
      count: groupedSumForCarSeverities[key],
    }));

    const summaryMap: { [key: string]: SeveritiesSummary } = {};
    this.configAuditReportSummaryDtos
      .filter((dto) => dto.namespaceName !== '')
      .forEach((item) => {
        if (!summaryMap[item.kind!]) {
          summaryMap[item.kind!] = {
            namespaceName: item.kind,
            details: [],
            isTotal: false,
          };
        }
        const existingDetail = summaryMap[item.kind!].details!.find((detail) => detail.id === item.severityId);
        if (existingDetail) {
          existingDetail.totalCount! += item.totalCount ?? 0;
          existingDetail.distinctCount! += item.distinctCount ?? 0;
        } else {
          summaryMap[item.kind!].details!.push({
            id: item.severityId,
            totalCount: item.totalCount,
            distinctCount: item.distinctCount,
          });
        }
      });

    this.severitesSummariesNamespace = Object.values(summaryMap);
    this.barchartDataKindByNs = PrimeNgChartUtils.getDataForHorizontalBarChartByNamespace(
      this.severitesSummariesNamespace,
      this.showDistinctValues,
    );
    this.barchartDataKindBySev = PrimeNgChartUtils.getDataForHorizontalBarChartBySeverity(
      this.severitesSummariesNamespace,
      this.showDistinctValues,
    );
  }

  private computeStatisticsByKind() {
    if (!this.configAuditReportSummaryDtos) {
      return;
    }
    const groupedSumForCarSeverities = this.configAuditReportSummaryDtos
      .filter((dto) => dto.namespaceName === '')
      .reduce(
        (acc, item) => {
          const severityName: string = SeverityUtils.getCapitalizedName(item.severityId!);
          if (!acc[severityName]) {
            acc[severityName] = 0;
          }
          acc[severityName] += this.showDistinctValues ? (item.distinctCount ?? 0) : (item.totalCount ?? 0);
          return acc;
        },
        {} as Record<string, number>,
      );

    this.carSeveritySummaries = Object.keys(groupedSumForCarSeverities).map((key) => ({
      severityName: key,
      count: groupedSumForCarSeverities[key],
    }));

    const summaryMap: { [key: string]: SeveritiesSummary } = {};
    this.configAuditReportSummaryDtos
      .filter((dto) => dto.namespaceName !== '')
      .forEach((item) => {
        if (!summaryMap[item.namespaceName!]) {
          summaryMap[item.namespaceName!] = {
            namespaceName: item.namespaceName,
            details: [],
            isTotal: false,
          };
        }
        const existingDetail = summaryMap[item.namespaceName!].details!.find((detail) => detail.id === item.severityId);
        if (existingDetail) {
          existingDetail.totalCount! += item.totalCount ?? 0;
          existingDetail.distinctCount! += item.distinctCount ?? 0;
        } else {
          summaryMap[item.namespaceName!].details!.push({
            id: item.severityId,
            totalCount: item.totalCount,
            distinctCount: item.distinctCount,
          });
        }
      });

    this.severitesSummariesNamespace = Object.values(summaryMap);
    this.barchartDataNsByNs = PrimeNgChartUtils.getDataForHorizontalBarChartByNamespace(
      this.severitesSummariesNamespace,
      this.showDistinctValues,
    );
    this.barchartDataNsBySev = PrimeNgChartUtils.getDataForHorizontalBarChartBySeverity(
      this.severitesSummariesNamespace,
      this.showDistinctValues,
    );
  }
}
