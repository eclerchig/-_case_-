using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace КП
{
	class Object
	{
		private int ID;
		private string TypeShape;
		private string Name = "";
		private List<string> attr = new List<string> { };
		private List<string> datas = new List<string> { };
		private List<int> outs = new List<int> { };
		private List<int> ins = new List<int> { };


		public Object(int id, string type)
		{
			this.ID = id;
			this.TypeShape = type;
		}

		public int getID()
		{ return this.ID; }

		public string getType()
		{ return this.TypeShape; }

		public void AddAttr(string att)
		{
			this.attr.Add(att);
		}

		public void AddData(string data)
		{
			this.datas.Add(data);
		}

		public List<string> ReadData()
		{
			return datas;
		}

		public List<string> ReadAttr()
		{
			return attr;
		}
		public void setName(string name)
		{
			this.Name = name;
		}
		public string getName()
		{
			return this.Name;
		}
		public void AddOuts(int id)
		{
			this.outs.Add(id);
		}
		public void AddIns(int id)
		{
			this.ins.Add(id);
		}

		public List<int> ReadOuts()
		{
			return this.outs;
		}
		public List<int> ReadIns()
		{
			return this.ins;
		}
	}
}
