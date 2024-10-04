import { SeverityDto } from '../../api/models/severity-dto';

export interface TrivyTableOptions {
  isClearSelectionVisible: boolean,
  isResetFiltersVisible: boolean,
  isExportCsvVisible: boolean,
  isRefreshVisible: boolean,
  isRefreshFiltrable: boolean,
  isFooterVisible: boolean,
  tableSelectionMode: null | "single" | "multiple",
  stateKey: string,

  dataKey: string | null,
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
  multiSelectType: "none" | "namespaces" | "severities";
  renderType: "standard" | "severityBadge" | "severityMultiTags" | "imageNameTag" | "link" | "date";
  extraFields?: string[];
}

export interface TrivyFilterData {
  namespaceName?: string | null;
  selectedSeverityIds: number[];
}

export class TrivyDetailsTableOptions {
  isHeaderVisible: boolean = true;
  columnsNo: number = 0;
  rowsNo: number = 0;
  get columnsArray(): number[] { return Array(this.columnsNo).fill(0).map((_, i) => i); };
  get rowsArray(): number[] { return Array(this.rowsNo).fill(0).map((_, i) => i); }
}

export interface TrivyDetailsCell<TData> {
  value: string;
  style: string;
  buttonLink: string | undefined;
  badge: string | undefined;
  //callbackFunction: ((dto: TData) => void) | null;
}
