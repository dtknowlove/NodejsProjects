using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublisherGlobal {

	public const string ROOTDIR_BUILDINGANIM = "./config_paibloks_buildanim";
	public const string PATH_CSV_SKU 		 = "ftpres/config_paibloks_platform/Sku.csv";
	public const string PATH_CSV_SKUCAR      = "ftpres/config_paibloks_platform/SkuCar.csv";
	public const string ROOTDIR_IOS          = "modelres_ios";
//	public const string ROOTDIR_ANDROID      = "modelres_gltf";
	public const string DIR_RESDATABASE      = "resdatabase";
	public const string DIR_FTPRES 			 = "ftpres";
//	public const string ROOTDIR_GLTF         = "modelres";
	
	public static string PrimitiveThumbsConfig
	{
		get { return "resdatabase/resconfig_thumbs_primitive.json"; }
	}

	public static string PrimitiveAndroidResConfig
	{
		get { return "resdatabase/resconfig_modelres_android.json"; }
	}

	public static string PrimitiveiOSResConfig
	{
		get { return "resdatabase/resconfig_modelres_ios.json"; }
	}
	
	public static class ItemOrder
	{
		public const int AppConfigs = 0;
		public const int MINI = 5;
	}
	
	
}
