export interface KnownTables {
  dataKey: string,
  description: string,
}

export class ClearTablesOptions {
  dataKey: string = "";
  description: string = "";
  filters: boolean = false;
  sort: boolean = false;
  columnWidths: boolean = false;
  columnOrder: boolean = false;
  all: boolean = false;

  constructor(knownTable: KnownTables) {
    this.dataKey = knownTable.dataKey;
    this.description = knownTable.description;
  }
}
