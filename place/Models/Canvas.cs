using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace place.Models
{
    /// <summary>
    /// Represents a canvas that can be "drawn on"
    /// </summary>
    public class Canvas
    {

        [Key]
        public string Name { get; set; }

        // The version of the canvas
        public int Version { get; set; }

        // Which version the bitmap image currently represents
        public int ImageVersion { get; set; }

    }
}