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
        private PlaceContext databaseContext = new PlaceContext();


        /// GET: CanvasImage?name=""&version=#
        /// Returns a .png image representing the canvas. 
        /// 
        public ActionResult Index(String name, String version)
        {
            // Check for cached image first
            System.IO.MemoryStream cachedMemoryStream = (System.IO.MemoryStream)System.Web.HttpContext.Current.Cache.Get(name + "&" + version);
            if(cachedMemoryStream != null)
            {
                return File(cachedMemoryStream.ToArray(), "image/png");
            }

            // Get canvas from database
            CanvasImage canvasImage = databaseContext.CanvasImages.Find(name);
            if (canvasImage == null)
            {
                return HttpNotFound();
            }

            // Apply all changed pixels, save, and generate png
            ApplyChangedPixelsToImage(canvasImage);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            canvasImage.GetBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            // Save to cache
            System.Web.HttpContext.Current.Cache.Insert(name + "&" + version, (object)ms);

            return File(ms.ToArray(), "image/png");
        }

        /// <summary>
        /// Helper method that applies any pixel changes between the image version and the latest version
        /// </summary>
        /// <param name="canvas">Which canvas to apply changes to</param>
        private void ApplyChangedPixelsToImage(CanvasImage canvasImage)
        {
            Canvas canvas = canvasImage.canvas;

            // Get changed pixels
            IEnumerable<PixelChange> pixelChanges = databaseContext.PixelChanges
                .Where(p => p.CanvasVersion > canvas.ImageVersion && p.Canvas.Name == canvas.Name)
                .OrderBy( p => p.CanvasVersion);

            if(!pixelChanges.Any())
            {
                return;
            }

            // Update bitmap with changes
            Bitmap bitmap = canvasImage.GetBitmap();
            foreach (PixelChange pixelChange in pixelChanges) {
                bitmap.SetPixel(pixelChange.X, pixelChange.Y, ColorTranslator.FromHtml(pixelChange.Color) );
                //placeContext.PixelChanges.Remove(pixelChange);
            }

            canvasImage.SetBitmap(bitmap);

            canvas.ImageVersion = pixelChanges.Last().CanvasVersion;
            databaseContext.SaveChanges();

        }

    }
}