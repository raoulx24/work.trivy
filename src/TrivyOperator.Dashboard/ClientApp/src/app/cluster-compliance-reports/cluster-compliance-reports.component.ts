import { Component } from '@angular/core';

import { ClusterComplianceReportDto } from '../../api/models/cluster-compliance-report-dto';
import { ClusterComplianceReportService } from '../../api/services/cluster-compliance-report.service';
import { GenericMasterDetailComponent } from '../generic-master-detail/generic-master-detail.component';
import { TrivyExpandTableOptions, TrivyFilterData, TrivyTableCellCustomOptions, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';

@Component({
  selector: 'app-cluster-compliance-reports',
  standalone: true,
  imports: [GenericMasterDetailComponent],
  templateUrl: './cluster-compliance-reports.component.html',
  styleUrl: './cluster-compliance-reports.component.scss'
})
export class ClusterComplianceReportsComponent {
  public dataDtos: ClusterComplianceReportDto[] = [];
  public activeNamespaces: string[] | null = [];

  public mainTableColumns: TrivyTableColumn[] = [];
  public mainTableOptions: TrivyTableOptions;
  public mainTableExpandTableOptions: TrivyExpandTableOptions;
  //public mainTableExpandCallbackDto: ClusterComplianceReportDto | null = null;
  public isMainTableLoading: boolean = true;

  public detailsTableColumns: TrivyTableColumn[] = [];
  public detailsTableOptions: TrivyTableOptions;

  constructor(private dataDtoService: ClusterComplianceReportService) {
    this.getDataDtos();

    this.mainTableColumns = [
      {
        field: 'name',
        header: 'Name',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'title',
        header: 'Title',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'totalFailCriticalCount',
        header: 'Severity C / H / M / L',
        isFiltrable: false,
        isSortable: false,
        multiSelectType: 'none',
        style: 'width: 170px; max-width: 145px; ',
        renderType: 'severityMultiTags',
        extraFields: ['totalFailHighCount', 'totalFailMediumCount', 'totalFailLowCount'],
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
      stateKey: 'Cluster Compliance Reports - Main',
      dataKey: 'uid',
      rowExpansionRender: 'table',
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
        field: 'id',
        header: 'Id',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 90px; max-width: 90px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'name',
        header: 'Name',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 320px; max-width: 320px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'description',
        header: 'Description',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'checks',
        header: 'Checks',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 140px; max-width: 140px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'commands',
        header: 'Commands',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 150px; max-width: 150px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'totalFail',
        header: 'Failed',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 140px; max-width: 140px; white-space: normal; text-align: right;',
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
      stateKey: 'Cluster Compliance Reports - Details',
      dataKey: null,
      rowExpansionRender: null,
      extraClasses: 'trivy-half',
    };
    this.mainTableExpandTableOptions = new TrivyExpandTableOptions(false, 2, 8);
  }

  onGetDataDtos(dtos: ClusterComplianceReportDto[]) {
    this.dataDtos = dtos;
  }

  public onRefreshRequested(_event: TrivyFilterData) {
    this.getDataDtos();
  }

  mainTableExpandCellOptions(
    dto: ClusterComplianceReportDto,
    type: 'header' | 'row',
    colIndex: number,
    rowIndex?: number,
  ): TrivyTableCellCustomOptions {
    rowIndex ?? 0;
    let celValue: string = '';
    let celStyle: string = '';
    let celBadge: string | undefined;
    let celButtonLink: string | undefined;
    let celUrl: string | undefined;

    switch (colIndex) {
      case 0:
        celStyle = 'width: 70px; min-width: 70px; height: 50px';
        switch (rowIndex) {
          case 0:
            celValue = 'Description';
            break;
          case 1:
            celValue = 'Platform';
            break;
          case 2:
            celValue = 'Type';
            break;
          case 3:
            celValue = 'Version';
            break;
          case 4:
            celValue = 'Report Type';
            break;
          case 5:
            celValue = 'Cron';
            break;
          case 6:
            celValue = 'Updated';
            break;
          case 7:
            celValue = 'Related Resources';
            break;
        }
        break;
      case 1:
        celStyle = 'white-space: normal; display: flex; align-items: center; height: 50px;';
        switch (rowIndex) {
          case 0:
            celValue = dto.description ?? "";
            break;
          case 1:
            celValue = dto.platform ?? "";
            break;
          case 2:
            celValue = dto.type ?? "";
            break;
          case 3:
            celValue = dto.version ?? "";
            break;
          case 4:
            celValue = dto.reportType ?? "";
            break;
          case 5:
            celValue = dto.cron ?? "";
            break;
          case 6:
            if (dto.updateTimestamp) {
              const date = new Date(dto.updateTimestamp);
              const year = date.getFullYear();
              const month = ('0' + (date.getMonth() + 1)).slice(-2);
              const day = ('0' + date.getDate()).slice(-2);
              celValue = `${year}-${month}-${day}`;
            }
            else {
              celValue = "";
            }
            break;
          case 7:
            celValue = dto.relatedResources ? dto.relatedResources[0] : "";
            celUrl = dto.relatedResources ? dto.relatedResources[0] : "";
            break;
        }
        break;
    }

    return {
      value: celValue,
      style: celStyle,
      badge: celBadge,
      buttonLink: celButtonLink,
      url: celUrl,
    };
  }

  private getDataDtos() {
    this.dataDtoService.getClusterComplianceReportDtos().subscribe({
      next: (res) => this.onGetDataDtos(res),
      error: (err) => console.error(err),
    });
  }
}
