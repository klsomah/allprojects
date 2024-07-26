var randomScalingFactor = function() {
    return Math.round(Math.random() * 100);
  };

var chartColorsSet1=["#59388b","#7688aa","#59388b","#a649e0","#5688ec"];
var chartColorsSet2=["#5688ec","#a649e0","#59388b","#ff4444","#ff9f40"];

var chartData={
    statisticPieChartCard:{
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
                backgroundColor: chartColorsSet1,
                label: 'Dataset 1'
            }],
            labels: chartColorsSet1
        },
        options: {
            responsive: true,
    legend: {
        display: false
    }
        }
    },
    statisticDoughnutChartCard:{
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
                backgroundColor: chartColorsSet1,
                label: 'Dataset 1'
            }],
            labels: chartColorsSet1
        },
        options: {
            responsive: true,
    legend: {
        display: false
    }
        }
    },
    statisticPolarChartCard:{
        datasets: [{
            data: [11,16,7,3,14],
            backgroundColor: chartColorsSet1,
            label: 'My dataset' // for legend
        }],
        labels: chartColorsSet2
    },
    statistic2LineChartCard:{
        // The type of chart we want to create
        type: 'line',
    
        // The data for our dataset
        data: {
            labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
            datasets: [{
                label: 'My First dataset',
                borderColor: chartColorsSet1[0],
                data: [0, 10, 5, 2, 20, 30, 45]
            },
            {
                label: 'My Second dataset',
                borderColor: chartColorsSet1[1],
                data: [20,30,15,25,5,40,45]
            }
        ]
        },
    
        // Configuration options go here
        options: {
            legend: {
                display: false
              }
        }
    },
    statistic1LineChartCard:{
        // The type of chart we want to create
        type: 'line',
    
        // The data for our dataset
        data: {
            labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
            datasets: [{
                label: 'My First dataset',
                borderColor: chartColorsSet1[0],
                data: [0, 10, 5, 2, 20, 30, 45]
            }
        ]
        },
    
        // Configuration options go here
        options: {
            legend: {
                display: false
              }
        }
    }
}