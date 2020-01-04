var ActionsTracker = (function () {
    var instance = undefined;
    const constr = function () {
        const obj = {
            lastTargets: {
                onClick: undefined,
            },
            onClick: function (event) {
                event = event || window.event;
                obj.lastTargets.onClick = event.target || event.srcElement;
            },
        };
        document.body.addEventListener('click', obj.onClick);
        return obj;
    };

    return function Construct_singletone() {
        if (instance) {
            return instance;
        }
        if (this && this.constructor === Construct_singletone) {
            return (instance = constr());
        }
        return new Construct_singletone();
    };
})();