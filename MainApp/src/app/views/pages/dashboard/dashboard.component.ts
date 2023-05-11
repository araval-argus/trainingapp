import { Component, OnInit, ViewChild } from "@angular/core";

import {
  ChartComponent,
  ApexAxisChartSeries,
  ApexNonAxisChartSeries,
  ApexGrid,
  ApexChart,
  ApexXAxis,
  ApexYAxis,
  ApexMarkers,
  ApexStroke,
  ApexLegend,
  ApexResponsive,
  ApexTooltip,
  ApexFill,
  ApexDataLabels,
  ApexPlotOptions,
  ApexTitleSubtitle,
} from "ng-apexcharts";

import { NgbDateStruct, NgbCalendar } from "@ng-bootstrap/ng-bootstrap";

// Ng2-charts
import {
  ChartType,
  ChartDataSets,
  RadialChartOptions,
  ChartConfiguration,
} from "chart.js";
import { Label, Color, SingleDataSet } from "ng2-charts";

// Progressbar.js
import ProgressBar from "progressbar.js";
import { NotificationService } from "src/app/core/service/notification.service";
import { LoggedInUserModel } from "src/app/core/models/loggedin-user";
import { AuthService } from "src/app/core/service/auth-service";
import { messageDataService } from "src/app/core/service/message-data-service";

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
  selector: "app-dashboard",
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.scss"],
  preserveWhitespaces: true,
})
export class DashboardComponent implements OnInit {
  @ViewChild("chart") chart: ChartComponent;
  public chartOptions: Partial<ChartOptions>;

  constructor(private messageDataService: messageDataService) {
  }

  ngOnInit() {
    this.messageDataService.fetchInsights().subscribe((data: any) => {
      this.chartOptions = {
        series: [
          {
            name: "Personal Messages",
            data: data.personalMessagesCount,
          },
          {
            name: "Group Messages",
            data: data.groupMessagesCount,
          },
        ],
        chart: {
          height: 400,
          type: "line",
        },
        dataLabels: {
          enabled: false,
        },
        stroke: {
          width: 5,
          curve: "straight",
          dashArray: [0, 11, 2],
        },
        title: {
          text: "No. of messages per day",
          align: "center",
        },
        legend: {
          tooltipHoverFormatter: function (val, opts) {
            return (
              val +
              " - <strong>" +
              opts.w.globals.series[opts.seriesIndex][opts.dataPointIndex] +
              "</strong>"
            );
          },
        },
        markers: {
          size: 0,
          hover: {
            sizeOffset: 6,
          },
        },
        xaxis: {
          labels: {
            trim: false,
          },
          categories: data.datesToBeSent,
        },
        tooltip: {
          y: [
            {
              title: {
                formatter: function (val) {
                  return val;
                },
              },
            },
            {
              title: {
                formatter: function (val) {
                  return val;
                },
              },
            },
          ],
        },
        grid: {
          borderColor: "#f1f1f1",
        },
      };
    });
  }
}

// import { Component, ViewChild } from "@angular/core";

// import {
//   ChartComponent,
//   ApexAxisChartSeries,
//   ApexChart,
//   ApexXAxis,
//   ApexDataLabels,
//   ApexStroke,
//   ApexMarkers,
//   ApexYAxis,
//   ApexGrid,
//   ApexTitleSubtitle,
//   ApexLegend
// } from "ng-apexcharts";
