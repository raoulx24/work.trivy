import { Component, ElementRef, ViewChild } from '@angular/core';

import cytoscape, { EdgeSingular, ElementDefinition, NodeSingular } from 'cytoscape';
import fcose, { FcoseLayoutOptions } from 'cytoscape-fcose';
cytoscape.use(fcose);

import { ButtonModule } from 'primeng/button';

// tests.sbom
import { SbomReportService } from '../../api/services/sbom-report.service';
import { SbomReportDto } from '../../api/models/sbom-report-dto';
import { SbomReportDetailDto } from '../../api/models/sbom-report-detail-dto';
//

@Component({
  selector: 'app-fcose',
  standalone: true,
  imports: [ButtonModule],
  templateUrl: './fcose.component.html',
  styleUrl: './fcose.component.scss'
})
export class FcoseComponent {
  @ViewChild('graphContainer', { static: true }) graphContainer!: ElementRef;
  private cy!: cytoscape.Core;
  private get hoveredNode(): NodeSingular | null {
    return this._hoveredNode;
  }
  private set hoveredNode(node: NodeSingular | null) {
    this._hoveredNode = node;
    if (node) {
      const x = this.dataDtos[0].details?.find(x => x.bomRef == node.id());
      if (x) {
        this.testText = `<b>Name:</b> ${x.name} - <b>Version:</b> ${x.version} - <b>Dependencies:</b> ${x.dependsOn?.length ?? 0}`;
      }
    }
    else {
      this.testText = "no info..."
    }
  }
  private _hoveredNode: NodeSingular | null = null;



  testText: string = "";

  private fcoseLayoutOptions: FcoseLayoutOptions = {
    name: "fcose",
    nodeRepulsion: (node: NodeSingular) => { return 20000 },
    numIter: 2500,
    animate: false,
    fit: true,
    padding: 10,
    sampleSize: 50,
    nodeSeparation: 500,
    tilingPaddingHorizontal: 1000,
    tilingPaddingVertical: 1000,
    idealEdgeLength: (edge: EdgeSingular) => { return 150; },
    edgeElasticity: (edge: EdgeSingular) => { return .15; }
  };


  constructor(private service: SbomReportService) {
    // tests.sbom
    this.getTableDataDtos();
    //
  }


  // tests.sbom
  getTableDataDtos() {
    this.service.getSbomReportDtos().subscribe({
      next: (res) => this.onGetDataDtos(res),
      error: (err) => console.error(err),
    });
  }

  onGetDataDtos(dtos: SbomReportDto[]) {
    this.dataDtos = dtos;

    const elements: ElementDefinition[] = this.getElementsByNodeId("00000000-0000-0000-0000-000000000000");

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
          }
        },
        {
          selector: '.nodeCommon',
          style: {
            'opacity': 1,
            'transition-property': 'opacity',
            'transition-duration': 300,
            'border-width': 1,
          }
        },
        {
          selector: '.nodePackage',
          style: {
            'label': 'data(label)',
            'width': 'mapData(label.length, 1, 30, 20, 200)',
            'height': '20px',
            'background-color': 'Aqua',
            'text-valign': 'center',
            'text-halign': 'center',
            'text-wrap': 'ellipsis',
            'text-max-width': '200px',
            'font-size': '10px',
            'border-color': '#000',
            'transition-property': 'width height background-color font-size border-color opacity',
            'transition-duration': 300,
          }
        },
        {
          selector: '.nodeBranch',
          style: {
            'shape': 'roundrectangle',
          }
        },
        {
          selector: '.nodeLeaf',
          style: {
            'shape': 'cut-rectangle',
          }
        },
        {
          selector: '.edgeCommon',
          style: {
            'width': 1,
            'line-color': '#ccc',
            'target-arrow-color': '#ccc',
            'target-arrow-shape': 'triangle',
            'transition-property': 'width line-color opacity',
            'transition-duration': 300,
            'opacity': 1,
          }
        },
        {
          selector: '.hoveredCommon',
          style: {
            'width': 'mapData(label.length, 1, 30, 20, 240)',
            'height': '24px',
            'font-size': '12px',
            'transition-property': 'width height background-color font-size border-color opacity',
            'transition-duration': 300,
          }
        },
        {
          selector: '.hovered',
          style: {
            'background-color': 'Silver',
          }
        },
        {
          selector: '.hoveredOutgoers',
          style: {
            'background-color': 'DeepSkyBlue',
          }
        },
        {
          selector: '.hoveredIncomers',
          style: {
            'background-color': 'RoyalBlue',
          }
        },
        {
          selector: '.hoveredHighlight',
          style: {
            'overlay-opacity': 0.5,
            'overlay-color': 'RoyalBlue',
            'font-style': 'italic',
          }
        },
        {
          selector: '.highlighted-edge',
          style: {
            'width': 3,
            "line-color": "Violet",
            'transition-property': 'line-color opacity',
            'transition-duration': 300,
          }
        },
        {
          selector: '.hidden',
          style: {
            'opacity': 0,
          }
        }
      ]
    });

    this.setupCyEvents();
  }

  setupCyEvents() {
    this.cy.on('mouseover', 'node', (event) => {
      if (this.hoveredNode || event.target.isParent()) {
        return;
      }
      this.hoveredNode = event.target;
      if (!this.hoveredNode) {
        return;
      }
      this.hoveredNode.addClass('hoveredCommon hovered');
      this.hoveredNode.incomers('node').forEach((depNode: NodeSingular) => {
        depNode.addClass('hoveredCommon ');
        // WTF? why it might be null?
        if (this.hoveredNode!.outgoers('node').has(depNode)) {
          depNode.addClass('hoveredHighlight');
        }
        else {
          depNode.addClass('hoveredIncomers');
        }
      });
      this.hoveredNode.outgoers('node').forEach((depNode: NodeSingular) => {
        depNode.addClass('hoveredCommon hoveredOutgoers');
      });

      this.hoveredNode.connectedEdges().forEach((edge: EdgeSingular) => {
        edge.addClass('highlighted-edge');
      });
    });

    this.cy.on('mouseout', 'node', (event) => {
      const node: NodeSingular = event.target;
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
    });

    this.cy.on('dblclick', 'node', (event) => {
      this.onDiveIn(event.target.id());
    });

    //this.cy.on('click', 'node', (event) => {
    //  const node = event.target;
    //  console.log('Single-clicked on node:', node.id());
    //});
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
      duration: 300
    });
  }

  onZoomFit(_event?: MouseEvent) {
    this.cy.animate({
      fit: {
        eles: this.cy.elements(),
        padding: 10,
      },
      duration: 300
    });
  }

  onResetView(_event: MouseEvent) {
    this.onDiveIn("00000000-0000-0000-0000-000000000000");
  }

  onDiveIn(nodeId: string) {
    this.cy.elements().addClass('hidden');

    setTimeout(() => {
      this.cy.elements().remove();

      const newElements = this.getElementsByNodeId(nodeId);
      this.cy.add(newElements);

      this.cy.elements().addClass('hidden');

      this.cy.layout(this.fcoseLayoutOptions as FcoseLayoutOptions).run();
      this.cy.fit();

      setTimeout(() => {
        this.cy.elements().removeClass('hidden');
      }, 500);
    }, 350);
  }

  private getElementsByNodeId(nodeId: string): ElementDefinition[] {
    const sbomDetailDtos: SbomReportDetailDto[] = [];
    const rootSbomDto = this.dataDtos[0].details?.find(x => x.bomRef == nodeId);
    if (rootSbomDto) {
      sbomDetailDtos.push(rootSbomDto);
      this.getSbomDtos(rootSbomDto, sbomDetailDtos);
    }

    const groupMap = new Map<string, number>();
    sbomDetailDtos.forEach(sbomDetailDto => {
      if (sbomDetailDto.purl?.startsWith("pkg:nuget/")) {
        const potentialNs = sbomDetailDto.name?.split('.')[0] ?? "unknown";
        const currentCount = (groupMap.get(potentialNs) || 0) + 1;
        groupMap.set(potentialNs, currentCount);
        }
    });

    const elements: ElementDefinition[] = [];
    sbomDetailDtos.forEach(sbomDetailDto => {
      if (sbomDetailDto) {
        let parentId: string | undefined = undefined;
        if (sbomDetailDto.purl?.startsWith("pkg:nuget/")) {  // && !sbomDetailDto.name.includes("Runtime.linux-x64")
          const potentialNs = sbomDetailDto.name?.split('.')[0] ?? "unknown";
          if ((groupMap.get(potentialNs) || 0) > 1) {
            elements.push({ data: { id: potentialNs, label: potentialNs }, classes: 'nodeCommon' });
            parentId = potentialNs;
          };
        };
        elements.push({
          data: {
            id: sbomDetailDto.bomRef,
            label: sbomDetailDto.name ?? "",
            parent: parentId,
          },
          classes: `nodeCommon nodePackage ${sbomDetailDto.dependsOn?.length ? 'nodeBranch' : 'nodeLeaf'}`,
        });
        sbomDetailDto.dependsOn?.forEach(depends => {
            elements.push({
              data: {
                source: sbomDetailDto.bomRef,
                target: depends
              },
              classes: 'edgeCommon'
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
    detailBomRefIds.forEach(bomRefId => {
      if (!sbomDetailDtos.find(x => x.bomRef === bomRefId)) {
        newDetailBomRefIds.push(bomRefId);
      }
    });
    const newSbomDetailDtos = this.dataDtos[0].details?.filter(x => newDetailBomRefIds.includes(x.bomRef ?? "")) ?? [];
    sbomDetailDtos.push(...newSbomDetailDtos);
    newSbomDetailDtos.forEach(sbomDetailDto => this.getSbomDtos(sbomDetailDto, sbomDetailDtos));
  }

  // tests sbom
  private dataDtos: SbomReportDto[] = [];
  //
}
