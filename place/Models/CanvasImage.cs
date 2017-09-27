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
    public class CanvasImage
    {
        private const int Width = 1000;
        private const int Height = 1000;
        private Color InitialBackgroundColor = Color.FromArgb(255, 255, 255);

        [Key]
        public String Name { get; set; }

        public virtual Canvas canvas { get; set; }

        public byte[] Image { get; set; }

        /// <summary>
        /// Helper method used to initialize the bitmap
        /// </summary>
        public void InitializeBitmap()
        {
            Bitmap bitmap = new Bitmap(Width, Height);

            using (Graphics gfx = Graphics.FromImage(bitmap))
            using (SolidBrush brush = new SolidBrush(InitialBackgroundColor))
            {
                gfx.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
            }

            this.SetBitmap(bitmap);
        }

        /// <summary>
        /// Set bitmap to specified image
        /// </summary>
        /// <param name="image">Image to set to</param>
        public void SetBitmap(Bitmap image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            this.Image = ms.ToArray();
        }

        /// <summary>
        /// Obtains bitmap image
        /// </summary>
        public Bitmap GetBitmap()
        {
            MemoryStream ms = new MemoryStream(this.Image);
            Bitmap returnImage = new Bitmap(Bitmap.FromStream(ms));
            return returnImage;
        }

    }
}