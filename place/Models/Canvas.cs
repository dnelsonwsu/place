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

            Random rand = new Random();
            Color prevColor = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
            int prevX = rand.Next(0, bitmap.Width - 1);
            int prevY = rand.Next(0, bitmap.Height - 1);

            int loops = 10000;

            while (loops > 0)
            {
                int newX = prevX + rand.Next(-2, 3);
                if (newX < 0)
                {
                    newX = 0;
                }
                if (newX > bitmap.Width - 1)
                {
                    newX = bitmap.Width - 1;
                }

                int newY = prevY + rand.Next(-2, 3);
                if (newY < 0)
                {
                    newY = 0;
                }
                if (newY > bitmap.Width - 1)
                {
                    newY = bitmap.Width - 1;
                }

                prevColor = Color.FromArgb(0, 255, 0);
                bitmap.SetPixel(newX, newY, prevColor);

                prevX = newX;
                prevY = newY;

                loops--;
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