/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    public class CodeGenFile
    {
        public string FileName { get; set; }

        public string FileContent
        {
            get { return mFileContent; }
            set { mFileContent = value.ToUnixLineEndings(); }
        }

        public string GeneratorName { get; set; }

        string mFileContent;

        public CodeGenFile(string fileName, string fileContent, string generatorName)
        {
            FileName = fileName;
            FileContent = fileContent;
            GeneratorName = generatorName;
        }
    }
}