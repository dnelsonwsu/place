using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using place.Models;

namespace place.DAL
{
    public class CanvasInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<CanvasContext>
    {
        protected override void Seed(CanvasContext context)
        {
            Canvas defaultCanvas = new Canvas();
            defaultCanvas.Name = "default";
            defaultCanvas.Version = 1;
            defaultCanvas.InitializeBitmap();

            context.Canvases.Add(defaultCanvas);
            context.SaveChanges();
        }
    }
}