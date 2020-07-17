using System;
using System.Runtime.Remoting;

namespace PCCommon
{
	/// <summary>
	/// SnapInItem 的摘要说明。
	/// </summary>
	public class SnapInItem{
		string _name;
		string _description;
		string _detailDescription;
		string _module;
		string _className;
		string _image;
		ISnapIn _snapin;
	
		public string Name{
			get{ return _name; }
		}

		public string Description{
			get{ return _description; }
		}

		public string DetailDescription {
			get { return _detailDescription; }
		}

		public string Module{
			get{ return _module; }
		}

		public string ClassName{
			get{ return _className; }
		}

		public string Image{
			get{ return _image; }
		}

		public ISnapIn Snapin{
			get{ 
				if (_snapin == null){
					ObjectHandle objHandle;
			
					objHandle = Activator.CreateInstance( _module, _className );
					_snapin = (ISnapIn)objHandle.Unwrap();
					
				}//if
				
				return _snapin;
			}
		}

		public SnapInItem(string name, string description, string detailDescription, string module, string className, string image) {
			_name = name;
			_description = description;
			_detailDescription = detailDescription;
			_module = module;
			_className = className;
			_image = image;
		}
	}
}
