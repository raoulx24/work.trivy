import {Component} from '@angular/core';
import {ColDef, CsvExportParams, GridApi, GridReadyEvent, ValueGetterParams,} from "ag-grid-community";
import {VulnerabilityReportsService} from "../../api/services/vulnerability-reports.service";
import {VulnerabilityDto} from "../../api/models/vulnerability-dto";
import {VulnerabilitySeverityRenderer} from "./vulnerability-severity-renderer.component";

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  styleUrl: './fetch-data.component.css',
})

export class FetchDataComponent {
  public rowSelection: "single" | "multiple" = "multiple";
  public vulnerabilities?: VulnerabilityDto[] | null | undefined;
  public columnDefs: ColDef[] = [
    {headerName: 'Namespace', field: "namespace", filter: true, flex: 3},
    {headerName: 'Container', field: "containerName", filter: true, flex: 3},
    {
      headerName: 'Image Name and Tag', field: "imageName", filter: true, flex: 9,
      valueGetter: (params: ValueGetterParams) =>
        params.data.imageName + ":" + params.data.imageTag,
    },
    {
      headerName: 'S',
      field: "severity",
      filter: true,
      flex: 1,
      minWidth: 60,
      cellRenderer: VulnerabilitySeverityRenderer
    },
    {headerName: 'Title', field: "title", filter: true, flex: 15, wrapText: true, autoHeight: true},
    {
      headerName: 'CVE', field: "vulnerabilityId", filter: true, flex: 6,
      cellRenderer: (params: ValueGetterParams) => (`<a href="${params.data.primaryLink}" target="_blank">${params.data.vulnerabilityId}</a>`)
    },
  ];
  public defaultColDef: ColDef = {
    floatingFilter: true,
    suppressFloatingFilterButton: true,
  };
  private gridExportParams: CsvExportParams = {columnSeparator: ",", onlySelected: true};
  private gridApi!: GridApi;

  constructor(vulnerabilityReportsService: VulnerabilityReportsService) {
    vulnerabilityReportsService.getAll$Json().subscribe(result => this.vulnerabilities = result, error => console.error(error))
  }

  onBtnExport() {
    this.gridApi.exportDataAsCsv(this.gridExportParams);
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }
}
