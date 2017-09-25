// Constants //
var POLLING_INTERVAL = 100000;


// Globals //
var selectedColor;
var selectedColorElement;
var canvasInfo;
var canvasHelper;


$(document).ready(function () {

    // Obtain the default canvas image
    canvasImage = new Image();
    canvasImage.src = "CanvasImage?name=default";
    canvasImage.onload = function () {
        canvasHelper = new canvasHelper(canvasImage);
        canvasHelper.setupScalingAndZooming();

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

    // Setup polling to check for updated version of canvas
    pollForCanvasUpdates();
});


/**
 *
 * @param {any} eventParams
 */
var c = 0
function canvasOnClick(eventParams) {
    var cc = c;
    c = c + 1;
    console.log('start: ' + cc.toString());
    if (selectedColorElement) {
        var canvas = $('canvas');
        var cursorX = eventParams.offsetX || (eventParams.pageX - canvas.offsetLeft);
        var cursorY = eventParams.offsetY || (eventParams.pageY - canvas.offsetTop);

        console.log('getPositionInImage: ' + cc.toString());
        var pos = canvasHelper.getPositionInImage(cursorX, cursorY);
        console.log('done getPositionInImage: ' + cc.toString());
        pos.x = parseInt(pos.x);
        pos.y = parseInt(pos.y);

        if (pos.x >= 0 && pos.y >= 0 && pos.x < canvasImage.width && pos.y < canvasImage.height) {
            console.log('if: ' + cc.toString());
            // Make API call
            $.ajax({
                type: "POST",
                url: 'api/Canvas/default/Pixel',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    x: pos.x,
                    y: pos.y,
                    color: selectedColor
                }),
                dataType: 'json',
                success: checkForCanvasUpdates
            }).done(function () { console.log('ajax success: ' + cc.toString());});

            // Update pixel on canvas
            canvasHelper.setPixel(pos.x, pos.y, selectedColor);
        }

    }
    console.log('done: ' + cc.toString());
}


function checkForCanvasUpdates() {
    var date = new Date();

    return $.ajax({
        url: 'api/Canvas/default',
        success: function(data) {
            if (data.version != canvasInfo.version) {
                canvasImage = new Image();
                canvasImage.src = "CanvasImage?name=default&version=" + data.version;
                canvasImage.onload = function () {
                    canvasHelper.setImage(canvasImage);
                }
            }
        },
        cache: false
    })
   
}

function pollForCanvasUpdates() {

    checkForCanvasUpdates().always(function () {
        setTimeout(pollForCanvasUpdates, POLLING_INTERVAL);
    });
    
}

/**
 *
 */
function canvasOnMouseMove(event) {
    var canvas = $('canvas');
    var cursorX = event.offsetX || (event.pageX - canvas.offsetLeft);
    var cursorY = event.offsetY || (event.pageY - canvas.offsetTop);

    var pos = canvasHelper.getPositionInImage(cursorX, cursorY);
    pos.x = parseInt(pos.x);
    pos.y = parseInt(pos.y);

    if (pos.x >= 0 && pos.y >= 0 && pos.x < canvasImage.width && pos.y < canvasImage.height) {
        $('#cursorPos').text('(' + pos.x + ', ' + pos.y + ')');
    }

}

/**
 * Callback method used to select/deselect a palette color
 */
function paletteColorOnClick() {

    // Un-highlight current palette color
    if (selectedColorElement) {
        $(selectedColorElement).removeClass('palette-color-selected');
    }    

    if (selectedColorElement == this) {
        canvasHelper.enableDragging();
        selectedColorElement = null;
        return;
    }
    else {
        selectedColorElement = this;
    }

    
    selectedColor = rgb2hex($(this).css('background-color'));

    // Highlight new palette color
    if (!$(this).hasClass('palette-color-selected')) {
        $(this).addClass('palette-color-selected');
    }

    // Make canvas show pointer cursor
    $('canvas').addClass('placing-pixel');    

    canvasHelper.disableDragging();
}

function rgb2hex(rgb) {
    rgb = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    function hex(x) {
        return ("0" + parseInt(x).toString(16)).slice(-2);
    }
    return "#" + hex(rgb[1]) + hex(rgb[2]) + hex(rgb[3]);
}