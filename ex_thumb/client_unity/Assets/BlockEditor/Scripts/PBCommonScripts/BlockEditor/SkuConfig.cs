using System;
using System.Collections.Generic;
using System.Linq;

namespace Block.Editor
{
	public partial class SkuConfig
	{
		public SkuItem GetItemBySkuId(int skuId)
		{
			SkuItem item = this.Items.FirstOrDefault(t => t.SkuId == skuId);
			if (item == null)
				throw new Exception(string.Format("sku id:{0} 不在配置sku.csv表里面", skuId));
			
			return item;
		}

		public List<SkuItem> GetBeDependedItems(int skuId)
		{
			List<SkuItem> items = new List<SkuItem>();

			for (int i = 0; i < this.m_Items.Count; i++)
			{

				if (m_Items[i].Depends != null && m_Items[i].Depends.Count > 0)
				{
					foreach (var depend in m_Items[i].Depends)
					{
						if (int.Parse(depend) == skuId)
						{
							items.Add(m_Items[i]);
							break;
						}
					}

				}
			}
			return items;
		}
	}
}
