using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using place.Models;

namespace place.DAL
{
    public class CanvasContext : DbContext
    {
        public CanvasContext() : base("CanvasContext")
        {
            Database.SetInitializer<CanvasContext>(new DropCreateDatabaseAlways<CanvasContext>());
        }

        public DbSet<Canvas> Canvases { get; set; }

    }
}