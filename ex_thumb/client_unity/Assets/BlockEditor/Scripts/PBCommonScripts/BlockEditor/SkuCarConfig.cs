using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PTGame.Utils.CVS;
using System.Linq;

namespace Block.Editor
{
	public partial class SkuCarConfig : DataConfigBase<SkuCarConfig>
	{

		private List<SkuCarItem> m_Items;

		public override string GetCsvFile()
		{
			return "SkuCar";
		}


		protected override System.Type GetItemType()
		{
			return typeof(SkuCarItem);
		}

		public List<SkuCarItem> Items
		{
			get
			{
				if (m_Items == null)
				{
					m_Items = itemSets.Values.ToList().ConvertAll(s => s as SkuCarItem);
				}
				return m_Items;
			}
		}


		public SkuCarItem GetItemById(object id)
		{
			IConfigItem dataItem = _GetItemById(id);

			if (dataItem != null)
			{
				return (SkuCarItem) dataItem;
			}
			else
			{
				return null;
			}
		}
	}

	public class SkuCarItem : IConfigItem
	{
		public int ID { get; set; }
		public string NameTextKey { get; set; }
		public string Thumb { get; set; }
		public string ConfigFile { get; set; }
		public bool New { get; set; }
		public bool IsVariety { get; set; }
		public bool IsNeedExtraLight { get; set; }
		public string FinishAnim { get; set; }
		public string SkuType { get; set; }
	}
}