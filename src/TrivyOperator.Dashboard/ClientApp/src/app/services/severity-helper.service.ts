import { Injectable } from "@angular/core";
import { lastValueFrom } from "rxjs";
import { SeverityDto } from "../../api/models/severity-dto";
import { SeveritiesService } from "../../api/services/severities.service"

@Injectable({ providedIn: 'root', })
export class SeverityHelperService {
  private _severitiesService: SeveritiesService;
  private _severityDtos?: SeverityDto[] | null | undefined;
  private get colorIntensity(): number { return 400; }

  public async getSeverityDtos(): Promise<SeverityDto[]> {
    if (this._severityDtos == null) {
      const x = lastValueFrom(this._severitiesService.getSeverities());
      x.then(result => { this._severityDtos = result; });
      return x;
    }
    else {
      return new Promise((resolve) => { resolve(this._severityDtos!); })
    }
  }

  constructor(severitiesService: SeveritiesService) {
    console.log("SeverityHelperService - constructor");
    this._severitiesService = severitiesService;
  }

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

  public getName(severityId: number): string {
    if (this._severityDtos == null) {
      return "";
    }

    for (let i = 0; i < this._severityDtos.length; i++) {
      if (this._severityDtos[i].id != null && this._severityDtos[i].id == severityId) {
        return this._severityDtos[i].name ? this._severityDtos[i].name : "";
      }
    }

    return "";
  }

  public getNames(severityIds: number[], maxDisplay?: number): string {
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
      severityIds.forEach((x) => { selectedSeverityNames.push(this.getName(x)); });
      return selectedSeverityNames.join(", ");
    }
  }

  
}
