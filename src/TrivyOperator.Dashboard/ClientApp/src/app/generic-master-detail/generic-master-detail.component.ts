import { Component, EventEmitter, Input, Output, ViewChild, input } from '@angular/core';

import { SeverityDto } from '../../api/models/severity-dto';
import { TrivyTableComponent } from '../trivy-table/trivy-table.component';
import { TrivyExpandTableOptions, TrivyFilterData, TrivyTableCellCustomOptions, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';


export interface IMasterDetail<TDetailDto> {
  details?: Array<TDetailDto> | null;
}

@Component({
  selector: 'app-generic-master-detail',
  standalone: true,
  imports: [TrivyTableComponent],
  templateUrl: './generic-master-detail.component.html',
  styleUrl: './generic-master-detail.component.scss'
})
export class GenericMasterDetailComponent<TDataDto extends IMasterDetail<TDetailDto>, TDetailDto> {
  /*@Input() dataDtos: TDataDto[] = [];*/
  @Input() set dataDtos(dataDtos: TDataDto[]) {
    this._dataDtos = dataDtos;
    this.onGetTDataDtos();
  }
  get dataDtos(): TDataDto[] | null { return this._dataDtos; }
  @Input() severityDtos: SeverityDto[] | null = [];
  @Input() activeNamespaces: string[] | null = [];

  @Input() mainTableColumns: TrivyTableColumn[] = [];
  @Input( { required: true } ) mainTableOptions!: TrivyTableOptions;
  @Input() mainTableExpandTableOptions: TrivyExpandTableOptions = new TrivyExpandTableOptions(false, 0, 0);
  @Input() public mainTableExpandCellOptions: (dto: TDataDto, type: "header" | "row", column: number, row?: number) => TrivyTableCellCustomOptions =
    (_dto, _type, _column, _row) => ({ value: "", style: "", buttonLink: undefined, badge: undefined });
  
  @Input() public isMainTableLoading: boolean = true;
  @ViewChild('mainTable', { static: true }) mainTable!: TrivyTableComponent<TDataDto>;

  @Input() public detailsTableColumns: TrivyTableColumn[] = [];
  @Input( { required: true } ) public detailsTableOptions!: TrivyTableOptions;

  @Output() refreshRequested = new EventEmitter<TrivyFilterData>();
  @Output() public mainTableExpandCallback = new EventEmitter<TDataDto>();

  selectedDataDto: TDataDto | null = null;

  private _dataDtos: TDataDto[] | null = [];

  onGetTDataDtos() {
    this.mainTable.onTableClearSelected();
    this.selectedDataDto = null;
    this.isMainTableLoading = false;
  }

  onMainTableSelectionChange(event: TDataDto[]) {
    if (event == null || event.length == 0) {
      this.selectedDataDto = null;
      return;
    }
    else {
      this.selectedDataDto = event[0];
    }
  }

  onRefreshRequested(event: TrivyFilterData) {
    this.refreshRequested.emit(event);
  }

  onMainTableExpandCallback(event: TDataDto) {
    this.mainTableExpandCallback.emit(event);
  } 
}
