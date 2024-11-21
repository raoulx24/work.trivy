import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
declare const mermaid: any;
import * as svgPanZoom from 'svg-pan-zoom'

export interface MermaidLink {
  sourceId: string,
  destId: string,
  counter: number,
}

export interface MermaidNode {
  id: string,
  line1: string | undefined,
  line2: string | undefined,
  counter: number | undefined,
}

@Component({
  selector: 'app-mermaid-tests',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './mermaid-tests.component.html',
  styleUrl: './mermaid-tests.component.scss'
})
export class MermaidTestsComponent implements OnInit, AfterViewInit {
  @ViewChild("mermaid") mermaid!: ElementRef;
  panZoom?: any = null;
  private isDragging = false;

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

  nodes: MermaidNode[] = [];
  links: MermaidLink[] = [];

  mermaidGraphDefinition: SafeHtml;

  constructor(private sanitizer: DomSanitizer) {
    this.nodes.push({ id: "A", line1: "A - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    this.nodes.push({ id: "B", line1: "B - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    this.nodes.push({ id: "C", line1: "C - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    this.nodes.push({ id: "D", line1: "D - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    this.nodes.push({ id: "E", line1: "E - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    this.nodes.push({ id: "F", line1: "F - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    this.nodes.push({ id: "G", line1: "G - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    this.nodes.push({ id: "H", line1: "H - Lorem ipsum dolor", line2: "sit amet", counter: 0 });

    this.links.push({ sourceId: "A", destId: "B", counter: 0 });
    this.links.push({ sourceId: "A", destId: "C", counter: 0 });
    this.links.push({ sourceId: "B", destId: "D", counter: 0 });
    this.links.push({ sourceId: "C", destId: "D", counter: 0 });
    this.links.push({ sourceId: "D", destId: "E", counter: 0 });
    this.links.push({ sourceId: "E", destId: "F", counter: 0 });
    this.links.push({ sourceId: "F", destId: "G", counter: 0 });
    this.links.push({ sourceId: "A", destId: "G", counter: 0 });
    this.links.push({ sourceId: "G", destId: "H", counter: 0 });

    this.mermaidGraphDefinition = this.sanitizer.bypassSecurityTrustHtml(this.getMermaidGraphDefinition());
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
      postRenderCallback: () => { console.log("mama_initialize_postrender"); }
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
    this.addClickhandlersToMermaidGraph();
  }

  addClickhandlersToMermaidGraph() {
    const svgElement = document.querySelector('.mermaid svg');
    if (svgElement) {
      svgElement.addEventListener('mousedown', () => {
        this.isDragging = false;
      });
      svgElement.addEventListener('mousemove', () => {
        this.isDragging = true;
      });
      svgElement.addEventListener('mouseup', (event) => {
        if (!this.isDragging) {
          const target = event.target as HTMLElement;
          const node = target.closest('.node');
          if (node && node.tagName === 'g') {
            this.onNodeClick(node);
          }
        }
        this.isDragging = false;
      });
      svgElement.querySelectorAll('.node').forEach((node) => {
        (node as HTMLElement).addEventListener('mouseover', this.onNodeMouseOver.bind(this));
        (node as HTMLElement).addEventListener('mouseout', this.onNodeMouseOut.bind(this));
      });
    }
  }

  onNodeClick(node: Element) {
    console.log(node.getAttribute('id'));
    console.log("mama");
    node.querySelectorAll('rect, circle').forEach((element) => {
      (element as HTMLElement).style.fill = 'blue';
    });
  }

  onNodeMouseOver(event: MouseEvent) {
    const target = event.currentTarget as HTMLElement;
    console.log("target node id " + target.id);
    target.querySelectorAll('rect, circle').forEach((element) => {
      const el = element as SVGElement;
      el.style.transform = 'scale(1.2)';
      el.style.transition = 'transform 0.3s ease';
    });
  }

  onNodeMouseOut(event: MouseEvent) {
    const target = event.currentTarget as HTMLElement;
    target.querySelectorAll('rect, circle').forEach((element) => {
      (element as SVGElement).style.transform = 'scale(1)';
      (element as SVGElement).style.transition = 'transform 0.3s ease';
    });
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

  getMermaidGraphDefinition(): string {
    const graphLines: string[] = [];
    this.links.forEach(link => {
      const sourceNode: MermaidNode = this.nodes.find(x => x.id == link.sourceId) ??
        { id: link.sourceId, line1: "", line2: "", counter: 0 };
      const destNode: MermaidNode = this.nodes.find(x => x.id == link.destId) ??
        { id: link.destId, line1: "", line2: "", counter: 0 };
      const sourceText1 = `<span class="mnodel1">${sourceNode.line1}</span>`;
      const sourceText2 = `<span class="mnodel1">${sourceNode.line2}</span>`;
      const destText1 = `<span class="mnodel2">${destNode.line1}</span>`;
      const destText2 = `<span class="mnodel2">${destNode.line2}</span>`;
      const line: string = `${sourceNode.id}([${sourceText1}<br/>${sourceText2}])-->${destNode.id}([${destText1}<br/>${destText2}])`

      console.log(line);

      graphLines.push(line);
    });
    const result: string = `<div id="mermaid" class="mermaid flex-grow-1 justify-content-center">graph TD; ${graphLines.join('; ')}</div>`;
    console.log(result);

    return result;
  }

  // mermaidGraphDefinition: string = `<div id="mermaid" class="mermaid flex-grow-1 justify-content-center">graph TD; </div>`;
}

// ids
// line: id="L_A_B_0"
// node: id="flowchart-A-0"
