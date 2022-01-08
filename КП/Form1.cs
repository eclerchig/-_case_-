using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace КП
{
	public partial class Form1 : Form
	{
		static string allText;
		static string tegPages;
		static string tegConnects;
		static List<String> tegShapes;
		static List<Object> objs = new List<Object> { };
		StreamReader reader;
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			openFileDialog1.Filter = "vsx files|*.vsx";
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				//Get the path of specified file
				//string filePath = openFileDialog1.FileName;

				//Read the contents of the file into a stream
				var fileStream = openFileDialog1.OpenFile();

				reader = new StreamReader(fileStream);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{ 
            if (reader == null)
			{
                return;
			}
            allText = reader.ReadToEnd();
            int posBeg = allText.IndexOf("<Pages>");
            int posEnd = allText.IndexOf("</Pages>");
            tegPages = allText.Substring(posBeg, posEnd - posBeg + 8);
            Console.WriteLine(tegPages);

            posBeg = tegPages.IndexOf("<Connects>");
            posEnd = tegPages.IndexOf("</Connects>");
            tegConnects = tegPages.Substring(posBeg + 10, posEnd - posBeg - 10);
            Console.WriteLine("Связи:");
            Console.WriteLine(tegConnects);

            string test = tegPages.Remove(posBeg, (posEnd - posBeg + 11));
            posBeg = test.IndexOf("<Shapes>");
            Console.WriteLine(posBeg);
            test = test.Remove(0, posBeg);
            posBeg = test.IndexOf("</Page>"); //удаление лишних тегов
            test = test.Substring(0, posBeg);
            test = test.Remove(0, 8);
            test = test.Substring(0, test.Length - 9);
            Console.WriteLine("Фигуры:");
            Console.WriteLine(test);
            tegShapes = FindShapes(test);
            for (int i = 0; i < tegShapes.Count; i++)
            {
                Console.WriteLine("ID: " + objs[i].getID());
                Console.WriteLine("Type: " + objs[i].getType());
                Console.WriteLine("Текст");
                Console.WriteLine(tegShapes[i]);
                if (objs[i].getType() == "Class")
                {
                    DefineProperties(i);
                    DefineNameClass(i);
                }
            }
            DefineConnect(tegConnects);
            ConvertToEKB();
        }

        static public List<string> FindShapes(string text)
        {
            List<string> allShapes = new List<string>();
            string teg;
            while (text.Length != 0)
            {
                int posBeg = text.IndexOf("<Shape");
                int posEnd = text.IndexOf(">");
                int pos;
                string subs;
                Console.WriteLine(posBeg + ", " + posEnd + ", " + (posEnd - posBeg + 1) + ", " + text.Length);
                teg = text.Substring(posBeg, posEnd - posBeg + 1);
                Console.WriteLine("воть:");
                Console.WriteLine(teg);
                if (DefineClass(teg))
                {
                    for (int j = 0; j < 10; j++)
                    {
                        Console.WriteLine();
                    }
                    text = text.Remove(0, teg.Length);
                    posBeg = text.IndexOf("<Shapes>");
                    text = text.Remove(0, posBeg + 8);
                    posEnd = text.IndexOf("</Shapes");
                    subs = text.Substring(0, posEnd);
                    // Console.WriteLine("гык:");
                    //    Console.WriteLine(text);
                    Console.WriteLine("конец:" + posEnd);
                    Console.WriteLine(subs);
                    allShapes.Add(text.Substring(0, posEnd));
                    text = text.Remove(0, posEnd + 17);
                    Console.WriteLine("тык:");
                    Console.WriteLine(text);
                }
                // allShapes.Add(text.Substring(posBeg, posEnd - posBeg + 9));
                //  Console.WriteLine(allShapes.Count+":");
                //  Console.WriteLine(allShapes[1]);
                //  text = text.Substring(posEnd - posBeg + 9);
                //  }
                else
                {
                    posEnd = text.IndexOf("</Shape>");
                    text = text.Remove(0, posEnd + 8);
                    Console.WriteLine("тык:");
                    Console.WriteLine(text);
                }
            }
            return allShapes;
        }

        static public bool DefineClass(string text)
        {
            int check1 = text.IndexOf("Type");
            int check2 = text.IndexOf("Group");
            if ((check1 != -1) && (check2 != -1))
            {
                string id;
                string Master;
                text = text.Remove(0, 7);
                int pos = text.IndexOf(" ");
                id = text.Substring(0, pos);
                pos = text.IndexOf("'");
                id = id.Remove(id.Length - 1, 1);
                id = id.Remove(0, pos + 1);
                Console.WriteLine("id:");
                Console.WriteLine(id);
                // objs.Add(new Object())

                pos = text.IndexOf("Master");
                if (pos != -1)
                {
                    Master = text.Remove(0, pos);

                    pos = Master.IndexOf(" ");
                    Master = Master.Substring(0, pos);

                    pos = Master.IndexOf("'");
                    Master = Master.Remove(Master.Length - 1, 1);
                    Master = Master.Remove(0, pos + 1);
                    Console.WriteLine("Master:");
                    Console.WriteLine(Master);
                    if (Master == "4")
                    {
                        objs.Add(new Object(Int32.Parse(id), "Connection"));
                    }
                    else
                    {
                        objs.Add(new Object(Int32.Parse(id), "Class"));
                    }
                    return true;
                }
            }
            return false;
        }

        static public void DefineProperties(int i)
        {
            Console.WriteLine("i: " + i);
            string text = tegShapes[i];
            string subs;
            int pos = text.IndexOf("NameU = 'Attributes'");
            text = text.Remove(0, pos + 21);
            Console.WriteLine("Убираем NameU: ");
            Console.WriteLine(text);
            pos = text.IndexOf("<Text>");
            text = text.Remove(0, pos + 6);
            Console.WriteLine("Убираем Text: ");
            Console.WriteLine(text);
            pos = text.IndexOf("</Text>");
            subs = text.Substring(0, pos);

            Console.WriteLine("Получили Attr: ");
            Console.WriteLine(subs);

            string a = "";
            string d = "";
            while (subs.Length != 0)
            {
                pos = subs.IndexOf(":");
                if (pos != -1)
                {
                    a = subs.Substring(1, pos - 1);
                    subs = subs.Remove(0, pos + 2);
                    pos = subs.IndexOf("\n");
                    d = subs.Substring(0, pos - 1);
                    subs = subs.Remove(0, pos + 1);
                    objs[i].AddAttr(a);
                    objs[i].AddData(d);
                }
                else
                {
                    pos = subs.IndexOf("\n");
                    a = subs.Substring(1, pos - 1);
                    subs = subs.Remove(0, pos + 1);
                    objs[i].AddAttr(a);
                    objs[i].AddData("");
                }
                Console.WriteLine("Свойство: ");
                Console.WriteLine(a);
                Console.WriteLine("Тип данных: ");
                Console.WriteLine(d);
            }
            Console.WriteLine("Абзац: ");
            Console.WriteLine(pos);
            //objs[i].AddAttr()
            //text = text.Remove(0, pos + 6);
        }

        static public void DefineNameClass(int i)
        {
            Console.WriteLine("ИМЯЯЯЯЯЯЯ: " + i);
            for (int j = 0; j < 10; j++)
            {
                Console.WriteLine();
            }
            string text = tegShapes[i];
            Console.WriteLine(text);
            Console.WriteLine("i: " + i);

            string subs;
            int pos = text.IndexOf("NameU='Name'");
            text = text.Remove(0, pos + 12);
            Console.WriteLine("Убираем NameU: ");
            Console.WriteLine(text);
            pos = text.IndexOf("<Text>");
            text = text.Remove(0, pos + 6);
            Console.WriteLine("Убираем Text: ");
            Console.WriteLine(text);
            pos = text.IndexOf("</Text>");
            subs = text.Substring(12, pos - 12);

            Console.WriteLine("Получили Name: ");
            Console.WriteLine(subs);

            objs[i].setName(subs);
        }

        static public void DefineConnect(string text)
        {
            Console.WriteLine("Connects: ");
            Console.WriteLine(text);
            string subs;
            int pos;
            string btype = null;
            string atype = null;
            int bid = 0;
            int aid = 0;
            int bfromsheet = 0;
            int afromsheet = 0;
            List<int> ints;

            while (text.Length != 0)
            {
                pos = text.IndexOf("<Connect");
                text = text.Remove(0, pos + 8);
                Console.WriteLine("Убрали Connect: ");
                Console.WriteLine(text);

                pos = text.IndexOf("FromSheet");
                text = text.Remove(0, pos + 11);
                Console.WriteLine("Убрали fromCsheet: ");
                Console.WriteLine(text);
                pos = text.IndexOf("'");
                subs = text.Substring(0, pos);
                if (bfromsheet == 0)
                {
                    bfromsheet = Int32.Parse(subs);
                }
                else
                {
                    afromsheet = Int32.Parse(subs);
                }
                text = text.Remove(0, pos + 1);
                Console.WriteLine(text);
                pos = text.IndexOf("FromCell");
                text = text.Remove(0, pos + 10);
                Console.WriteLine("Убрали fromCell: ");
                Console.WriteLine(text);
                pos = text.IndexOf("'");
                if (afromsheet == 0 && bfromsheet != 0)
                {
                    btype = text.Substring(0, pos);
                    Console.WriteLine("btype: " + btype);
                }
                else
                {
                    atype = text.Substring(0, pos);
                }
                text = text.Remove(0, pos + 1);

                pos = text.IndexOf("ToSheet");
                text = text.Remove(0, pos + 9);
                Console.WriteLine("Убрали ToSheet: ");
                Console.WriteLine(text);
                pos = text.IndexOf("'");
                subs = text.Substring(0, pos);
                if (afromsheet == 0 && bfromsheet != 0)
                {
                    bid = Int32.Parse(subs);
                }
                else
                {
                    aid = Int32.Parse(subs);
                }

                Console.WriteLine("b a: " + bfromsheet + " " + afromsheet);
                if ((bfromsheet != 0) && (afromsheet != 0))
                {
                    Console.WriteLine("ПРОШЛО!: ");
                    Console.WriteLine("btype: " + btype);
                    if (btype == "BeginX")
                    {
                        Console.WriteLine("bid = " + bid + "aid = " + aid);
                        for (int i = 0; i < objs.Count; i++)
                        {
                            if (objs[i].getID() == bid)
                            {
                                Console.WriteLine("objs[i].getID() = " + objs[i].getID());
                                objs[i].AddOuts(aid);
                            }
                        }
                        for (int i = 0; i < objs.Count; i++)
                        {
                            if (objs[i].getID() == aid)
                            {
                                objs[i].AddIns(bid);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < objs.Count; i++)
                        {
                            if (objs[i].getID() == aid)
                            {
                                objs[i].AddOuts(bid);
                            }
                        }
                        for (int i = 0; i < objs.Count; i++)
                        {
                            if (objs[i].getID() == bid)
                            {
                                objs[i].AddIns(aid);
                            }
                        }
                    }
                    bfromsheet = 0;
                    afromsheet = 0;
                }

                pos = text.IndexOf("/>");
                Console.WriteLine("pos: " + pos);
                text = text.Remove(0, pos + 2);
                Console.WriteLine("Убрали остаток2: ");
                Console.WriteLine(text);
            }
            for (int i = 0; i < objs.Count; i++)
            {
                Console.WriteLine("Класс ID:" + objs[i].getID());
                ints = objs[i].ReadIns();
                for (int j = 0; j < ints.Count; j++)
                {
                    Console.WriteLine("Ins:" + ints[j]);
                }
                Console.WriteLine();
            }
        }

        static public void ConvertToEKB()
        {
            Random rnd = new Random();
            int id_kb = rnd.Next(100000000, 999999999);
            Console.WriteLine("id_kb:" + id_kb);
            string text = string.Concat("<Structure><KnowledgeBase><ID>", id_kb, "</ID><Name>База знаний ", id_kb, "</Name><ShortName></ShortName><Kind>0</Kind><Description></Description><Vars/><Templates>");
            for (int i = 1; i <= objs.Count; i++)
            {
                if (objs[i - 1].getType() == "Class")
                {
                    text = string.Concat(text, "<Template>");
                    string id_class = i.ToString();
                    if (id_class.Length == 1)
                    {
                        text = string.Concat(text, "<ID>T00", id_class, "</ID><Name>", objs[i - 1].getName(), "</Name><ShortName>Shablon-T00", id_class, "</ShortName><Description>Описание шаблона T00", id_class, "</Description>");
                    }
                    else
                    {
                        text = string.Concat(text, "<ID>T0", id_class, "</ID><Name>", objs[i - 1].getName(), "</Name><ShortName>Shablon-T0", id_class, "</ShortName><Description>Описание шаблона T0", id_class, "</Description>");
                    }
                    text = string.Concat(text, "<PackageName></PackageName><RootPackageName></RootPackageName><DrawParams></DrawParams><Slots>");
                    for (int j = 1; j <= objs[i - 1].ReadAttr().Count; j++)
                    {
                        List<string> attr = objs[i - 1].ReadAttr();
                        List<string> datas = objs[i - 1].ReadData();
                        text = string.Concat(text, "<Slot><Name>", attr[j - 1], "</Name><ShortName>Slot-", j.ToString(), "</ShortName><Description></Description><Value></Value>");
                        if (datas[j - 1] == "string")
                        {
                            text = string.Concat(text, "<DataType>String</DataType><Constraint></Constraint></Slot>");
                        }
                        else
                        {
                            text = string.Concat(text, "<DataType>Number</DataType><Constraint></Constraint></Slot>");
                        }
                    }
                    text = string.Concat(text, "</Slots>");
                    text = string.Concat(text, "</Template>");
                }
                Console.WriteLine("Результат:", i - 1);
                Console.WriteLine(text);
            }
            text = string.Concat(text, "</Templates><Facts/><GRules>");
            int countAct = 0;
            for (int i = 0; i < objs.Count; i++)
            {
                if (objs[i].ReadIns().Count > 0) //поиск классов с ins
                {
                    text = string.Concat(text, "<GRule>");
                    string id_class = (i + 1).ToString();
                    string idAct = (countAct + 1).ToString();
                    if (idAct.Length == 1)
                    {
                        text = string.Concat(text, "<ID>G00", idAct, "</ID><Name>Шаблон-правила-G00", idAct, "</Name><ShortName>Shablon-pravila-G00", idAct, "</ShortName><Description>Описание шаблона правила G00", idAct, "</Description>");
                    }
                    else
                    {
                        text = string.Concat(text, "<ID>G0", idAct, "</ID><Name>Шаблон-правила-G0", idAct, "</Name><ShortName>Shablon-pravila-G0", idAct, "</ShortName><Description>Описание шаблона правила G0", idAct, "</Description>");
                    }
                    text = string.Concat(text, "<PackageName></PackageName><RootPackageName></RootPackageName><DrawParams></DrawParams><Conditions>");
                    List<int> ins = objs[i].ReadIns();
                    int count_ins = 0;
                    for (int j = 0; j < ins.Count; j++) //перебираем id ins
                    {
                        for (int k = 0; k < objs.Count; k++) //перебираем классы с id
                        {
                            if (ins[j] == objs[k].getID())
                            {
                                string id_class2 = (k + 1).ToString();
                                if (id_class2.Length == 1)
                                {
                                    text = string.Concat(text, "<C", count_ins, ">Shablon-T00", id_class2, "</C", count_ins, ">");
                                }
                                else
                                {
                                    text = string.Concat(text, "<C", count_ins, ">Shablon-T0", id_class2, "</C", count_ins, ">");
                                }
                                count_ins++;
                            }
                        }
                    }
                    text = string.Concat(text, "</Conditions><Actions>");
                    if (id_class.Length == 1)
                    {
                        text = string.Concat(text, "<A0>Shablon-T00", id_class, "</A0>");
                    }
                    else
                    {
                        text = string.Concat(text, "<A0>Shablon-T0", id_class, "</A0>");
                    }
                    text = string.Concat(text, "</Actions>");
                    countAct++;
                    text = string.Concat(text, "</GRule>");
                }
            }
            text = string.Concat(text, "</GRules><Rules/><Functions/><Tasks/><FScales/><TempPackageList/><FactPackageList/><RulePackageList/><GRulePackageList/></KnowledgeBase></Structure>");
            Console.WriteLine("Результат: ");
            Console.WriteLine(text);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "EKB file|*.ekb";
            saveFileDialog1.Title = "Save an EKB File";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "" && text != "")
            {
                string filename = saveFileDialog1.FileName;
				File.WriteAllText(filename, text);
            }
        }
    }
      
}
