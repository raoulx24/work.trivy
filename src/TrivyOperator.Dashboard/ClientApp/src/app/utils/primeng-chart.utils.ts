import { SeverityUtils } from './severity.utils'
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
  public getDataForPieChart(severitiesSummary: SeveritiesSummary[], title: string = 'generic'): PrimeNgPieChartData[] {
    let pieChartData: PrimeNgPieChartData[] = [];
    const severityDtos = SeverityUtils.severityDtos;
    let severityLabels: string[] = [];
    let cssColors: string[] = [];
    let cssColorHovers: string[] = [];
    severityDtos.forEach(x => {
      severityLabels.push(x.name);
      cssColors.push(SeverityUtils.getCssColor(x.id));
      cssColorHovers.push(SeverityUtils.getCssColorHover(x.id));
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

    return pieChartData;
  }

  public static getDataForHorizontalBarChartByNamespace(severitiesSummary: SeveritiesSummary[], distinct: boolean): PrimeNgHorizontalBarChartData {
    // TODO make required everything in severitiesDto and severitiesSummaryDto
    let chartData: PrimeNgHorizontalBarChartData = {
      datasets: [],
      labels: [],
      title: 'a title',
    };
    severitiesSummary.filter(x => !x.isTotal).forEach(x => { chartData.labels.push(x.namespaceName!); });
    const severities = severitiesSummary[0].details!.map(x => x.id);
    severities.forEach(severity => {
      let totalVulnerabilities: number[] = [];
      severitiesSummary.filter(x => !x.isTotal).forEach(severitySummary => {
        let severityDetail = severitySummary.details!.filter(x => x.id! == severity)[0];
        totalVulnerabilities.push(distinct ? severityDetail.distinctCount! : severityDetail.totalCount!);
      });
      chartData.datasets.push({
        label: SeverityUtils.getCapitalizedName(severity!),
        data: totalVulnerabilities,
        backgroundColor: SeverityUtils.getCssColor(severity!),
        hoverBackgroundColor: SeverityUtils.getCssColorHover(severity!),
        hidden: false,
      });
    });

    return chartData;
  }

  public static getDataForHorizontalBarChartBySeverity(severitiesSummary: SeveritiesSummary[], distinct: boolean): PrimeNgHorizontalBarChartData {
    // TODO make required everything in severitiesDto and severitiesSummaryDto
    let chartData: PrimeNgHorizontalBarChartData = {
      datasets: [],
      labels: [],
      title: 'a title',
    };
    const severities = severitiesSummary[0].details!.map(x => x.id);
    let namespacesCounter: number = 0;
    severities.forEach(x => { chartData.labels.push(SeverityUtils.getCapitalizedName(x!)); });
    severitiesSummary.filter(x => !x.isTotal).forEach(severitySummary => {
      let totalVulnerabilities: number[] = [];
      severities.forEach(severity => {
        let severityDetail = severitySummary.details!.filter(x => x.id == severity)[0];
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
