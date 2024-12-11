import { Component } from '@angular/core';

import { ClusterComplianceReportDto } from '../../api/models/cluster-compliance-report-dto';
import { SeverityDto } from '../../api/models/severity-dto';
import { ClusterComplianceReportService } from '../../api/services/cluster-compliance-report.service';

import { ClusterComplianceReportDenormalizedDto } from '../../api/models';
import { TrivyTableComponent } from '../trivy-table/trivy-table.component';
import { ExportColumn, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';
import { TrivyTableUtils } from '../utils/trivy-table.utils';

@Component({
  selector: 'app-cluster-compliance-reports-detailed',
  standalone: true,
  imports: [TrivyTableComponent],
  templateUrl: './cluster-compliance-reports-detailed.component.html',
  styleUrl: './cluster-compliance-reports-detailed.component.scss',
})
export class ClusterComplianceReportsDetailedComponent {
  public dataDtos?: ClusterComplianceReportDto[] | null;
  public severityDtos: SeverityDto[] = [];
  public isLoading: boolean = false;

  public csvFileName: string = 'Cluster.Compliance.Reports';

  public exportColumns: ExportColumn[];

  public trivyTableColumns: TrivyTableColumn[];
  public trivyTableOptions: TrivyTableOptions;

  constructor(private dataDtoService: ClusterComplianceReportService) {
    this.getTableDataDtos();

    this.trivyTableColumns = [
      {
        field: 'name',
        header: 'Name',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 120px; max-width: 120px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'description',
        header: 'Description',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 260px; max-width: 260px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'platform',
        header: 'Platf',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 130px; max-width: 130px; white-space: normal;',
        renderType: 'standard',
      },
      //{
      //  field: 'relatedResources',
      //  header: 'RelatedResources',
      //  isFiltrable: true,
      //  isSortable: true,
      //  multiSelectType: 'none',
      //  style: 'width: 240px; max-width: 240px; white-space: normal;',
      //  renderType: 'standard',
      //},
      {
        field: 'title',
        header: 'Title',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 290px; max-width: 290px; white-space: normal;',
        renderType: 'link',
        extraFields: ['relatedResources'],
      },
      {
        field: 'type',
        header: 'Type',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 110px; max-width: 110px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'version',
        header: 'Ver',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 100px; max-width: 100px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'cron',
        header: 'Cron',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 110px; max-width: 110px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'reportType',
        header: 'RepType',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 140px; max-width: 140px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'totalPassCount',
        header: 'PassCount',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 150px; max-width: 150px; white-space: normal; text-align: right;',
        renderType: 'standard',
      },
      {
        field: 'totalFailCount',
        header: 'FailCount',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 150px; max-width: 150px; white-space: normal; text-align: right;',
        renderType: 'standard',
      },
      {
        field: 'updateTimestamp',
        header: 'Updated',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 140px; max-width: 140px; white-space: normal;',
        renderType: 'date',
      },
      {
        field: 'detailId',
        header: 'Id',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 110px; max-width: 110px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'detailName',
        header: 'Detail Name',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 340px; max-width: 340px; white-space: normal;',
        renderType: 'standard',
      },
      {
        field: 'detailDescription',
        header: 'Detail Description',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 380px; max-width: 380px; white-space: normal;',
        renderType: 'standard',
      },
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
        header: 'TotFail',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'width: 140px; max-width: 140px; white-space: normal; text-align: right;',
        renderType: 'standard',
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
      tableStyle: { width: '3040px' },
      stateKey: 'Cluster Compliance Reports Detailed',
      dataKey: null,
      rowExpansionRender: null,
      extraClasses: '',
    };
    this.exportColumns = TrivyTableUtils.convertFromTableColumnToExportColumn(this.trivyTableColumns);
  }

  getTableDataDtos() {
    this.isLoading = true;
    this.dataDtoService.getClusterComplianceReportDenormalizedDtos().subscribe({
      next: (res) => this.onGetDataDtos(res),
      error: (err) => console.error(err),
    });
  }

  onGetDataDtos(dtos: ClusterComplianceReportDenormalizedDto[]) {
    this.dataDtos = dtos;
    this.isLoading = false;
  }
}
