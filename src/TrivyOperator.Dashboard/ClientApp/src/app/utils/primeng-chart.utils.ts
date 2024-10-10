import { SeverityHelperService } from '../services/severity-helper.service'
import { ColorHelper } from './color.utils'

export type PrimeNgPieChartData = {
  labels: string[],
  datasets: Array<{
    data: number[],
    backgroundColor: string[],
    hoverBackgroundColor: string[],
  }>,
  title: string;
}

export type PrimeNgHorizontalBarChartData = {
  labels: string[],
  datasets: Array<{
    label: string | null,
    data: number[],
    backgroundColor: string | null,
    hoverBackgroundColor: string | null,
    hidden: boolean,
  }>,
  title: string;
}

export interface SeveritiesSummary {
  namespaceName?: string | null;
  details?: Array<{
    id?: number;
    totalCount?: number;
    distinctCount?: number;
  }>;
  uid?: string;
  isTotal?: boolean;
}

export class PrimeNgChartUtils {
  private _severityHelperService: SeverityHelperService;

  constructor(severityHelperService: SeverityHelperService) {
    this._severityHelperService = severityHelperService;
  }

  public getDataForPieChart(severitiesSummary: SeveritiesSummary[], title: string = 'generic'): PrimeNgPieChartData[] {
    let pieChartData: PrimeNgPieChartData[] = [];
    this._severityHelperService.getSeverityDtos().then(severityDtos => {
      let severityLabels: string[] = [];
      let cssColors: string[] = [];
      let cssColorHovers: string[] = [];
      severityDtos.forEach(x => {
        severityLabels.push(x.name);
        cssColors.push(this._severityHelperService.getCssColor(x.id));
        cssColorHovers.push(this._severityHelperService.getCssColorHover(x.id));
      });
      severitiesSummary.filter(x => !x.isTotal).forEach(severitySummary => {
        let values: number[] = [];
        if (severitySummary.details != null) {

          severitySummary.details
            .sort((a, b) => a.id! - b.id!)
            .forEach(x => { values.push(x.distinctCount!); });
        }
        let chartData: PrimeNgPieChartData = {
          labels: severityLabels,
          datasets: [
            {
              data: values,
              backgroundColor: cssColors,
              hoverBackgroundColor: cssColorHovers,
            }
          ],
          title: severitySummary.namespaceName ? severitySummary.namespaceName : title,
        };
        pieChartData.push(chartData);
      });
    });

    return pieChartData;
  }

  public async getDataForHorizontalBarChartByNamespace(severitiesSummary: SeveritiesSummary[], distinct: boolean): Promise<PrimeNgHorizontalBarChartData> {
    // TODO make required everything in severitiesDto and severitiesSummaryDto
    let chartData: PrimeNgHorizontalBarChartData = {
      datasets: [],
      labels: [],
      title: 'a title',
    };
    severitiesSummary.filter(x => !x.isTotal).forEach(x => { chartData.labels.push(x.namespaceName!); });
    let severities = await this._severityHelperService.getSeverityDtos();
    severities.forEach(severity => {
      let totalVulnerabilities: number[] = [];
      severitiesSummary.filter(x => !x.isTotal).forEach(severitySummary => {
        let severityDetail = severitySummary.details!.filter(x => x.id! == severity!.id!)[0];
        totalVulnerabilities.push(distinct ? severityDetail.distinctCount! : severityDetail.totalCount!);
      });
      chartData.datasets.push({
        label: this._severityHelperService.getCapitalizedString(severity.name!),
        data: totalVulnerabilities,
        backgroundColor: this._severityHelperService.getCssColor(severity.id!),
        hoverBackgroundColor: this._severityHelperService.getCssColorHover(severity.id!),
        hidden: false,
      });
    });

    return chartData;
  }

  public async getDataForHorizontalBarChartBySeverity(severitiesSummary: SeveritiesSummary[], distinct: boolean): Promise<PrimeNgHorizontalBarChartData> {
    // TODO make required everything in severitiesDto and severitiesSummaryDto
    let chartData: PrimeNgHorizontalBarChartData = {
      datasets: [],
      labels: [],
      title: 'a title',
    };
    let severities = await this._severityHelperService.getSeverityDtos();
    let namespacesCounter: number = 0;
    severities.forEach(x => { chartData.labels.push(this._severityHelperService.getCapitalizedString(x.name)); });
    severitiesSummary.filter(x => !x.isTotal).forEach(severitySummary => {
      let totalVulnerabilities: number[] = [];
      severities.forEach(severity => {
        let severityDetail = severitySummary.details!.filter(x => x.id! == severity!.id!)[0];
        totalVulnerabilities.push(distinct ? severityDetail.distinctCount! : severityDetail.totalCount!);
      });
      let color: string = ColorHelper.rainbow(severitiesSummary.length, namespacesCounter);
      chartData.datasets.push({
        label: severitySummary.namespaceName!,
        data: totalVulnerabilities,
        backgroundColor: color,
        hoverBackgroundColor: color,
        hidden: false,
      });
      namespacesCounter++;
    });

    return chartData;
  }
}
