import { Component } from '@angular/core';

import { RbacAssessmentReportDto } from '../../api/models/rbac-assessment-report-dto';
import { RbacAssessmentReportService } from '../../api/services/rbac-assessment-report.service';
import { GenericMasterDetailComponent } from '../generic-master-detail/generic-master-detail.component';
import { TrivyFilterData, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';

@Component({
  selector: 'app-rbac-assessment-reports',
  standalone: true,
  imports: [GenericMasterDetailComponent],
  templateUrl: './rbac-assessment-reports.component.html',
  styleUrl: './rbac-assessment-reports.component.scss'
})
export class RbacAssessmentReportsComponent {
  dataDtos: RbacAssessmentReportDto[] = [];
  activeNamespaces: string[] | null = [];

  mainTableColumns: TrivyTableColumn[] = [];
  mainTableOptions: TrivyTableOptions;
  isMainTableLoading: boolean = true;

  detailsTableColumns: TrivyTableColumn[] = [];
  detailsTableOptions: TrivyTableOptions;

  constructor(private dataDtoService: RbacAssessmentReportService) {
    dataDtoService.getRbacAssessmentReportDtos().subscribe({
      next: (res) => this.onGetDataDtos(res),
      error: (err) => console.error(err),
    });
    dataDtoService.getRbacAssessmentReportActiveNamespaces().subscribe({
      next: (res) => this.onGetActiveNamespaces(res),
      error: (err) => console.error(err),
    });

    this.mainTableColumns = [
      {
        field: 'resourceNamespace',
        header: 'NS',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'namespaces',
        style: 'width: 130px; max-width: 130px;',
        renderType: 'standard',
      },
      {
        field: 'resourceName',
        header: 'Name',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'criticalCount',
        header: 'Severity C / H / M / L',
        isFiltrable: false,
        isSortable: false,
        multiSelectType: 'none',
        style: 'width: 170px; max-width: 145px; ',
        renderType: 'severityMultiTags',
        extraFields: ['highCount', 'mediumCount', 'lowCount'],
      },
    ];
    this.mainTableOptions = {
      isClearSelectionVisible: false,
      isExportCsvVisible: false,
      isResetFiltersVisible: true,
      isRefreshVisible: true,
      isRefreshFiltrable: false,
      isFooterVisible: true,
      tableSelectionMode: 'single',
      tableStyle: {},
      stateKey: 'RBAC Assessment Reports - Main',
      dataKey: null,
      rowExpansionRender: null,
      extraClasses: 'trivy-half',
    };
    this.detailsTableColumns = [
      {
        field: 'severityId',
        header: 'Sev',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'severities',
        style: 'width: 90px; max-width: 90px;',
        renderType: 'severityBadge',
      },
      {
        field: 'category',
        header: 'Category',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 140px; max-width: 140px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'checkId',
        header: 'Id',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 95px; max-width: 95px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'title',
        header: 'Title',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 180px; max-width: 180px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'description',
        header: 'Description',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 360px; max-width: 360px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'remediation',
        header: 'Remediation',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 360px; max-width: 360px; white-space: normal;',
        renderType: 'standard',
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
      stateKey: 'RBAC Assessment Reports - Details',
      dataKey: 'uid',
      rowExpansionRender: 'messages',
      extraClasses: 'trivy-half',
    };
  }

  onGetDataDtos(dtos: RbacAssessmentReportDto[]) {
    this.dataDtos = dtos;
  }

  onGetActiveNamespaces(activeNamespaces: string[]) {
    this.activeNamespaces = activeNamespaces.sort((x, y) => (x > y ? 1 : -1));
  }

  public onRefreshRequested(_event: TrivyFilterData) {
    this.dataDtoService.getRbacAssessmentReportDtos().subscribe({
      next: (res) => this.onGetDataDtos(res),
      error: (err) => console.error(err),
    });
  }
}
