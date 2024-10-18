import { Component, Input, Output, ViewChild, input } from '@angular/core';

import { SeverityDto } from '../../api/models/severity-dto';
import { TrivyTableComponent } from '../trivy-table/trivy-table.component';
import { TrivyExpandTableOptions, TrivyFilterData, TrivyTableCellCustomOptions, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';


export interface IMasterDetail<TDetailDto> {
  details?: TDetailDto[];
}

@Component({
  selector: 'app-generic-master-detail',
  standalone: true,
  imports: [TrivyTableComponent],
  templateUrl: './generic-master-detail.component.html',
  styleUrl: './generic-master-detail.component.scss'
})
export class GenericMasterDetailComponent<TDataDto extends IMasterDetail<TDetailDto>, TDetailDto> {
  @Input() dataDtos: TDataDto[] = [];
  @Input() selectedDataDto: TDataDto | null = null;
  @Input() severityDtos: SeverityDto[] | null = [];
  @Input() activeNamespaces: string[] | null = [];

  @Input() mainTableColumns: TrivyTableColumn[] = [];
  @Input( { required: true } ) mainTableOptions!: TrivyTableOptions;
  @Input() mainTableExpandTableOptions: TrivyExpandTableOptions = new TrivyExpandTableOptions(false, 0, 0);
  @Input() public mainTableExpandCellOptions: (dto: TDataDto, type: "header" | "row", column: number, row?: number) => TrivyTableCellCustomOptions =
    (_dto, _type, _column, _row) => ({ value: "", style: "", buttonLink: undefined, badge: undefined });
  @Input() public mainTableExpandCallback: (dto: TDataDto) => void = (_dto) => { };
  
  @Input() public isMainTableLoading: boolean = true;
  @ViewChild('mainTable', { static: true }) mainTable!: TrivyTableComponent<TDataDto>;

  @Input() public detailsTableColumns: TrivyTableColumn[] = [];
  @Input( { required: true } ) public detailsTableOptions!: TrivyTableOptions;

  @Input() public onRefreshRequested: (event: TrivyFilterData) => void = (_event) => { };
  

  @Output() public mainTableExpandCallbackDto: TDataDto | null | undefined;

  onMainTableSelectionChange(event: TDataDto[]) {
    if (event == null || event.length == 0) {
      this.selectedDataDto = null;
      return;
    }
    else {
      this.selectedDataDto = event[0];
    }
  }
}
