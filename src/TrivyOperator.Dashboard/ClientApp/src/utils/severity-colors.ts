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

export class ServerityColors {
  public static Test(severityName: string): string {
    const documentStyle = getComputedStyle(document.documentElement);
    let cssStyle: string = '';

    switch (severityName) {
      case "CRITICAL":
        cssStyle = documentStyle.getPropertyValue('--red-500');
        break;
      case "HIGH":
        cssStyle = documentStyle.getPropertyValue('--orange-500');
        break;
      case "MEDIUM":
        cssStyle = documentStyle.getPropertyValue('--yellow-500');
        break;
      case "LOW":
        cssStyle = documentStyle.getPropertyValue('--cyan-500');
        break;
      case "UNKNOWN":
        cssStyle = documentStyle.getPropertyValue('--blue-500');
        break;
    }

    return cssStyle;
  }

  public static TestHover(severityName: string): string {
    const documentStyle = getComputedStyle(document.documentElement);
    let cssStyle: string = '';

    switch (severityName) {
      case "CRITICAL":
        cssStyle = documentStyle.getPropertyValue('--red-400');
        break;
      case "HIGH":
        cssStyle = documentStyle.getPropertyValue('--orange-400');
        break;
      case "MEDIUM":
        cssStyle = documentStyle.getPropertyValue('--yellow-400');
        break;
      case "LOW":
        cssStyle = documentStyle.getPropertyValue('--cyan-400');
        break;
      case "UNKNOWN":
        cssStyle = documentStyle.getPropertyValue('--blue-400');
        break;
    }

    return cssStyle;
  }
}

export class PrimeNgHelpers {
  public static GetDataForPrimeNgChart(values: number[], title: string): PrimeNgChartData {
    let chartData: PrimeNgChartData = {
      labels: ["CRITICAL", "HIGH", "MEDIUM", "LOW", "UNKNOWN"],
      datasets: [
        {
          data: values,
          backgroundColor: [ServerityColors.Test("CRITICAL"), ServerityColors.Test("HIGH"), ServerityColors.Test("MEDIUM"), ServerityColors.Test("LOW"), ServerityColors.Test("UNKNOWN")],
          hoverBackgroundColor: [ServerityColors.TestHover("CRITICAL"), ServerityColors.TestHover("HIGH"), ServerityColors.TestHover("MEDIUM"), ServerityColors.TestHover("LOW"), ServerityColors.TestHover("UNKNOWN")],
        }
      ],
      title: title,
    }

    return chartData;
  }
}
