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
        private PlaceContext databaseContext = new PlaceContext();

        /// GET PixelChange?canvasName=""&afterVersion=#
        /// Queries for all pixel changes on specified canvas after specified version
        [HttpGet]
        public IEnumerable<PixelChange> GetPixelChanges([FromUri]String canvasName, [FromUri]int afterVersion)
        {
            IEnumerable<PixelChange> pixelChanges = databaseContext.PixelChanges.Where(p => p.CanvasVersion > afterVersion && p.Canvas.Name == canvasName);
            return pixelChanges.ToList();
        }

        /// POST PixelChange?canvasName=""
        /// Creates a new pixel change(effectively sets a pixel on specified canvas)
        [HttpPost]
        public PixelChange SetPixelChange([FromUri]String canvasName, [FromBody]PixelChange pixelChange)
        {
            pixelChange.Canvas = databaseContext.Canvases.Find(canvasName);
            pixelChange.Canvas.Version++;
            pixelChange.CanvasVersion = pixelChange.Canvas.Version;

            databaseContext.PixelChanges.Add(pixelChange);
            databaseContext.SaveChanges();

            return pixelChange;
        }

    }
}
