using UnityEngine;
using UnityEngine.EventSystems;

namespace PTGame.Core
{
	public static class PTVoidDelegate
	{
		public delegate void WithVoid();

		public delegate void WithGo(GameObject go);

		public delegate void WithParams(params object[] paramList);

		public delegate void WithEvent(BaseEventData data);

		public delegate void WithObj(Object obj);

		public delegate void WithBool(bool value);

		public delegate void WithString(string str);

		public delegate void WithGeneric<in T>(T value);

		public delegate void WithGeneric<in T, in K>(T t,K k);
	}
}