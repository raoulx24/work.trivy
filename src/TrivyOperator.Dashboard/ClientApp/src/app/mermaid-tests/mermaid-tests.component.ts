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
    console.log("ngAfterViewInit()");
    this.initializeMermaid();
    //this.initializePanZoom();
  }

  initializeMermaid() {
    mermaid.initialize({
      startOnLoad: true,
      postRenderCallback: () => { console.log("mama"); }
    });
    //mermaid.init();
    mermaid.run({
      querySelector: '.mermaid',
      postRenderCallback: this.mermaidCallback.bind(this)
    });
    //mermaid.contentLoaded();
  }

  initializePanZoom() {
    const svgElement = document.querySelector('.mermaid svg') as SVGAElement;
    
    if (!svgElement) {
      return;
    }
    this.panZoom = svgPanZoom(svgElement, {
      zoomEnabled: true,
      controlIconsEnabled: true,
      fit: true,
      center: true,
      onZoom: this.handleSvgSize.bind(this)
    });
    this.handleSvgSize();
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

  mermaidCallback(id: string) {
    console.log("mama " + id);
    this.initializePanZoom();
  }

  handleSvgSize() {
    const mermaidDiv = document.querySelector('div.mermaid') as Element;
    if (!mermaidDiv) {
      return;
    }

    const { x, y, width, height } = mermaidDiv.getBoundingClientRect();
    console.log(`${x} ${y} ${width} ${height}`);
    const svgElement = mermaidDiv.querySelector('svg');
    if (svgElement) {
      svgElement.setAttribute('viewBox', `${x} ${y} ${width} ${height}`);
      svgElement.style.maxWidth = `${width}px`;
    }
  }
}
