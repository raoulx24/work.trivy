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

export class Severity {
  public static GetCssColor(severityId: number): string {
    const documentStyle = getComputedStyle(document.documentElement);
    const color: string = '--' + this.GetColor(severityId) + '-' + (this.ColorIntensity() + 100);

    return documentStyle.getPropertyValue(color);
  }

  public static GetCssColorHover(severityId: number): string {
    const documentStyle = getComputedStyle(document.documentElement);
    const color: string = '--' + this.GetColor(severityId) + '-' + (this.ColorIntensity());

    return documentStyle.getPropertyValue(color);
  }

  public static GetShortName(severityId: number): string {
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

  public static GetColor(severityId: number): string {
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

  public static GetSeverityIndex(severityName: string, severityDtos: SeverityDto[]): number {
    switch (severityName) {
      case "CRITICAL":
        return 0;
      case "HIGH":
        return 1;
      case "MEDIUM":
        return 2;
      case "LOW":
        return 3;
      case "UNKNOWN":
        return 4;
      default:
        return -1;
    }
  }

  public static ColorIntensity(): number {
    return 400;
  }
}

export class PrimeNgHelpers {
  public static GetDataForPrimeNgChart(values: number[], title: string): PrimeNgChartData {
    let chartData: PrimeNgChartData = {
      labels: ["CRITICAL", "HIGH", "MEDIUM", "LOW", "UNKNOWN"],
      datasets: [
        {
          data: values,
          backgroundColor: [Severity.GetCssColor(0), Severity.GetCssColor(1), Severity.GetCssColor(2), Severity.GetCssColor(3), Severity.GetCssColor(4)],
          hoverBackgroundColor: [Severity.GetCssColorHover(0), Severity.GetCssColorHover(1), Severity.GetCssColorHover(2), Severity.GetCssColorHover(3), Severity.GetCssColorHover(4)],
        }
      ],
      title: title,
    }

    return chartData;
  }
}
