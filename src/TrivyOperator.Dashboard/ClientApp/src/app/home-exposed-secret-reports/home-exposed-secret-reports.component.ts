import { Component, Input } from '@angular/core';

import { ExposedSecretReportService } from '../../api/services/exposed-secret-report.service'
import { EsSeveritiesByNsSummaryDto } from '../../api/models/es-severities-by-ns-summary-dto'

import { SeverityUtils } from '../utils/severity.utils';

import { ButtonModule } from 'primeng/button';
import { CarouselModule } from 'primeng/carousel';
import { ChartModule } from 'primeng/chart';
import { DialogModule } from 'primeng/dialog';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';



@Component({
  selector: 'app-home-exposed-secret-reports',
  standalone: true,
  imports: [ButtonModule, CarouselModule, ChartModule, DialogModule, TableModule, TagModule],
  templateUrl: './home-exposed-secret-reports.component.html',
  styleUrl: './home-exposed-secret-reports.component.scss'
})
export class HomeExposedSecretReportsComponent {
  @Input() set showDistinctValues(value: boolean) {
    this.localShowDistinctValues = value;
    this.onDistinctSwitch();
  }
  get showDistinctValues(): boolean {
    return this.localShowDistinctValues;
  }

  exposedSecretReportSummaryDtos: EsSeveritiesByNsSummaryDto[] = [];

  private localShowDistinctValues: boolean = true;

  constructor(private exposedSecretReportService: ExposedSecretReportService) {
    console.log("constructor - esr");
    this.exposedSecretReportService.getExposedSecretReportSummaryDtos()
      .subscribe({
        next: (res) => this.onDtos(res),
        error: (err) => console.error(err)
      });
  }

  private onDtos(dtos: EsSeveritiesByNsSummaryDto[]) {
    this.exposedSecretReportSummaryDtos = dtos;
  }

  private onDistinctSwitch() {

  }

  onEsrMore(_event: MouseEvent) {

  }

  public severitiesForTable(): number[] {
    const summary = this.exposedSecretReportSummaryDtos.find(x => x.isTotal);
    console.log(summary?.details?.map(x => x.id ?? 0));
    return summary?.details?.map(x => x.id ?? 0) ?? [];
  }
}
