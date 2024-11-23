import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

import { ButtonModule } from 'primeng/button';

declare const mermaid: any;
import * as svgPanZoom from 'svg-pan-zoom'

// tests.sbom
import { ClusterSbomReportService } from '../../api/services/cluster-sbom-report.service';
import { ClusterSbomReportDto } from '../../api/models/cluster-sbom-report-dto';
//


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
  imports: [CommonModule, ButtonModule],
  templateUrl: './mermaid-tests.component.html',
  styleUrl: './mermaid-tests.component.scss'
})
export class MermaidTestsComponent implements OnInit, AfterViewInit {
  @ViewChild("mermaid") mermaid!: ElementRef;
  panZoom?: any = null;
  private isDragging = false;
  private selectedNode: MermaidNode | undefined = undefined;
  private hoveredNode: MermaidNode | undefined = undefined;

  private selectedNodeColor: string = 'Gray';
  private hoveredNodeColor: string = 'Silver';
  private sourceNodeColor: string = 'RoyalBlue';
  private destNodeColor: string = 'DeepSkyBlue';
  private unfocusedNodeColor: string = 'White';

  private mermaidConfig = {
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

  private pansvgConfig = {
    zoomEnabled: true,
    fit: true,
    center: true,
    onZoom: this.handleSvgSize.bind(this)
  };

  nodes: MermaidNode[] = [];
  links: MermaidLink[] = [];

  mermaidGraphDefinition: SafeHtml;

  constructor(private sanitizer: DomSanitizer, private service: ClusterSbomReportService) {
    //this.nodes.push({ id: "A", line1: "A - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    //this.nodes.push({ id: "B", line1: "B - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    //this.nodes.push({ id: "C", line1: "C - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    //this.nodes.push({ id: "D", line1: "D - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    //this.nodes.push({ id: "E", line1: "E - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    //this.nodes.push({ id: "F", line1: "F - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    //this.nodes.push({ id: "G", line1: "G - Lorem ipsum dolor", line2: "sit amet", counter: 0 });
    //this.nodes.push({ id: "H", line1: "H - Lorem ipsum dolor", line2: "sit amet", counter: 0 });

    //this.links.push({ sourceId: "A", destId: "B", counter: 0 });
    //this.links.push({ sourceId: "A", destId: "C", counter: 0 });
    //this.links.push({ sourceId: "B", destId: "D", counter: 0 });
    //this.links.push({ sourceId: "C", destId: "D", counter: 0 });
    //this.links.push({ sourceId: "D", destId: "E", counter: 0 });
    //this.links.push({ sourceId: "E", destId: "F", counter: 0 });
    //this.links.push({ sourceId: "F", destId: "G", counter: 0 });
    //this.links.push({ sourceId: "A", destId: "G", counter: 0 });
    //this.links.push({ sourceId: "G", destId: "H", counter: 0 });



    // tests.sbom
    this.mermaidGraphDefinition = this.sanitizer.bypassSecurityTrustHtml('');
    this.getTableDataDtos();
    //
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    console.log("ngAfterViewInit()");
    //this.initializeMermaid();
  }

  initializeMermaid() {
    mermaid.initialize(this.mermaidConfig);
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
    this.handleSvgSize();
    this.panZoom = svgPanZoom(svgElement, this.pansvgConfig);
    this.handleSvgSize();
    window.addEventListener('resize', () => {
      this.handleSvgSize();
      
    });
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
        (node as HTMLElement).addEventListener('mouseenter', this.onNodeMouseEnter.bind(this));
        (node as HTMLElement).addEventListener('mouseleave', this.onNodeMouseLeave.bind(this));
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
    this.selectedNode = this.selectedNode && this.selectedNode == mermaidNode ? undefined : mermaidNode;
    this.hoveredNode == this.hoveredNode ?? mermaidNode;
    if (mermaidNode) {
      this.setNodeColor(mermaidNode, this.selectedNodeColor, this.hoveredNodeColor);
    }
  }

  onNodeMouseEnter(event: MouseEvent) {
    console.log("onNodeMouseOver - enter - hoverNode ", this.hoveredNode?.id);
    if (this.hoveredNode || this.selectedNode) {
      console.log("cucu");
      return;
    }
    const target = event.currentTarget as HTMLElement;
    console.log("onNodeMouseOver - target node id " + target.id);
    this.hoveredNode = this.getMermaidNodeByElementId(target.id);
    console.log("onNodeMouseOver - hoveredNode " + this.hoveredNode?.id);
    target.querySelectorAll('rect, circle').forEach((element) => {
      const el = element as SVGElement;
      el.style.transform = 'scale(1.2)';
      el.style.transition = 'transform 0.3s ease';
    });

    const targetMermaidNode = this.getMermaidNodeByElementId(target.id);
    if (targetMermaidNode) {
      this.setFocusedNode(targetMermaidNode);
    }
  }

  onNodeMouseLeave(event: MouseEvent) {
    if (this.selectedNode) {
      this.hoveredNode = undefined;
      return;
    }
    console.log("onNodeMouseOut - enter");
    const target = event.currentTarget as HTMLElement;
    console.log("onNodeMouseOut - " + target.id);
    target.querySelectorAll('rect, circle').forEach((element) => {
      (element as SVGElement).style.transform = 'scale(1)';
      (element as SVGElement).style.transition = 'transform 0.3s ease';
    });
    this.hoveredNode = undefined;

    const targetMermaidNode = this.getMermaidNodeByElementId(target.id);
    if (targetMermaidNode) {
      this.setFocusedNode(targetMermaidNode);
    }
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
      svgElement.setAttribute('viewBox', `0 0 ${width} ${height}`);
      svgElement.style.maxWidth = `${width}px`;
    }
  }

  getMermaidGraphDefinition(): string {
    const graphLines: string[] = [];
    this.nodes.forEach(node => {
      // tests.sbom - uncomment next 3 lines
      //const sourceText1 = `<span class="mnodel1">${node.line1}</span>`;
      //const sourceText2 = `<span class="mnodel2">${node.line2}</span>`;
      //const line = `${node.id}([${sourceText1}<br/>${sourceText2}])`;

      const line = `${node.id}([${node.line1}])`;
      graphLines.push(line);
    });

    this.links.forEach(link => {
      const line = `${link.sourceId} --> ${link.destId}`;

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

  private setFocusedNode(mainNode: MermaidNode) {
    const destIds = this.links.filter(x => x.sourceId == mainNode.id).map(x => x.destId);
    const destNodes = this.nodes.filter(x => destIds.includes(x.id));
    const sourceIds = this.links.filter(x => x.destId == mainNode.id).map(x => x.sourceId);
    const sourceNodes = this.nodes.filter(x => sourceIds.includes(x.id));

    destNodes.forEach(x => {
      this.setNodeColor(x, this.destNodeColor);
    });

    sourceNodes.forEach(x => {
      this.setNodeColor(x, this.sourceNodeColor);
    });

    this.setNodeColor(mainNode, this.hoveredNodeColor);

    const links = this.links.filter(x => x.sourceId == mainNode.id || x.destId == mainNode.id);
    links.forEach(x => {
      const elementLink = document.getElementById(`L_${x.sourceId}_${x.destId}_${x.counter}`);
      if (elementLink) {
        elementLink.style.strokeWidth = this.hoveredNode ? '5px' : '1px';
      }
    });

    // ids
    // line: id="L_A_B_0"
    // node: id="flowchart-A-0"
  }

  private setNodeColor(node: MermaidNode, focusColor: string, unfocusColor: string = this.unfocusedNodeColor) {
    const elementNode = document.getElementById(`flowchart-${node.id}-${node.counter}`);
    if (elementNode) {
      elementNode.querySelectorAll('rect, circle').forEach((element) => {
        (element as HTMLElement).style.fill = this.hoveredNode ? focusColor : unfocusColor;
      });
    }
  }

  onZoomIn(_event: MouseEvent) {
    this.panZoom.zoomIn();
  }

  onZoomOut(_event: MouseEvent) {
    this.panZoom.zoomOut();
  }

  onZoomReset(_event: MouseEvent) {
    this.panZoom.resetZoom();
    this.panZoom.resetPan();
  }

  // tests.sbom
  getTableDataDtos() {
    this.service.getClusterSbomReportDtos().subscribe({
      next: (res) => this.onGetDataDtos(res),
      error: (err) => console.error(err),
    });
  }

  onGetDataDtos(dtos: ClusterSbomReportDto[]) {
    this.dataDtos = dtos;

    this.dataDtos[0].details?.forEach(detail => {
      const bomRef = detail.bomRef?.replace(/-/g, "") ?? "";
      //this.nodes.push({ id: bomRef, line1: detail.name ?? "", line2: detail.version ?? "", counter: 0 });
      this.nodes.push({ id: bomRef, line1: "Line 1", line2: "Line 2", counter: 0 });
      const newLinks: MermaidLink[] = detail.dependsOn?.map(x => {
        return { sourceId: bomRef, destId: x.replace(/-/g, ""), counter: 0 }
      }) ?? [];
      this.links.push(...newLinks);
    });

    // IMPORTANT - move it in Mermaid Nodes and Links setter (or maybe in dataDots)
    const tempText = this.getMermaidGraphDefinition();
    console.log(tempText);
    this.mermaidGraphDefinition = this.sanitizer.bypassSecurityTrustHtml(tempText);
    this.initializeMermaid();
  }

  private dataDtos: ClusterSbomReportDto[] = [];
  //
}


