function onResize() {
    if (!window.console.canvas)
        return;

    console.canvas.width = window.innerWidth;
    console.canvas.height = window.innerHeight;

    console.instance.invokeMethodAsync('OnResize', console.canvas.width, console.canvas.height);
}

window.consoleWindowResize = (instance) => {
    onResize();
};

window.initConsole = (instance) => {
    var canvasContainer = document.getElementById('divCanvas'),
        canvases = canvasContainer.getElementsByTagName('canvas') || [];
    window.console = {
        instance: instance,
        canvas: canvases.length ? canvases[0] : null
    };

    if (window.console.canvas) {
        //window.console.canvas.onmousemove = (e) => {
        //    console.instance.invokeMethodAsync('OnClick', e.clientX, e.clientY);
        //};
        window.console.canvas.onmousedown = (e) => {
            var me = {
                Button: e.button,
                ClientX: e.clientX,
                ClientY: e.clientY
            };
            console.instance.invokeMethodAsync('OnClick', me);
        };
        //window.console.canvas.onmouseup = (e) => {
        //    var me = {
        //        Button: e.button
        //    };
        //    console.instance.invokeMethodAsync('OnMouseUp', me);
        //};

        window.console.canvas.onkeydown = (e) => {
            console.instance.invokeMethodAsync('OnKeyDown', e);
        };
        //window.console.canvas.onkeyup = (e) => {
        //    console.instance.invokeMethodAsync('OnKeyUp', e.keyCode);
        //};
        //window.console.canvas.onblur = (e) => {
        //    window.console.canvas.focus();
        //};
        window.console.canvas.tabIndex = 0;
        window.console.canvas.focus();
    }

    window.addEventListener("resize", onResize);
};
