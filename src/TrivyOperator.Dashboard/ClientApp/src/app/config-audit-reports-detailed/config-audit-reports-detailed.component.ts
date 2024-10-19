import { Component } from '@angular/core';

import { ConfigAuditReportService } from '../../api/services/config-audit-report.service'
import { ConfigAuditReportDenormalizedDto } from '../../api/models/config-audit-report-denormalized-dto'
import { SeverityDto } from '../../api/models/severity-dto';

import { TrivyTableComponent } from '../trivy-table/trivy-table.component';
import { ExportColumn, TrivyTableColumn, TrivyTableOptions } from "../trivy-table/trivy-table.types";


@Component({
  selector: 'app-config-audit-reports-detailed',
  standalone: true,
  imports: [TrivyTableComponent],
  templateUrl: './config-audit-reports-detailed.component.html',
  styleUrl: './config-audit-reports-detailed.component.scss'
})
export class ConfigAuditReportsDetailedComponent {
  public dataDtos?: ConfigAuditReportDenormalizedDto[] | null;
  public severityDtos: SeverityDto[] = [];
  public activeNamespaces: string[] = [];
  public isLoading: boolean = false;

  public csvFileName: string = "Config.Audit.Reports";

  public exportColumns: ExportColumn[];

  public trivyTableColumns: TrivyTableColumn[];
  public trivyTableOptions: TrivyTableOptions;

  constructor(private dataDtoService: ConfigAuditReportService) {
    this.getTableDataDtos();

    this.exportColumns = [
      { dataKey: 'resourceNamespace', title: 'NS' },
      { dataKey: 'resourceName', title: 'Name' },
      { dataKey: 'resourceKind', title: 'Kind' },
      { dataKey: 'severityId', title: 'Sev' },
      { dataKey: 'category', title: 'Category' },
      { dataKey: 'checkId', title: 'CheckId' },
      { dataKey: 'title', title: 'Title' },
      { dataKey: 'description', title: 'Description' },
      { dataKey: 'remediation', title: 'Remediation' },
      { dataKey: 'messages', title: 'Messages' },

    ];
    this.trivyTableColumns = [
      {
        field: "resourceNamespace", header: "NS",
        isFiltrable: true, isSortable: true, multiSelectType: "namespaces",
        style: "width: 130px; max-width: 130px;", renderType: "standard",
      },
      {
        field: "resourceName", header: "Name",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 240px; max-width: 240px; white-space: normal;", renderType: "standard",
      },
      {
        field: "resourceKind", header: "Kind",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 100px; max-width: 100px;", renderType: "standard",
      },
      {
        field: "severityId", header: "Sev",
        isFiltrable: true, isSortable: true, multiSelectType: "severities",
        style: "width: 90px; max-width: 90px;", renderType: "severityBadge",
      },
      {
        field: "category", header: "Category",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 140px; max-width: 140px; white-space: normal;", renderType: "standard",
      },
      {
        field: "checkId", header: "Id",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 95px; max-width: 95px; white-space: normal;", renderType: "standard",
      },
      {
        field: "title", header: "Title",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 180px; max-width: 180px; white-space: normal;", renderType: "standard",
      },
      {
        field: "description", header: "Description",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 360px; max-width: 360px; white-space: normal;", renderType: "standard",
      },
      {
        field: "remediation", header: "Remediation",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 360px; max-width: 360px; white-space: normal;", renderType: "standard",
      },
      {
        field: "messages", header: "Messages",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 500px; max-width: 500px; white-space: normal;", renderType: "multiline",
      },
    ];
    this.trivyTableOptions = {
      isClearSelectionVisible: false,
      isExportCsvVisible: true,
      isResetFiltersVisible: true,
      isRefreshVisible: true,
      isRefreshFiltrable: false,
      isFooterVisible: true,
      tableSelectionMode: null,
      tableStyle: { 'width': '2200px' },
      stateKey: "Config Audit Reports Detailed",
      dataKey: null,
      extraClasses: "",
    };
  }

  public getTableDataDtos() {
    this.isLoading = true;
    this.dataDtoService.getConfigAuditReportDenormalizedDto()
      .subscribe({
        next: (res) => this.onGetDataDtos(res),
        error: (err) => console.error(err)
      });
    this.dataDtoService.getConfigAuditReportActiveNamespaces()
      .subscribe({
        next: (res) => this.onGetActiveNamespaces(res),
        error: (err) => console.error(err)
      })
  }

  onGetDataDtos(dtos: ConfigAuditReportDenormalizedDto[]) {
    this.dataDtos = dtos;
    this.isLoading = false;
  }

  onGetActiveNamespaces(dtos: string[]) {
    this.activeNamespaces = dtos.sort((x, y) => x > y ? 1 : -1);
  }
}
