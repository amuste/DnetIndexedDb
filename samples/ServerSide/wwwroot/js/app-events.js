window.dnetoverlay = (function () {

    var sub = {};

    return {

        addWindowEventListeners: function (dotnetClass) {

            var Rx = window['rxjs'];

            var change = Rx.merge(Rx.fromEvent(window, 'resize'), Rx.fromEvent(window, 'orientationchange')).pipe(Rx.operators.auditTime(100));

            sub = change.subscribe((e) => {

                console.log('Windows resized');

                dotnetClass.invokeMethodAsync('OnWindowResized', {
                    Width: window.innerWidth,
                    Height: window.innerHeight
                });
            });

            return true;
        },

        removeWindowEventListeners: function () {

            sub.unsubscribe();
        },

        getViewportScrollPosition: function () {

            var documentElement = document.documentElement;

            var documentRect = documentElement.getBoundingClientRect();

            var top = -documentRect.top || document.body.scrollTop || window.scrollY ||
                documentElement.scrollTop || 0;

            var left = -documentRect.left || document.body.scrollLeft || window.scrollX || documentElement.scrollLeft || 0;

            return { Top: top, Left: left };
        },

        getViewportSize: function () {

            return { Width: window.innerWidth, Height: window.innerHeight };
        },

        getViewportSizeNoScroll: function () {

            return { Width: document.documentElement.clientWidth, Height: document.documentElement.clientHeight };
        },

        getBoundingClientRect: function (elementRef) {

            var tt = elementRef.getBoundingClientRect();

            return tt;
        },

        getDocumentBoundingClientRect: function () {

            var documentElement = document.documentElement;

            var documentRect = documentElement.getBoundingClientRect();

            console.log('documentRect', documentRect);

            //var clientHeight = documentElement.clientHeight;

            return documentRect;
        }

    };
})();