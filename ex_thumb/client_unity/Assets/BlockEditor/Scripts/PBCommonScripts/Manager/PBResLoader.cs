#if !BLOCK_EDITOR
using PTGame.ResKit;
using Object = UnityEngine.Object;

namespace Putao.PaiBloks.Common
{
	public class PBResLoader : PTGame.Core.PTSingleton<PBResLoader>
	{
		private PBResLoader()
		{

		}

		private ResLoader mPrivateResLoader = null;

		private ResLoader mResLoader
		{
			get { return mPrivateResLoader ?? (mPrivateResLoader = ResLoader.Allocate()); }
		}

		public T Load<T>(string assetName) where T : Object
		{
			return mResLoader.LoadSync<T>(assetName);
		}

		public T Load<T>(string bundleName, string assetName) where T : Object
		{
			return mResLoader.LoadSync<T>(assetName,bundleName);
		}

		public void Release()
		{
			if (mPrivateResLoader != null)
				mPrivateResLoader.Recycle2Cache();
			mPrivateResLoader = null;
		}
	}
}

#endif