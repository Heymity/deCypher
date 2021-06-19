using deCypher.core.Steganography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace deCypherTest.Steg
{
    [TestClass]
    public class ObsoleteSteganographyTest
    {

        public const int MessageTestSize = (ImageTestSize * ImageTestSize * 3 / 4) - 6;//12282;
        public const int MessageTestSize24bpp = (ImageTestSize * ImageTestSize * 3 / 5) + 500;//12282;
        public const int ImageTestSize = 128;

        [TestMethod]
        public void EncodeDecode32bpp()
        {
            byte[] message = new byte[MessageTestSize];

            for (int i = 0; i < MessageTestSize; i++)
                message[i] = (byte)i;

            Bitmap baseImage = new Bitmap(ImageTestSize, ImageTestSize);
            var steg = new Steganography(baseImage, message);

            var encrypted = steg.LeastImportantBitsEncrypt(message, false);
            steg.bitmap = encrypted;

            var messageOut = steg.LeastImportantBitsDecrypt<byte[]>(false);

            for (int i = 0; i < MessageTestSize; i++)
                Assert.IsTrue(message[i] == messageOut[i]);
        }

        [TestMethod]
        public void EncodeDecode24bpp()
        {
            byte[] message = new byte[MessageTestSize24bpp];

            for (int i = 0; i < MessageTestSize24bpp; i++)
                message[i] = (byte)i;

            Bitmap baseImage = new Bitmap(ImageTestSize, ImageTestSize, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var steg = new Steganography(baseImage, message);

            var encrypted = steg.LeastImportantBitsEncrypt(message, false);
            steg.bitmap = encrypted;

            var messageOut = steg.LeastImportantBitsDecrypt<byte[]>(false);

            for (int i = 0; i < MessageTestSize24bpp; i++)
                Assert.IsTrue(message[i] == messageOut[i]);
        }
    }
}
