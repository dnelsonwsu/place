using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace place.Models
{
    /// <summary>
    /// Represents a modification to a canvas pixel
    /// </summary>
    public class PixelChange
    {
        [Key]
        public int Id { get; set; }

        public string Color { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        // Which version of the canvas does this change correspond to
        public int CanvasVersion { get; set; }

        [IgnoreDataMember]
        public virtual Canvas Canvas { get; set; }

    }
}