window.ComplementaryGoods = {};
window.ComplementaryGoods.getResult = function(r) {
    r.json().then(j => DotNet.invokeMethodAsync('GetResult', j));
};
window.ComplementaryGoods.chart = undefined;
window.ComplementaryGoods.setResult = function (jsonData) {
	if (window.ComplementaryGoods.chart) window.ComplementaryGoods.chart.destroy();
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
            labels: ['Категории товаров'],
			datasets: datasets,
		},
		options: {
			responsive: true,
			title: {
				display: true,
				text: 'Соотношение количества корзин с товара из обоих категорий к количеству корзин с товарами лишь из одной категории'
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
    window.ComplementaryGoods.chart = new Chart(context, config);
}