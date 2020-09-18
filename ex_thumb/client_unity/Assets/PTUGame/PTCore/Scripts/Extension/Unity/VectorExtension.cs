/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using UnityEngine;

namespace PTGame.Core
{
    public static class VectorExtension
    {
        public static float SmallestValueBetweenXY(this Vector3 selfVector)
        {
            return selfVector.x < selfVector.y ? selfVector.x : selfVector.y;
        }
        
        public static float SmallestValueBetweenXY(this Vector2 selfVector)
        {
            return selfVector.x < selfVector.y ? selfVector.x : selfVector.y;
        }
        
        public static Vector3 X(this Vector3 selfVector,float value)
        {
            selfVector.x = value;
            return selfVector;
        }
        
        public static Vector3 Y(this Vector3 selfVector,float value)
        {
            selfVector.y = value;
            return selfVector;
        }
        
        public static Vector2 X(this Vector2 selfVector,float value)
        {
            selfVector.x = value;
            return selfVector;
        }
        
        public static Vector2 Y(this Vector2 selfVector,float value)
        {
            selfVector.y = value;
            return selfVector;
        }

        public static Vector2 ToScreenPoint(this Vector3 selfVector,Camera camera)
        {
            return camera.WorldToScreenPoint(selfVector);
        }
    }
}