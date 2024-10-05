import { Injectable } from "@angular/core";
import { lastValueFrom } from "rxjs";
import { SeverityDto } from "../../api/models/severity-dto";
import { SeveritiesService } from "../../api/services/severities.service"

@Injectable({ providedIn: 'root', })
export class SeverityHelperService {
  private severityDtos?: SeverityDto[] | null | undefined;
  private get colorIntensity(): number { return 400; }

  public async getSeverityDtos(): Promise<SeverityDto[]> {
    if (this.severityDtos == null) {
      const x = lastValueFrom(this.severitiesService.getSeverities());
      x.then(result => { this.severityDtos = result; });
      return x;
    }
    else {
      return new Promise((resolve) => { resolve(this.severityDtos!); })
    }
  }

  constructor(private severitiesService: SeveritiesService) {
    severitiesService.getSeverities()
      .subscribe({
        next: (res) => this.severityDtos = res,
        error: (err) => console.error(err)
      });
    console.log("SeverityHelperService - constructor");
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
    if (this.severityDtos == null) {
      return "";
    }

    for (let i = 0; i < this.severityDtos.length; i++) {
      if (this.severityDtos[i].id != null && this.severityDtos[i].id == severityId) {
        return this.severityDtos[i].name ? this.severityDtos[i].name : "";
      }
    }

    return "";
  }

  public getCapitalizedName(severityId: number): string {
    let severityName = this.getName(severityId).toLowerCase();
    return this.getCapitalizedString(severityName);
  }

  public getCapitalizedString(severityName: string): string {
    severityName = severityName.toLowerCase();
    return severityName.length == 0 ? "" : severityName.charAt(0).toUpperCase() + severityName.slice(1);
  }

  public getSeverityIds(): number[] {
    return this.severityDtos ? this.severityDtos.map(x => x.id).sort((a, b) => a - b) : [];
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
