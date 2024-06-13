import {Component} from '@angular/core';
import {ColDef} from "ag-grid-community";
import { VulnerabilityReportsService } from "../../api/services/vulnerability-reports.service";
import { VulnerabilityDto } from "../../api/models/vulnerability-dto";

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
})
export class FetchDataComponent {
  public vulnerabilities: VulnerabilityDto[] = [];
  public columnDefs: ColDef[] = [
    {headerName: 'Date', field: "date", filter: true, flex: 1},
    {headerName: 'Temp. (C)', field: "temperatureC", filter: true, flex: 1},
    {headerName: 'Temp. (F)', field: "temperatureF", filter: true, flex: 1},
    {headerName: 'Summary', field: "summary", filter: true, flex: 1}
  ];

  constructor(vulnerabilityReportsService: VulnerabilityReportsService) {
    vulnerabilityReportsService.getAll$Json().subscribe(result => this.vulnerabilities = result, error => console.error(error))
  }
}
