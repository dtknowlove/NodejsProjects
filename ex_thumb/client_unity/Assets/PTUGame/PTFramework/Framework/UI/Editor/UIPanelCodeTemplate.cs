/****************************************************************************
 * 2017 ~ 2018.6 liqingyun
****************************************************************************/

using System;
using UnityEngine;

namespace PTGame.Framework.Editor
{
	using System.Text;
	using System.IO;

	public static class UIPanelCodeTemplate
	{
		public static void Generate(string generateFilePath, string behaviourName, string nameSpace)
		{
			var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
			var strBuilder = new StringBuilder();

			strBuilder.AppendLine("/****************************************************************************");
			strBuilder.AppendFormat(" * {0}.{1} {2}\n", DateTime.Now.Year, DateTime.Now.Month, SystemInfo.deviceName);
			strBuilder.AppendLine(" ****************************************************************************/");
			strBuilder.AppendLine();

			strBuilder.AppendLine("using System;");
			strBuilder.AppendLine("using System.Collections.Generic;");
			strBuilder.AppendLine("using UnityEngine;");
			strBuilder.AppendLine("using UnityEngine.UI;");
			strBuilder.AppendLine("using PTGame.Framework;").AppendLine();

			strBuilder.AppendLine("namespace " + nameSpace);
			strBuilder.AppendLine("{");
			strBuilder.Append("\t").AppendFormat("public class {0}Data : UIData", behaviourName).AppendLine();
			strBuilder.Append("\t").AppendLine("{");
			strBuilder.Append("\t\t").AppendLine("// TODO: Query Mgr's Data");
			strBuilder.Append("\t").AppendLine("}");
			strBuilder.AppendLine();
			strBuilder.AppendFormat("\tpublic partial class {0} : UIView", behaviourName);
			strBuilder.AppendLine();
			strBuilder.AppendLine("\t{");
			strBuilder.Append("\t\t").AppendLine("protected override void InitUI(IUIData uiData = null)");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t").Append("\t")
				.AppendLine("mData = uiData as " + behaviourName + "Data ?? new " + behaviourName + "Data();");
			strBuilder.Append("\t\t").Append("\t").AppendLine("//please add init code here");
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendLine("protected override void ProcessMsg (int eventId,PTMsg msg)");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t\t").AppendLine("throw new System.NotImplementedException ();");
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendLine("protected override void RegisterUIEvent()");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendLine("protected override void OnShow()");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t\t").AppendLine("base.OnShow();");
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendLine("protected override void OnHide()");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t\t").AppendLine("base.OnHide();");
			strBuilder.Append("\t\t").AppendLine("}").AppendLine();
			strBuilder.Append("\t\t").AppendLine("protected override void OnClose()");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t\t").AppendLine("base.OnClose();");
			strBuilder.Append("\t\t").AppendLine("}");
			strBuilder.AppendLine("\t}");
			strBuilder.Append("}");

			sw.Write(strBuilder);
			sw.Flush();
			sw.Close();
		}
	}

	public static class UiPanelComponentsCodeTemplate
	{
		public static void Generate(string generateFilePath, string behaviourName, string nameSpace,
			PanelCodeData panelCodeData)
		{
			var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
			var strBuilder = new StringBuilder();
			// TODO: 这里可以加上打开界面的方法
			strBuilder.AppendLine("/****************************************************************************");
			strBuilder.AppendFormat(" * {0}.{1} {2}\n", DateTime.Now.Year, DateTime.Now.Month, SystemInfo.deviceName);
			strBuilder.AppendLine(" ****************************************************************************/");
			strBuilder.AppendLine();
			strBuilder.AppendLine("namespace " + nameSpace);
			strBuilder.AppendLine("{");
			strBuilder.AppendLine("\tusing UnityEngine;");
			strBuilder.AppendLine("\tusing UnityEngine.UI;");
			strBuilder.AppendLine("\tusing PTGame.Framework;");
			strBuilder.AppendLine();
			strBuilder.AppendFormat("\tpublic partial class {0}", behaviourName);
			strBuilder.AppendLine();
			strBuilder.AppendLine("\t{");
			strBuilder.AppendFormat("\t\tpublic const string NAME = \"{0}\";", behaviourName);
			strBuilder.AppendLine();
			strBuilder.AppendLine();

			foreach (var objInfo in panelCodeData.MarkedObjInfos)
			{
				var strUIType = objInfo.MarkObj.ComponentName;
				strBuilder.AppendFormat("\t\tpublic {0} {1};\r\n",
					strUIType, objInfo.Name);
			}

			strBuilder.AppendLine();
			strBuilder.AppendFormat("\t\tprivate {0}Data mPrivateData = null;\n", behaviourName);
			strBuilder.AppendLine();
			strBuilder.AppendFormat("\t\tpublic {0}Data mData\n", behaviourName);
			strBuilder.AppendLine("\t\t{");
			strBuilder.Append("\t\t\tget { return mPrivateData ?? (mPrivateData = new ").Append(behaviourName).Append("Data()); }")
				.AppendLine();
			strBuilder.AppendLine("\t\t\tset");
			strBuilder.AppendLine("\t\t\t{");
			strBuilder.AppendLine("\t\t\t\tmUIData = value;");
			strBuilder.AppendLine("\t\t\t\tmPrivateData = value;");
			strBuilder.AppendLine("\t\t\t}");
			strBuilder.AppendLine("\t\t}");
			strBuilder.AppendLine("\t}");
			strBuilder.AppendLine("}");
			sw.Write(strBuilder);
			sw.Flush();
			sw.Close();
		}
	}
}