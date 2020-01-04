window.formSubmitHandler = function (evt, fetchResultHandler) {
    if (evt && evt.currentTarget && evt.currentTarget.parentElement && evt.currentTarget.parentElement.submit) {
        evt.preventDefault();
        const form = evt.currentTarget.parentElement;
        let url = new URL(form.action);
        let requestOpt = {
            method: form.method,
            headers: {
                'RequestVerificationToken': document.getElementById('RequestVerificationToken').value,
            },
        };
        const formData = new FormData(form);
        if (!(requestOpt.method === 'get' || requestOpt.method === 'head')) {
            requestOpt.body = formData;
        } else {
            for (let key of formData.keys()) {
                url.searchParams.append(key,
                    Array.prototype.map.call(formData.get(key), encodeURIComponent).join(''));
            }
        }
        fetch(url, requestOpt).then(r => {
            if (fetchResultHandler) {
                return  fetchResultHandler(r);
            }
        });
    }
};

window.formSubmitResultHandler = function(r) {
    console.log(r);
};

window.formDisablerWithJs = function (submitId, submitResultHandlerName) {
    const submitResultHandler = window.deepFind(window, submitResultHandlerName);
    const func = function(evt) { return window.formSubmitHandler(evt, submitResultHandler); };
    return formDisablerBlazorWrapper(submitId, func);
};

window.formDisablerBlazorWrapperWithBlazor = function (submitId, blazorFuncName, blazorBuildName) {
    const func = function (evt) {
        evt.preventDefault();
        if (blazorBuildName) {
            DotNet.invokeMethodAsync(blazorBuildName, blazorFuncName);
        } else {
            DotNet.invokeMethodAsync(blazorFuncName);
        }
    };
    return formDisablerBlazorWrapper(submitId, func);
};

window.formDisablerBlazorWrapper = function (submitId, func) {
    const submit = document.getElementById(submitId);
    return formDisabler(submit, func);
};

window.formDisabler = function (submit, func) {
    if (submit.addEventListener) {
        submit.addEventListener('click', func, true);
    }
    else if (submit.attachEvent) {
        submit.attachEvent('onclick', func);
    } else {
        console.error("Forms submit is still enabled since no 'addEventListener' nor 'attachEvent' methods were found, possibly unsupported browser.");
    }
};

window.deepFind = function (obj, path) {
    const paths = path.split('.');
    let current = obj;

    for (let i = 0; i < paths.length; i++) {
        if (current[paths[i]] === undefined) {
            return undefined;
        } else {
            current = current[paths[i]];
        }
    }
    return current;
};

window.chart = {};

window.chart.getColors = function (length) {
    let colors = [];
    //colors are from https://github.com/d3/d3-scale-chromatic lib
    if (length <= 10) {
        colors = ["#1f77b4", "#ff7f0e", "#2ca02c", "#d62728", "#9467bd", "#8c564b", "#e377c2", "#7f7f7f", "#bcbd22", "#17becf"];
    }
    else {
        colors = ["#8dd3c7", "#ffffb3", "#bebada", "#fb8072", "#80b1d3", "#fdb462", "#b3de69", "#fccde5", "#d9d9d9", "#bc80bd", "#ccebc5", "#ffed6f"];
        if (length > 12) {
            console.warn("Too many lines, not implemented");
            for (let i = 12; i < length; i++) {
                colors.push("#000000");
            }
        }
    }
    return colors;
};

window.chart.getContext = function (chartName, id) {
    if (window.charts) {
        if (window.charts[name]) {
            window.charts[name].destroy();
        }
    }
    else {
        window.charts = {};
    }
    const element = document.getElementById(id);
    if (!element) return undefined;
    const context = element.getContext('2d');
    if (!context) return undefined;
};

window.chart.createChart = function (chart, canvasId) {
    if (canvasId && chart) {
        if (window.charts[name]) {
            window.charts[name].destroy();
        }
    }
    else {
        window.charts = {};
    }
    const element = document.getElementById(id);
    if (!element) return undefined;
    const context = element.getContext('2d');
    if (!context) return undefined;
};