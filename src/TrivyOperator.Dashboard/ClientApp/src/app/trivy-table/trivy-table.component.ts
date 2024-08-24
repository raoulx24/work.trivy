import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { Table, TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { Column, ExportColumn, TrivyTableColumn, TrivyTableOptions } from "./trivy-table.types";
import { SeverityHelperService } from "../services/severity-helper.service"
import { SeverityDto } from "../../api/models/severity-dto"


@Component({
  selector: 'app-trivy-table',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, InputTextModule, MultiSelectModule, OverlayPanelModule, TableModule, TagModule],
  templateUrl: './trivy-table.component.html',
  styleUrl: './trivy-table.component.scss'
})

export class TrivyTableComponent<TData> {
  @Input() dataDtos?: TData[] | null | undefined;
  @Input() activeNamespaces?: string[] | null | undefined = [];

  @Input() csvFileName: string = "Vulnerability.Reports";

  @Input() exportColumns!: ExportColumn[];
  @Input() tableColumns!: Column[];
  @ViewChild('trivyTable') trivyTable?: Table;

  @Input() trivyTableColumns: TrivyTableColumn[] = [];
  @Input() trivyTableOptions?: TrivyTableOptions;

  @Output() selectedRowsChanged = new EventEmitter<TData[]>();

  public selectedDataDtos: TData[] = [];
  public filterSeverityOptions: number[] = []
  public filterSelectedSeverityIds: number[] | null = [];
  public filterActiveNamespaces: string[] | null = [];

  public get trivyTableTotalRecords(): number {
    return this.dataDtos ? this.dataDtos.length : 0;
  }
  public get trivyTableSelectedRecords(): number {
    return this.selectedDataDtos ? this.selectedDataDtos.length : 0;
  }
  public get trivyTableFilteredRecords(): number {
    return this.trivyTable?.filteredValue ? this.trivyTable.filteredValue.length : 0;
  }

  public get severityHelper(): SeverityHelperService {
    return this._severityHelper;
  };
  @Input() set severityHelper(severityHelper: SeverityHelperService) {
    this._severityHelper = severityHelper;
    this._severityHelper.getSeverityDtos().then(result => this.onGetSeverities(result));
    // more on input setter: https://stackoverflow.com/questions/36653678/angular2-input-to-a-property-with-get-set
  }
  private _severityHelper!: SeverityHelperService;

  private severityDtos?: SeverityDto[] | null | undefined;

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

  onSelectionChange(event: TData[]): void {
    console.log('TrivyTable - onSelectionChange - Selected row:', event);
    console.log('TrivyTable - onSelectionChange - Selected products:', this.selectedDataDtos);
    if (this.trivyTableOptions?.exposeSelectedRowsEvent) {
      this.selectedRowsChanged.emit(event);
    }
  }
}

// clear filters on reset table: https://stackoverflow.com/questions/51395624/reset-filter-value-on-primeng-table
