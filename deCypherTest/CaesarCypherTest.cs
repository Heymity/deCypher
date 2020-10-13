using deCypher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace deCypherTest
{
    [TestClass]
    public class CaesarCypherTest
    {
        [TestMethod]
        public void EncodeCaesar()
        {
            // Create an instance to test:
            CaesarCypher caesarCypher = new CaesarCypher("Hi I`m Gabriel Pasquale", Alphabet.defaultAlphabet, false, 3);
            caesarCypher.Encode();

            Assert.AreEqual("Kl L`p Jdeulho Sdvtxdoh", caesarCypher.text);
        }

        [TestMethod]
        public void DecodeCaesar()
        {
            // Create an instance to test:
            CaesarCypher caesarCypher = new CaesarCypher("Kl L`p Jdeulho Sdvtxdoh", Alphabet.defaultAlphabet, false, 3);
            caesarCypher.Decode();

            Assert.AreEqual("Hi I`m Gabriel Pasquale", caesarCypher.text);
        }

        [TestMethod]
        public void DecodeBruteForceCaesar()
        {
            // Create an instance to test:
            CaesarCypher caesarCypher = new CaesarCypher("Kl L`p Jdeulho Sdvtxdoh", Alphabet.defaultAlphabet, false, 3);
            var list = caesarCypher.BruteForceDecode();

            Assert.AreEqual("Hi I`m Gabriel Pasquale", list[23]);
        }

        [TestMethod]
        public void ShortDecodeBruteForceCaesar()
        {
            // Create an instance to test:
            CaesarCypher caesarCypher = new CaesarCypher("Kl L`p Jdeulho Sdvtxdoh", Alphabet.defaultAlphabet, false, 3);
            var list = caesarCypher.ShortBruteForceDecode();

            Assert.AreEqual("Hi I`m Gab", list[23]);
        }

        [TestMethod]
        public void DecodeBruteForceByMatchCaesar()
        {
            // Create an instance to test:
            CaesarCypher caesarCypher = new CaesarCypher("Kl L`p Jdeulho Sdvtxdoh", Alphabet.defaultAlphabet, false, 3);
            var list = caesarCypher.MatchBruteForceDecode("Hi");

            Assert.AreEqual("Hi I`m Gabriel Pasquale", list[0]);
        }

        [TestMethod]
        public void DecodeBruteForceCaesarWithDict()
        {
            // Create an instance to test:
            CaesarCypher caesarCypher = new CaesarCypher("Kl L`p Jdeulho Sdvtxdoh", Alphabet.defaultAlphabet, false, 3);
            var list = caesarCypher.DictMatchBruteForceDecode("Hi");

            Assert.AreEqual("Hi I`m Gabriel Pasquale", list.ElementAt(23).Key);
        }

        [TestMethod]
        public void CaesarCypherConstructorWithOnlyString()
        {
            // Create an instance to test:
            CaesarCypher caesarCypher = new CaesarCypher("Hi I`m Gabriel Pasquale");

            // Assert that values are corrects
            Assert.IsNotNull(caesarCypher);
            Assert.AreEqual("Hi I`m Gabriel Pasquale", caesarCypher.text);
            Assert.AreEqual(Alphabet.defaultAlphabet, caesarCypher.alphabet);
            Assert.AreEqual(false, caesarCypher.ignoreCase);
            Assert.AreEqual(3, caesarCypher.rot);
        }

        [TestMethod]
        public void BigStringEncodingTest()
        {
            // Create an instance to test:
            CaesarCypher caesarCypher = new CaesarCypher("Lorem ipsum dolor sit amet, consectetur adipiscing elit.Quisque fringilla iaculis gravida.In sed augue dapibus, dapibus ligula elementum, ullamcorper mi.Sed placerat pulvinar tellus, eu facilisis risus accumsan vitae.Aenean sapien ante, interdum non augue commodo, vestibulum lobortis risus.Duis lobortis tempor tellus in eleifend.Maecenas vitae feugiat ante.Ut mauris ante, posuere eget tristique quis, condimentum at diam.Proin ut viverra leo.Sed venenatis interdum nisl, sit amet consectetur enim condimentum a.Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.Ut eu dictum ipsum.Duis porttitor quis dolor sed varius. Nunc pulvinar nisl a magna posuere tempor.Quisque nibh turpis, tristique a porta sed, sagittis id dui.Integer at turpis ut quam ornare luctus vel vitae turpis.Duis non vulputate nunc.Pellentesque sit amet rhoncus eros, eget tristique dolor.Curabitur nulla ex, vulputate id magna sed, volutpat faucibus eros.Nam porta quam sed felis lacinia, ac fusce. ", Alphabet.defaultAlphabet, false, 3);

            caesarCypher.Encode();

            Assert.AreEqual("Oruhp lsvxp groru vlw dphw, frqvhfwhwxu dglslvflqj holw.Txlvtxh iulqjlood ldfxolv judylgd.Lq vhg dxjxh gdslexv, gdslexv oljxod hohphqwxp, xoodpfrushu pl.Vhg sodfhudw sxoylqdu whooxv, hx idflolvlv ulvxv dffxpvdq ylwdh.Dhqhdq vdslhq dqwh, lqwhugxp qrq dxjxh frpprgr, yhvwlexoxp oreruwlv ulvxv.Gxlv oreruwlv whpsru whooxv lq hohlihqg.Pdhfhqdv ylwdh ihxjldw dqwh.Xw pdxulv dqwh, srvxhuh hjhw wulvwltxh txlv, frqglphqwxp dw gldp.Surlq xw ylyhuud ohr.Vhg yhqhqdwlv lqwhugxp qlvo, vlw dphw frqvhfwhwxu hqlp frqglphqwxp d.Rufl ydulxv qdwrtxh shqdwlexv hw pdjqlv glv sduwxulhqw prqwhv, qdvfhwxu ulglfxoxv pxv.Xw hx glfwxp lsvxp.Gxlv sruwwlwru txlv groru vhg ydulxv. Qxqf sxoylqdu qlvo d pdjqd srvxhuh whpsru.Txlvtxh qlek wxuslv, wulvwltxh d sruwd vhg, vdjlwwlv lg gxl.Lqwhjhu dw wxuslv xw txdp ruqduh oxfwxv yho ylwdh wxuslv.Gxlv qrq yxosxwdwh qxqf.Shoohqwhvtxh vlw dphw ukrqfxv hurv, hjhw wulvwltxh groru.Fxudelwxu qxood ha, yxosxwdwh lg pdjqd vhg, yroxwsdw idxflexv hurv.Qdp sruwd txdp vhg iholv odflqld, df ixvfh. ", caesarCypher.text);
        }
    }
}
