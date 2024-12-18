import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { SbomReportDetailDto } from '../../api/models/sbom-report-detail-dto';
import { SbomReportDto } from '../../api/models/sbom-report-dto';
import { SbomReportService } from '../../api/services/sbom-report.service';

import { FcoseComponent } from '../fcose/fcose.component';
import { TrivyTableComponent } from '../trivy-table/trivy-table.component';
import { TrivyFilterData, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';

import { DropdownModule } from 'primeng/dropdown';
import { CardModule } from 'primeng/card';

export interface ImageDto {
  uid: string;
  imageNameTag: string;
}

@Component({
  selector: 'app-sbom-reports',
  standalone: true,
  imports: [FormsModule, FcoseComponent, TrivyTableComponent, DropdownModule, CardModule],
  templateUrl: './sbom-reports.component.html',
  styleUrl: './sbom-reports.component.scss'
})
export class SbomReportsComponent {
  dataDtos: SbomReportDto[] | null = null;
  activeNamespaces: string[] | undefined = [];
  imageDtos: ImageDto[] | undefined = [];

  get selectedNamespace(): string | null {
    return this._selectedNamespace;
  }
  set selectedNamespace(value: string | null) {
    this._selectedNamespace = value;

  }
  private _selectedNamespace: string | null = "";

  get selectedImageDto(): ImageDto | null {
    return this._imageDto;
  }
  set selectedImageDto(value: ImageDto | null) {
    this._imageDto = value;
    this.getFullSbomDto(value?.uid);
    
  }
  private _imageDto: ImageDto | null = null;


  set selectedInnerNodeId(value: string | undefined) {
    console.log("sbom - selectedInnerNodeId - value - " + value);
    this._selectedInnerNodeId = value;
    const temp1 = this.dataDtos?.
      find(x => x.uid = this.selectedImageDto?.uid);
    console.log("sbom - selectedInnerNodeId - temp1 - " + temp1?.uid);
    const temp2 = this.fullSbomDataDto?.details?.
      find(x => x.bomRef == value);
    console.log("sbom - selectedInnerNodeId - temp2 - " + temp2);
    const temp3 = temp1?.details?.map(x => x.bomRef).join(", ");
    console.log("sbom - selectedInnerNodeId - temp3 - " + temp3);
    this.selectedSbomDetailDto = temp2;
    console.log("sbom - selectedInnerNodeId - " + this.selectedSbomDetailDto);
  }
  get selectedInnerNodeId(): string | undefined {
    return this._selectedInnerNodeId;
  }
  private _selectedInnerNodeId: string | undefined = undefined;

  selectedSbomDetailDto: SbomReportDetailDto | undefined = undefined;


  public mainTableColumns: TrivyTableColumn[] = [];
  public mainTableOptions: TrivyTableOptions;
  public isMainTableLoading: boolean = true;

  set selectedDataDto(dataDto: SbomReportDto | null) {
    this.getFullSbomDto(dataDto?.uid);
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

  getFullSbomDto(uid: string | null | undefined) {
    if (uid) {
      this.service.getSbomReportDtoByUid({ uid: uid }).subscribe({
        next: (res) => this.fullSbomDataDto = res,
        error: (err) => console.error(err),
      });
    }
    this.fullSbomDataDto = null;
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

  filterImageDtos() {
    this.imageDtos = this.dataDtos?.filter(x => x.resourceNamespace == this.selectedNamespace)
      .map(x => ({ uid: x.uid ?? "", imageNameTag: `${x.imageName}:${x.imageTag}` }));
  }
}
