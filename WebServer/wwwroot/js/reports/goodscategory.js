window.goodsCategory = {};
window.goodsCategory.getResult = function(r) {
    r.json().then(j => DotNet.invokeMethodAsync('GetResult', j));
};
window.goodsCategory.chart = undefined;
window.goodsCategory.setResult = function (jsonData) {
	if (window.goodsCategory.chart) window.goodsCategory.chart.destroy();
	const element = document.getElementById('report-chart');
	const context = element ? element.getContext('2d') : element;
	if (!context) return;
	const labels = Array.from(Array(24).keys()).map((a, i, arr) => a + ":00");
	const parsedData = JSON.parse(jsonData);
	const colors = window.chart.getColors(parsedData.length);
    const datasets = parsedData.map((ds, i) => {
        return {
            label: ds.Name,
            backgroundColor: colors[i],
            borderColor: colors[i],
            data: ds.Points,
            fill: false,
        };
    });
    const config = {
		type: 'line',
		data: {
			labels: labels,
			datasets: datasets,
		},
		options: {
			responsive: true,
			title: {
				display: true,
				text: 'Количество запросов в час'
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
				xAxes: [{
					display: true,
					scaleLabel: {
						display: true,
						labelString: 'Время дня'
					}
				}],
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
    window.goodsCategory.chart = new Chart(context, config);
};