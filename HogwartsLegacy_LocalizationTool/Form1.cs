using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HogwartsLegacy_LocalizationTool
{
    public partial class Form1 : Form
    {
        public string Magic = "AVAFDICT 2.0   \0";
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
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                BinaryReader rd = new BinaryReader(new MemoryStream(File.ReadAllBytes(ofd.FileName)));
                long CountOfStrings, HeaderSize, StartOfStringsWithoutHeader, StartOfStrings, SizeOfStringsBlock;
                string Magic, ID, Str;

                Magic = Encoding.Unicode.GetString(rd.ReadBytes(32));
                if (Magic != this.Magic) throw new Exception("Magic is not supported!");
                CountOfStrings = rd.ReadInt64();
                HeaderSize = rd.ReadInt64();
                StartOfStringsWithoutHeader = rd.ReadInt64();
                StartOfStrings = rd.ReadInt64();
                SizeOfStringsBlock = rd.ReadInt64();
                List<string> Strings = new List<string>();
                List<string> IDs = new List<string>();
                for (int i = 0; i < CountOfStrings; i++)
                {

                    long StringOffset = rd.ReadInt64();
                    int StringLen = rd.ReadInt32();
                    ID = rd.ReadStringAtOffset(StringOffset + StartOfStrings, StringLen);
                    IDs.Add(Helpers.RemoveNewLine(ID));

                    StringOffset = rd.ReadInt64();
                    StringLen = rd.ReadInt32();
                    Str = rd.ReadStringAtOffset(StringOffset + StartOfStrings, StringLen);
                    Strings.Add(Helpers.RemoveNewLine(Str));
                }
                rd.Close();
                File.WriteAllLines(ofd.FileName + ".txt", Strings.ToArray());
                File.WriteAllLines(ofd.FileName + ".ids", IDs.ToArray());
                MessageBox.Show("Export Done!");

            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Text Files|*.txt"
            };
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                if(ofd.CheckFileExists && File.Exists(Path.ChangeExtension(ofd.FileName, ".ids")))
                {
                    List<string> strings = new List<string>(File.ReadAllLines(ofd.FileName));
                    List<string> ids = new List<string>(File.ReadAllLines(Path.ChangeExtension(ofd.FileName, ".ids")));
                    if (strings.Count != ids.Count) throw new Exception("strings and ids are not equal");
                    BinaryWriter wr = new BinaryWriter(new MemoryStream());
                    wr.Write(Encoding.Unicode.GetBytes(this.Magic));
                    wr.Write((long)strings.Count);
                    wr.Write((long)72);
                    wr.Write((long)(strings.Count * 2 * 12));
                    wr.Write((long)(strings.Count * 2 * 12 + 72));
                    wr.Write((long)0);
                    long offset = 0;
                    List<byte[]> textblock = new List<byte[]>();
                    for(int i = 0;i< strings.Count;i++)
                    {
                        byte[] id = Encoding.UTF8.GetBytes(Helpers.AddNewLine(ids[i]));
                        wr.Write(offset);
                        offset += id.Length;
                        wr.Write(id.Length);
                        textblock.Add(id);

                        byte[] str = Encoding.UTF8.GetBytes(Helpers.AddNewLine(strings[i]));
                        wr.Write(offset);
                        offset += str.Length;
                        wr.Write(str.Length);
                        textblock.Add(str);
                    }
                    byte[] ConvertedTextBlock = textblock.SelectMany(a => a).ToArray();
                    wr.Write(ConvertedTextBlock);
                    wr.Seek(64, SeekOrigin.Begin);
                    wr.Write((long)ConvertedTextBlock.Length);
                    byte[] FinalData = Helpers.ToByteArray(wr.BaseStream);
                    string newfilename = Path.ChangeExtension(ofd.FileName.Replace(".bin", ""), ".bin");
                    if (File.Exists(newfilename)) File.Move(newfilename, newfilename + "_bk");
                    File.WriteAllBytes(newfilename, FinalData);
                    wr.Close();
                    MessageBox.Show("Import Done!");
                    
                }
            }
        }
    }
}
