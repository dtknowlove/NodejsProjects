//using System.IO;
//using UnityEditor;
//using UnityEngine;
//
//namespace com.putao.paibloks.editor
//{
//    public class PEThumbPostProcessor : AssetPostprocessor
//    {
//        public static void OnPostprocessAllAssets(
//            string[] importedAssets
//            , string[] deletedAssets
//            , string[] movedAssets
//            , string[] movedFromAssetPaths)
//        {
//            foreach (string str in importedAssets)
//            {
//                string dirName = Path.GetDirectoryName(str);
//                if (dirName.EndsWith("Block_Thumbs"))
//                {
//                    PEThumbEditor.CorrectThumb(str);
//                }
//                else if (dirName.EndsWith("Texture_Textures"))
//                {
//                    PEThumbEditor.CorrectTexture(str);
//                }
//            }
//        }
//    }
//}