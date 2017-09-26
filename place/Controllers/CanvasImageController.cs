using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using place.DAL;
using place.Models;
using System;
using System.Drawing;

namespace place.Controllers
{
    public class CanvasImageController : Controller
    {
        private PlaceContext placeContext = new PlaceContext();

        // GET: CanvasImage
        public ActionResult Index(String name)
        {
            Canvas canvas = placeContext.Canvases.Find(name);
            if (canvas == null)
            {
                return HttpNotFound();
            }

            ApplyChangedPixelsToImage(canvas);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            canvas.GetBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }

        private void ApplyChangedPixelsToImage(Canvas canvas)
        {
            // Get changed pixels
            IEnumerable<PixelChange> pixelChanges = placeContext.PixelChanges
                .Where(p => p.CanvasVersion > canvas.ImageVersion && p.Canvas.Name == canvas.Name)
                .OrderBy( p => p.CanvasVersion);

            if(!pixelChanges.Any())
            {
                return;
            }

            // Update bitmap with changes
            Bitmap bitmap = canvas.GetBitmap();
            foreach (PixelChange pixelChange in pixelChanges) {
                bitmap.SetPixel(pixelChange.X, pixelChange.Y, ColorTranslator.FromHtml(pixelChange.Color) );
                //placeContext.PixelChanges.Remove(pixelChange);
            }

            canvas.SetBitmap(bitmap);

            canvas.ImageVersion = pixelChanges.Last().CanvasVersion;
            placeContext.SaveChanges();

        }

    }
}