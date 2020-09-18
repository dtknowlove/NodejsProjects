public class BlockData
{
	public string model { get; set; }
	public string prefab { get; set; }
	public string category { get; set; }
	public string type { get; set; }
	public string size { get; set; }
	public string scale { get; set; }
	public string color { get; set; }
	public string material { get; set; }
	public string material_high { get; set; }
	public string partnum { get; set; }
	public string description { get; set; }
	public string price { get; set; }
	public bool   isAllColor { get; set; }
}

public class BlockModelData
{
	public string model { get; set; }
	public string category { get; set; }
	public string type { get; set; }
	public string size { get; set; }
	public string scale { get; set; }
	public string partnum { get; set; }
	public string partversion { get; set; }
	public string isAllColor { get; set; }
}

public class BlockPrefabData 
{
	public string partnum { get; set; }
	public string prefab { get; set; }
	public string model{ get; set;}
	public string color{ get; set;}
	public string material{ get; set;}
	public string material_high{ get; set;}
	public string description{ get; set;}
}

public class BlockColorData
{
	public string code { get; set; }
	public string colorCategory { get; set; }
	public string color { get; set; }
	public string newCode { get; set; }
}

public class PartNumData
{
	public string new_num { get; set; }
	public string old_num { get; set; }
}

