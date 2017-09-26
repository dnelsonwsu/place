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

        // GET: Canvas
        [System.Web.Http.Route("")]
        public IEnumerable<Canvas> Get()
        {
            return canvasContext.Canvases.ToList();
        }

        // GET: Canvas/{name}
        [System.Web.Http.Route("{name}")]
        public Canvas GetByName(String name)
        {
            return canvasContext.Canvases.Find(name);
        }

        //PUT Canvas/{name}
        [Route("{name}")]
        [HttpPut]
        public void CreateCanvas(String name)
        {
            Canvas newCanvas = new Canvas();
            newCanvas.Name = name;
            newCanvas.Version = 1;
            newCanvas.InitializeBitmap();

            canvasContext.Canvases.Add(newCanvas);
            canvasContext.SaveChanges();
        }


        private static Dictionary<string, object> canvasLocks = new Dictionary<string, object>();

        //POST Canvas/{name}/Pixel
        [Route("{name}/Pixel")]
        [HttpPost]
        public void SetPixel(String name, [FromBody]PixelChange pixel)
        {
            if(!canvasLocks.ContainsKey(name))
            {
                canvasLocks[name] = new object();
            }
            lock(canvasLocks[name])
            {
                Canvas canvas = canvasContext.Canvases.Find(name);
                if (canvas == null)
                {
                    NotFound();
                }

                Color colorToSet = ColorTranslator.FromHtml(pixel.Color);
                Bitmap bitmap = canvas.GetBitmap();
                bitmap.SetPixel(pixel.X, pixel.Y, colorToSet);
                canvas.SetBitmap(bitmap);

                canvas.Version++;

                canvasContext.SaveChanges();
            }
        }
    }
}
