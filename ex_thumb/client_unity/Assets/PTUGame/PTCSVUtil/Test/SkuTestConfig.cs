using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PTGame.Utils.CVS;
using System.Linq;

public partial class SkuTestConfig : DataConfigBase<SkuTestConfig> {

	private List<SkuTestItem> m_Items;

	public override string GetCsvFile ()
	{
		return "SkuTest";
	}


	protected override System.Type GetItemType ()
	{
		return typeof(SkuTestItem);
	}

	public List<SkuTestItem> Items
	{
		get
		{ 
			if (m_Items == null) {
				m_Items = itemSets.Values.ToList().ConvertAll(s => s as SkuTestItem);
			}
			return m_Items;
		}
	}


	public SkuTestItem GetItemById(object id)
	{
		IConfigItem dataItem = _GetItemById(id);

		if(dataItem!=null)
		{
			return (SkuTestItem)dataItem;
		}else{
			return null;
		}
	}


}

public class SkuTestItem : IConfigItem
{
	public int Id { get; set; }
	public string Name { get; set; }
	public Vector3 Position { get; set; }
	public string Desc { get; set; }
	public float Scale { get; set; }
	public List<string> Weapons { get; set; }

}
