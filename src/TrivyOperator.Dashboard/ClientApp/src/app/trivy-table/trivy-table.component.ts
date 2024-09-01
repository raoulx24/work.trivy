import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { Table, TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { Column, ExportColumn, TrivyFilterData, TrivyTableColumn, TrivyTableOptions } from "./trivy-table.types";
import { SeverityHelperService } from "../services/severity-helper.service"
import { SeverityDto } from "../../api/models/severity-dto"


@Component({
  selector: 'app-trivy-table',
  standalone: true,
  imports: [CommonModule,
    FormsModule,
    ButtonModule,
    CheckboxModule,
    DropdownModule,
    InputTextModule,
    MultiSelectModule,
    OverlayPanelModule,
    TableModule,
    TagModule, ],
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
  @ViewChild('serverFilterDataOp') serverFilterDataOp?: OverlayPanel;

  @Input() set tableHeight(tableHeight: string) {
    console.log("TrivyTableComponent<TData> - set tableHeight - ", tableHeight);
    this._tableHeight = tableHeight;
    if (this.trivyTable) {
      this.trivyTable.scrollHeight = tableHeight;
      this.isTableVisible = false;
      setTimeout(() => this.isTableVisible = true, 0);
    }
  }
  public get tableHeight(): string {
    return this._tableHeight;
  }
  private _tableHeight: string = "";

  @Input() trivyTableColumns: TrivyTableColumn[] = [];
  public get trivyTableOptions(): TrivyTableOptions { return this._trivyTableOptions!; }
  @Input() set trivyTableOptions(trivyTableOptions: TrivyTableOptions) {
    if (trivyTableOptions != null && trivyTableOptions.tableHeight != "") {
      this.tableHeight = trivyTableOptions.tableHeight;
    }

    this._trivyTableOptions = trivyTableOptions;
  }
  private _trivyTableOptions?: TrivyTableOptions;

  @Output() selectedRowsChanged = new EventEmitter<TData[]>();
  @Output() refreshRequested = new EventEmitter<TrivyFilterData>();

  public selectedDataDtos?: TData[] | null;
  public filterSeverityOptions: number[] = []
  public filterSelectedSeverityIds: number[] | null = [];
  public filterActiveNamespaces: string[] | null = [];
  public filterRefreshActiveNamespace: string | null = "";
  public filterRefreshSeverities: SeverityDto[] = [];

  public get trivyTableTotalRecords(): number {
    return this.dataDtos ? this.dataDtos.length : 0;
  }
  public get trivyTableSelectedRecords(): number {
    if (this.trivyTableOptions?.tableSelectionMode === "single") {
      return this.selectedDataDtos ? 1 : 0;
    }
    else {
      return this.selectedDataDtos ? this.selectedDataDtos.length : 0;
    }
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

  public isTableVisible: boolean = true;
  public severityDtos?: SeverityDto[] | null | undefined;

  onGetSeverities(severityDtos: SeverityDto[]) {
    this.severityDtos = severityDtos;
    severityDtos.forEach((x) => {
      this.filterSeverityOptions.push(x.id);
      if (this.trivyTableOptions.isRefreshFiltrable)
        this.filterRefreshSeverities.push(x);
    })

  }

  public onTableClearSelected() {
    this.selectedDataDtos = [];
  }

  public isTableRowSelected(): boolean {
    return this.selectedDataDtos ? this.selectedDataDtos.length > 0 : false;
  }

  onSelectionChange(event: any): void {
    if (event == null) {
      return;
    }
    if (this.trivyTableOptions?.exposeSelectedRowsEvent) {
      if (this.trivyTableOptions.tableSelectionMode === "single") {
        this.selectedRowsChanged.emit([event]);
      }
      else {
        this.selectedRowsChanged.emit(event);
      }
    }
  }

  onFilterRefresh(event: MouseEvent) {
    if (this.trivyTableOptions.isRefreshFiltrable) {
      this.serverFilterDataOp?.toggle(event)
    }
    else {
      this.onFilterData();
    }
  }

  onFilterData() {
    let event: TrivyFilterData = {
      namespaceName: this.filterRefreshActiveNamespace,
      selectedSeverities: this.filterRefreshSeverities,
    }
    this.serverFilterDataOp?.hide();
    this.refreshRequested.emit(event);
  }

  onRowUnselect(event: any) {
    if (this.trivyTableOptions.tableSelectionMode === "single" && this.trivyTable != null) {
      this.trivyTable.selection = event.data;
    }
  }

  public selectRow(data: TData) {
    if (data == null) {
      return;
    }
    this.selectedDataDtos = [data];
  }
}

// clear filters on reset table: https://stackoverflow.com/questions/51395624/reset-filter-value-on-primeng-table
