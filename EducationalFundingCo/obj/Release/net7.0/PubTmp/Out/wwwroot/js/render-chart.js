function renderChart(elementId,configurations){
    var ctx = document.getElementById(elementId).getContext('2d');
    new Chart(ctx, configurations);
}
function renderPieChart(elementId,configurations){
    configurations = configurations || chartData.statisticPieChartCard;
    renderChart(elementId,configurations);
}
function renderDoughnutChart(elementId,configurations){
    configurations = configurations||chartData.statisticDoughnutChartCard;
    renderChart(elementId,configurations);
}

function renderPolarChart(elementId,configurations){
    configurations = configurations||chartData.statisticPolarChartCard;
    var optionsDailysales = {
        legend: false
    }
    var dailySales = document.getElementById(elementId);
    new Chart(dailySales, { type: 'polarArea', data: configurations, options: optionsDailysales });
}

function renderLine2Chart(elementId,configurations){
    configurations = configurations||chartData.statistic2LineChartCard;
    renderChart(elementId,configurations);
}

function renderLine1Chart(elementId,configurations){
    configurations = configurations||chartData.statistic1LineChartCard;
    renderChart(elementId,configurations);
}

function renderAmBarChart(elementId){
    am4core.ready(function() {

        // Themes begin
        am4core.useTheme(am4themes_animated);
        // Themes end
        
        var chart = am4core.create(elementId, am4charts.XYChart);
        chart.hiddenState.properties.opacity = 0; // this creates initial fade-in
        
        chart.data = [
          {
            country: "Jul",
            visits: 1500
          },
          {
            country: "Aug",
            visits: 1652
          },
          {
            country: "Sept",
            visits: 1068
          },
          {
            country: "Oct",
            visits: 690
          }
        ];
        
        var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
        categoryAxis.renderer.grid.template.location = 0;
        categoryAxis.dataFields.category = "country";
        categoryAxis.renderer.minGridDistance = 40;
        categoryAxis.fontSize = 11;
        
        var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
        valueAxis.min = 0;
        valueAxis.max = 3000;
        valueAxis.strictMinMax = true;
        valueAxis.renderer.minGridDistance = 30;
        
        var series = chart.series.push(new am4charts.ColumnSeries());
        series.dataFields.categoryX = "country";
        series.dataFields.valueY = "visits";
        series.columns.template.tooltipText = "{valueY.value}";
        series.columns.template.tooltipY = 0;
        series.columns.template.strokeOpacity = 0;
        
        // as by default columns of the same series are of the same color, we add adapter which takes colors from chart.colors color set
        series.columns.template.adapter.add("fill", function(fill, target) {
          return chart.colors.getIndex(target.dataItem.index);
        });
        
        }); // end am4core.ready()
}

var rgRenderChart={
    chartJS:{
        pieChart:renderPieChart,
        doughnutChart:renderDoughnutChart,
        polarChart:renderPolarChart,
        line1Chart:renderLine1Chart,
        line2Chart:renderLine2Chart
    },
    amChart:{
        barChart:renderAmBarChart
    }
}