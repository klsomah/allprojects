function barChart()
{    
    var ctx = document.getElementById('barChart').getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
            datasets: [{
                label: '# of Votes',
                data: [12, 19, 3, 5, 2, 3],
                backgroundColor: [
                    'rgba(255, 99, 132, 0.2)',
                    'rgba(54, 162, 235, 0.2)',
                    'rgba(255, 206, 86, 0.2)',
                    'rgba(75, 192, 192, 0.2)',
                    'rgba(153, 102, 255, 0.2)',
                    'rgba(255, 159, 64, 0.2)'
                ],
                borderColor: [
                    'rgba(255, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });
}

function lineChart(){
    var ctx = document.getElementById('lineChart').getContext('2d');
var chart = new Chart(ctx, {
    // The type of chart we want to create
    type: 'line',

    // The data for our dataset
    data: {
        labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
        datasets: [{
            label: 'My First dataset',
            borderColor: 'red',
            data: [0, 10, 5, 2, 20, 30, 45]
        },
        {
            label: 'My Second dataset',
            borderColor: 'blue',
            data: [20,30,15,25,5,40,45]
        }
    ]
    },

    // Configuration options go here
    options: {}
});
}

function scatterChart(){
    var ctx = document.getElementById('scatterChart').getContext('2d');

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
            title: {
                display: true,
                text: 'Scatter Chart'
            },
        }
    });



}

function comboBarLine(){
    
		var chartData = {
			labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
			datasets: [{
				type: 'line',
				label: 'Dataset 1',
				borderColor: "blue",
				borderWidth: 2,
				fill: false,
				data: [
					10,20,30,15,25,35,50,80,45					
				]
			}, {
				type: 'bar',
				label: 'Dataset 2',
				backgroundColor: "red",
				data: [
					50,30,25,60,15,35,65
				],
				borderColor: 'white',
				borderWidth: 2
			}, {
				type: 'bar',
				label: 'Dataset 3',
				backgroundColor: "green",
				data: [
                    40,50,35,20,60,70,10
				]
			}]

        };

    var ctx = document.getElementById('combo-bar-line').getContext('2d');
    window.myMixedChart = new Chart(ctx, {
        type: 'bar',
        data: chartData,
        options: {
            responsive: true,
            title: {
                display: true,
                text: 'Chart.js Combo Bar Line Chart'
            },
            tooltips: {
                mode: 'index',
                intersect: true
            }
        }
    });
}

$(document).ready(function() {
    lineChart();
    barChart();
    scatterChart();
    comboBarLine();
} );