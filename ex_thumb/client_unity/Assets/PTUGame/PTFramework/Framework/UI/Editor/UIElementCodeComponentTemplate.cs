/****************************************************************************
 * 2017 ~ 2018.5 liqingyun
 ****************************************************************************/

using System;
using UnityEngine;

namespace PTGame.Framework.Editor
{
	using System.Text;
	using System.IO;

	public static class UIElementCodeTemplate
	{
		public static void Generate(string generateFilePath, string behaviourName, string nameSpace,
			ElementCodeData elementCodeData)
		{
			var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
			var strBuilder = new StringBuilder();

			var markType = elementCodeData.MarkedObjInfo.MarkObj.GetUIMarkType();
			var componentName = elementCodeData.MarkedObjInfo.MarkObj.ComponentName;

		    strBuilder.AppendLine("/****************************************************************************");
		    strBuilder.AppendFormat(" * {0}.{1} {2}\n", DateTime.Now.Year, DateTime.Now.Month,SystemInfo.deviceName);
		    strBuilder.AppendLine(" ****************************************************************************/");
		    strBuilder.AppendLine();
		    
		    strBuilder.AppendLine("using System;");
		    strBuilder.AppendLine("using System.Collections.Generic;");
		    strBuilder.AppendLine("using UnityEngine;");
		    strBuilder.AppendLine("using UnityEngine.UI;");
		    strBuilder.AppendLine("using PTGame.Framework;").AppendLine();

		    strBuilder.AppendLine("namespace " + nameSpace);
		    strBuilder.AppendLine("{");
			strBuilder.AppendFormat("\tpublic partial class {0} : {1}", behaviourName,
				markType == UIMarkType.Component ? "UIComponent" : "UIElement");
		    strBuilder.AppendLine();
		    strBuilder.AppendLine("\t{");
			strBuilder.Append("\t\t").AppendLine("private void Awake()");
		    strBuilder.Append("\t\t").AppendLine("{");
		    strBuilder.Append("\t\t").AppendLine("}").AppendLine();
		    strBuilder.Append("\t}").AppendLine();
		    strBuilder.Append("}");

		    sw.Write(strBuilder);
		    sw.Flush();
		    sw.Close();
		}
	}

	public static class UIElementCodeComponentTemplate
	{
		public static void Generate(string generateFilePath, string behaviourName, string nameSpace,
			ElementCodeData elementCodeData)
		{
			var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
			var strBuilder = new StringBuilder();

			strBuilder.AppendLine("/****************************************************************************");
			strBuilder.AppendFormat(" * {0}.{1} {2}\n", DateTime.Now.Year, DateTime.Now.Month,SystemInfo.deviceName);
			strBuilder.AppendLine(" ****************************************************************************/");
			strBuilder.AppendLine();
			strBuilder.AppendLine("using UnityEngine;");
			strBuilder.AppendLine("using UnityEngine.UI;");
			strBuilder.AppendLine();
			strBuilder.AppendLine("namespace " + nameSpace);
			strBuilder.AppendLine("{");
			strBuilder.AppendFormat("\tpublic partial class {0}", behaviourName);
			strBuilder.AppendLine();
			strBuilder.AppendLine("\t{");

			foreach (var markInfo in elementCodeData.MarkedObjInfos)
			{
				var strUIType = markInfo.MarkObj.ComponentName;
				strBuilder.AppendFormat("\t\t[SerializeField] public {0} {1};\r\n",
					strUIType, markInfo.Name);
			}

			strBuilder.AppendLine();

			strBuilder.Append("\t\t").AppendLine("public void Clear()");
			strBuilder.Append("\t\t").AppendLine("{");
			foreach (var markInfo in elementCodeData.MarkedObjInfos)
			{
				strBuilder.AppendFormat("\t\t\t{0} = null;\r\n",
					markInfo.Name);
			}

			strBuilder.Append("\t\t").AppendLine("}");
			strBuilder.AppendLine();

			strBuilder.Append("\t\t").AppendLine("public override string ComponentName");
			strBuilder.Append("\t\t").AppendLine("{");
			strBuilder.Append("\t\t\t");
			strBuilder.AppendLine("get { return \"" +  elementCodeData.MarkedObjInfo.MarkObj.ComponentName + "\";}");
			strBuilder.Append("\t\t").AppendLine("}");
			strBuilder.AppendLine("\t}");
			strBuilder.AppendLine("}");
			sw.Write(strBuilder);
			sw.Flush();
			sw.Close();
		}
	}
}