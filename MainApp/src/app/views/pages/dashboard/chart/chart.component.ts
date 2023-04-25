import { Component, OnInit, ViewChild } from '@angular/core';

import {
  ApexChart,
  ApexAxisChartSeries,
  ApexTitleSubtitle,
  ApexDataLabels,
  ApexFill,
  ApexYAxis,
  ApexXAxis,
  ApexTooltip,
  ApexMarkers,
  ApexAnnotations,
  ApexStroke,
  ApexOptions,
  ApexGrid,
  ApexLegend,
  ApexNonAxisChartSeries,
  ApexPlotOptions,
  ApexResponsive
} from "ng-apexcharts";


import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { GroupService } from 'src/app/core/service/group-service';


export type apexChartOptions = {
  series: ApexAxisChartSeries;
  nonAxisSeries: ApexNonAxisChartSeries;
  colors: string[];
  grid: ApexGrid;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis;
  markers: ApexMarkers,
  stroke: ApexStroke,
  legend: ApexLegend,
  responsive: ApexResponsive[],
  tooltip: ApexTooltip,
  fill: ApexFill
  dataLabels: ApexDataLabels,
  plotOptions: ApexPlotOptions,
  labels: string[],
  title: ApexTitleSubtitle
};


@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.scss']
})
export class ChartComponent {
  public series: ApexAxisChartSeries;
  public chart: ApexChart;
  public dataLabels: ApexDataLabels;
  public markers: ApexMarkers;
  public title: ApexTitleSubtitle;
  public fill: ApexFill;
  public yaxis: ApexYAxis;
  public xaxis: ApexXAxis;
  public tooltip: ApexTooltip;
  public legend: ApexLegend;
  public stroke: ApexStroke;

  dataSeries = [];

  groupDataSeries = [];

  constructor(private authService: AuthService, private chatService: ChatService, private groupService: GroupService) { }

  ngOnInit(): void {
    this.getChatData();
    this.GetGroupChatData();
    this.initChartData();
  }

  getChatData() {
    this.chatService.getChatData().subscribe(
      (data: []) => {
        this.dataSeries = data;
        this.initChartData();
      }
    )
  }

  GetGroupChatData() {
    this.groupService.getChatData().subscribe(
      (data: []) => {
        this.groupDataSeries = data;
        this.initChartData();

      }
    )
  }

  public initChartData(): void {
    let dates = [];
    for (let i = 0; i < this.dataSeries.length; i++) {
      dates.push([this.dataSeries[i].date, this.dataSeries[i].value]);
    }

    let dates2 = [];
    for (let i = 0; i < this.groupDataSeries.length; i++) {
      dates2.push([this.groupDataSeries[i].date, this.groupDataSeries[i].value]);
    }

    this.series = [
      {
        name: "Messages",
        data: dates
      },
      {
        name: "Group Messages",
        data: dates2
      }
    ];

    this.chart = {
      type: "line",
      height: 350
    };

    this.dataLabels = {
      enabled: false
    };

    this.title = {
      text: "All Chat Data",
      align: "left"
    };

    this.legend = {
      tooltipHoverFormatter: function (val, opts) {
        return (
          val +
          " - <strong>" +
          opts.w.globals.series[opts.seriesIndex][opts.dataPointIndex] +
          "</strong>"
        );
      }
    };

    this.markers = {
      size: 0,
      hover: { sizeOffset: 6 }
    };

    this.tooltip = {
      y: [
        {
          title: {
            formatter: function (val) {
              return val;
            }
          }
        },
        {
          title: {
            formatter: function (val) {
              return val;
            }
          }
        }
      ]
    },

      this.xaxis = {
        labels: {
          trim: false
        },
        type: "datetime"
      };
  }
}
