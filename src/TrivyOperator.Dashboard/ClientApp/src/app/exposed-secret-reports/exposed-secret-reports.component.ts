import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GenericMasterDetailComponent } from '../generic-master-detail/generic-master-detail.component'
import { ExposedSecretReportImageDto } from '../../api/models/exposed-secret-report-image-dto'
import { SeverityDto } from '../../api/models/severity-dto';
import { TrivyExpandTableOptions, TrivyFilterData, TrivyTableCellCustomOptions, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';
import { ExposedSecretReportService } from '../../api/services/exposed-secret-report.service'
import { GetExposedSecretReportImageDtos$Params } from '../../api/fn/exposed-secret-report/get-exposed-secret-report-image-dtos'
import { SeverityHelperService } from '../services/severity-helper.service';

import { DialogModule } from 'primeng/dialog';
import { TableModule } from 'primeng/table';


@Component({
  selector: 'app-exposed-secret-reports',
  standalone: true,
  imports: [ CommonModule, GenericMasterDetailComponent, DialogModule, TableModule ],
  templateUrl: './exposed-secret-reports.component.html',
  styleUrl: './exposed-secret-reports.component.scss'
})
export class ExposedSecretReportsComponent {
  public dataDtos: ExposedSecretReportImageDto[] = [];
  public selectedVulnerabilityReportDto: ExposedSecretReportImageDto | null = null;
  public severityDtos: SeverityDto[] | null = [];
  public activeNamespaces: string[] | null = [];

  public mainTableColumns: TrivyTableColumn[] = [];
  public mainTableOptions: TrivyTableOptions;
  public mainTableExpandTableOptions: TrivyExpandTableOptions;
  public mainTableExpandCallbackDto: ExposedSecretReportImageDto | null = null;
  public isMainTableLoading: boolean = true;

  public detailsTableColumns: TrivyTableColumn[] = [];
  public detailsTableOptions: TrivyTableOptions;

  public isImageUsageDialogVisible: boolean = false;

  constructor(private dataDtoService: ExposedSecretReportService, public severityHelperService: SeverityHelperService) {
    dataDtoService.getExposedSecretReportImageDtos()
      .subscribe({
        next: (res) => this.onGetDataDtos(res),
        error: (err) => console.error(err)
      });
    dataDtoService.getExposedSecretReportActiveNamespaces()
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
        field: "criticalCount", header: "Severity C / H / M / L",
        isFiltrable: false, isSortable: false, multiSelectType: "none",
        style: "width: 145px; max-width: 145px; ", renderType: "severityMultiTags",
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
      stateKey: "Exposed Secret Reports - Main",
      dataKey: "uid",
      rowExpansionRender: "table",
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
        field: "ruleId", header: "Id",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 95px; max-width: 95px; white-space: normal;", renderType: "standard",
      },
      {
        field: "match", header: "Match",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 130px; max-width: 130px", renderType: "standard",
      },
      {
        field: "target", header: "Target",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 130px; max-width: 130px", renderType: "standard",
      },
      {
        field: "title", header: "Title",
        isFiltrable: true, isSortable: false, multiSelectType: "none",
        style: "min-with: 200px; white-space: normal;", renderType: "standard",
      },
    ]
    this.detailsTableOptions = {
      isClearSelectionVisible: false,
      isExportCsvVisible: false,
      isResetFiltersVisible: true,
      isRefreshVisible: false,
      isRefreshFiltrable: false,
      isFooterVisible: false,
      tableSelectionMode: null,
      tableStyle: {},
      stateKey: "Exposed Secret Reports - Details",
      dataKey: null,
      rowExpansionRender: null,
      extraClasses: "trivy-half",
    };
    this.mainTableExpandTableOptions = new TrivyExpandTableOptions(false, 2, 2);
  }

  onGetDataDtos(vrDtos: ExposedSecretReportImageDto[]) {
    this.dataDtos = vrDtos;
  }

  onGetActiveNamespaces(activeNamespaces: string[]) {
    this.activeNamespaces = activeNamespaces.sort((x, y) => x > y ? 1 : -1);
  }

  onMainTableExpandCallback(dto: ExposedSecretReportImageDto) {
    this.mainTableExpandCallbackDto = dto;
    this.isImageUsageDialogVisible = true;
  }

  onRefreshRequested(event: TrivyFilterData) {
    let excludedSeverities = this.severityHelperService.getSeverityIds()
      .filter(severityId => !event.selectedSeverityIds.includes(severityId)) || [];

    let params: GetExposedSecretReportImageDtos$Params = {
      namespaceName: event.namespaceName ?? undefined,
      excludedSeverities: excludedSeverities.length > 0 ? excludedSeverities.join(",") : undefined,
    }
    this.isMainTableLoading = true;
    this.dataDtoService.getExposedSecretReportImageDtos(params)
      .subscribe({
        next: (res) => this.onGetDataDtos(res),
        error: (err) => console.error(err)
      });
  }

  mainTableExpandCellOptions(dto: ExposedSecretReportImageDto, type: "header" | "row", colIndex: number, rowIndex?: number): TrivyTableCellCustomOptions {
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
