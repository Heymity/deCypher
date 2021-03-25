using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace deCypher.core.Steganography
{
    public class Steganography : ICypher<byte[]>
    {
        public byte[] messageBytes;
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

        public Steganography(Bitmap bitmap, string outputPath, byte[] message)
        {
            this.bitmap = bitmap;
            this.outputPath = outputPath;
            width = bitmap.Width;
            height = bitmap.Height;
            messageBytes = message;
        }

        public byte[] Encode()
        {
            LeastImportantBitsEncrypt(messageBytes, false);
            return messageBytes;
        }

        public byte[] Decode()
        {
            return LeastImportantBitsDecrypt<byte[]>();
        }

        public static void Encode(string inputPath, string outputPath, string messagePath)
        {
            byte[] fileBytes = File.ReadAllBytes(messagePath);
            Bitmap image = new Bitmap(inputPath);

            Steganography steg = new Steganography(image, outputPath);
            steg.LeastImportantBitsEncrypt(fileBytes, false);
        }

        public static void Decode(string inputPath, string outputPath)
        {
            Bitmap image = new Bitmap(inputPath);
            Steganography steg = new Steganography(image, outputPath);
            byte[] outputMessage = steg.LeastImportantBitsDecrypt<byte[]>(false);

            File.WriteAllBytes(outputPath, outputMessage);
        }

        unsafe public void LeastImportantBitsEncrypt(object message, bool serialize = true)
        {
            if (!serialize) if (message.GetType() != typeof(byte[])) throw new ArgumentException("Message is not byte[] and serialize is false");

            width = bitmap.Width;
            height = bitmap.Height;
            imageData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            //stride = imageData.Stride / 4;
            stride = width;


            uint* ptr = (uint*)imageData.Scan0.ToPointer();
            int messageIndex = 0;
            int pixelIndex = 0;
            int offsetIndex = 5;
            bool breakLoop = false;
            byte[] bytes = serialize ? ObjectToByteArray(message) : (byte[])message;

            byte[] bytesLenght = GetLenghtInByteArray(bytes, 6);
            for (int y = 0; y < height; y++)
            {
                if (breakLoop) break;
                for (int x = 0; x < width; x++)
                {
                    if (breakLoop) break;

                    var pixelNum = y * stride + x;
                    uint pixel = *(ptr + pixelNum);

                    byte a = (byte)((0xFF000000 & pixel) >> 24);
                    byte r = (byte)((0x00FF0000 & pixel) >> 16);
                    byte g = (byte)((0x0000FF00 & pixel) >> 8);
                    byte b = (byte)((0x000000FF & pixel));

                    HideMessage(ref r, ref g, ref b, bytesLenght, messageIndex, pixelIndex, ref offsetIndex);

                    *(ptr + y * stride + x) = ((uint)(a << 24) | (uint)(r << 16) | (uint)(g << 8) | b);
                    pixelIndex++;
                    if (pixelIndex % 4 == 0)
                    {
                        messageIndex += 3;
                        pixelIndex = 0;
                    }
                    if (pixelNum == 7)
                    {
                        messageIndex = 0;
                        pixelIndex = 0;
                        offsetIndex = 5;
                        breakLoop = true;
                    }

                }
            }

            breakLoop = false;
            for (int y = 0; y < height; y++)
            {
                if (breakLoop) break;
                for (int x = 0; x < width; x++)
                {
                    if (breakLoop) break;
                    var pixelNum = y * stride + x;
                    if (pixelNum < 8) continue;
                    uint pixel = *(ptr + pixelNum);

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

        unsafe public T LeastImportantBitsDecrypt<T>(bool deserialize = true)
        {
            width = bitmap.Width;
            height = bitmap.Height;
            imageData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            //stride = imageData.Stride / 4;
            stride = width;


            uint* ptr = (uint*)imageData.Scan0.ToPointer();
            bool breakLoop = false;
            int shiftIndex = 0;
            byte currentByte = 0x0000_0000;
            List<byte> bytes = new List<byte>();
            int lenght = 0;
            for (int y = 0; y < height; y++)
            {
                if (breakLoop) break;
                for (int x = 0; x < width; x++)
                {
                    if (breakLoop) break;
                    var pixelNum = y * stride + x;
                    uint pixel = *(ptr + pixelNum);
                    
                    byte a = (byte)((0xFF000000 & pixel) >> 24);
                    byte r = (byte)((0x00FF0000 & pixel) >> 16);
                    byte g = (byte)((0x0000FF00 & pixel) >> 8);
                    byte b = (byte)((0x000000FF & pixel));

                    UnHideMessage(ref r, ref g, ref b, ref bytes, ref currentByte, ref shiftIndex);
                    if (pixelNum == 7)
                    {
                        byte[] intFormatBytes = bytes.ToArray()[2..bytes.Count];
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(intFormatBytes);
                        lenght = BitConverter.ToInt32(intFormatBytes, 0);
                        bytes = new List<byte>();
                    }
                    else if (pixelNum > 7) breakLoop = bytes.Count >= lenght;
                }
            }

            bitmap.UnlockBits(imageData);

            if (typeof(T) == typeof(byte[]) && !deserialize) return (T)(object)bytes.ToArray();
            return ObjectFromByteArray<T>(bytes.ToArray());
        }

        static bool HideMessage(ref byte r, ref byte g, ref byte b, byte[] bytes, int messageIndex, int pixelIndex, ref int offsetIndex)
        {
            SetBit(ref r, 1, GetBitForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
            SetBit(ref r, 0, GetBitForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
            if (offsetIndex < 0) offsetIndex = 5;

            SetBit(ref g, 1, GetBitForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
            SetBit(ref g, 0, GetBitForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
            if (offsetIndex < 0) offsetIndex = 5;

            SetBit(ref b, 1, GetBitForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
            SetBit(ref b, 0, GetBitForIndex(bytes, messageIndex, pixelIndex, offsetIndex--));
            if (offsetIndex < 0) offsetIndex = 5;

            return messageIndex <= bytes.Length + 3;
        }

        unsafe static void UnHideMessage(ref byte r, ref byte g, ref byte b, ref List<byte> bytes, ref byte currentByte, ref int shiftIndex)
        {
            var tmp = GetBit(b, 0);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            tmp = GetBit(b, 1);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            if (shiftIndex >= 8)
            {
                bytes.Add(currentByte);
                currentByte = 0;
                shiftIndex = 0;
            }

            tmp = GetBit(g, 0);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            tmp = GetBit(g, 1);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            if (shiftIndex >= 8)
            {
                bytes.Add(currentByte);
                currentByte = 0;
                shiftIndex = 0;
            }

            tmp = GetBit(r, 0);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            tmp = GetBit(r, 1);
            currentByte += (byte)(Convert.ToByte(tmp) << shiftIndex++);
            if (shiftIndex >= 8)
            {
                bytes.Add(currentByte);
                currentByte = 0;
                shiftIndex = 0;
            }
        }

        static bool GetBitForIndex(byte[] bytes, int messageIndex, int pixelIndex, int pos)
        {
            int unifiedBytes = 0;
            if (messageIndex < bytes.Length) unifiedBytes += bytes[messageIndex];
            if (messageIndex < bytes.Length - 1) unifiedBytes += (bytes[messageIndex + 1] << 8);
            if (messageIndex < bytes.Length - 2) unifiedBytes += (bytes[messageIndex + 2] << 16);

            var debug = GetBit(unifiedBytes, (pixelIndex * 6) + pos);
            return debug;
        }

        static void SetBit(ref byte aByte, int pos, bool value)
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

        static bool GetBit(byte aByte, int pos)
        {
            //left-shift 1, then bitwise AND, then check for non-zero
            return ((aByte & (1 << pos)) != 0);
        }

        static bool GetBit(int aByte, int pos)
        {
            //left-shift 1, then bitwise AND, then check for non-zero
            return ((aByte & (1 << pos)) != 0);
        }

        static byte[] GetLenghtInByteArray(byte[] bytes, int resultSize)
        {
            byte[] lenght = BitConverter.GetBytes(bytes.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lenght);
            byte[] result = new byte[resultSize];
            for(int i = 0; i < lenght.Length; i++)
            {
                int index = i + result.Length - lenght.Length;
                if (index >= result.Length) break;
                result[index] = lenght[i];
            }
            return result;
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

        static T ObjectFromByteArray<T>(byte[] data)
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
