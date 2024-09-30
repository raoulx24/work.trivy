import { Component } from '@angular/core';

import { WatcherStateInfoService } from '../../api/services/watcher-state-info.service';
import { WatcherStateInfoDto } from '../../api/models/watcher-state-info-dto'

import { TrivyTableComponent } from '../trivy-table/trivy-table.component';
import { ExportColumn, Column, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';

@Component({
  selector: 'app-watcher-state',
  standalone: true,
  imports: [ TrivyTableComponent ],
  templateUrl: './watcher-state.component.html',
  styleUrl: './watcher-state.component.scss',
  
})

export class WatcherStateComponent {
  public watcherStateInfoDtos: WatcherStateInfoDto[] = [];
  public isLoading: boolean = false;
  public exportColumns!: ExportColumn[];
  public tableColumns!: Column[];

  public trivyTableColumns: TrivyTableColumn[] = [];
  public trivyTableOptions: TrivyTableOptions;

  constructor(private watcherStateInfoService: WatcherStateInfoService) {
    this.getTableDataDtos();

    this.tableColumns = [
      { field: 'kubernetesObjectType', header: 'k8s Object' },
      { field: 'namespaceName', header: 'NS' },
      { field: 'status', header: 'Status' },
      { field: 'mitigationMessage', header: 'Mitigation' },
      { field: 'lastException', header: 'Last Exception' },
    ];

    this.exportColumns = [];

    this.trivyTableColumns = [
      {
        field: "kubernetesObjectType", header: "k8s Object",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 130px; max-width: 200px;", renderType: "standard",
      },
      {
        field: "namespaceName", header: "NS",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 130px; max-width: 130px;", renderType: "standard",
      },
      {
        field: "status", header: "Status",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 130px; max-width: 130px;", renderType: "standard",
      },
      {
        field: "mitigationMessage", header: "Mitigation",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 330px; max-width: 330px; white-space: normal;", renderType: "standard",
      },
      {
        field: "lastException", header: "Last Exception",
        isFiltrable: true, isSortable: true, multiSelectType: "none",
        style: "width: 330px; max-width: 330px; white-space: normal;", renderType: "standard",
      },
    ]

    this.trivyTableOptions = {
      isClearSelectionVisible: false,
      isExportCsvVisible: false,
      isResetFiltersVisible: true,
      isRefreshVisible: true,
      isRefreshFiltrable: false,
      isFooterVisible: true,
      tableSelectionMode: null,
      stateKey: 'ws.table-all',
    };
  };

  public getTableDataDtos() {
    this.isLoading = true;
    this.watcherStateInfoService.getWatcherStateInfos()
      .subscribe({
        next: (res) => this.onGetWatcherStateInfos(res),
        error: (err) => console.error(err)
      });
  }

  onGetWatcherStateInfos(dtos: WatcherStateInfoDto[]) {
    this.watcherStateInfoDtos = dtos;
    this.isLoading = false;
  }
}
