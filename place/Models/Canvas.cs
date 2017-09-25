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
    public class Canvas
    {

        [Key]
        [Column("Name")]
        public string Name { get; set; }

        public int Version { get; set; }

        [IgnoreDataMember]
        public byte[] Image { get; set; }

        
        public void InitializeBitmap()
        {
            Bitmap bitmap = new Bitmap(1000, 1000);

            using (Graphics gfx = Graphics.FromImage(bitmap))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255)))
            {
                gfx.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
            }

            this.SetBitmap(bitmap);
        }

        public void SetBitmap(Bitmap image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            Image = ms.ToArray();
        }

        public Bitmap GetBitmap()
        {
            MemoryStream ms = new MemoryStream(this.Image);
            Bitmap returnImage = new Bitmap(Bitmap.FromStream(ms));
            return returnImage;
        }

    }
}