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
        private CanvasContext canvasContext = new CanvasContext();

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

        //GET Canvas/{name}/Pixel
        [Route("{name}/Pixel")]
        [HttpGet]
        public Pixel GetPixel(String name, [FromUri] int x, [FromUri] int y)
        {
            Canvas canvas = canvasContext.Canvases.Find(name);
            if(canvas == null)
            {
                NotFound();
            }

            String colorInHex = System.Drawing.ColorTranslator.ToHtml(canvas.GetBitmap().GetPixel(x, y));
            Pixel pixel = new Pixel{ X = x, Y = y, Color = colorInHex};
            return pixel;
        }

        //POST Canvas/{name}/Pixel
        [Route("{name}/Pixel")]
        [HttpPost]
        public void SetPixel(String name, [FromBody]Pixel pixel)
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

            canvasContext.SaveChanges();
        }
        
    }
}
