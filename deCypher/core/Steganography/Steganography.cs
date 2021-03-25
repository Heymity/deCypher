using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
            int offsetIndex = 5;
            bool breakLoop = false;
            byte[] bytes = ObjectToByteArray(message);
            for (int y = 0; y < height; y++)
            {
                if (breakLoop) break;
                for (int x = 0; x < width; x++)
                {
                    if (breakLoop) break;
                    uint pixel = *(ptr + y * stride + x);

                    byte a = (byte)((0xFF000000 & pixel) >> 24);
                    byte r = (byte)((0x00FF0000 & pixel) >> 16);
                    byte g = (byte)((0x0000FF00 & pixel) >> 8);
                    byte b = (byte)((0x000000FF & pixel));

                    breakLoop = !HideMessage(ref r, ref g, ref b, bytes, messageIndex, pixelIndex, ref offsetIndex);

                    pixelIndex++;
                    if (pixelIndex % 4 == 0)
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

        unsafe public T LeastImportantBitsDecrypt<T>()
        {
            width = bitmap.Width;
            height = bitmap.Height;
            imageData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            stride = imageData.Stride / 4;


            uint* ptr = (uint*)imageData.Scan0.ToPointer();
            bool breakLoop = false;
            int shiftIndex = 0;
            byte currentByte = 0x0000_0000;
            List<byte> bytes = new List<byte>();
            for (int y = 0; y < height; y++)
            {
                if (breakLoop) break;
                for (int x = 0; x < width; x++)
                {
                    if (breakLoop) break;
                    uint pixel = *(ptr + y * stride + x);
                    
                    byte a = (byte)((0xFF000000 & pixel) >> 24);
                    byte r = (byte)((0x00FF0000 & pixel) >> 16);
                    byte g = (byte)((0x0000FF00 & pixel) >> 8);
                    byte b = (byte)((0x000000FF & pixel));

                    //breakLoop = !UnHideMessage(ref r, ref g, ref b, bytes, messageIndex, pixelIndex);

                    UnHideMessage(ref r, ref g, ref b, ref bytes, ref currentByte, ref shiftIndex);
                }
            }

            bitmap.UnlockBits(imageData);

            return FromByteArray<T>(bytes.ToArray());
        }

        static bool HideMessage(ref byte r, ref byte g, ref byte b, byte[] bytes, int messageIndex, int pixelIndex, ref int offsetIndex)
        {
            if (messageIndex < bytes.Length)
            {
                SetByte(ref r, 0, GetByteForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
                SetByte(ref r, 1, GetByteForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
                if (offsetIndex < 0) offsetIndex = 5;
            }
            else return false;
            if (messageIndex < bytes.Length - 1)
            {
                SetByte(ref g, 0, GetByteForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
                SetByte(ref g, 1, GetByteForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
                if (offsetIndex < 0) offsetIndex = 5;
            }
            else return false;
            if (messageIndex < bytes.Length - 2)
            {
                SetByte(ref b, 0, GetByteForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
                SetByte(ref b, 1, GetByteForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
                if (offsetIndex < 0) offsetIndex = 5;
            }
            else return false;
            return true;
        }

        unsafe static void UnHideMessage(ref byte r, ref byte g, ref byte b, ref List<byte> bytes, ref byte currentByte, ref int shiftIndex)
        {
            var tmp = GetByte(b, 0);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            tmp = GetByte(b, 1);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            if (shiftIndex >= 8)
            {
                bytes.Add(currentByte);
                currentByte = 0;
                shiftIndex = 0;
            }

            tmp = GetByte(g, 0);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            tmp = GetByte(g, 1);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            if (shiftIndex >= 8)
            {
                bytes.Add(currentByte);
                currentByte = 0;
                shiftIndex = 0;
            }

            tmp = GetByte(r, 0);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            tmp = GetByte(r, 1);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            if (shiftIndex >= 8)
            {
                bytes.Add(currentByte);
                currentByte = 0;
                shiftIndex = 0;
            }
        }

        static bool GetByteForIndex(byte[] bytes, int messageIndex, int pixelIndex, int pos)
        {
            int unifiedBytes = 0;
            if (messageIndex < bytes.Length) unifiedBytes += bytes[messageIndex];
            if (messageIndex < bytes.Length - 1) unifiedBytes += (bytes[messageIndex + 1] << 8);
            if (messageIndex < bytes.Length - 2) unifiedBytes += (bytes[messageIndex + 2] << 16);

            var debug = GetByte(unifiedBytes, (pixelIndex * 6) + pos);
            return debug;
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
                var debug = ms.ToArray();
                return ms.ToArray();
            }
        }

        static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
}
