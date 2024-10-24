import { Component } from '@angular/core';

import { GenericMasterDetailComponent } from '../generic-master-detail/generic-master-detail.component'
import { ClusterRbacAssessmentReportService } from '../../api/services/cluster-rbac-assessment-report.service'
import { ClusterRbacAssessmentReportDto } from '../../api/models/cluster-rbac-assessment-report-dto'
import { SeverityDto } from '../../api/models/severity-dto';
import { TrivyFilterData, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';

@Component({
  selector: 'app-cluster-rbac-assessment-reports',
  standalone: true,
  imports: [GenericMasterDetailComponent],
  templateUrl: './cluster-rbac-assessment-reports.component.html',
  styleUrl: './cluster-rbac-assessment-reports.component.scss'
})
export class ClusterRbacAssessmentReportsComponent {
  public dataDtos: ClusterRbacAssessmentReportDto[] = [];
  public activeNamespaces: string[] | null = [];

  public mainTableColumns: TrivyTableColumn[] = [];
  public mainTableOptions: TrivyTableOptions;
  public mainTableExpandCallbackDto: ClusterRbacAssessmentReportDto | null = null;
  public isMainTableLoading: boolean = true;

  public detailsTableColumns: TrivyTableColumn[] = [];
  public detailsTableOptions: TrivyTableOptions;

  constructor(private dataDtoService: ClusterRbacAssessmentReportService) {
    dataDtoService.getClusterRbacAssessmentReportDtos()
      .subscribe({
        next: (res) => this.onGetDataDtos(res),
        error: (err) => console.error(err)
      });
    this.mainTableColumns = [
      {
        field: "resourceName", header: "Name",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "white-space: normal;", renderType: "standard",
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
      isRefreshFiltrable: false,
      isFooterVisible: true,
      tableSelectionMode: "single",
      tableStyle: {},
      stateKey: "Cluster RBAC Assessment Reports - Main",
      dataKey: null,
      rowExpansionRender: null,
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
      stateKey: "Cluster RBAC Assessment Reports - Details",
      dataKey: 'uid',
      rowExpansionRender: 'messages',
      extraClasses: "trivy-half",
    }
  }

  onGetDataDtos(dtos: ClusterRbacAssessmentReportDto[]) {
    this.dataDtos = dtos;
  }

  public onRefreshRequested(_event: TrivyFilterData) {
    this.dataDtoService.getClusterRbacAssessmentReportDtos()
      .subscribe({
        next: (res) => this.onGetDataDtos(res),
        error: (err) => console.error(err)
      });
  }
}
