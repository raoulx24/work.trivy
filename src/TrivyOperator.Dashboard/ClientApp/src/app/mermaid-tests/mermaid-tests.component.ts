import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
declare const mermaid: any;
import * as svgPanZoom from 'svg-pan-zoom'

@Component({
  selector: 'app-mermaid-tests',
  standalone: true,
  imports: [],
  templateUrl: './mermaid-tests.component.html',
  styleUrl: './mermaid-tests.component.scss'
})
export class MermaidTestsComponent implements OnInit, AfterViewInit {
  @ViewChild("mermaid") mermaid!: ElementRef;
  panZoom?: any = null;

  config = {
    theme: "neutral",
    startOnLoad: false,
    securityLevel: "loose",
    flowChart: {
      useMaxWidth: true,
      htmlLabels: true,
    },
    themeVariables: {
      fontSize: "12px",
      primaryColor: "#607D8B",
    },
  };

  constructor() {
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.initializeMermaid();
    //this.initializePanZoom();
  }

  initializeMermaid() {
    mermaid.initialize({ startOnLoad: true });
    mermaid.init();
  }

  initializePanZoom() {
    this.panZoom = svgPanZoom('.mermaid svg', {
      zoomEnabled: true,
      controlIconsEnabled: true
    });
  }

  onZoomInClick() {
    if (!this.panZoom) {
      this.initializePanZoom();
    }

    console.log(JSON.stringify(this.panZoom));
    console.log(this.panZoom);
    this.panZoom.zoomIn()
  }

  onZoomOutClick() {
    if (!this.panZoom) {
      this.initializePanZoom();
    }

    console.log(JSON.stringify(this.panZoom));
    console.log(this.panZoom);
    this.panZoom.zoomOut()
  }

    //const mermaidConfig = {
    //  startOnLoad: true
    //};
    //mermaid.initialize(mermaidConfig);
    //mermaid.contentLoaded();
}
