import { Component } from '@angular/core';

import { SbomReportDetailDto } from '../../api/models/sbom-report-detail-dto';
import { SbomReportDto } from '../../api/models/sbom-report-dto';
import { SbomReportService } from '../../api/services/sbom-report.service';

import { FcoseComponent } from '../fcose/fcose.component';
import { TrivyTableComponent } from '../trivy-table/trivy-table.component';
import { TrivyFilterData, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';

@Component({
  selector: 'app-sbom-reports',
  standalone: true,
  imports: [FcoseComponent, TrivyTableComponent],
  templateUrl: './sbom-reports.component.html',
  styleUrl: './sbom-reports.component.scss'
})
export class SbomReportsComponent {
  dataDtos: SbomReportDto[] | null = null;
  activeNamespaces: string[] | null = [];

  public mainTableColumns: TrivyTableColumn[] = [];
  public mainTableOptions: TrivyTableOptions;
  public isMainTableLoading: boolean = true;

  set selectedDataDto(dataDto: SbomReportDto | null) {
    if (dataDto) {
      this.getFullSbomDto(dataDto.uid ?? "");
    }
    else {
      this.fullSbomDataDto = null;
    }
  }
  private _selectedDataDto: SbomReportDto | null = null;

  fullSbomDataDto: SbomReportDto | null = null;

  constructor(private service: SbomReportService) {
    this.getTableDataDtos();

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
        field: 'imageName',
        header: 'Image Name - Tag',
        isFiltrable: true,
        isSortable: true,
        multiSelectType: 'none',
        style: 'white-space: normal;',
        renderType: 'imageNameTag',
        extraFields: ['imageTag', 'imageEosl'],
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
      stateKey: 'SBOM Reports - Main',
      dataKey: null,
      rowExpansionRender: null,
      extraClasses: 'trivy-half',
    };

  }

  getTableDataDtos() {
    this.service.getSbomReportDtos().subscribe({
      next: (res) => this.onGetDataDtos(res),
      error: (err) => console.error(err),
    });
    this.service.getSbomReportActiveNamespaces().subscribe({
      next: (res) => this.activeNamespaces = res,
      error: (err) => console.error(err),
    });
  }

  getFullSbomDto(uid: string) {
    this.service.getSbomReportDtoByUid({ uid: uid }).subscribe({
      next: (res) => this.fullSbomDataDto = res,
      error: (err) => console.error(err),
    });
  }

  onGetDataDtos(dtos: SbomReportDto[]) {
    this.dataDtos = dtos;
    this.isMainTableLoading = false;
  }

  onMainTableSelectionChange(event: SbomReportDto[]) {
    if (event == null || event.length == 0) {
      this.selectedDataDto = null;
      return;
    } else {
      this.selectedDataDto = event[0];
    }
  }

  public onRefreshRequested(event: TrivyFilterData) {
    //const excludedSeverities =
    //  SeverityUtils.getSeverityIds().filter((severityId) => !event.selectedSeverityIds.includes(severityId)) || [];

    //const params: GetVulnerabilityReportImageDtos$Params = {
    //  namespaceName: event.namespaceName ?? undefined,
    //  excludedSeverities: excludedSeverities.length > 0 ? excludedSeverities.join(',') : undefined,
    //};
    this.isMainTableLoading = true;
    this.service.getSbomReportDtos().subscribe({
      next: (res) => this.onGetDataDtos(res),
      error: (err) => console.error(err),
    });
  }

}
