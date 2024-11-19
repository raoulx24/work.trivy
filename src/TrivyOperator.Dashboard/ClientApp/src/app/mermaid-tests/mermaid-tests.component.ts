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
  panZoom: any;

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
    mermaid.initialize({
      ...this.config,
    });
    setTimeout(() => {
      mermaid.init();
      const element = this.mermaid.nativeElement;
      const svg = element.querySelector('svg');
      this.panZoom = svgPanZoom(svg, {
        panEnabled: true,
        zoomEnabled: true,
        mouseWheelZoomEnabled: true,
        preventMouseEventsDefault: true,
        center: false
      });

      console.log(JSON.stringify(this.panZoom));
      console.log(svg);

    }, 200);
  }


  onZoomInClick() {
    console.log(JSON.stringify(this.panZoom));
    console.log(this.panZoom);
    this.panZoom.zoomIn()
  }

  onZoomOutClick() {
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
