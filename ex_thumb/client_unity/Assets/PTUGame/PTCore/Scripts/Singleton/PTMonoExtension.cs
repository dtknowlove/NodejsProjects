/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PTMonoExtension {
    
    public static T GetOrAddComponent<T> (this Component child) where T: Component {
        T result = child.GetComponent<T>();
        if (result == null) {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }

}
