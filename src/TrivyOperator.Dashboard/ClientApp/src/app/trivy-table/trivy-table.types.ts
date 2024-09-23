import { SeverityDto } from '../../api/models/severity-dto';

export interface TrivyTableOptions {
  isClearSelectionVisible: boolean,
  isResetFiltersVisible: boolean,
  isExportCsvVisible: boolean,
  isRefreshVisible: boolean,
  isRefreshFiltrable: boolean,
  isFooterVisible: boolean,
  tableSelectionMode: null | "single" | "multiple",
  exposeSelectedRowsEvent: boolean,
  stateKey: string,
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
  renderType: "standard" | "severityBadge" | "severityMultiTags" | "imageNameTag" | "link";
  extraFields?: string[];
}

export interface TrivyFilterData {
  namespaceName?: string | null;
  selectedSeverities: SeverityDto[];
}
