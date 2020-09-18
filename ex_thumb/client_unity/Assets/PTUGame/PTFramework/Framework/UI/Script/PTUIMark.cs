/****************************************************************************
 * 2017 ~ 2018.5 liqingyun
****************************************************************************/

namespace PTGame.Framework
{
	using UnityEngine;
	using UnityEngine.UI;
	
	/// <summary>
	/// UI的标记
	/// </summary>
	public class PTUIMark : MonoBehaviour /* UIBinding */, IUIMark
	{
		public UIMarkType MarkType = UIMarkType.DefaultUnityElement;

		public Transform Transform
		{
			get { return transform; }
		}

		public string CustomComponentName;

		public UIMarkType GetUIMarkType()
		{
			return MarkType;
		}

		public virtual string ComponentName
		{
			get
			{
				if (MarkType == UIMarkType.DefaultUnityElement)
				{
					if (null != GetComponent("SkeletonAnimation"))
						return "SkeletonAnimation";
					if (null != GetComponent<ScrollRect>())
						return "ScrollRect";
					if (null != GetComponent<InputField>())
						return "InputField";
					if (null != GetComponent<Text>())
						return "Text";
					if (null != GetComponent<Button>())
						return "Button";
					if (null != GetComponent<RawImage>())
						return "RawImage";
					if (null != GetComponent<Toggle>())
						return "Toggle";
					if (null != GetComponent<Slider>())
						return "Slider";
					if (null != GetComponent<Scrollbar>())
						return "Scrollbar";
					if (null != GetComponent<Image>())
						return "Image";
					if (null != GetComponent<ToggleGroup>())
						return "ToggleGroup";
					if (null != GetComponent<Animator>())
						return "Animator";
					if (null != GetComponent<Canvas>())
						return "Canvas";
					if (null != GetComponent<RectTransform>())
						return "RectTransform";

					return "Transform";
				}

				return CustomComponentName;
			}
		}
	}
}