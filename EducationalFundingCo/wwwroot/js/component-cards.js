function renderChart1(){
      rgRenderChart.chartJS.pieChart('statistics-card-chart');
	}

  function renderChart2(){		
		rgRenderChart.chartJS.doughnutChart('statistics-card-chart2');
  }
  
  function renderChart3(){    
    rgRenderChart.chartJS.polarChart('statistics-card-chart3');
    
}

function renderChart4(){
    rgRenderChart.chartJS.line2Chart('chart4');
}

function renderChart5(){
  rgRenderChart.chartJS.line1Chart('chart5');
}

function amBarChart(){
  rgRenderChart.amChart.barChart("amBarChart");
}


  window.onload = function() {
    renderChart1();
    renderChart2();
    renderChart3();
    renderChart4();
    renderChart5();
    amBarChart();
    document.getElementById("page-loader").style.display = "none";
		};

  
  