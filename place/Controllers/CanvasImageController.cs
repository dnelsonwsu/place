using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using place.DAL;
using place.Models;
using System;

namespace place.Controllers
{
    public class CanvasImageController : Controller
    {
        private CanvasContext canvasContext = new CanvasContext();

        // GET: CanvasImage
        public ActionResult Index(String name)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Canvas canvas = canvasContext.Canvases.Find(name);
            if(canvas == null)
            {
                return HttpNotFound();
            }

            canvas.GetBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }

    }
}