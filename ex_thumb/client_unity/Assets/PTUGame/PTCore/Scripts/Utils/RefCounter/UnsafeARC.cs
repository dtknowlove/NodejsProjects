/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    /// Automatic Entity Reference Counting (AERC)
    /// is used internally to prevent pooling retained entities.
    /// If you use retain manually you also have to
    /// release it manually at some point.
    /// UnsafeARC doesn't check if the entity has already been
    /// retained or released. It's faster, but you lose the information
    /// about the owners.
    public sealed class UnsafeARC : RefCounter
    {
    }
}