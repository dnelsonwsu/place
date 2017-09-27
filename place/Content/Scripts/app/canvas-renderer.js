// Constants //
var INITIAL_ZOOM_LEVEL = 20;
var CANVAS_BACKDROP_COLOR = "#2b2b2b"
var SCALE_FACTOR = 1.1;


function canvasRenderer(initialImage) {

    // This is what will get redrawn onto the main canvas.
    var hiddenCanvas = document.createElement('canvas');    

    // Main canvas's context
    var ctx;

    var draggingDisabled = false;
    var lastX, lastY;

    /**
     * Initializes canvas
     */
    function initializeCanvas() {

        //Initialize canvases drawable area
        var canvas = document.getElementsByTagName('canvas')[0];

        canvas.width = canvas.offsetWidth;
        canvas.height = canvas.offsetHeight;

        ctx = canvas.getContext('2d');
        ctx.imageSmoothingEnabled = false;

        lastX = initialImage.width / 2;
        lastY = initialImage.height / 2;

        // Decorates various methods to keep track of zooming/panning transforms
        trackTransforms(ctx);
    }
    initializeCanvas();
    
    /**
     * Redraws the canvas
     */
    function redraw() {

        // Clear the entire canvas
        var p1 = ctx.transformedPoint(0, 0);
        var p2 = ctx.transformedPoint(canvas.width, canvas.height);

        ctx.clearRect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);

        ctx.save();
        ctx.setTransform(1, 0, 0, 1, 0, 0);
        ctx.fillStyle = CANVAS_BACKDROP_COLOR;
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        ctx.restore();

        ctx.drawImage(hiddenCanvas, 0, 0);

    }


   /**
    * Registers event callback methods to handle zooming/panning on the canvas
    */
    this.setupPanningAndZooming = function () {
       

        var dragStart, dragged;

        /**
         * Helper method used to draw a border around the canvas pixel that cursor is currently over
         */
        var drawCurrentPixelBorder = function() {
            var pixelPt = ctx.transformedPoint(lastX, lastY);
            pixelPt.x = parseInt(pixelPt.x);
            pixelPt.y = parseInt(pixelPt.y);

            ctx.lineWidth = .1;
            ctx.strokeStyle = 'red';
            ctx.strokeRect(pixelPt.x, pixelPt.y, 1, 1);
        }

       /**
        * Callback for when the mouse is pressed down on canvas. If dragging record start position
        */
        canvas.addEventListener('mousedown', function (evt) {

            if (!draggingDisabled) {
                document.body.style.mozUserSelect = document.body.style.webkitUserSelect = document.body.style.userSelect = 'none';
                lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
                lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);
                dragStart = ctx.transformedPoint(lastX, lastY);
                dragged = false;
            }
            
        }, false);

       /**
        * Callback for when the mouse cursor moves. If dragging, translate canvas.
        * Also updates current pixel rectangle
        */
        canvas.addEventListener('mousemove', function (evt) {

            lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
            lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);

            dragged = true;
            if (dragStart) {
                var pt = ctx.transformedPoint(lastX, lastY);
                var moveX = pt.x - dragStart.x;
                var moveY = pt.y - dragStart.y;

                //Make sure canvas is within bounds
                var upperLeft = ctx.transformedPoint(0, 0);
                var lowerRight = ctx.transformedPoint(canvas.width, canvas.height);
                
                // Check X
                ctx.translate(moveX, 0);

                upperLeft = ctx.transformedPoint(0, 0);
                lowerRight = ctx.transformedPoint(canvas.width, canvas.height);

                if (moveX > 0 &&            // Moving left
                    upperLeft.x < 0) {      // Left side of canvas is out of bounds
                    ctx.translate(-moveX, 0);   // Undo translation
                }
                if (moveX < 0 &&                            // Moving right
                    lowerRight.x > canvasImage.width) {     // Right side of canvas is out of bounds
                    ctx.translate(-moveX, 0);   // Undo translation
                }

                // Check Y
                ctx.translate(0, moveY);

                upperLeft = ctx.transformedPoint(0, 0);
                lowerRight = ctx.transformedPoint(canvas.width, canvas.height);

                if (moveY > 0 &&            // Moving up
                    upperLeft.y < 0) {      // Top side of canvas is out of bounds
                    ctx.translate(0, -moveY);   // Undo translation
                }

                if (moveY < 0 &&                            // Moving down
                    lowerRight.y > canvasImage.height) {    // Bottom side of canvas is out of bounds
                    ctx.translate(0, -moveY);   // Undo translation
                }

            }

            redraw();
            drawCurrentPixelBorder();

        }, false);

       /**
        * Callback for when the mouse button is released. Stops dragging.
        */
        canvas.addEventListener('mouseup', function (evt) {
            dragStart = null;
        }, false);

        /**
         * Zooms in or out of canvas
         * @param clicks - How many steps to zoom in(if positive) or out(if negative)
         */
        var zoom = function (clicks) {
            var pt = ctx.transformedPoint(lastX, lastY);

            // Apply zoom
            ctx.translate(pt.x, pt.y);
            var factor = Math.pow(SCALE_FACTOR, clicks);
            ctx.scale(factor, factor);
            ctx.translate(-pt.x, -pt.y);

            // Check to ensure canvas is in bounds
            var lowerRight = ctx.transformedPoint(canvas.width, canvas.height);

            if (lowerRight.x > canvasImage.width) {    // If canvas is off screen to left(If the canvas pixel under (width, height) is greater than width)
                ctx.translate(-(canvasImage.width - lowerRight.x), 0)   // Move canvas to right
            }

            if (lowerRight.y > canvasImage.height) {    // If canvas is off screen to top(If the canvas pixel under (width, height) is greater than height)
                ctx.translate(0, -(canvasImage.height - lowerRight.y))
            }


            var lowerRight = ctx.transformedPoint(canvas.width, canvas.height);
            var upperLeft = ctx.transformedPoint(0, 0); //where on image is screen(0,0)

            if (upperLeft.x < 0) {  // If canvas is off screen to the right(if the canvas pixel under(0,0) is < 0)
                ctx.translate(upperLeft.x, 0);
            }
            if (upperLeft.y < 0) {  // If canvas is off screen to the left(if the canvas pixel under(0,0) is < 0
                ctx.translate(0, upperLeft.y);
            }

            redraw();
            drawCurrentPixelBorder();
        }

        /**
         * Callback method that is called when the mouse's scroll wheel is scrolled. Zooms in/out of canvas
         */
        var handleScroll = function (eventParams) {
            var delta = eventParams.wheelDelta ? eventParams.wheelDelta / 40 : eventParams.detail ? -eventParams.detail : 0;
            if (delta) zoom(delta);
            return eventParams.preventDefault() && false;
        };

        /**
         * Callback method that is called when window is resized
         */
        window.addEventListener('resize', function () {
            initializeCanvas();

            redraw();
        }, true)

        canvas.addEventListener('DOMMouseScroll', handleScroll, false);
        canvas.addEventListener('mousewheel', handleScroll, false);

        zoom(INITIAL_ZOOM_LEVEL);

    }

   
    /**
     * Obtains pixel coordinates for screen X, Y coordinates(ie which pixel is at (500,500) )
     */
    this.getPositionInImage = function (x, y) {
        return ctx.transformedPoint(x, y);
    };

    /**
     * Obtains the canvas image
     */
    this.getCanvasImage = function () {
        return canvasImage;
    };

    /**
     * Sets the canvas image to specified image
     */
    this.setImage = function (image) {

        hiddenCanvas.width = image.width;
        hiddenCanvas.height = image.height;

        var imageCanvasCtx = hiddenCanvas.getContext('2d');
        imageCanvasCtx.drawImage(image, 0, 0);

        redraw();
    };

    /**
     * Sets the specified pixel on canvas
     */
    this.setPixel = function (x, y, color) {

        var hiddenCanvasCtx = hiddenCanvas.getContext('2d');

        hiddenCanvasCtx.fillStyle = color;
        hiddenCanvasCtx.fillRect(x, y, 1, 1);
        hiddenCanvasCtx.save();
        redraw();
    };

    /**
     * Disable canvas dragging
     */
    this.disableDragging = function() {
        draggingDisabled = true;
    }

    /**
     * Enable canvas dragging
     */
    this.enableDragging = function() {
        draggingDisabled = false;
    }

    this.setImage(initialImage);

};

/**
 * Decorates context methods to keep track of current transformations
 * 
 */
function trackTransforms(ctx) {
    var svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    var xform = svg.createSVGMatrix();
    ctx.getTransform = function () { return xform; };

    var savedTransforms = [];
    var save = ctx.save;
    ctx.save = function () {
        savedTransforms.push(xform.translate(0, 0));
        return save.call(ctx);
    };

    var restore = ctx.restore;
    ctx.restore = function () {
        xform = savedTransforms.pop();
        return restore.call(ctx);
    };

    var scale = ctx.scale;
    ctx.scale = function (sx, sy) {
        xform = xform.scaleNonUniform(sx, sy);
        return scale.call(ctx, sx, sy);
    };

    var rotate = ctx.rotate;
    ctx.rotate = function (radians) {
        xform = xform.rotate(radians * 180 / Math.PI);
        return rotate.call(ctx, radians);
    };

    var translate = ctx.translate;
    ctx.translate = function (dx, dy) {
        xform = xform.translate(dx, dy);
        return translate.call(ctx, dx, dy);
    };

    var transform = ctx.transform;
    ctx.transform = function (a, b, c, d, e, f) {
        var m2 = svg.createSVGMatrix();
        m2.a = a; m2.b = b; m2.c = c; m2.d = d; m2.e = e; m2.f = f;
        xform = xform.multiply(m2);
        return transform.call(ctx, a, b, c, d, e, f);
    };

    var setTransform = ctx.setTransform;
    ctx.setTransform = function (a, b, c, d, e, f) {
        xform.a = a;
        xform.b = b;
        xform.c = c;
        xform.d = d;
        xform.e = e;
        xform.f = f;
        return setTransform.call(ctx, a, b, c, d, e, f);
    };

    var pt = svg.createSVGPoint();
    ctx.transformedPoint = function (x, y) {
        pt.x = x; pt.y = y;
        return pt.matrixTransform(xform.inverse());
    }
}