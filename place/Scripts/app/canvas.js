var INITIAL_ZOOM_LEVEL = 20;
var CANVAS_BACKDROP_COLOR = "#2b2b2b"

function canvasHelper(canvasImage) {

    var imageCanvas = document.createElement('imageCanvas');


    //Initialize canvases drawable area
    var canvas = document.getElementsByTagName('canvas')[0];

    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;

    var ctx = canvas.getContext('2d');
    ctx.imageSmoothingEnabled = false;

    var draggingDisabled = false;

    // Adds enhanced transformation methods onto context instance
    trackTransforms(ctx);

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

        ctx.drawImage(imageCanvas, 0, 0);

    }
    redraw();

    this.setupScalingAndZooming = function () {
        var lastX = canvas.width / 2, lastY = canvas.height / 2;

        var dragStart, dragged;

        canvas.addEventListener('mousedown', function (evt) {

            if (!draggingDisabled) {
                document.body.style.mozUserSelect = document.body.style.webkitUserSelect = document.body.style.userSelect = 'none';
                lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
                lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);
                dragStart = ctx.transformedPoint(lastX, lastY);
                dragged = false;
            }
            
        }, false);

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
                if (upperLeft.x >= 0 && lowerRight.x <= canvasImage.width) {

                    ctx.translate(moveX, 0);

                    upperLeft = ctx.transformedPoint(0, 0);
                    lowerRight = ctx.transformedPoint(canvas.width, canvas.height);

                    if (upperLeft.x < 0) {
                        ctx.translate(-moveX, 0);
                    }
                    if (lowerRight.x > canvasImage.width) {
                        ctx.translate(-moveX, 0);
                    }

                }
                if (upperLeft.y >= 0 && lowerRight.y <= canvasImage.height) {
                    ctx.translate(0, moveY);

                    upperLeft = ctx.transformedPoint(0, 0);
                    lowerRight = ctx.transformedPoint(canvas.width, canvas.height);

                    if (upperLeft.y < 0) {
                        ctx.translate(0, -moveY);
                    }

                    if (lowerRight.y > canvasImage.height) {
                        ctx.translate(0, -moveY);
                    }

                }
                redraw();

            }
        }, false);

        canvas.addEventListener('mouseup', function (evt) {
            dragStart = null;
        }, false);

        var scaleFactor = 1.1;

        var zoom = function (clicks) {
            var pt = ctx.transformedPoint(lastX, lastY);

            ctx.translate(pt.x, pt.y);
            var factor = Math.pow(scaleFactor, clicks);
            ctx.scale(factor, factor);
            ctx.translate(-pt.x, -pt.y);


            var lowerRight = ctx.transformedPoint(canvas.width, canvas.height);

            if (lowerRight.x > canvasImage.width) {
                ctx.translate(-(canvasImage.width - lowerRight.x), 0)
            }

            if (lowerRight.y > canvasImage.height) {
                ctx.translate(0, -(canvasImage.height - lowerRight.y))
            }
            var lowerRight = ctx.transformedPoint(canvas.width, canvas.height);


            var upperLeft = ctx.transformedPoint(0, 0); //where on image is screen(0,0)
            if (upperLeft.x < 0) {
                ctx.translate(upperLeft.x, 0);
            }
            if (upperLeft.y < 0) {
                ctx.translate(0, upperLeft.y);
            }

            redraw();
        }

        var handleScroll = function (evt) {
            var delta = evt.wheelDelta ? evt.wheelDelta / 40 : evt.detail ? -evt.detail : 0;
            if (delta) zoom(delta);
            return evt.preventDefault() && false;
        };

        canvas.addEventListener('DOMMouseScroll', handleScroll, false);
        canvas.addEventListener('mousewheel', handleScroll, false);

        zoom(INITIAL_ZOOM_LEVEL);
    }

   

    this.getPositionInImage = function (x, y) {
        return ctx.transformedPoint(x, y);
    };

    this.getCanvasImage = function () {
        return canvasImage;
    };

    this.setImage = function (image) {

        imageCanvas.width = image.width;
        imageCanvas.height = image.height;

        var imageCanvasCtx = canvas.getContext('2d');
        imageCanvasCtx.drawImage(imageCanvas, 0, 0);

        redraw();
    };

    this.setPixel = function (x, y, color) {

        ctx.fillStyle = color;
        ctx.fillRect(x, y, 1, 1);
        ctx.save();
    };

    this.disableDragging = function() {
        draggingDisabled = true;
    }

    this.enableDragging = function() {
        draggingDisabled = false;
    }

};

// Adds ctx.getTransform() - returns an SVGMatrix
// Adds ctx.transformedPoint(x,y) - returns an SVGPoint
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