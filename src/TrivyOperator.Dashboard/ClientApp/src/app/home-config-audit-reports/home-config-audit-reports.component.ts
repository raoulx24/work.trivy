import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ConfigAuditReportService } from '../../api/services/config-audit-report.service'
import { ConfigAuditReportSummaryDto } from '../../api/models/config-audit-report-summary-dto'
import { SeverityHelperService } from '../services/severity-helper.service'
import { CarSeveritySummary } from './home-config-audit-reports.types'

import { ButtonModule } from 'primeng/button';
import { ChartModule } from 'primeng/chart';
import { DialogModule } from 'primeng/dialog';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TabViewModule } from 'primeng/tabview';


@Component({
  selector: 'app-home-config-audit-reports',
  standalone: true,
  imports: [CommonModule, ButtonModule, ChartModule, DialogModule, TableModule, TabViewModule, TagModule],
  templateUrl: './home-config-audit-reports.component.html',
  styleUrl: './home-config-audit-reports.component.scss'
})

export class HomeConfigAuditReportsComponent {
  @Input() set showDistinctValues(value: boolean) {
    this.localShowDistinctValues = value;
    this.onDistinctSwitch();
  }
  get showDistinctValues(): boolean {
    return this.localShowDistinctValues;
  }

  configAuditReportSummaryDtos: ConfigAuditReportSummaryDto[] = [];
  namespaceNames: string[] = [];
  kinds: string[] = [];
  severities: number[] = [];
  carSeveritySummaries: CarSeveritySummary[] = [];

  isCarDetailsDialogVisible: boolean = false;

  private localShowDistinctValues: boolean = true;

  constructor(private configAuditReportService: ConfigAuditReportService, public severityHelperService: SeverityHelperService) {
    console.log("constructor - car");
    this.configAuditReportService.getConfigAuditReportSumaryDtos()
      .subscribe({
        next: (res) => this.onDtos(res),
        error: (err) => console.error(err)
      });
  }

  private onDtos(dtos: ConfigAuditReportSummaryDto[]) {
    this.configAuditReportSummaryDtos = dtos;

    this.getArraysFromDtos();
    this.computeStatistics();
  }

  private getArraysFromDtos() {
    const result = this.configAuditReportSummaryDtos.reduce((acc, item) => {
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
    this.namespaceNames = namespaceNames.sort();
    this.kinds = kinds.sort();
    this.severities = severities.sort((a, b) => a - b);
  }

  private computeStatistics() {
    const groupedSum = this.configAuditReportSummaryDtos
      .filter(dto => dto.namespaceName === "")
      .reduce((acc, item) => {
        const severityName: string = this.severityHelperService.getCapitalizedName(item.severityId!);
        if (!acc[severityName]) {
          acc[severityName] = 0;
        }
        acc[severityName] += this.showDistinctValues ? item.distinctCount ?? 0 : item.totalCount ?? 0;
        return acc;
      }, {} as Record<string, number>);

    this.carSeveritySummaries = Object.keys(groupedSum).map(key => ({ severityName: key, count: groupedSum[key] }))
  }

  onCarsMore(_event: MouseEvent) {
    console.log("mama");
    this.isCarDetailsDialogVisible = true;
  }

  onDistinctSwitch() {
    if (this.configAuditReportSummaryDtos) {
      this.computeStatistics();
    }
  }

  getCountFromConfigAuditReportSummaryDtos(namespaceName: string, kind: string, severityId: number): string {
    const result = this.configAuditReportSummaryDtos
      .filter(dto => dto.namespaceName === namespaceName)
      .filter(dto => dto.kind === kind)
      .find(dto => dto.severityId === severityId);
    if (result) {
      return (this.showDistinctValues ? result.distinctCount ?? 0 : result.totalCount ?? 0).toString();
    }
    else {
      return "0";
    }
  }
}
