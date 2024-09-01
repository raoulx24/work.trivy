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
    let namespacesCounter: number = 0;
    severities.forEach(x => { chartData.labels.push(x.name); });
    severitiesSummary.forEach(severitySummary => {
      let totalVulnerabilities: number[] = [];
      severities.forEach(severity => {
        totalVulnerabilities.push(severitySummary.details!.filter(x => x.id! == severity!.id!)[0].totalCount!);
      });
      let color: string = ColorHelper.rainbow(severitiesSummary.length, namespacesCounter);
      console.log(`Picked color: ${color}`);
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

export class ColorHelper {
  public static rainbow(numOfSteps: number, step: number) {
    // This function generates vibrant, "evenly spaced" colours (i.e. no clustering). This is ideal for creating easily distinguishable vibrant markers in Google Maps and other apps.
    // Adam Cole, 2011-Sept-14
    // HSV to RBG adapted from: http://mjijackson.com/2008/02/rgb-to-hsl-and-rgb-to-hsv-color-model-conversion-algorithms-in-javascript
    // source: https://stackoverflow.com/questions/1484506/random-color-generator
    let r = 0, g = 0, b = 0;
    let h = step / numOfSteps;
    let i = ~~(h * 6);
    let f = h * 6 - i;
    let q = 1 - f;
    switch (i % 6) {
      case 0: r = 1; g = f; b = 0; break;
      case 1: r = q; g = 1; b = 0; break;
      case 2: r = 0; g = 1; b = f; break;
      case 3: r = 0; g = q; b = 1; break;
      case 4: r = f; g = 0; b = 1; break;
      case 5: r = 1; g = 0; b = q; break;
    }
    let c = "#" + ("00" + (~ ~(r * 255)).toString(16)).slice(-2) + ("00" + (~ ~(g * 255)).toString(16)).slice(-2) + ("00" + (~ ~(b * 255)).toString(16)).slice(-2);
    return (c);
  }
}
