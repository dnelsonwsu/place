// Constants //
var POLLING_INTERVAL = 5000;


// Globals //
var selectedColor;
var selectedColorElement;
var canvasInfo;
var canvasRenderer;


$(document).ready(function () {

    // Obtain the default canvas image
    canvasImage = new Image();
    canvasImage.src = "CanvasImage?name=default";
    canvasImage.onload = function () {
        canvasRenderer = new canvasRenderer(canvasImage);
        canvasRenderer.setupPanningAndZooming();

        $('#loading-spinner').remove();

        // Callback methods for user interaction with canvas
        $('canvas').on('mousemove', canvasOnMouseMove);
        $('canvas').on('click', canvasOnClick);
    }

    // Obtain current canvas info
    $.get('api/Canvas/default', function (data) {
        canvasInfo = data;
    });

    // Setup callback methods for palette colors
    $(".palette-color").each(function () {
        $(this).on("click", paletteColorOnClick);
    });

    // Poll to check for updated version of canvas
    pollForCanvasUpdates();
});

/**
 * Helper method that returns whether a palette color is currently selected.
 * @return Boolean indicating whether color is selected
 */
function isAColorSelected() {
    return selectedColorElement != null;
}

/**
 * Called when canvas is clicked on. Checks to see if a pixel needs to be set at current cursor position.
 * @param eventParams  - parameters for the click event
 */
function canvasOnClick(eventParams) {

    if (isAColorSelected()) {
        var canvas = $('canvas');

        // Get the cavas pixel that cursor is over
        var cursorX = eventParams.offsetX || (eventParams.pageX - canvas.offsetLeft);
        var cursorY = eventParams.offsetY || (eventParams.pageY - canvas.offsetTop);

        var pos = canvasRenderer.getPositionInImage(cursorX, cursorY);
        pos.x = parseInt(pos.x);
        pos.y = parseInt(pos.y);

        // Set the pixel
        if (pos.x >= 0 && pos.y >= 0 && pos.x < canvasImage.width && pos.y < canvasImage.height) {

            // Tell server that pixel has been set
            $.ajax({
                type: "POST",
                url: 'api/PixelChange?canvasName=default',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    x: pos.x,
                    y: pos.y,
                    color: selectedColor
                }),
                dataType: 'json'
            })

            // Update pixel on canvas
            canvasRenderer.setPixel(pos.x, pos.y, selectedColor);
        }

    }
}

/**
 * Checks if there have been any changes between the current canvas version and the server's latest canvas version.
 * Applies any changes.
 * @return Promise that will resolve when the check-for-update/perform-update logic has completed
 */
function checkAndApplyCanvasUpdates() {
    var date = new Date();

    // Get server's latest canvas version
    return $.ajax({
        url: 'api/Canvas/default',
        cache: false
    }).then(function (serversCanvasInfo) {

        if (serversCanvasInfo.version > canvasInfo.version) {

            // Get all pixel changes after local canvas version
            return $.ajax({
                url: 'api/PixelChange?canvasName=default&afterVersion=' + canvasInfo.version,
                cache: false
            }).then(function (pixelChanges) {

                // Apply pixel changes
                for (var i = 0; i < pixelChanges.length; i++) {
                    canvasRenderer.setPixel(pixelChanges[i].x, pixelChanges[i].y, pixelChanges[i].color);
                }
                canvasInfo = serversCanvasInfo;
            });
        }
    });
}

/**
 * Periodically poll for any canvas updates
 */
var firstTime = true;
function pollForCanvasUpdates() {

    if (firstTime) {
        firstTime = false;
        setTimeout(pollForCanvasUpdates, POLLING_INTERVAL);
        return;
    }
    
    checkAndApplyCanvasUpdates().always(function () {
        setTimeout(pollForCanvasUpdates, POLLING_INTERVAL);
    });
    
}

/**
 * Callback method for when the mouse cursor moves over the canvas. 
 */
function canvasOnMouseMove(event) {

    // Get the cavas pixel that cursor is over
    var canvas = $('canvas');
    var cursorX = event.offsetX || (event.pageX - canvas.offsetLeft);
    var cursorY = event.offsetY || (event.pageY - canvas.offsetTop);

    var pos = canvasRenderer.getPositionInImage(cursorX, cursorY);
    pos.x = parseInt(pos.x);
    pos.y = parseInt(pos.y);

    if (pos.x >= 0 && pos.y >= 0 && pos.x < canvasImage.width && pos.y < canvasImage.height) {
        // Update position label text
        $('#cursorPos').text('(' + pos.x + ', ' + pos.y + ')');
    }

}

/**
 * Callback method called when a palette color is clicked. Selects/deselects a palette color
 */
function paletteColorOnClick() {

    // Un-highlight current palette color
    if (selectedColorElement) {
        $(selectedColorElement).removeClass('palette-color-selected');
    }    

    if (selectedColorElement == this) {
        // Click was on already-selected color
        canvasRenderer.enableDragging();
        selectedColorElement = null;
        return;
    }
    else {
        selectedColorElement = this;
    }

    // Save off the color
    selectedColor = rgb2hex($(this).css('background-color'));

    // Highlight palette color item
    if (!$(this).hasClass('palette-color-selected')) {
        $(this).addClass('palette-color-selected');
    }

    // Make canvas show pointer cursor
    $('canvas').addClass('placing-pixel');    

    canvasRenderer.disableDragging();
}

/**
 * Helper method to convert rgb(#,#,#) string to a hex value
 * @param rgb string in the format rgb(#,#,#)
 * @return hexcode for color
 */
function rgb2hex(rgb) {
    rgb = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    function hex(x) {
        return ("0" + parseInt(x).toString(16)).slice(-2);
    }
    return "#" + hex(rgb[1]) + hex(rgb[2]) + hex(rgb[3]);
}