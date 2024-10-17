import { Component, Input, Output, ViewChild } from '@angular/core';

import { SeverityDto } from '../../api/models/severity-dto';
import { TrivyTableComponent } from '../trivy-table/trivy-table.component';
import { TrivyExpandTableOptions, TrivyTableColumn, TrivyTableOptions } from '../trivy-table/trivy-table.types';


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
export class GenericMasterDetailComponent<TData extends IMasterDetail<TDetailDto>, TDetailDto> {
  @Input() public dataDtos: TData[] = [];
  @Input() public selectedDataDto: TData | null | undefined;
  @Input() public severityDtos?: SeverityDto[] | null | undefined;
  @Input() public activeNamespaces?: string[] | null | undefined = [];

  @Input() public mainTableColumns: TrivyTableColumn[] = [];
  @Input() public mainTableOptions: TrivyTableOptions;
  @Input() public mainTableExpandTableOptions: TrivyExpandTableOptions;
  
  @Input() public isMainTableLoading: boolean = true;
  @ViewChild('mainTable', { static: true }) mainTable!: TrivyTableComponent<TData>;

  @Input() public detailsTableColumns: TrivyTableColumn[] = [];
  @Input() public detailsTableOptions: TrivyTableOptions;

  @Output() public mainTableExpandCallbackDto: TData | null | undefined;
}
