import { Component } from '@angular/core';

import { PanelModule } from 'primeng/panel';

import { HomeVulnerabilityReportsComponent } from '../home-vulnerability-reports/home-vulnerability-reports.component'


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [HomeVulnerabilityReportsComponent, PanelModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})

export class HomeComponent {
  public expandedPanels: string[] = ["vr"];

  private readonly localstorageKeyPrefix = "Home.";
  public isVrPanelCollapsed: boolean = false;

  constructor() {
    this.expandPanels();
  }

  onVrPanelChange(event: boolean) {
    this.onPanelChange("vr", event);
  }

  onLiPanelChange(event: boolean) {
    this.onPanelChange("li", event);
  }

  private onPanelChange(panelKey: string, isCollapsed: boolean) {
    if (isCollapsed) {
      this.expandedPanels = this.expandedPanels.filter(x => x !== panelKey);
    }
    else {
      this.expandedPanels.push(panelKey);
    }
    localStorage.setItem(this.localstorageKeyPrefix + "PanelStates", this.expandedPanels.join(","));
  }

  private expandPanels() {
    let savedStates = localStorage.getItem(this.localstorageKeyPrefix + "PanelStates");
    this.expandedPanels = savedStates !== null
      ? savedStates.split(",")
      : this.expandedPanels;
  }
}
