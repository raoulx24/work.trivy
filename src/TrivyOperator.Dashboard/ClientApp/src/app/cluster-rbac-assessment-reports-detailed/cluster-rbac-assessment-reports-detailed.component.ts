import { Component } from '@angular/core';

import { ClusterRbacAssessmentReportService } from '../../api/services/cluster-rbac-assessment-report.service'
import { ClusterRbacAssessmentReportDenormalizedDto } from '../../api/models/cluster-rbac-assessment-report-denormalized-dto'
import { SeverityDto } from '../../api/models/severity-dto';

import { TrivyTableComponent } from '../trivy-table/trivy-table.component';
import { ExportColumn, TrivyTableColumn, TrivyTableOptions } from "../trivy-table/trivy-table.types";
import { TrivyTableUtils } from '../utils/trivy-table.utils'

@Component({
  selector: 'app-cluster-rbac-assessment-reports-detailed',
  standalone: true,
  imports: [TrivyTableComponent],
  templateUrl: './cluster-rbac-assessment-reports-detailed.component.html',
  styleUrl: './cluster-rbac-assessment-reports-detailed.component.scss'
})
export class ClusterRbacAssessmentReportsDetailedComponent {
  public dataDtos?: ClusterRbacAssessmentReportDenormalizedDto[] | null;
  public severityDtos: SeverityDto[] = [];
  public activeNamespaces: string[] = [];
  public isLoading: boolean = false;

  public csvFileName: string = "Cluster.Rbac.Assessment.Reports";

  public exportColumns: ExportColumn[];

  public trivyTableColumns: TrivyTableColumn[];
  public trivyTableOptions: TrivyTableOptions;

  constructor(private dataDtoService: ClusterRbacAssessmentReportService) {
    this.getTableDataDtos();

    this.trivyTableColumns = [
      {
        field: "resourceName", header: "Name",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 240px; max-width: 240px; white-space: normal;", renderType: "standard",
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
      tableStyle: { 'width': '1970px' },
      stateKey: "Cluster RBAC Assessment Reports Detailed",
      dataKey: null,
      rowExpansionRender: null,
      extraClasses: "",
    };
    this.exportColumns = TrivyTableUtils.convertFromTableColumnToExportColumn(this.trivyTableColumns);
  }

  public getTableDataDtos() {
    this.isLoading = true;
    this.dataDtoService.getClusterRbacAssessmentReportDenormalizedDto()
      .subscribe({
        next: (res) => this.onGetDataDtos(res),
        error: (err) => console.error(err)
      });
  }

  onGetDataDtos(dtos: ClusterRbacAssessmentReportDenormalizedDto[]) {
    this.dataDtos = dtos;
    this.isLoading = false;
  }
}
