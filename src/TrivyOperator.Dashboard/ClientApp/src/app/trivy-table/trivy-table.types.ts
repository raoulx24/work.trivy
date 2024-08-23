export type TrivyTableOptions = {
  isClearSelectionVisible: boolean,
  isResetFiltersVisible: boolean,
  isExportCsvVisible: boolean,
  tableSelectionMode: null | "single" | "multiple"
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
  renderType: "standard" | "severityBadge" | "imageNameTag" | "link";
  extraFields?: string[];
}