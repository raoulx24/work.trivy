import { SeverityDto } from "../api/models/severity-dto";

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

export class SeverityHelper {
  public getCssColor(severityId: number): string {
    const documentStyle = getComputedStyle(document.documentElement);
    const color: string = '--' + this.getColor(severityId) + '-' + (this.colorIntensity + 100);

    return documentStyle.getPropertyValue(color);
  }

  public getCssColorHover(severityId: number): string {
    const documentStyle = getComputedStyle(document.documentElement);
    const color: string = '--' + this.getColor(severityId) + '-' + (this.colorIntensity);

    return documentStyle.getPropertyValue(color);
  }

  public getShortName(severityId: number): string {
    switch (severityId) {
      case 0:
        return 'CRIT';
      case 1:
        return 'HIGH';
      case 2:
        return 'MED';
      case 3:
        return 'LOW';
      case 4:
        return 'UNKN';
      default:
        return '';
    }
  }

  public getColor(severityId: number): string {
    switch (severityId) {
      case 0:
        return 'red';
      case 1:
        return 'orange';
      case 2:
        return 'yellow';
      case 3:
        return 'cyan';
      case 4:
        return 'blue';
      default:
        return '';
    }
  }

  public getName(severityId: number, severityDtos?: SeverityDto[]): string {
    if (severityDtos == null) {
      return "";
    }

    for (let i = 0; i < severityDtos.length; i++) {
      if (severityDtos[i].id != null && severityDtos[i].id == severityId) {
        return severityDtos[i].name ? severityDtos[i].name : "";
      }
    }

    return "";
  }

  public getNames(severityIds: number[], severityDtos?: SeverityDto[], maxDisplay?: number): string {
    severityIds = severityIds ? severityIds : [];
    severityDtos = severityDtos ? severityDtos : [];
    maxDisplay = maxDisplay ? maxDisplay : 0;

    if (severityIds.length == 0) {
      console.log("severities is empty");
      return "Any"
    }
    if (severityIds.length > maxDisplay) {
      return `${severityIds.length} selected`
    }
    else {
      let selectedSeverityNames: string[] = [];
      severityIds.forEach((x) => { selectedSeverityNames.push(this.getName(x, severityDtos)); });
      return selectedSeverityNames.join(", ");
    }
  }

  public get colorIntensity(): number {
    return 400;
  }
}

export class PrimeNgHelper {
  public get severityHelper(): SeverityHelper { return this._severityHelper; };
  private _severityHelper: SeverityHelper = new SeverityHelper();

  public GetDataForPrimeNgChart(values: number[], title: string, severityDtos?: SeverityDto[]): PrimeNgChartData {
    severityDtos = severityDtos ? severityDtos : [];
    let severityLabels: string[] = [];
    let cssColors: string[] = [];
    let cssColorHovers: string[] = [];
    severityDtos.forEach(x => {
      severityLabels.push(x.name);
      cssColors.push(this.severityHelper.getCssColor(x.id));
      cssColorHovers.push(this.severityHelper.getCssColorHover(x.id));
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
