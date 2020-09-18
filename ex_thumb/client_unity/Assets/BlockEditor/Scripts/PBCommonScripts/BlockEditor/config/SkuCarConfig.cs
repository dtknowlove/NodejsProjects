using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PTGame.Utils.CVS;
using UnityEngine;

namespace Block.Editor
{
	public partial class SkuCarConfig : DataConfigBase<SkuCarConfig> {

	
		public List<SkuCarItem> GetAssembleCars(string skuType)
		{
			var carItems = Items.Where(s => s.SkuType == skuType);
			if (!carItems.Any())
			{
				Debug.LogError("不存在"+skuType+"类型的百变");
				return null;
			}
			

			return carItems.ToList<SkuCarItem>();
		}
	}

}

