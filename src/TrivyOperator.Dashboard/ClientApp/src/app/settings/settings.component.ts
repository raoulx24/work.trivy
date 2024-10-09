import { CommonModule } from '@angular/common';
import { Component, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { PanelModule } from 'primeng/panel';
import { TableModule } from 'primeng/table';

import { ClearTablesOptions, SavedCsvFileName } from './settings.types'
import { PrimengTableStateUtil } from '../utils/primeng-table-state.util'
import { LocalStorageUtils } from '../utils/local-storage.utils'

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, CardModule, CheckboxModule, InputTextModule, PanelModule, TableModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent {
  public clearTablesOptions: ClearTablesOptions[] = [];
  public csvFileNames: SavedCsvFileName[] = [];


  constructor() {
    this.clearTablesOptions = LocalStorageUtils
      .getKeysWithPrefix(LocalStorageUtils.trivyTableKeyPrefix)
      .map(x => {
        return new ClearTablesOptions(x, x.slice(LocalStorageUtils.trivyTableKeyPrefix.length))
      })

    this.csvFileNames = LocalStorageUtils
      .getKeysWithPrefix(LocalStorageUtils.csvFileNameKeyPrefix)
      .map(x => {
        return {
          dataKey: x,
          description: x.slice(LocalStorageUtils.csvFileNameKeyPrefix.length),
          savedCsvName: localStorage.getItem(x) ?? "",
        }
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

  onupdateCsvFileNames(event: MouseEvent) {
    this.csvFileNames.forEach(x => {
      localStorage.setItem(x.dataKey, x.savedCsvName);
    })
  }
}
