
  		var randomScalingFactor = function() {
			return Math.round(Math.random() * 100);
		};

  function renderChart1(){
		var config = {
			type: 'pie',
			data: {
				datasets: [{
					data: [
						randomScalingFactor(),
						randomScalingFactor(),
						randomScalingFactor(),
						randomScalingFactor(),
						randomScalingFactor(),
					],
					backgroundColor: [
						"red",
						"orange",
						"yellow",
						"green",
						"blue",
					],
					label: 'Dataset 1'
				}],
				labels: [
					'Red',
					'Orange',
					'Yellow',
					'Green',
					'Blue'
				]
			},
			options: {
				responsive: true,
        legend: {
            display: false
        }
			}
		};	
		var ctx = document.getElementById('chart1').getContext('2d');
			window.myPie = new Chart(ctx, config);
	}

  function renderChart2(){
		var chart2config = {
			type: 'doughnut',
			data: {
				datasets: [{
					data: [
						randomScalingFactor(),
						randomScalingFactor(),
						randomScalingFactor(),
						randomScalingFactor(),
						randomScalingFactor(),
					],
					backgroundColor: [
						"pink",
						"brown",
						"coral",
						"cyan",
						"blue",
					],
					label: 'Dataset 1'
				}],
				labels: [
        "pink",
						"brown",
						"coral",
						"cyan",
						"blue",
				]
			},
			options: {
				responsive: true,
        legend: {
            display: false
        }
			}
		};

    var ctx1 = document.getElementById('chart2').getContext('2d');
    window.myPie1 = new Chart(ctx1, chart2config);
		
  }
  
  function renderChart3(){
    var ctx = document.getElementById('chart3').getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Red', 'Blue', 'Yellow'],
            datasets: [{
                label: '# of Votes',
                data: [12, 19, 3, 5, 2, 3],
                backgroundColor: [
                    'rgba(255, 99, 132, 0.2)',
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(255, 206, 86, 0.2)'
                ],
                borderColor: [
                    'rgba(255, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
          legend: {
            display: false
          }
        }
    });
}

function renderChart4(){
    var ctx = document.getElementById('chart4').getContext('2d');

    var scatterChartData = {
        datasets: [{
            label: 'My First dataset',
            borderColor: "red",
            backgroundColor: "red",
            data: [{x:10,y:15},{x:30,y:25},{x:14,y:35},{x:10,y:50},{x:30,y:45}]
        }, {
            label: 'My Second dataset',
            borderColor: "blue",
            backgroundColor: "blue",
            data: [{x:15,y:10},{x:53,y:25},{x:12,y:26},{x:15,y:15},{x:31,y:15}]
        }]
    };

    window.myScatter = Chart.Scatter(ctx, {
        data: scatterChartData,
        options: {
          legend: {
            display: false
          }
        }
    });



}


  window.onload = function() {
    renderChart1();
    renderChart2();
    renderChart3();
	renderChart4();
	document.getElementById("page-loader").style.display = "none";
		};

  
  