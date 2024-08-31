import { SeverityDto } from "../api/models/severity-dto";
import { SeverityHelperService } from "../app/services/severity-helper.service"

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
  }>;
  uid?: string;
}

export class PrimeNgHelper {
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
      severitiesSummary.forEach(severitySummary => {
        let values: number[] = [];
        if (severitySummary.details != null) {

          severitySummary.details
            .sort((a, b) => a.id! - b.id!)
            .forEach(x => { values.push(x.totalCount!); });
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

  public async getDataForHorizontalBarChartByNamespace(severitiesSummary: SeveritiesSummary[]): Promise<PrimeNgHorizontalBarChartData> {
    // TODO make required everything in severitiesDto and severitiesSummaryDto
    let chartData: PrimeNgHorizontalBarChartData = {
      datasets: [],
      labels: [],
      title: 'a title',
    };
    severitiesSummary.forEach(x => { chartData.labels.push(x.namespaceName!); });
    let severities = await this._severityHelperService.getSeverityDtos();
    severities.forEach(severity => {
      let totalVulnerabilities: number[] = [];
      severitiesSummary.forEach(severitySummary => {
        totalVulnerabilities.push(severitySummary.details!.filter(x => x.id! == severity!.id!)[0].totalCount!);
      });
      console.log(`PrimeNgHelper - getDataForHorizontalBarChartByNamespace - ${severity.name}`);
      console.log(`PrimeNgHelper - getDataForHorizontalBarChartByNamespace - ${totalVulnerabilities}`);
      chartData.datasets.push({
        label: severity.name!,
        data: totalVulnerabilities,
        backgroundColor: this._severityHelperService.getCssColor(severity.id!),
        hoverBackgroundColor: this._severityHelperService.getCssColorHover(severity.id!),
        hidden: false,
      });
    });

    return chartData;
  }

  public async getDataForHorizontalBarChartBySeverity(severitiesSummary: SeveritiesSummary[]): Promise<PrimeNgHorizontalBarChartData> {
    // TODO make required everything in severitiesDto and severitiesSummaryDto
    let chartData: PrimeNgHorizontalBarChartData = {
      datasets: [],
      labels: [],
      title: 'a title',
    };
    let severities = await this._severityHelperService.getSeverityDtos();
    severities.forEach(x => { chartData.labels.push(x.name); });
    severitiesSummary.forEach(severitySummary => {
      let totalVulnerabilities: number[] = [];
      severities.forEach(severity => {
        totalVulnerabilities.push(severitySummary.details!.filter(x => x.id! == severity!.id!)[0].totalCount!);
      });

      chartData.datasets.push({
        label: severitySummary.namespaceName!,
        data: totalVulnerabilities,
        backgroundColor: null,
        hoverBackgroundColor: null,
        hidden: false,
      })
    });

    return chartData;
  //  severities.forEach(severity => {
  //    let totalVulnerabilities: number[] = [];
  //    severitiesSummary.forEach(severitySummary => {
  //      totalVulnerabilities.push(severitySummary.details!.filter(x => x.id! == severity!.id!)[0].totalCount!);
  //    });
  //    chartData.datasets.push({
  //      label: severity.name!,
  //      data: totalVulnerabilities,
  //      backgroundColor: null,
  //      hoverBackgroundColor: null,
  //      hidden: false,
  //    });
  //  });
  }
}
