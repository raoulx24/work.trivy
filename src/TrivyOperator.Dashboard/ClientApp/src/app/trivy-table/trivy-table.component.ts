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

import { Column, ExportColumn, TrivyFilterData, TrivyTableColumn, TrivyTableOptions, TrivyDetailsTableOptions } from "./trivy-table.types";
import { SeverityHelperService } from "../services/severity-helper.service"
import { SeverityDto } from "../../api/models/severity-dto"
import { TableState } from 'primeng/api';


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

  @Input() public tableHeight: string = "10vh";
  @Input() public isLoading: boolean = false;

  @Input() trivyTableColumns: TrivyTableColumn[] = [];
  @Input( { required: true } ) trivyTableOptions!: TrivyTableOptions;

  @Input() trivyDetailsTableOptions: TrivyDetailsTableOptions = new TrivyDetailsTableOptions();
  @Input() detailsFunction: (dto: TData, type: "header" | "row", column: number, row?: number) => string = () => 'Default value';

  @Output() selectedRowsChanged = new EventEmitter<TData[]>();
  @Output() refreshRequested = new EventEmitter<TrivyFilterData>();

  

  public selectedDataDtos?: any | null;
  public filterSeverityOptions: number[] = []
  public filterSelectedSeverityIds: number[] | null = [];
  public filterActiveNamespaces: string[] | null = [];
  public filterRefreshActiveNamespace: string | null = "";
  public filterRefreshSeverities: SeverityDto[] | undefined;

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
    return this.trivyTable?.filteredValue ? this.trivyTable.filteredValue.length : this.trivyTableTotalRecords;
  }

  public isTableVisible: boolean = true;
  public severityDtos?: SeverityDto[] | null | undefined;

  constructor(public severityHelper: SeverityHelperService) {
    severityHelper.getSeverityDtos().then(result => this.onGetSeverities(result));
  }

  onGetSeverities(severityDtos: SeverityDto[]) {
    this.severityDtos = severityDtos;
    this.filterSeverityOptions = severityDtos.map(x => x.id);
    this.filterRefreshSeverities = [...severityDtos];
  }

  public onTableClearSelected() {
    this.selectedDataDtos = [];
  }

  public isTableRowSelected(): boolean {
    return this.selectedDataDtos ? this.selectedDataDtos.length > 0 : false;
  }

  onSelectionChange(event: any): void {
    if (!event) {
      this.selectedRowsChanged.emit([]);
      return;
    }
    if (this.trivyTableOptions.tableSelectionMode === "single") {
      this.selectedRowsChanged.emit([event]);
    }
    else {
      this.selectedRowsChanged.emit(event);
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
      selectedSeverityIds: this.filterRefreshSeverities?.map(x => x.id) ?? [],
    }
    this.serverFilterDataOp?.hide();
    this.refreshRequested.emit(event);
  }

  //onRowUnselect(event: any) {
  //  // don't let unselect
  //  if (this.trivyTableOptions.tableSelectionMode === "single" && this.trivyTable != null) {
  //    this.trivyTable.selection = event.data;
  //  }
  //}

  //public selectRow(data: TData) {
  //  if (data == null) {
  //    return;
  //  }
  //  this.selectedDataDtos = data;
  //  this.onSelectionChange(data);
  //}

  // custom back overlay
  public overlayVisible: boolean = false;

  public onOverlayToogle() {
    this.overlayVisible = !this.overlayVisible;
  }

  //tests expand
  expandedRows = {};
}



// clear filters on reset table: https://stackoverflow.com/questions/51395624/reset-filter-value-on-primeng-table
