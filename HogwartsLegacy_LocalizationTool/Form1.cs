using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HogwartsLegacy_LocalizationTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Bin Files|*.bin"
            };
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                BinaryReader rd = new BinaryReader(new MemoryStream(File.ReadAllBytes(ofd.FileName)));
                long CountOfStrings, HeaderSize, StartOfStringsWithoutHeader, StartOfStrings, SizeOfStringsBlock;
                string Magic, ID, Str;
                Magic = Encoding.Unicode.GetString(rd.ReadBytes(32));
                CountOfStrings = rd.ReadInt64();
                HeaderSize = rd.ReadInt64();
                StartOfStringsWithoutHeader = rd.ReadInt64();
                StartOfStrings = rd.ReadInt64();
                SizeOfStringsBlock = rd.ReadInt64();
                List<string> Strings = new List<string>();
                List<string> IDs= new List<string>();
                for(int i = 0;i< CountOfStrings; i++)
                {
                    long StringOffset = rd.ReadInt64();
                    int StringLen = rd.ReadInt32();
                    ID = rd.ReadStringAtOffset(StringOffset + StartOfStrings, StringLen);
                    IDs.Add(RemoveNewLine(ID));

                    StringOffset = rd.ReadInt64();
                    StringLen = rd.ReadInt32();
                    Str = rd.ReadStringAtOffset(StringOffset + StartOfStrings, StringLen);
                    Strings.Add(RemoveNewLine(Str));
                }
                rd.Close();
                File.WriteAllLines(ofd.FileName+".txt", Strings.ToArray());
                File.WriteAllLines(ofd.FileName + ".ids", IDs.ToArray());
                MessageBox.Show("Done!");

            }

        }
        private string RemoveNewLine(string str)
        {
            string ret = str;
            ret = ret.Replace("\r\n", "<cf>");
            ret = ret.Replace("\n", "<lf>");
            ret = ret.Replace("\r", "<cr>");
            return ret;
        }
        private string AddNewLine(string str)
        {
            string Text = str;
            Text = Text.Replace("<cf>", "\r\n");
            Text = Text.Replace("<lf>", "\n");
            Text = Text.Replace("<cr>", "\r");
            return Text;
        }

    }
}
