using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
using System.IO;

public class TestLoadConfig : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		I18nConfig t = I18nConfig.LoadConfigFile () as I18nConfig;
//		Debug.Log(t.GetItemById ("TurnOn").CN);

//		SkuConfig t = SkuConfig.LoadConfigFile () as SkuConfig;
//		Debug.Log(t.GetItemById (1).Weapons);


//		using (ExcelPackage package = new ExcelPackage(new FileStream("Assets/PTUGame/PTCSVUtil/Test/i18n.xlsx", FileMode.Open)))
//		{
//			for (int i = 1; i <= package.Workbook.Worksheets.Count; ++i)
//			{
//				ExcelWorksheet sheet = package.Workbook.Worksheets[i];
//				for (int j = sheet.Dimension.Start.Column, k = sheet.Dimension.End.Column; j <= k; j++)
//				{
//					for (int m = sheet.Dimension.Start.Row, n = sheet.Dimension.End.Row; m <= n; m++)
//					{
//						Debug.Log(sheet.Cells[m,j].Value+" m:"+m);
////						string str = GetValue(sheet, m, j);
////						if (str != null)
////						{
////							// do something
////						}
//					}
//				}
//			}
//		}
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
