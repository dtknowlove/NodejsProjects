using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace PTGame.Utils.CVS
{
	public class PTCSV2Class {

//		public const char CommaCharacter = ',';
		public const string CommaCharacter = "@@@\t";

	//	[MenuItem("PutaoTool/TestGenerate")]
	//	public static void TestGenerateClass()
	//	{
	//		string content = File.ReadAllText (Application.dataPath+"/PTUGame/PTConfigUtil/Resources/TTT.csv");
	//		CenerateClassFromCSV (content,Application.dataPath,"TTT");
	//	}
	//
	//	public static void LoadCSV<T>()
	//	{
	//	
	//	}

		public static void GenerateClassFromCsvFile(string path,string classDir)
		{
			string content = File.ReadAllText (path);
			string className = Path.GetFileNameWithoutExtension (path);
			className = className.Substring (0, 1).ToUpper () + className.Substring (1);
			CenerateClassFromCSV (content,classDir,className);
		}

		public static void CenerateClassFromCSV(string content,string classDir,string className)
		{
			content = content.Replace ("\r", "");
			
			string[] hList = Regex.Split(content,"@@@@@@\n",RegexOptions.None);

//			StringReader streamReader = new StringReader (content);

			string lineOne = hList[0];

			string[] types = Regex.Split(lineOne,CommaCharacter,RegexOptions.None);

			foreach (var g in types)
			{
				Debug.LogError(g);
			}
			
//			streamReader.ReadLine ();

			string lineTwo = hList[2];
			string[] fields = Regex.Split(lineTwo,CommaCharacter,RegexOptions.None);

			Dictionary<string,string>  props = new Dictionary<string, string> ();

			for(int i=0;i<fields.Length;i++)
			{
                Debug.LogError(">>>>>"+fields[i]);
				props.Add (fields[i],types[i]);
			}

//			streamReader.Close ();

			GenerateConfigClass (classDir,props,className);

		}

		public static void GenerateConfigClass(string classDir,Dictionary<string,string> props,string className)
		{

			FileStream fileStream = new FileStream(Path.Combine(classDir,className+"Config.cs"),FileMode.OpenOrCreate);


			string templetePath = Application.dataPath+"/PTUGame/PTCSVUtil/Templete/ConfigTemplete.txt";


			string proStr = @"public {0} {1} { get; set; }";

			string  propStrs="";

			foreach(var prop in props)
			{
				propStrs += "\t";
				string fieldName = prop.Key.Substring (0, 1).ToUpper () + prop.Key.Substring (1);

				propStrs += proStr.Replace ("{0}",prop.Value).Replace("{1}",fieldName);
				propStrs += "\n";

			}

			string classText = File.ReadAllText(templetePath);

			classText = classText.Replace("{0}",className);

			classText = classText.Replace ("{3}",propStrs);

			StreamWriter outfile =  new StreamWriter(fileStream);

			outfile.Write(classText);

			outfile.Close();

			fileStream.Close();

			AssetDatabase.Refresh ();
		}

		public static void GenerateClass(string classFile,Dictionary<string,string> props)
		{

			string className = Path.GetFileNameWithoutExtension (classFile);

			if(File.Exists(classFile))
			{
				File.Delete(classFile);
			}

			FileStream fileStream = new FileStream(classFile,FileMode.OpenOrCreate);

			StreamWriter outfile =  new StreamWriter(fileStream);

			outfile.WriteLine("using UnityEngine;");
			outfile.WriteLine("using System.Collections;");

			outfile.WriteLine("");
			outfile.WriteLine("public class "+className+"{");
			outfile.WriteLine(" ");
			outfile.WriteLine(" ");

			string proStr = @"public {0} {1} { get; set; }";

			foreach(var prop in props)
			{
				string fieldName = prop.Key.Substring (0, 1).ToUpper () + prop.Key.Substring (1);

				string t = proStr.Replace ("{0}",prop.Value).Replace("{1}",fieldName);

				outfile.WriteLine(" ");

				outfile.WriteLine(t);

				outfile.WriteLine(" ");
		
			}
			outfile.WriteLine(" ");

			outfile.WriteLine("}");

			outfile.Close();
			fileStream.Close();

			AssetDatabase.Refresh();
		}

	}
}
