using System;
using System.Collections;

namespace PCCommon
{
	/// <summary>
	/// SnapInGroup 的摘要说明。
	/// </summary>
	public class SnapInGroup{
		string _name;
		string _description;
		string _detailDescription;
		ArrayList _snapIns = new ArrayList();
	
		public string Name{
			get{ return _name; }
		}

		public string Description{
			get{ return _description; }
		}

		public string DetailDescription {
			get { return _detailDescription; }
		}

		public ArrayList SnapIns{
			get{ return _snapIns; }
		}

		public SnapInGroup(string name, string description, string detailDescription) {
			_name = name;
			_description = description;
			_detailDescription = detailDescription;
		}
	}
}
