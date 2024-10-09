import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { Table, TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { Column, ExportColumn, TrivyFilterData, TrivyTableColumn, TrivyTableOptions, TrivyExpandTableOptions, TrivyTableCellCustomOptions } from "./trivy-table.types";
import { SeverityHelperService } from "../services/severity-helper.service"
import { SemaphoreStatusHelperService } from "../services/semaphore-status-helper.service"
import { SeverityDto } from "../../api/models/severity-dto"
import { PrimengTableStateUtil } from "../utils/primeng-table-state.util"
import { LocalStorageUtils } from '../utils/local-storage.utils'


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

export class TrivyTableComponent<TData> implements OnInit {
  @Input() dataDtos?: TData[] | null | undefined;
  @Input() activeNamespaces?: string[] | null | undefined = [];

  @Input() csvStoragekey: string = "default";
  @Input() csvFileName: string = "Default.csv.FileName";

  @Input() exportColumns!: ExportColumn[];
  @Input() tableColumns!: Column[];
  @ViewChild('trivyTable') trivyTable!: Table;
  @ViewChild('serverFilterDataOp') serverFilterDataOp?: OverlayPanel;

  @Input() public tableHeight: string = "10vh";
  @Input() public isLoading: boolean = false;

  @Input() trivyTableColumns: TrivyTableColumn[] = [];
  @Input( { required: true } ) trivyTableOptions!: TrivyTableOptions;

  @Input() trivyExpandTableOptions: TrivyExpandTableOptions = new TrivyExpandTableOptions(false, 0, 0);
  @Input() trivyExpandTableFunction: (dto: TData, type: "header" | "row", column: number, row?: number) => TrivyTableCellCustomOptions =
    (dto, type, column, row) => ({ value: "", style: "", buttonLink: undefined, badge: undefined });
  @Output() trivyDetailsTableCallback = new EventEmitter<TData>();

  @Output() selectedRowsChanged = new EventEmitter<TData[]>();
  @Output() refreshRequested = new EventEmitter<TrivyFilterData>();

  tableStateKey: string | undefined = undefined;

  public selectedDataDtos?: any | null;
  public filterSeverityOptions: number[] = []
  public filterSelectedSeverityIds: number[] | null = [];
  public filterSelectedActiveNamespaces: string[] | null = [];
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

  constructor(public severityHelper: SeverityHelperService, public semaphoreStatusHelper: SemaphoreStatusHelperService) {
    severityHelper.getSeverityDtos().then(result => this.onGetSeverities(result));
  }

  ngOnInit() {
    let savedCsvFileName = localStorage.getItem(LocalStorageUtils.csvFileNameKeyPrefix + this.csvStoragekey);
    if (savedCsvFileName) {
      this.csvFileName = savedCsvFileName;
    }
    this.tableStateKey = LocalStorageUtils.trivyTableKeyPrefix + this.trivyTableOptions.stateKey;
  }

  onGetSeverities(severityDtos: SeverityDto[]) {
    this.severityDtos = severityDtos;
    this.filterSeverityOptions = severityDtos.map(x => x.id);
    this.filterRefreshSeverities = [...severityDtos];
  }

  public onTableClearSelected() {
    this.selectedDataDtos = null;
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

  // custom back overlay
  public overlayVisible: boolean = false;

  public onOverlayToogle() {
    this.overlayVisible = !this.overlayVisible;
  }

  public isTableFilteredOrSorted(): boolean {
    if (!this.trivyTable || this.isLoading) {
      return false;
    }
    return (this.trivyTable.filteredValue ? true : false) ||
      (this.trivyTable.multiSortMeta == null ? false : this.trivyTable.multiSortMeta.length > 0);
  }
  // there is an NG0100 error from here

  public onClearSortFilters() {
    let currentFilters = JSON.parse(JSON.stringify(this.trivyTable.filters));
    PrimengTableStateUtil.clearFilters(this.trivyTable.filters);
    this.trivyTable.clear();
    this.filterSelectedActiveNamespaces = [];
    this.filterSelectedSeverityIds = [];
    if (this.trivyTableOptions.stateKey) {
      let tableState = localStorage.getItem(this.trivyTableOptions.stateKey);
      if (!tableState) {
        return;
      }
      let tableStateJson = JSON.parse(tableState);
      PrimengTableStateUtil.clearTableFilters(tableStateJson);
      PrimengTableStateUtil.clearTableMultiSort(tableStateJson);
      localStorage.setItem(this.trivyTableOptions.stateKey, JSON.stringify(tableStateJson));
    }
  }

  //rows expand
  expandedRows = {};

  onTrivyDetailsTableCallback(dto: TData) {
    this.trivyDetailsTableCallback.emit(dto);
  }

  onTableCollapseAll() {
    this.expandedRows = {};
    if (this.trivyTableOptions.stateKey) {
      let tableState = localStorage.getItem(this.trivyTableOptions.stateKey);
      if (!tableState) {
        return;
      }

      let tableStateJson = JSON.parse(tableState);
      if (tableStateJson.hasOwnProperty('expandedRowKeys')) {
        delete tableStateJson.expandedRowKeys;
      }
      localStorage.setItem(this.trivyTableOptions.stateKey, JSON.stringify(tableStateJson));
    }
  }

  isAnyRowExpanded(): boolean {
    return JSON.stringify(this.expandedRows) != '{}'
  }

  onExportToCsv(exportType: string) {
    localStorage.setItem(LocalStorageUtils.csvFileNameKeyPrefix + this.csvStoragekey, this.csvFileName);
    switch (exportType) {
      case "all":
        this.trivyTable.exportCSV({ allValues: true });
        break;
      case "filtered":
        this.trivyTable.exportCSV();
        break;
    }
  }
}



// clear filters on reset table: https://stackoverflow.com/questions/51395624/reset-filter-value-on-primeng-table
