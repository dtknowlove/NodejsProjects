/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    /// Implement this interface if you want to create a custom code generator.
    public interface ICodeGenerator : ICodeGeneratorInterface
    {
        CodeGenFile[] Generate(CodeGeneratorData[] data);
    }
}