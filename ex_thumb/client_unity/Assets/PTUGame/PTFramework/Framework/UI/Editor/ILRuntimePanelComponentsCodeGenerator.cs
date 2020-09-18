/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

namespace PTGame.Framework.Editor
{
    using System.Text;
    using System.IO;
    using System.Collections.Generic;

    public class ILRuntimePanelComponentsCodeGenerator
    {
         public static void Generate(string generateFilePath,string behaviourName,string nameSpace, PanelCodeData panelCodeData)
        {
            var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
            var strBuilder = new StringBuilder();

            strBuilder.AppendLine("using UnityEngine;");
            strBuilder.AppendLine("using UnityEngine.UI;");
            strBuilder.AppendLine("using PTGame.Framework;");
            strBuilder.AppendLine ();
            strBuilder.AppendLine ("namespace " + nameSpace);
            strBuilder.AppendLine ("{");
            strBuilder.AppendFormat("\tpublic partial class {0}", behaviourName);
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t{");
            strBuilder.AppendFormat("\t\tpublic const string NAME = \"{0}\";\n",behaviourName);
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t\tpublic Transform transform;");
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t\tpublic GameObject gameObject { get { return transform.gameObject; }}");
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t\tpublic ILRuntimePanel Behaviour; // TODO:这里要改成继承的");
            strBuilder.AppendLine();
            strBuilder.AppendFormat("\t\tpublic {0}(Transform trans)\n",behaviourName);
            strBuilder.AppendLine("\t\t{");
            strBuilder.AppendLine("\t\t\ttransform = trans;");
            strBuilder.AppendLine("\t\t\tBehaviour = trans.GetComponent<ILRuntimePanel>();");
            strBuilder.AppendLine("\t\t\tInternalInitView(transform);");
            strBuilder.AppendLine("\t\t\tInitUI();");
            strBuilder.AppendLine("\t\t\tRegisterUIEvent();");
            strBuilder.AppendLine("\t\t}");
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t\tpublic void CloseSelf()");
            strBuilder.AppendLine("\t\t{");
            strBuilder.AppendLine("\t\t\tUIMgr.ClosePanel(NAME);");
            strBuilder.AppendLine("\t\t}");
            strBuilder.AppendLine();
            strBuilder.AppendLine("\t\tpublic void InternalClose()");
            strBuilder.AppendLine("\t\t{");
            strBuilder.AppendLine("\t\t\tInternalClearView();");
            strBuilder.AppendLine("\t\t\tOnClose();");
            strBuilder.AppendLine("\t\t}");
            strBuilder.AppendLine();

            foreach (var markInfo in panelCodeData.MarkedObjInfos)
            {
                var strUIType = markInfo.MarkObj.ComponentName;
                strBuilder.AppendFormat("\t\tpublic {0} {1};\r\n",
                    strUIType,markInfo.Name);
            }

            strBuilder.AppendLine ();

            strBuilder.Append("\t\t").AppendLine("private void InternalInitView(Transform transform)");
            strBuilder.Append("\t\t").AppendLine("{");
            foreach (var markInfo in panelCodeData.MarkedObjInfos)
            {
                var strUIType = markInfo.MarkObj.ComponentName;
                strBuilder.AppendFormat("\t\t\t{0} = transform.Find(\"{1}\").GetComponent<{2}>();\r\n",markInfo.Name,markInfo.PathToElement, strUIType);
            }

            strBuilder.Append("\t\t").AppendLine("}");
            strBuilder.AppendLine();
			
            strBuilder.Append("\t\t").AppendLine("public void InternalClearView()");
            strBuilder.Append("\t\t").AppendLine("{");
            foreach (var markInfo in panelCodeData.MarkedObjInfos)
            {
                strBuilder.AppendFormat("\t\t\t{0} = null;\r\n",
                    markInfo.Name);
            }

            strBuilder.Append("\t\t").AppendLine("}").AppendLine();
            strBuilder.AppendLine("\t}");
            strBuilder.AppendLine();
            strBuilder.AppendFormat("\tpublic static class {0}Extention\n", behaviourName);
            strBuilder.AppendLine("\t{");
            strBuilder.AppendFormat("\t\tpublic static {0} As{0}(this PTUIBehaviour selfRuntimePanel)\n", behaviourName);
            strBuilder.AppendLine("\t\t{");
            strBuilder.AppendFormat("\t\t\treturn selfRuntimePanel.AsiIlRuntimePanel().ILObject as {0};\n",
                behaviourName);
            strBuilder.AppendLine("\t\t}");
            strBuilder.AppendLine("\t}");
            strBuilder.AppendLine("}");
    
            sw.Write(strBuilder);
            sw.Flush();
            sw.Close();
        }
    }
}