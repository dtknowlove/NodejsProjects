/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    public interface ICodeGeneratorInterface
    {
        string Name { get; }
        
        int Priority { get; }
        
        bool IsEnabledByDefault { get; }

        bool RunInDryMode { get; }
    }
}