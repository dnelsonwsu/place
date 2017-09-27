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
        private PlaceContext databaseContext = new PlaceContext();

        /// GET: Canvas
        /// Obtains list of all canvases
        /// 
        [System.Web.Http.Route("")]
        public IEnumerable<Canvas> Get()
        {
            return databaseContext.Canvases.ToList();
        }

        /// GET: Canvas/{name}
        /// Obtains specified canvas by name
        /// 
        [System.Web.Http.Route("{name}")]
        public Canvas GetByName(String name)
        {
            return databaseContext.Canvases.Find(name);
        }

        /// PUT Canvas/{name}
        /// Creates a new empty canvas with the specified name
        /// 
        [Route("{name}")]
        [HttpPut]
        public Canvas CreateCanvas(String name)
        {
            if(databaseContext.Canvases.Find(name) != null)
            {
                BadRequest("A canvas with this name already exists");
            }

            Canvas newCanvas = new Canvas();
            newCanvas.Name = name;
            newCanvas.Version = 1;
            databaseContext.Canvases.Add(newCanvas);
            databaseContext.SaveChanges();

            CanvasImage canvasImage = new CanvasImage();
            canvasImage.InitializeBitmap();
            canvasImage.canvas = databaseContext.Canvases.Find(name);
            canvasImage.Name = name;
            databaseContext.CanvasImages.Add(canvasImage);

            databaseContext.SaveChanges();

            return newCanvas;
        }

        /// DELETE Canvas/{name}
        /// Deletes canvas and associated resources
        ///
        [Route("{name}")]
        [HttpDelete]
        public void DeleteCanvas(String name)
        {
            Canvas canvas = databaseContext.Canvases.Find(name);
            if (canvas == null)
            {
                NotFound();
            }
            databaseContext.Canvases.Remove(canvas);

            CanvasImage canvasImage = databaseContext.CanvasImages.Find(name);
            if(canvasImage != null)
            {
                databaseContext.CanvasImages.Remove(canvasImage);
            }
            

            List<PixelChange> pixelChanges = databaseContext.PixelChanges.Where(p => p.Canvas.Name == canvas.Name).ToList();
            foreach(PixelChange pixelChange in pixelChanges)
            {
                databaseContext.PixelChanges.Remove(pixelChange);
            }

            databaseContext.SaveChanges();

            
        }


    }
}
