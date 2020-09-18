/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
****************************************************************************/

namespace PTGame.Framework.Editor
{
    using System.Text;
    using System.IO;
    
    public class ILRuntimePanelCodeTemplate
    {
	    public static void Generate(string generateFilePath, string behaviourName,string nameSpace)
	    {
		    var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
		    var strBuilder = new StringBuilder();

		    strBuilder.AppendLine("using PTGame.Framework;").AppendLine();

		    strBuilder.AppendLine("namespace " + nameSpace);
		    strBuilder.AppendLine("{");
		    strBuilder.AppendLine();
		    strBuilder.AppendFormat("\tpublic partial class {0}", behaviourName);
		    strBuilder.AppendLine();
		    strBuilder.AppendLine("\t{");
		    strBuilder.Append("\t\t").AppendLine("void InitUI()");
		    strBuilder.Append("\t\t").AppendLine("{");
		    strBuilder.Append("\t\t").Append("\t").AppendLine("//please add init code here");
		    strBuilder.Append("\t\t").AppendLine("}").AppendLine();
		    strBuilder.Append("\t\t").AppendLine("void RegisterUIEvent()");
		    strBuilder.Append("\t\t").AppendLine("{");
		    strBuilder.Append("\t\t").AppendLine("}").AppendLine();
		    strBuilder.Append("\t\t").AppendLine("void OnClose()");
		    strBuilder.Append("\t\t").AppendLine("{");
		    strBuilder.Append("\t\t").AppendLine("}").AppendLine();
		    strBuilder.Append("\t}").AppendLine();
		    strBuilder.Append("}");

		    sw.Write(strBuilder);
		    sw.Flush();
		    sw.Close();
	    }
    }
}