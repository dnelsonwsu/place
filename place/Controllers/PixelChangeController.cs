using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using place.Models;
using place.DAL;

namespace place.Controllers
{
    [System.Web.Http.RoutePrefix("api/PixelChange")]
    public class PixelChangeController : ApiController
    {
        private PlaceContext placeContext = new PlaceContext();

        [HttpGet]
        public IEnumerable<PixelChange> GetPixelChanges([FromUri]String canvasName, [FromUri]int afterVersion)
        {
            IEnumerable<PixelChange> pixelChanges = placeContext.PixelChanges.Where(p => p.CanvasVersion > afterVersion && p.Canvas.Name == canvasName);
            return pixelChanges.ToList();
        }

        [HttpPost]
        public PixelChange SetPixelChange([FromUri]String canvasName, [FromBody]PixelChange pixelChange)
        {
            pixelChange.Canvas = placeContext.Canvases.Find(canvasName);
            pixelChange.Canvas.Version++;
            pixelChange.CanvasVersion = pixelChange.Canvas.Version;

            placeContext.PixelChanges.Add(pixelChange);
            placeContext.SaveChanges();

            return pixelChange;
        }

    }
}
