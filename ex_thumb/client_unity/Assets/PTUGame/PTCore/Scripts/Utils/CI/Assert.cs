/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 * https://github.com/neuecc/ChainingAssertion
 ****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace PTGame.Core
{
    /// <summary>
    /// 断言用来单元测试的
    /// </summary>
    public class QAssert : MonoBehaviour
    {
        public static void AreStringEqual(string a, string b, object msg = null)
        {
            Assert.IsTrue(string.Equals(a, b), msg == null ? "" : msg.ToString());
        }

        public static void AreArrayEqual(List<string> a, List<string> b)
        {
            a.Equals(b);
            if (a != null && b != null && a.Count == b.Count)
            {
            }
        }
        
        /// <summary>
        /// Check that two list have the same content.
        /// </summary>
        public static void IsEqualList<T>(List<T> list1, List<T> list2)
        {
            Assert.AreEqual(list1.Count, list2.Count);
            for (var i = 0; i < list1.Count; i++)
            {
                Assert.AreEqual(list1[i], list2[i]);
            }
        }
        
        /// <summary>
        /// Check that two arrays have the same content.
        /// </summary>
        public static void IsEqualArrays<T>(T[] array1, T[] array2)
        {
            Assert.AreEqual(array1.Length, array2.Length);
            for (var i = 0; i < array1.Length; i++)
            {
                Assert.AreEqual(array1[i], array2[i]);
            }
        }
    }
}