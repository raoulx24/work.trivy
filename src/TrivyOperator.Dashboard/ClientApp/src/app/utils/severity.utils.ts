import { Severity } from "../../api/models";
import { SeverityDto } from "../../api/models/severity-dto";

export class SeverityUtils {
  static severityDtos: SeverityDto[] = [
    { id: 0, name: 'CRITICAL' },
    { id: 1, name: 'HIGH' },
    { id: 2, name: 'MEDIUM' },
    { id: 3, name: 'LOW' },
    { id: 4, name: 'UNKNOWN' }, 
  ];
  private static colorIntensity: number = 400;

  public static getCssColor(severityId: number): string {
    const documentStyle = getComputedStyle(document.documentElement);
    const color: string = '--' + this.getColor(severityId) + '-' + (this.colorIntensity + 100);

    return documentStyle.getPropertyValue(color);
  }

  public static getCssColorHover(severityId: number): string {
    const documentStyle = getComputedStyle(document.documentElement);
    const color: string = '--' + this.getColor(severityId) + '-' + (this.colorIntensity);

    return documentStyle.getPropertyValue(color);
  }

  public static getColor(severityId: number): string {
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

  public static getName(severityId: number): string {
    if (SeverityUtils.severityDtos == null) {
      return "";
    }

    for (let i = 0; i < SeverityUtils.severityDtos.length; i++) {
      if (SeverityUtils.severityDtos[i].id != null && SeverityUtils.severityDtos[i].id == severityId) {
        return SeverityUtils.severityDtos[i].name ?? "";
      }
    }

    return "";
  }

  public static getCapitalizedName(severityId: number): string {
    let severityName = SeverityUtils.getName(severityId).toLowerCase();
    return SeverityUtils.getCapitalizedString(severityName);
  }

  public static getCapitalizedString(severityName: string): string {
    severityName = severityName.toLowerCase();
    return severityName.length == 0 ? "" : severityName.charAt(0).toUpperCase() + severityName.slice(1);
  }

  public static getSeverityIds(): number[] {
    return SeverityUtils.severityDtos ? SeverityUtils.severityDtos.map(x => x.id).sort((a, b) => a - b) : [];
  }

  public static getNames(severityIds: number[], maxDisplay?: number): string {
    severityIds = severityIds ? severityIds : [];
    maxDisplay = maxDisplay ? maxDisplay : 0;

    if (severityIds.length == 0) {
      return "Any"
    }
    if (severityIds.length > maxDisplay) {
      return `${severityIds.length} selected`
    }
    else {
      let selectedSeverityNames: string[] = [];
      severityIds.forEach((x) => { selectedSeverityNames.push(SeverityUtils.getName(x)); });
      return selectedSeverityNames.join(", ");
    }
  }

  
}
