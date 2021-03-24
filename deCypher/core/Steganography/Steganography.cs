using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace deCypher.core.Steganography
{
    public class Steganography
    {
        public Bitmap bitmap;
        public string outputPath;

        private BitmapData imageData;
        private int width;
        private int height;
        private int stride;

        public Steganography(Bitmap bitmap, string outputPath)
        {
            this.bitmap = bitmap;
            this.outputPath = outputPath;
            width = bitmap.Width;
            height = bitmap.Height;      
        }

        unsafe public void LeastImportantBitsEncrypt(object message)
        {
            width = bitmap.Width;
            height = bitmap.Height;
            imageData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            stride = imageData.Stride / 4;


            uint* ptr = (uint*)imageData.Scan0.ToPointer();
            int messageIndex = 0;
            int pixelIndex = 0;
            byte[] bytes = ObjectToByteArray(message);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    uint pixel = *(ptr + y * stride + x);

                    byte a = (byte)((0xFF000000 & pixel) >> 24);
                    byte r = (byte)((0x00FF0000 & pixel) >> 16);
                    byte g = (byte)((0x0000FF00 & pixel) >> 8);
                    byte b = (byte)((0x000000FF & pixel));

                    //Put these in a function and verify there if the message has ended so it can stop the program returning a bool
                    SetByte(ref r, 0, GetByteForIndex(bytes, messageIndex, pixelIndex, 5)); 
                    SetByte(ref r, 1, GetByteForIndex(bytes, messageIndex, pixelIndex, 4));
                    SetByte(ref g, 0, GetByteForIndex(bytes, messageIndex, pixelIndex, 3));
                    SetByte(ref g, 1, GetByteForIndex(bytes, messageIndex, pixelIndex, 2));
                    SetByte(ref b, 0, GetByteForIndex(bytes, messageIndex, pixelIndex, 1));
                    SetByte(ref b, 1, GetByteForIndex(bytes, messageIndex, pixelIndex, 0));

                    pixelIndex++;
                    if (x % 4 == 0)
                    {
                        messageIndex += 3;
                        pixelIndex = 0;
                    }

                    *(ptr + y * stride + x) = ((uint)(a << 24) | (uint)(r << 16) | (uint)(g << 8) | b);
                }
            }

            bitmap.UnlockBits(imageData);
            bitmap.Save(outputPath);
        }
        
        static bool GetByteForIndex(byte[] bytes, int messageIndex, int pixelIndex, int pos)
        {
            int unifiedBytes = 0;
            if (messageIndex < bytes.Length) unifiedBytes += bytes[messageIndex];
            if (messageIndex < bytes.Length - 1) unifiedBytes += (bytes[messageIndex + 1] << 8);
            if (messageIndex < bytes.Length - 2) unifiedBytes += (bytes[messageIndex + 2] << 16);

            return GetByte(unifiedBytes, (pixelIndex * 6) + pos);
        }

        static void SetByte(ref byte aByte, int pos, bool value)
        {
            if (value)
            {
                //left-shift 1, then bitwise OR
                aByte = (byte)(aByte | (1 << pos));
            }
            else
            {
                //left-shift 1, then take complement, then bitwise AND
                aByte = (byte)(aByte & ~(1 << pos));
            }
        }

        static bool GetByte(byte aByte, int pos)
        {
            //left-shift 1, then bitwise AND, then check for non-zero
            return ((aByte & (1 << pos)) != 0);
        }

        static bool GetByte(int aByte, int pos)
        {
            //left-shift 1, then bitwise AND, then check for non-zero
            return ((aByte & (1 << pos)) != 0);
        }

        static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
