import { Component, OnInit, ViewChild } from '@angular/core';

import { ApexAxisChartSeries, ApexNonAxisChartSeries, ApexGrid, ApexChart, ApexXAxis, ApexYAxis, ApexMarkers, ApexStroke, ApexLegend, ApexResponsive, ApexTooltip, ApexFill, ApexDataLabels, ApexPlotOptions, ApexTitleSubtitle, ChartComponent } from 'ng-apexcharts';

// import { NgbDateStruct, NgbCalendar } from '@ng-bootstrap/ng-bootstrap';

// Ng2-charts
// import {ChartType, ChartDataSets, RadialChartOptions } from 'chart.js';
// import { Label, Color, SingleDataSet } from 'ng2-charts';

// Progressbar.js
// import ProgressBar from 'progressbar.js';
import { SampleService } from 'src/app/core/service/sample-service';
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
  tooltip: ApexTooltip;
  dataLabels: ApexDataLabels;
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

  constructor(private sampleService: SampleService , private signalRService:SignalRService,private authService:AuthService,private dashboardService:DashBoardService) {}
  ngOnInit(): void {
   // this.sampleService.runSampleAPI().subscribe((result) => {
     // console.log(result);
   // }, (err) => {
      this.signalRService.startConnection(this.authService.getLoggedInUserInfo().userName);
   // })
  // }
    this.dashboardService.getChartDetails().subscribe((data:any)=>{
      {
        this.chartOptions = {
          series: [
            {
              name: "PersonalChat",
              data: data.chat,
            },
            {
              name: "GroupChat",
              data: data.group,
            },
          ],
          chart: {
            height: 350,
            type: "area",
          },
          dataLabels: {
            enabled: false,
          },
          stroke: {
            curve: "smooth",
          },
          xaxis: {
            type: "datetime",
            categories: data.dates,
          },
          tooltip: {
            x: {
              format: "dd/MM/yy",
            },
          },
        };
      }
    });
  }
}
