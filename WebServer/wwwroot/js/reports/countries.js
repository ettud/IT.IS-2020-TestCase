window.countries = {};
window.countries.getResult = function(r) {
    r.json().then(j => DotNet.invokeMethodAsync('GetResult', j));
};
window.countries.chart = undefined;
window.countries.setResult = function (jsonData) {
	if (window.countries.chart) window.countries.chart.destroy();
	const element = document.getElementById('report-chart');
	const context = element ? element.getContext('2d') : element;
	if (!context) return;
	const parsedData = JSON.parse(jsonData);
	const colors = window.chart.getColors(parsedData.length);
    const datasets = parsedData.map((ds, i) => {
        return {
            label: ds.Name,
            backgroundColor: colors[i],
            borderColor: colors[i],
            data: [ds.Number],
            fill: false,
        };
    });
    const config = {
		type: 'bar',
		data: {
            labels: ['Страны'],
			datasets: datasets,
		},
		options: {
			responsive: true,
			title: {
				display: true,
				text: 'Количество запросов по странам'
			},
			tooltips: {
				mode: 'index',
				intersect: false,
			},
			hover: {
				mode: 'nearest',
				intersect: true
			},
			scales: {
				yAxes: [{
					display: true,
					scaleLabel: {
						display: true,
						labelString: 'Количество запросов'
					}
				}]
			}
		}
	};
    window.countries.chart = new Chart(context, config);
};