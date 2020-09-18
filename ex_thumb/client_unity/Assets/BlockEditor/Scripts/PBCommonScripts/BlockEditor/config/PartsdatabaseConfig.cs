using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PTGame.Utils.CVS;
using System.Linq;

namespace Block.Editor
{
	public partial class PartsdatabaseConfig : DataConfigBase<PartsdatabaseConfig> {

		private List<PartsdatabaseItem> m_Items;

		public override string GetCsvFile ()
		{
			return "Partsdatabase";
		}


		protected override System.Type GetItemType ()
		{
			return typeof(PartsdatabaseItem);
		}

		public List<PartsdatabaseItem> Items
		{
			get
			{ 
				if (m_Items == null) {
					m_Items = itemSets.Values.ToList().ConvertAll(s => s as PartsdatabaseItem);
				}
				return m_Items;
			}
		}


		public PartsdatabaseItem GetItemById(object id)
		{
			IConfigItem dataItem = _GetItemById(id);

			if(dataItem!=null)
			{
				return (PartsdatabaseItem)dataItem;
			}else{
				return null;
			}
		}


	}

	public class PartsdatabaseItem : IConfigItem
	{
		public string Partnum { get; set; }
		public string Name { get; set; }
		public string Fullname { get; set; }

	}

}
