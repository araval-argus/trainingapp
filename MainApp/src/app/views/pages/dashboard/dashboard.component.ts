import { Component, OnInit, ViewChild } from '@angular/core';

import { ApexAxisChartSeries, ApexGrid, ApexChart, ApexXAxis, ApexYAxis, ApexMarkers, ApexStroke, ApexLegend, ApexDataLabels, ApexTitleSubtitle, ChartComponent } from 'ng-apexcharts';

// import { NgbDateStruct, NgbCalendar } from '@ng-bootstrap/ng-bootstrap';

// Ng2-charts
// import {ChartType, ChartDataSets, RadialChartOptions } from 'chart.js';
// import { Label, Color, SingleDataSet } from 'ng2-charts';

// Progressbar.js
// import ProgressBar from 'progressbar.js';
import { SignalRService } from 'src/app/core/service/signalr-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { DashBoardService } from 'src/app/core/service/dashboard-service';

// export type apexChartOptions = {
//   series: ApexAxisChartSeries;
//   nonAxisSeries: ApexNonAxisChartSeries;
//   colors: string[];
//   grid: ApexGrid;
//   chart: ApexChart;
//   xaxis: ApexXAxis;
//   yaxis: ApexYAxis;
//   markers: ApexMarkers,
//   stroke: ApexStroke,
//   legend: ApexLegend,
//   responsive: ApexResponsive[],
//   tooltip: ApexTooltip,
//   fill: ApexFill
//   dataLabels: ApexDataLabels,
//   plotOptions: ApexPlotOptions,
//   labels: string[],
//   title: ApexTitleSubtitle
// };

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  stroke: ApexStroke;
  dataLabels: ApexDataLabels;
  markers: ApexMarkers;
  tooltip: any; // ApexTooltip;
  yaxis: ApexYAxis;
  grid: ApexGrid;
  legend: ApexLegend;
  title: ApexTitleSubtitle;
};

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  preserveWhitespaces: true
})

export class DashboardComponent implements OnInit {

  @ViewChild("chart") chart: ChartComponent;
  public chartOptions: Partial<ChartOptions>;

  constructor(private signalRService:SignalRService,private authService:AuthService,private dashboardService:DashBoardService) {}
  ngOnInit(): void {
   // this.sampleService.runSampleAPI().subscribe((result) => {
     // console.log(result);
   // }, (err) => {
      this.signalRService.startConnection(this.authService.getLoggedInUserInfo().userName);
   // })
  // }
    this.dashboardService.getChartDetails().subscribe((data:any)=>{

      this.chartOptions = {
        series: [
          {
            name: "Total Message",
            data: data.total
          },
          {
            name: "Chat Message",
            data: data.chat
          },
          {
            name: "Group Message",
            data: data.group
          }
        ],
        chart: {
          height: 350,
          type: "line"
        },
        dataLabels: {
          enabled: false
        },
        stroke: {
          width: 5,
          curve: "smooth",
          dashArray: [0, 8, 5]
        },
        title: {
          text: "Message Statistics",
          align: "left"
        },
        legend: {
          tooltipHoverFormatter: function(val, opts) {
            return (
              val +
              " - <strong>" +
              opts.w.globals.series[opts.seriesIndex][opts.dataPointIndex] +
              "</strong>"
            );
          }
        },
        markers: {
          size: 0,
          hover: {
            sizeOffset: 6
          }
        },
        xaxis: {
          labels: {
            trim: false
          },
          categories: data.dates
        },
        tooltip: {
          y: [
            {
              title: {
                formatter: function(val) {
                  return val ;
                }
              }
            },
            {
              title: {
                formatter: function(val) {
                  return val;
                }
              }
            },
            {
              title: {
                formatter: function(val) {
                  return val;
                }
              }
            }
          ]
        },
        grid: {
          borderColor: "#f1f1f1"
        }
      };
    })}}


