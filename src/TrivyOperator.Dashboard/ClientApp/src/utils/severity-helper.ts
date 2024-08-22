import { SeverityDto } from "../api/models/severity-dto";
import { SeverityHelperService } from "../app/services/severity-helper.service"

export type PrimeNgChartData = {
  labels: string[],
  datasets: [
    {
      data: number[],
      backgroundColor: string[],
      hoverBackgroundColor: string[],
    }
  ],
  title: string;
}

export class PrimeNgHelper {
  private _severityHelperService: SeverityHelperService;

  constructor(severityHelperService: SeverityHelperService) {
    this._severityHelperService = severityHelperService;
  }

  public GetDataForPrimeNgChart(values: number[], title: string, severityDtos?: SeverityDto[]): PrimeNgChartData {
    severityDtos = severityDtos ? severityDtos : [];
    let severityLabels: string[] = [];
    let cssColors: string[] = [];
    let cssColorHovers: string[] = [];
    severityDtos.forEach(x => {
      severityLabels.push(x.name);
      cssColors.push(this._severityHelperService.getCssColor(x.id));
      cssColorHovers.push(this._severityHelperService.getCssColorHover(x.id));
    });

    let chartData: PrimeNgChartData = {
      labels: severityLabels,
      datasets: [
        {
          data: values,
          backgroundColor: cssColors,
          hoverBackgroundColor: cssColorHovers,
        }
      ],
      title: title,
    }

    return chartData;
  }
}
