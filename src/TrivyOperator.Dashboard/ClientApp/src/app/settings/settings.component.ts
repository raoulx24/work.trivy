import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';

import { ClearTablesOptions, KnownTables } from './settings.types'
import { PrimengTableStateUtil } from '../utils/primeng-table-state.util'

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [FormsModule, ButtonModule, CheckboxModule, InputTextModule, PanelModule, TableModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent {
  private knownTables: KnownTables[] = [];
  public clearTablesOptions: ClearTablesOptions[] = [];

  constructor() {
    this.knownTables = [
      { dataKey: 'vr.table-main', description: 'Vulnerability Reports - Main' },
      { dataKey: 'vr.table-details', description: 'Vulnerability Reports - Details' },
      { dataKey: 'vrd.table-all', description: 'Vulnerability Reports Detailed' },
      { dataKey: 'ws.table-all', description: 'Watcher States' },
    ];

    this.clearTablesOptions = this.knownTables.map(x => {
      return new ClearTablesOptions(x);
    });
  }

  onClearTableStatesSelected(event: MouseEvent) {
    this.clearTablesOptions.forEach(option => {
      let tableStateJson = localStorage.getItem(option.dataKey);
      if (tableStateJson) {
        if (option.all) {
          localStorage.removeItem(option.dataKey);
        }
        else {
          let tableState = JSON.parse(tableStateJson);
          if (option.filters) {
            PrimengTableStateUtil.clearTableFilters(tableState);
          }
          if (option.sort) {
            PrimengTableStateUtil.clearTableMultiSort(tableState);
          }
          if (option.columnWidths) {
            PrimengTableStateUtil.clearTableColumnWidths(tableState);
          }
          if (option.columnOrder) {
            PrimengTableStateUtil.clearTableColumnOrder(tableState);
          }
          localStorage.setItem(option.dataKey, JSON.stringify(tableState));
        }
      }
    });
  }

  onClearTableStatesAll(event: MouseEvent) {
    this.clearTablesOptions.forEach(option => {
      let tableStateJson = localStorage.getItem(option.dataKey);
      if (tableStateJson) {
        localStorage.removeItem(option.dataKey);
      }
    });
  }
}