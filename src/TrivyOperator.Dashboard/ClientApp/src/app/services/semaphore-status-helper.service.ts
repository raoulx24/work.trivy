import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SemaphoreStatusHelperService {
  private get colorIntensity(): number {
    return 400;
  }

  public getCssColor(statusName: string): string {
    const documentStyle = getComputedStyle(document.documentElement);
    const color: string = '--' + this.getColor(statusName) + '-' + (this.colorIntensity + 100);

    return documentStyle.getPropertyValue(color);
  }

  public getColor(statusName: string): string {
    switch (statusName.toLowerCase()) {
      case 'green':
        return 'green';
      case 'yellow':
        return 'yellow';
      case 'red':
        return 'red';
      case 'unknown':
        return 'blue';
      default:
        return 'blue';
    }
  }
}
