using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HogwartsLegacy_LocalizationTool
{
    internal class Helpers
    {
        public static string RemoveNewLine(string str)
        {
            string ret = str;
            ret = ret.Replace("\r\n", "<cf>");
            ret = ret.Replace("\n", "<lf>");
            ret = ret.Replace("\r", "<cr>");
            return ret;
        }
        public static string AddNewLine(string str)
        {
            string Text = str;
            Text = Text.Replace("<cf>", "\r\n");
            Text = Text.Replace("<lf>", "\n");
            Text = Text.Replace("<cr>", "\r");
            return Text;
        }
        public static byte[] ToByteArray(Stream a)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                a.Position = 0;
                a.CopyTo(ms);
                return ms.ToArray();
            }

        }
    }
}
