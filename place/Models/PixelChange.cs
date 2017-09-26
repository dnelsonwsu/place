using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace place.Models
{
    public class PixelChange
    {
        [Key]
        public int Id { get; set; }

        public string Color { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public int CanvasVersion { get; set; }

        [IgnoreDataMember]
        public virtual Canvas Canvas { get; set; }

    }
}