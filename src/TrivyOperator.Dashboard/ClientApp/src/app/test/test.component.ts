import { Component, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GenericMasterDetailComponent } from '../generic-master-detail/generic-master-detail.component'
import { VulnerabilityReportImageDto } from '../../api/models/vulnerability-report-image-dto';
import { SeverityDto } from '../../api/models/severity-dto';
import { TrivyExpandTableOptions, TrivyFilterData, TrivyTableCellCustomOptions, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';
import { VulnerabilityReportsService } from '../../api/services/vulnerability-reports.service';
import { SeverityHelperService } from '../services/severity-helper.service';
import { GetVulnerabilityReportImageDtos$Params } from '../../api/fn/vulnerability-reports/get-vulnerability-report-image-dtos';
import { VulnerabilityReportDetailDto } from '../../api/models';

import { DialogModule } from 'primeng/dialog';
import { TableModule } from 'primeng/table';

@Component({
  selector: 'app-test',
  standalone: true,
  imports: [ CommonModule, GenericMasterDetailComponent, DialogModule, TableModule ],
  templateUrl: './test.component.html',
  styleUrl: './test.component.scss'
})
export class TestComponent {
  public vulnerabilityReportDtos: VulnerabilityReportImageDto[] = [];
  public selectedVulnerabilityReportDto: VulnerabilityReportImageDto | null =  null;
  public severityDtos: SeverityDto[] | null = [];
  public activeNamespaces: string[] | null = [];

  public mainTableColumns: TrivyTableColumn[] = [];
  public mainTableOptions: TrivyTableOptions;
  public mainTableExpandTableOptions: TrivyExpandTableOptions;
  public mainTableExpandCallbackDto: VulnerabilityReportImageDto | null = null;
  public isMainTableLoading: boolean = true;

  public detailsTableColumns: TrivyTableColumn[] = [];
  public detailsTableOptions: TrivyTableOptions;

  public isImageUsageDialogVisible: boolean = false;

  @ViewChild('genericMasterDetail', { static: true }) genericMasterDetail!: GenericMasterDetailComponent<VulnerabilityReportImageDto, VulnerabilityReportDetailDto>;

  constructor(private vulnerabilityReportsService: VulnerabilityReportsService, public severityHelperService: SeverityHelperService) {
    vulnerabilityReportsService.getVulnerabilityReportImageDtos()
      .subscribe({
        next: (res) => this.onGetVulnerabilityReportDtos(res),
        error: (err) => console.error(err)
      });
    vulnerabilityReportsService.getVulnerabilityReportActiveNamespaces()
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
        field: "imageName", header: "Image Name - Tag",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "white-space: normal;", renderType: "imageNameTag",
        extraFields: ["imageTag", "imageEosl"],
      },
      {
        field: "criticalCount", header: "Severity C / H / M / L / U",
        isFiltrable: false, isSortable: false, multiSelectType: "none",
        style: "width: 170px; max-width: 170px; ", renderType: "severityMultiTags",
        extraFields: ["highCount", "mediumCount", "lowCount", "unknownCount"],
      },
    ]
    this.mainTableOptions = {
      isClearSelectionVisible: false,
      isExportCsvVisible: false,
      isResetFiltersVisible: true,
      isRefreshVisible: true,
      isRefreshFiltrable: true,
      isFooterVisible: true,
      tableSelectionMode: "single",
      tableStyle: {},
      stateKey: "Vulnerability Reports - Main",
      dataKey: "uid",
      extraClasses: "trivy-half",
    };
    this.detailsTableColumns = [
      {
        field: "severityId", header: "Sev",
        isFiltrable: true, isSortable: true, multiSelectType: "severities",
        style: "width: 90px; max-width: 90px;", renderType: "severityBadge",
      },
      {
        field: "resource", header: "Resource",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 130px; max-width: 130px", renderType: "standard",
      },
      {
        field: "title", header: "Title",
        isFiltrable: true, isSortable: false, multiSelectType: "none",
        style: "min-with: 200px; white-space: normal;", renderType: "standard",
      },
      {
        field: "installedVersion", header: "Installed Ver",
        isFiltrable: true, isSortable: false, multiSelectType: "none",
        style: "width: 120px; max-width: 120px", renderType: "standard",
      },
      {
        field: "fixedVersion", header: "Fixed Ver",
        isFiltrable: true, isSortable: false, multiSelectType: "none",
        style: "width: 120px; max-width: 120px", renderType: "standard",
      },
      {
        field: "publishedDate", header: "Publish",
        isFiltrable: false, isSortable: true, multiSelectType: "none",
        style: "width: 900px; max-width: 90px", renderType: "date",
      },
      {
        field: "lastModifiedDate", header: "Modif",
        isFiltrable: false, isSortable: true, multiSelectType: "none",
        style: "width: 90px; max-width: 90px", renderType: "date",
      },
      {
        field: "score", header: "Score",
        isFiltrable: false, isSortable: true, multiSelectType: "none",
        style: "width: 70px; max-width: 70px", renderType: "standard",
      },
      {
        field: "vulnerabilityId", header: "CVE",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 150px; max-width: 150px;", renderType: "link",
        extraFields: ["primaryLink"],
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
      stateKey: "Vulnerability Reports - Details",
      dataKey: null,
      extraClasses: "trivy-half",
    };
    this.mainTableExpandTableOptions = new TrivyExpandTableOptions(false, 2, 3);
  }

  onGetVulnerabilityReportDtos(vrDtos: VulnerabilityReportImageDto[]) {
    this.vulnerabilityReportDtos = vrDtos;
    this.genericMasterDetail.onGetTDataDtos();
  }

  onGetActiveNamespaces(activeNamespaces: string[]) {
    this.activeNamespaces = activeNamespaces.sort((x, y) => x > y ? 1 : -1);
  }

  onMainTableExpandCallback(dto: VulnerabilityReportImageDto) {
    this.mainTableExpandCallbackDto = dto;
    this.isImageUsageDialogVisible = true;
  }

  public onRefreshRequested(event: TrivyFilterData) {
    let excludedSeverities = this.severityHelperService.getSeverityIds()
      .filter(severityId => !event.selectedSeverityIds.includes(severityId)) || [];

    let params: GetVulnerabilityReportImageDtos$Params = {
      namespaceName: event.namespaceName ?? undefined,
      excludedSeverities: excludedSeverities.length > 0 ? excludedSeverities.join(",") : undefined,
    }
    this.isMainTableLoading = true;
    this.vulnerabilityReportsService.getVulnerabilityReportImageDtos(params)
      .subscribe({
        next: (res) => this.onGetVulnerabilityReportDtos(res),
        error: (err) => console.error(err)
      });
  }

  mainTableExpandCellOptions(dto: VulnerabilityReportImageDto, type: "header" | "row", colIndex: number, rowIndex?: number): TrivyTableCellCustomOptions {
    rowIndex ?? 0;
    let celValue: string = "";
    let celStyle: string = "";
    let celBadge: string | undefined;
    let celButtonLink: string | undefined;

    switch (colIndex) {
      case 0:
        celStyle = "width: 70px; min-width: 70px; height: 50px"
        switch (rowIndex) {
          case 0:
            celValue = "Repository";
            break;
          case 1:
            celValue = "Used By";
            break;
          case 2:
            celValue = "OS Family";
            break;
        }
        break;
      case 1:
        celStyle = "white-space: normal; display: flex; align-items: center; height: 50px;"
        switch (rowIndex) {
          case 0:
            celValue = dto.imageRepository!;
            break;
          case 1:
            let resourceNames: string[] = dto!.resources!.map(x => x.name!);
            let narrowedResourceNames: string;
            let narrowedResourceNamesLink: string | null = null;
            if (resourceNames.length > 2) {
              narrowedResourceNames = resourceNames[0] + ", " + resourceNames[1];
              narrowedResourceNamesLink = " [+" + (resourceNames.length - 2) + "]";
            }
            else {
              narrowedResourceNames = resourceNames.join(", ");
              narrowedResourceNamesLink = "[...]"
            }
            celValue = narrowedResourceNames;
            celButtonLink = narrowedResourceNamesLink;
            break;
          case 2:
            celValue = dto.imageOsFamily! + ' - ' + dto.imageOsName!;
            celBadge = dto.imageEosl ? 'End of Service Life' : undefined;
            break;
        }
        break;
    }

    return {
      value: celValue,
      style: celStyle,
      badge: celBadge,
      buttonLink: celButtonLink,
    }
  }

  getPanelHeaderText() {
    return `Image Usage for ${this.mainTableExpandCallbackDto?.imageName}:${this.mainTableExpandCallbackDto?.imageTag} in namespace ${this.mainTableExpandCallbackDto?.resourceNamespace}`;
  }
}
