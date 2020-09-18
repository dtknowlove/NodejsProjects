using System.Collections.Generic;
using System.IO;
using Block.Editor;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Putao.PaiBloks.Common;
using UnityEditor;
using UnityEngine;

public class ExportSkuInfo
{
    private const string PATH_CSV_SKU = "ftpres/config_paibloks_platform/Sku.csv";
    private const string PATH_CSV_SKUCAR = "ftpres/config_paibloks_platform/SkuCar.csv";
    
    private class SkuMainConfig_BlockNum_AnimSteps_Data
    {
        public int SkuID;
        public string Config;
        public int BlockNum;
        public int AnimSteps;
    }
    
    [MenuItem("BlockTool/临时工具/统计主搭建零件数、步骤数")]
    private static void GetSkuMainConfig_BlockNum_AnimSteps()
    {
        var content = File.ReadAllText(PATH_CSV_SKU);
        SkuConfig skuConfig = SkuConfig.LoadConfig (content) as SkuConfig;
		
        content = File.ReadAllText(PATH_CSV_SKUCAR);
        SkuCarConfig skuCarConfig = SkuCarConfig.LoadConfig(content) as SkuCarConfig;

        List<SkuMainConfig_BlockNum_AnimSteps_Data> dataList = new List<SkuMainConfig_BlockNum_AnimSteps_Data>();
        foreach (SkuItem sku in skuConfig.Items)
        {
            SkuMainConfig_BlockNum_AnimSteps_Data data = new SkuMainConfig_BlockNum_AnimSteps_Data();
            data.SkuID = sku.SkuId;

            if (sku.SkuId == 1002)
            {
                data.Config = "config_01002_sdlr_01_mk";
            }
            else if (sku.SkuId == 1003)
            {
                data.Config = "config_01003_01_Xmas_set";
            }
            else
            {
                SkuCarItem mainCar = skuCarConfig.Items.Find(carItem => carItem.SkuType == sku.ConfigFile && carItem.ConfigFile.EndsWith("01"));
                if (mainCar == null)
                {
                    Debug.LogFormat("<color=yellow> sku {0} 找不到主搭建config！</color>", sku.SkuId);
                    continue;
                }
                data.Config = mainCar.ConfigFile;
            }

            PPBlockConfigInfo configInfo = PBBlockConfigManager.LoadBlockInfos(data.Config + ".txt");
            if (configInfo == null)
            {
                Debug.LogFormat("<color=red> {0} 不存在！ </color>", data.Config);
                continue;
            }
            
            data.BlockNum = configInfo.BlockNum;
            data.AnimSteps = configInfo.KeyfameInfos.Count - 1;
            
            dataList.Add(data);
        }

        string exportPath = Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets")) + "sku统计.xlsx";
        using (ExcelPackage package = new ExcelPackage(new FileInfo(exportPath)))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("info");

            worksheet.Cells[1, 1].Value = "SkuID";
            worksheet.Cells[1, 2].Value = "Config";
            worksheet.Cells[1, 3].Value = "零件数";
            worksheet.Cells[1, 4].Value = "步骤数";

            int rowIndex = 2;
            foreach (SkuMainConfig_BlockNum_AnimSteps_Data data in dataList)
            {
                worksheet.Cells[rowIndex, 1].Value = data.SkuID;
                worksheet.Cells[rowIndex, 2].Value = data.Config;
                worksheet.Cells[rowIndex, 3].Value = data.BlockNum;
                worksheet.Cells[rowIndex, 4].Value = data.AnimSteps;
                rowIndex++;
            }
            
            package.Save();
        }
    }
}