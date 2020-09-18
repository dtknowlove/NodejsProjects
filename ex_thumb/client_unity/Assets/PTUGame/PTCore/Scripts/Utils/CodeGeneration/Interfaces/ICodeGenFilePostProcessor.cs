/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    /// Implement this interface if you want to create a custom post processor.
    public interface ICodeGenFilePostProcessor : ICodeGeneratorInterface
    {
        CodeGenFile[] PostProcess(CodeGenFile[] files);
    }
}