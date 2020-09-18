using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PTGame.Utils.CVS;
using System.Linq;

namespace Block.Editor
{
	public partial class SkuConfig : DataConfigBase<SkuConfig>
	{
		private List<SkuItem> m_Items;

		public override string GetCsvFile()
		{
			return "Sku";
		}


		protected override System.Type GetItemType()
		{
			return typeof(SkuItem);
		}

		public List<SkuItem> Items
		{
			get
			{
				if (m_Items == null)
				{
					m_Items = itemSets.Values.ToList().ConvertAll(s => s as SkuItem);
				}
				return m_Items;
			}
		}


		public SkuItem GetItemById(object id)
		{
			IConfigItem dataItem = _GetItemById(id);

			if (dataItem != null)
			{
				return (SkuItem) dataItem;
			}
			else
			{
				return null;
			}
		}
	}

	public class SkuItem : IConfigItem
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string SeriesTitleTextKey { get; set; }
		public string SubTitleTextKey { get; set; }
		public int SkuId { get; set; }
		public string SKUType { get; set; }
		public int SkuTheme { get; set; }
		public string Thumb { get; set; }
		public bool IsActived { get; set; }
		public string SceneSkuConfig { get; set; }
		public string ConfigFile { get; set; }
		public string MusicBG { get; set; }
		public string FinishAnim { get; set; }
		public string CellKey { get; set; }
		public string SKUCtlType { get; set; }
		public string SceneType { get; set; }
		public string SkuMenuConfig { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }
		public float Scale { get; set; }
		public bool IsShow { get; set; }
		public string BuildImg { get; set; }
		public string CarListItemBG { get; set; }
		public bool IsRecommended { get; set; }
		public List<string> Depends { get; set; }
	}
}
