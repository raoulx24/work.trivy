import { Component, ElementRef, Input, ViewChild } from '@angular/core';

import cytoscape, { EdgeSingular, ElementDefinition, NodeSingular } from 'cytoscape';
import fcose, { FcoseLayoutOptions } from 'cytoscape-fcose';

import { BreadcrumbItemClickEvent, BreadcrumbModule } from 'primeng/breadcrumb';
import { ButtonModule } from 'primeng/button';

// tests.sbom
import { SbomReportDetailDto } from '../../api/models/sbom-report-detail-dto';
import { SbomReportDto } from '../../api/models/sbom-report-dto';
import { SbomReportService } from '../../api/services/sbom-report.service';
import { MenuItem } from 'primeng/api';

cytoscape.use(fcose);

//

@Component({
  selector: 'app-fcose',
  standalone: true,
  imports: [BreadcrumbModule, ButtonModule],
  templateUrl: './fcose.component.html',
  styleUrl: './fcose.component.scss',
})
export class FcoseComponent {
  @ViewChild('graphContainer', { static: true }) graphContainer!: ElementRef;
  testText: string = '';

  private readonly rootNodeId: string = "00000000-0000-0000-0000-000000000000";
  private selectedRootNodeId: string = this.rootNodeId;
  private isDivedIn: boolean = false;

  navItems: MenuItem[] = [];
  navHome: MenuItem = { id: this.rootNodeId, icon: 'pi pi-sitemap' };
  private cy!: cytoscape.Core;
  private fcoseLayoutOptions: FcoseLayoutOptions = {
    name: 'fcose',
    nodeRepulsion: (node: NodeSingular) => {
      return 20000;
    },
    numIter: 2500,
    animate: false,
    fit: true,
    padding: 10,
    sampleSize: 50,
    nodeSeparation: 500,
    tilingPaddingHorizontal: 1000,
    tilingPaddingVertical: 1000,
    idealEdgeLength: (edge: EdgeSingular) => {
      return 150;
    },
    edgeElasticity: (edge: EdgeSingular) => {
      return 0.15;
    },
  };

  get dataDtos(): SbomReportDto | null {
    return this._dataDtos;
  }
  @Input() set dataDtos(sbomDto: SbomReportDto | null) {
    this._dataDtos = sbomDto;
    this.onGetDataDtos(sbomDto);
  }
  private _dataDtos: SbomReportDto | null = null;

  constructor(private service: SbomReportService) {
    // tests.sbom
    //this.getTableDataDtos();
    //
  }

  private _hoveredNode: NodeSingular | null = null;

  private get hoveredNode(): NodeSingular | null {
    return this._hoveredNode;
  }

  private set hoveredNode(node: NodeSingular | null) {
    this._hoveredNode = node;
    if (node) {
      const x = this.getDataDetailDtoById(node.id());
      if (x) {
        this.testText = `<b>Name:</b> ${x.name} - <b>Version:</b> ${x.version} - <b>Dependencies:</b> ${x.dependsOn?.length ?? 0}`;
      }
    } else {
      this.testText = 'no info...';
    }
  }

  // tests.sbom
  getTableDataDtos() {
    this.service.getSbomReportDtoByUid({ uid: "25d158fe-10b9-49a3-ac5d-c393c12d040c" }).subscribe({
      next: (res) => this.onGetDataDtos(res),
      error: (err) => console.error(err),
    });
  }

  onGetDataDtos(dtos: SbomReportDto | null) {
    if (!dtos) {
      return;
    }

    console.log("fcose - onGetDateDtos");
    
    const elements: ElementDefinition[] = this.getElementsByNodeId(this.rootNodeId);

    this.cy = cytoscape({
      container: this.graphContainer.nativeElement,
      elements: elements,
      layout: this.fcoseLayoutOptions as FcoseLayoutOptions,
      style: [
        {
          selector: '$node > .nodeCommon',
          style: {
            'background-color': 'gray',
            'background-opacity': 0.2,
            //'label': 'data(label)',
            'text-valign': 'top',
            'text-halign': 'center',
            'text-background-color': 'aqua',
            'font-size': '14px',
            'font-weight': 'bold',
          },
        },
        {
          selector: '.nodeCommon',
          style: {
            opacity: 1,
            'transition-property': 'opacity',
            'transition-duration': 300,
            'border-width': 1,
          },
        },
        {
          selector: '.nodePackage',
          style: {
            label: 'data(label)',
            width: 'mapData(label.length, 1, 30, 20, 200)',
            height: '20px',
            'background-color': 'Aqua',
            'text-valign': 'center',
            'text-halign': 'center',
            'text-wrap': 'ellipsis',
            'text-max-width': '200px',
            'font-size': '10px',
            'border-color': '#000',
            'transition-property': 'width height background-color font-size border-color opacity',
            'transition-duration': 300,
          },
        },
        {
          selector: '.nodeBranch',
          style: {
            shape: 'roundrectangle',
          },
        },
        {
          selector: '.nodeLeaf',
          style: {
            shape: 'cut-rectangle',
          },
        },
        {
          selector: '.edgeCommon',
          style: {
            width: 1,
            'line-color': '#ccc',
            'target-arrow-color': '#ccc',
            'target-arrow-shape': 'triangle',
            'transition-property': 'width line-color opacity',
            'transition-duration': 300,
            opacity: 1,
          },
        },
        {
          selector: '.hoveredCommon',
          style: {
            width: 'mapData(label.length, 1, 30, 20, 240)',
            height: '24px',
            'font-size': '12px',
            'transition-property': 'width height background-color font-size border-color opacity',
            'transition-duration': 300,
          },
        },
        {
          selector: '.hovered',
          style: {
            'background-color': 'Silver',
          },
        },
        {
          selector: '.hoveredOutgoers',
          style: {
            'background-color': 'DeepSkyBlue',
          },
        },
        {
          selector: '.hoveredIncomers',
          style: {
            'background-color': 'RoyalBlue',
          },
        },
        {
          selector: '.hoveredHighlight',
          style: {
            'overlay-opacity': 0.5,
            'overlay-color': 'RoyalBlue',
            'font-style': 'italic',
          },
        },
        {
          selector: '.highlighted-edge',
          style: {
            width: 3,
            'line-color': 'Violet',
            'transition-property': 'line-color opacity',
            'transition-duration': 300,
          },
        },
        {
          selector: '.hidden',
          style: {
            opacity: 0,
          },
        },
      ],
    });

    this.setupCyEvents();
  }

  private setupCyEvents() {
    this.cy.on('mouseover', 'node', (event) => {
      this.highlightNode(event.target as NodeSingular);
    });

    this.cy.on('mouseout', 'node', (event) => {
      this.unhighlightNode(event.target as NodeSingular);
    });

    this.cy.on('dblclick', 'node', (event) => {
      this.selectNode(event.target as NodeSingular);
    });

    //this.cy.on('click', 'node', (event) => {
    //  const node = event.target;
    //  console.log('Single-clicked on node:', node.id());
    //});
  }

  private highlightNode(node: NodeSingular) {
    if (this.hoveredNode?.id() == node.id() || node.isParent()) {
      console.log("cucu0 - " + this.hoveredNode?.id());
      console.log("cucu0 - " + node.id());
      console.log("cucu0 - " + node.isParent());
      return;
    }
    console.log("cucu1 - " + this.hoveredNode?.id());
    console.log("cucu1 - " + node.id());
    if (this.hoveredNode) {
      console.log("cucu2");
      this.unhighlightNode(this.hoveredNode);
    }
    console.log("cucu3 - " + this.hoveredNode?.id());
    console.log("cucu3 - " + node.id());
    this.hoveredNode = node;
    console.log("cucu4 - " + this.hoveredNode?.id());
    console.log("cucu4 - " + node.id());
    this.hoveredNode.addClass('hoveredCommon hovered');
    this.hoveredNode.incomers('node').forEach((depNode: NodeSingular) => {
      depNode.addClass('hoveredCommon ');
      // WTF? why it might be null?
      if (this.hoveredNode!.outgoers('node').has(depNode)) {
        depNode.addClass('hoveredHighlight');
      } else {
        depNode.addClass('hoveredIncomers');
      }
    });
    this.hoveredNode.outgoers('node').forEach((depNode: NodeSingular) => {
      depNode.addClass('hoveredCommon hoveredOutgoers');
    });

    this.hoveredNode.connectedEdges().forEach((edge: EdgeSingular) => {
      edge.addClass('highlighted-edge');
    });
  }

  private unhighlightNode(node: NodeSingular) {
    if (node.isParent()) {
      return;
    }
    if (this.isDivedIn) {
      this.isDivedIn = false;
      return;
    }
    console.log("lost focus on - " + node.id());
    node.removeClass('hoveredCommon hovered');

    node.outgoers('node').forEach((depNode: NodeSingular) => {
      depNode.removeClass('hoveredCommon hoveredOutgoers hoveredHighlight');
    });
    node.incomers('node').forEach((depNode: NodeSingular) => {
      depNode.removeClass('hoveredCommon hoveredIncomers');
    });

    node.connectedEdges().forEach((edge: EdgeSingular) => {
      edge.removeClass('highlighted-edge');
    });
    this.hoveredNode = null;
  }

  private selectNode(node: NodeSingular) {
    if (node.isParent() || node.hasClass('nodeLeaf')) {
      return;
    }
    this.hoveredNode = null;
    this.graphDiveIn(node.id());
    console.log("dived in - " + node.id());
  }

  onZoomIn(_event: MouseEvent) {
    this.cy.animate({
      zoom: this.cy.zoom() + 0.1,
      duration: 300,
    });
  }

  onZoomOut(_event: MouseEvent) {
    this.cy.animate({
      zoom: this.cy.zoom() - 0.1,
      duration: 300,
    });
  }

  onZoomFit(_event?: MouseEvent) {
    this.cy.animate({
      fit: {
        eles: this.cy.elements(),
        padding: 10,
      },
      duration: 300,
    });
  }

  private graphDiveIn(nodeId: string) {
    this.cy.elements().addClass('hidden');

    setTimeout(() => {
      this.cy.elements().remove();

      const newElements = this.getElementsByNodeId(nodeId);
      this.cy.add(newElements);

      this.cy.elements().addClass('hidden');

      this.cy.layout(this.fcoseLayoutOptions as FcoseLayoutOptions).run();
      this.cy.fit();

      this.updateNavMenuItems(nodeId);
      setTimeout(() => {
        this.cy.elements().removeClass('hidden');
        const newRootNode = this.cy.$(`#${nodeId}`);
        if (newRootNode) {
          this.highlightNode(newRootNode);
        }
      }, 500);
    }, 350);
    this.isDivedIn = true;
  }

  private getElementsByNodeId(nodeId: string): ElementDefinition[] {
    const sbomDetailDtos: SbomReportDetailDto[] = [];
    const rootSbomDto = this.dataDtos?.details?.find((x) => x.bomRef == nodeId);
    if (rootSbomDto) {
      sbomDetailDtos.push(rootSbomDto);
      this.getSbomDtos(rootSbomDto, sbomDetailDtos);
    }

    const groupMap = new Map<string, number>();
    sbomDetailDtos.forEach((sbomDetailDto) => {
      if (sbomDetailDto.purl?.startsWith('pkg:nuget/')) {
        const potentialNs = sbomDetailDto.name?.split('.')[0] ?? 'unknown';
        const currentCount = (groupMap.get(potentialNs) || 0) + 1;
        groupMap.set(potentialNs, currentCount);
      }
    });

    const elements: ElementDefinition[] = [];
    sbomDetailDtos.forEach((sbomDetailDto) => {
      if (sbomDetailDto) {
        let parentId: string | undefined = undefined;
        if (sbomDetailDto.purl?.startsWith('pkg:nuget/')) {
          // && !sbomDetailDto.name.includes("Runtime.linux-x64")
          const potentialNs = sbomDetailDto.name?.split('.')[0] ?? 'unknown';
          if ((groupMap.get(potentialNs) || 0) > 1) {
            elements.push({ data: { id: potentialNs, label: potentialNs }, classes: 'nodeCommon' });
            parentId = potentialNs;
          }
        }
        elements.push({
          data: {
            id: sbomDetailDto.bomRef,
            label: sbomDetailDto.name ?? '',
            parent: parentId,
          },
          classes: `nodeCommon nodePackage ${sbomDetailDto.dependsOn?.length ? 'nodeBranch' : 'nodeLeaf'}`,
        });
        sbomDetailDto.dependsOn?.forEach((depends) => {
          elements.push({
            data: {
              source: sbomDetailDto.bomRef,
              target: depends,
            },
            classes: 'edgeCommon',
          });
        });
      }
    });

    return elements;
  }

  private getSbomDtos(sbomDetailDto: SbomReportDetailDto, sbomDetailDtos: SbomReportDetailDto[]) {
    if (!sbomDetailDto) {
      return;
    }
    const detailBomRefIds = sbomDetailDto.dependsOn;
    if (!detailBomRefIds) {
      return;
    }
    const newDetailBomRefIds: string[] = [];
    detailBomRefIds.forEach((bomRefId) => {
      if (!sbomDetailDtos.find((x) => x.bomRef === bomRefId)) {
        newDetailBomRefIds.push(bomRefId);
      }
    });
    const newSbomDetailDtos =
      this.dataDtos?.details?.filter((x) => newDetailBomRefIds.includes(x.bomRef ?? '')) ?? [];
    sbomDetailDtos.push(...newSbomDetailDtos);
    newSbomDetailDtos.forEach((sbomDetailDto) => this.getSbomDtos(sbomDetailDto, sbomDetailDtos));
  }

  onNavItemClick(event: BreadcrumbItemClickEvent) {
    console.log(event.item);
    if (event.item.icon) {
      this.graphDiveIn(this.rootNodeId);
      return;
    }
    if (event.item.id) {
      this.graphDiveIn(event.item.id)
    }
  }

  private updateNavMenuItems(nodeId: string) {
    if (this.selectedRootNodeId === nodeId) {
      return;
    }

    if (this.rootNodeId === nodeId) {
      this.selectedRootNodeId = nodeId;
      this.navItems = [];
      return;
    }

    const potentialIndex = this.navItems.map(x => x.id).indexOf(nodeId);
    if (potentialIndex !== -1) {
      this.navItems = this.navItems.slice(0, potentialIndex + 1);
      this.navItems[potentialIndex].styleClass = "breadcrumb-size";
      this.selectedRootNodeId = nodeId;
      return;
    }

    if (this.navItems.length > 0) {
      this.navItems[this.navItems.length - 1].styleClass = "breadcrumb-pointer";
    }
    const newDataDetailDto = this.getDataDetailDtoById(nodeId);
    this.navItems = [...this.navItems, {
      id: nodeId,
      label: newDataDetailDto?.name ?? "no-name",
      styleClass: 'breadcrumb-size',
    }];
    this.selectedRootNodeId = nodeId;
  }

  private getDataDetailDtoById(id: string): SbomReportDetailDto | undefined {
    return this.dataDtos?.details?.find((x) => x.bomRef == id);
  }
}
