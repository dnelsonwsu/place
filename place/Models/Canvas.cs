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

            /*Random rand = new Random();
            Color prevColor = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));

            int r = 0;
            int g = 0;
            int b = 0;

            for(int y = 0; y < bitmap.Height; y++)
            {
                for(int x = 0; x < bitmap.Width; x++)
                {
                    r+=5;
                    if(r > 255)
                    {
                        r = 0;
                        g+=5;
                        if(g > 255)
                        {
                            g = 0;
                            r = 0;
                            b+=5;
                            if(b > 255)
                            {
                                g = 0;
                                r = 0;
                                b = 0;
                            }
                        }
                    }
                    bitmap.SetPixel(x, y, Color.FromArgb(r,g,b));
                }
            }*/


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