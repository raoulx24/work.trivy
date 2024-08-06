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
  public static GetCssColor(severityName: string): string {
    const documentStyle = getComputedStyle(document.documentElement);
    const color: string = '--' + this.GetColor(severityName) + '-' + (this.ColorIntensity() + 100);

    return documentStyle.getPropertyValue(color);
  }

  public static GetCssColorHover(severityName: string): string {
    const documentStyle = getComputedStyle(document.documentElement);
    const color: string = '--' + this.GetColor(severityName) + '-' + (this.ColorIntensity());

    return documentStyle.getPropertyValue(color);
  }

  public static GetShortName(severityName: string): string {
    switch (severityName) {
      case "CRITICAL":
        return 'CRIT';
      case "HIGH":
        return 'HIGH';
      case "MEDIUM":
        return 'MED';
      case "LOW":
        return 'LOW';
      case "UNKNOWN":
        return 'UNKN';
      default:
        return '';
    }
  }

  public static GetColor(severityName: string): string {
    switch (severityName) {
      case "CRITICAL":
        return 'red';
      case "HIGH":
        return 'orange';
      case "MEDIUM":
        return 'yellow';
      case "LOW":
        return 'cyan';
      case "UNKNOWN":
        return 'blue';
      default:
        return '';
    }
  }

  public static GetSeverityIndex(severityName: string): number {
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
          backgroundColor: [Severity.GetCssColor("CRITICAL"), Severity.GetCssColor("HIGH"), Severity.GetCssColor("MEDIUM"), Severity.GetCssColor("LOW"), Severity.GetCssColor("UNKNOWN")],
          hoverBackgroundColor: [Severity.GetCssColorHover("CRITICAL"), Severity.GetCssColorHover("HIGH"), Severity.GetCssColorHover("MEDIUM"), Severity.GetCssColorHover("LOW"), Severity.GetCssColorHover("UNKNOWN")],
        }
      ],
      title: title,
    }

    return chartData;
  }
}
