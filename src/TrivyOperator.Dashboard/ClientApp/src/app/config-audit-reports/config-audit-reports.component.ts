import { Component } from '@angular/core';

import { GenericMasterDetailComponent } from '../generic-master-detail/generic-master-detail.component'
import { ConfigAuditReportService } from '../../api/services/config-audit-report.service'
import { SeverityHelperService } from '../services/severity-helper.service';
import { ConfigAuditReportDto } from '../../api/models/config-audit-report-dto'
import { ConfigAuditReportDetailDto } from '../../api/models/config-audit-report-detail-dto';
import { SeverityDto } from '../../api/models/severity-dto';
import { TrivyFilterData, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';


@Component({
  selector: 'app-config-audit-reports',
  standalone: true,
  imports: [GenericMasterDetailComponent],
  templateUrl: './config-audit-reports.component.html',
  styleUrl: './config-audit-reports.component.scss'
})
export class ConfigAuditReportsComponent {
  public dataDtos: ConfigAuditReportDto[] = [];
  public severityDtos: SeverityDto[] | null = [];
  public activeNamespaces: string[] | null = [];

  public mainTableColumns: TrivyTableColumn[] = [];
  public mainTableOptions: TrivyTableOptions;
  public mainTableExpandCallbackDto: ConfigAuditReportDto | null = null;
  public isMainTableLoading: boolean = true;

  public detailsTableColumns: TrivyTableColumn[] = [];
  public detailsTableOptions: TrivyTableOptions;

  constructor(private dataDtoService: ConfigAuditReportService, public severityHelperService: SeverityHelperService) {
    dataDtoService.getConfigAuditReportDtos()
      .subscribe({
        next: (res) => this.onGetDataDtos(res),
        error: (err) => console.error(err)
      });
    dataDtoService.getConfigAuditReportActiveNamespaces()
      .subscribe({
        next: (res) => this.onGetActiveNamespaces(res),
        error: (err) => console.error(err)
      });
    this.mainTableColumns = [
      {
        field: "resourceNamespace", header: "NS",
        isFiltrable: true, isSortable: true, multiSelectType: "namespaces",
        style: "width: 130px; max-width: 130px;", renderType: "standard",
      },
      {
        field: "resourceName", header: "Name",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "white-space: normal;", renderType: "standard",
      },
      {
        field: "resourceKind", header: "Kind",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 100px; max-width: 100px;", renderType: "standard",
      },
      {
        field: "criticalCount", header: "Severity C / H / M / L",
        isFiltrable: false, isSortable: false, multiSelectType: "none",
        style: "width: 170px; max-width: 145px; ", renderType: "severityMultiTags",
        extraFields: ["highCount", "mediumCount", "lowCount"],
      },
    ];
    this.mainTableOptions = {
      isClearSelectionVisible: false,
      isExportCsvVisible: false,
      isResetFiltersVisible: true,
      isRefreshVisible: true,
      isRefreshFiltrable: true,
      isFooterVisible: true,
      tableSelectionMode: "single",
      tableStyle: {},
      stateKey: "Config Audit Reports - Main",
      dataKey: null,
      extraClasses: "trivy-half",
    };
    this.detailsTableColumns = [
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
    ];
    this.detailsTableOptions = {
      isClearSelectionVisible: false,
      isExportCsvVisible: false,
      isResetFiltersVisible: true,
      isRefreshVisible: false,
      isRefreshFiltrable: false,
      isFooterVisible: false,
      tableSelectionMode: null,
      tableStyle: {},
      stateKey: "Config Audit Reports - Details",
      dataKey: null,
      extraClasses: "trivy-half",
    };
  }

  onGetDataDtos(dtos: ConfigAuditReportDto[]) {
    this.dataDtos = dtos;
  }

  onGetActiveNamespaces(activeNamespaces: string[]) {
    this.activeNamespaces = activeNamespaces.sort((x, y) => x > y ? 1 : -1);
  }

  public onRefreshRequested(_event: TrivyFilterData) { }
}
