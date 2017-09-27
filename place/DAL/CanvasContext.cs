using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using place.Models;

namespace place.DAL
{
    public class PlaceContext : DbContext
    {
        public PlaceContext() : base("CanvasContext")
        { }

        public DbSet<Canvas> Canvases { get; set; }
        public DbSet<CanvasImage> CanvasImages { get; set; }
        public DbSet<PixelChange> PixelChanges { get; set; }

    }
}