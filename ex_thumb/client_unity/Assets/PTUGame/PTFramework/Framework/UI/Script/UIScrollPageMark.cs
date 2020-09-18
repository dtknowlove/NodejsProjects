using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace PTGame.Framework
{
    public class UIScrollPageMark : MonoBehaviour
    {
        public UIScrollPage scrollPage;
        public ToggleGroup toggleGroup;
        public Toggle togglePrefab;
        
        [Tooltip("页签中心位置")]
        public Vector2 centerPos;
        [Tooltip("每个页签之间的间距")]
        public Vector2 interval;

        public List<Toggle> toggleList = new List<Toggle>();

        void Awake()
        {
            AdjustTogglePos();
            scrollPage.AddPageChangeListener(OnScrollPageChanged);
        }

        public void OnScrollPageChanged(int pageCount, int currentPageIndex)
        {
            if (pageCount != toggleList.Count)
            {
                if (pageCount > toggleList.Count)
                {
                    var cc = pageCount - toggleList.Count;
                    for (var i = 0; i < cc; i++)
                    {
                        toggleList.Add(CreateToggle(i));
                    }
                }
                else if (pageCount < toggleList.Count)
                {
                    while (toggleList.Count > pageCount)
                    {
                        var t = toggleList[toggleList.Count - 1];
                        toggleList.Remove(t);
                        DestroyImmediate(t.gameObject);
                    }
                }

                AdjustTogglePos();
            }

            toggleGroup.gameObject.SetActive(pageCount > 1);
            if (currentPageIndex >= 0)
            {
                for (var i = 0; i < toggleList.Count; i++)
                {
                    if (i == currentPageIndex) toggleList[i].isOn = true;
                    else toggleList[i].isOn = false;
                }
            }
        }

        private Toggle CreateToggle(int i)
        {
            var t = Instantiate(togglePrefab);
            t.gameObject.SetActive(true);
            t.transform.SetParent(toggleGroup.transform);
            t.transform.localScale = Vector3.one;
            t.transform.localPosition = Vector3.zero;
            return t;
        }

        void AdjustTogglePos()
        {
            var startPos = centerPos - 0.5f * (toggleList.Count - 1) * interval;
            for (var i = 0; i < toggleList.Count; i++)
            {
                toggleList[i].GetComponent<RectTransform>().anchoredPosition = startPos + i * interval;
            }
        }
    }
}
