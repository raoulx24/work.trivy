export interface TrivyTableOptions {
  isClearSelectionVisible: boolean;
  isResetFiltersVisible: boolean;
  isExportCsvVisible: boolean;
  isRefreshVisible: boolean;
  isRefreshFiltrable: boolean;
  isFooterVisible: boolean;
  tableSelectionMode: null | 'single' | 'multiple';
  tableStyle: { [klass: string]: any };
  stateKey: string | null;
  dataKey: string | null;
  rowExpansionRender: null | 'table' | 'messages';
  extraClasses: string;
}

export interface Column {
  field: string;
  header: string;
  customExportHeader?: string;
}

export interface ExportColumn {
  title: string;
  dataKey: string;
}

export interface TrivyTableColumn extends Column {
  isSortable: boolean;
  isFiltrable: boolean;
  style: string;
  multiSelectType: 'none' | 'namespaces' | 'severities';
  renderType:
    | 'standard'
    | 'severityBadge'
    | 'severityMultiTags'
    | 'imageNameTag'
    | 'link'
    | 'date'
    | 'eosl'
    | 'semaphore'
    | 'multiline';
  extraFields?: string[];
}

export interface TrivyFilterData {
  namespaceName?: string | null;
  selectedSeverityIds: number[];
}

export class TrivyExpandTableOptions {
  isHeaderVisible: boolean = true;
  columnsNo: number = 0;
  rowsNo: number = 0;

  constructor(isHeaderVisible: boolean, columnsNo: number, rowsNo: number) {
    this.isHeaderVisible = isHeaderVisible;
    this.columnsNo = columnsNo;
    this.rowsNo = rowsNo;
  }

  get columnsArray(): number[] {
    return Array(this.columnsNo)
      .fill(0)
      .map((_, i) => i);
  }

  // or return Array.from({ length: this.columnsNo }, (_, i) => i);

  get rowsArray(): number[] {
    return Array(this.rowsNo)
      .fill(0)
      .map((_, i) => i);
  }
}

export interface TrivyTableCellCustomOptions {
  value: string;
  style: string;
  buttonLink: string | undefined;
  badge: string | undefined;
  url: string | undefined;
}
