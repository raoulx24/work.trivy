import { Component, Input, ViewChild } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Table } from 'primeng/table';

import { Column, ExportColumn, TrivyTableColumn, TrivyTableOptions } from "./trivy-table.types";
import { SeverityHelperService } from "../services/severity-helper.service"
import { SeverityDto } from "../../api/models/severity-dto"

@Component({
  selector: 'app-trivy-table',
  standalone: true,
  imports: [],
  templateUrl: './trivy-table.component.html',
  styleUrl: './trivy-table.component.scss'
})
export class TrivyTableComponent<TData> {
  @Input() dataDtos?: TData[] | null | undefined;
  @Input() severityDtos?: SeverityDto[] | null | undefined;
  @Input() activeNamespaces?: string[] | null | undefined = [];

  public filterSeverityOptions: number[] = []
  public filterSelectedSeverityIds: number[] | null = [];
  public filterActiveNamespaces: string[] | null = [];

  public selectedDataDtos!: TData[];
  public metaKey: boolean = false;
  @Input() csvFileName: string = "Vulnerability.Reports";

  public exportCoumns!: ExportColumn[];
  public tableColumns!: Column[];
  public saveCsvMenuItems!: MenuItem[];
  @ViewChild('trivyTable') trivyTable?: Table;

  @Input() trivyTableColumns: TrivyTableColumn[] = [];
  @Input() trivyTableOptions?: TrivyTableOptions;

  public get trivyTableTotalRecords(): number {
    return this.dataDtos ? this.dataDtos.length : 0;
  }
  public get trivyTableSelectedRecords(): number {
    return this.trivyTable?.selection ? this.trivyTable.selection.length : 0;
  }
  public get trivyTableFilteredRecords(): number {
    return this.trivyTable?.filteredValue ? this.trivyTable.filteredValue.length : 0;
  }

  public get severityHelper(): SeverityHelperService {
    return this._severityHelper;
  };
  private set severityHelper(severityHelper: SeverityHelperService) {
    this._severityHelper = severityHelper;
    this._severityHelper.getSeverityDtos().then(result => this.onGetSeverities(result));
  }
  private _severityHelper!: SeverityHelperService;

  onGetSeverities(severityDtos: SeverityDto[]) {
    console.log("trivyTable - onGetSeverities - severityDtos " + severityDtos);
    this.severityDtos = severityDtos;
    severityDtos.forEach((x) => { this.filterSeverityOptions.push(x.id); })
  }

  public onTableClearSelected() {
    this.selectedDataDtos = [];
  }

  public isTableRowSelected(): boolean {
    return this.selectedDataDtos ? this.selectedDataDtos.length > 0 : false;
  }
}
