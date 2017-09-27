using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using place.DAL;
using place.Models;
using System.Web.Http;
using System.Drawing;

namespace place.Controllers
{
    [System.Web.Http.RoutePrefix("api/Canvas")]
    public class CanvasController : ApiController
    {
        private PlaceContext canvasContext = new PlaceContext();

        /// GET: Canvas
        /// Obtains list of all canvases
        /// 
        [System.Web.Http.Route("")]
        public IEnumerable<Canvas> Get()
        {
            return canvasContext.Canvases.ToList();
        }

        /// GET: Canvas/{name}
        /// Obtains specified canvas by name
        /// 
        [System.Web.Http.Route("{name}")]
        public Canvas GetByName(String name)
        {
            return canvasContext.Canvases.Find(name);
        }

        /// PUT Canvas/{name}
        /// Creates a new empty canvas with the specified name
        /// 
        [Route("{name}")]
        [HttpPut]
        public Canvas CreateCanvas(String name)
        {
            if(canvasContext.Canvases.Find(name) != null)
            {
                BadRequest("A canvas with this name already exists");
            }

            Canvas newCanvas = new Canvas();
            newCanvas.Name = name;
            newCanvas.Version = 1;
            newCanvas.InitializeBitmap();

            canvasContext.Canvases.Add(newCanvas);
            canvasContext.SaveChanges();

            return newCanvas;
        }


    }
}
