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
  private selectedNode: MermaidNode | undefined = undefined;
  private hoveredNode: MermaidNode | undefined = undefined;

  config = {
    theme: 'neutral',
    startOnLoad: false,
    securityLevel: 'loose',
    flowChart: {
      useMaxWidth: true,
      htmlLabels: true,
    },
    themeVariables: {
      fontSize: '12px',
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
    mermaid.initialize(this.config);
    //mermaid.init();
    mermaid.run({
      querySelector: '.mermaid',
      postRenderCallback: this.onMermaidRendered.bind(this)
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

  onMermaidRendered(id: string) {
    console.log("mama " + id);
    this.initializePanZoom();
    this.addClickhandlersToMermaidGraph();
    this.getMermaidIds();
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

  private getMermaidIds() {
    const svgElement = document.querySelector('.mermaid svg');
    // ids
    // line: id="L_A_B_0"
    // node: id="flowchart-A-0"
    if (svgElement) {
      svgElement.querySelectorAll('.node').forEach((nodeElement) => {
        const splittedElementId = nodeElement.id.split('-');
        if (splittedElementId.length == 3) {
          const nodeCounter = Number(splittedElementId[2]);
          const node = this.getMermaidNodeByElementId(nodeElement.id);
          if (node) {
            node.counter = isNaN(nodeCounter) ? -1 : nodeCounter;
          }
        }
      });
      svgElement.querySelectorAll('.flowchart-link').forEach((linkElement) => {
        const splittedElementId = linkElement.id.split('_');
        if (splittedElementId.length == 4) {
          const sourceNodeId = splittedElementId[1];
          const destNodeId = splittedElementId[2];
          const linkCounter = Number(splittedElementId[3]);
          const link = this.links.find(x => x.sourceId == sourceNodeId && x.destId == destNodeId);
          if (link) {
            link.counter = isNaN(linkCounter) ? -1 : linkCounter;
          }
        }
      });
    }
    // debug
    this.nodes.forEach(x => console.log(x));
    this.links.forEach(x => console.log(x));
  }

  onNodeClick(node: Element) {
    console.log(node.getAttribute('id'));
    console.log("mama");
    const mermaidNode = this.getMermaidNodeByElementId(node.id);
    this.selectedNode = this.selectedNode && this.selectedNode == mermaidNode ? undefined : this.getMermaidNodeByElementId(node.id);
    node.querySelectorAll('rect, circle').forEach((element) => {
      (element as HTMLElement).style.fill = this.selectedNode ? 'blue' : 'white';
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
      const sourceText2 = `<span class="mnodel2">${sourceNode.line2}</span>`;
      const destText1 = `<span class="mnodel1">${destNode.line1}</span>`;
      const destText2 = `<span class="mnodel2">${destNode.line2}</span>`;
      const line: string = `${sourceNode.id}([${sourceText1}<br/>${sourceText2}])-->${destNode.id}([${destText1}<br/>${destText2}])`

      console.log(line);

      graphLines.push(line);
    });
    const result: string = `<div id="mermaid" class="mermaid h-full">graph TD; ${graphLines.join('; ')}</div>`;
    console.log(result);

    return result;
  }

  private getMermaidNodeByElementId(elementId: string): MermaidNode | undefined {
    const splittedElementId = elementId.split('-');
    if (splittedElementId.length == 3) {
      const nodeId = splittedElementId[1];
      const node = this.nodes.find(x => x.id == nodeId);

      return node;
    }

    return undefined;
  }

  // mermaidGraphDefinition: string = `<div id="mermaid" class="mermaid flex-grow-1 justify-content-center">graph TD; </div>`;
}


