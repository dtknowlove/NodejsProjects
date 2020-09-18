using System.Collections.Generic;
using System.Linq;


namespace Putao.PaiBloks.Common
{
    public enum PartType
    {
        /// <summary>
        /// 大颗粒
        /// </summary>
        Large,
        /// <summary>
        /// 小颗粒
        /// </summary>
        Small,
        /// <summary>
        /// 贴纸
        /// </summary>
        Sticker
    }

    public class PPBlockDB
    {
        private PPBlockDB() {}
        #region For APP

#if !BLOCK_EDITOR
        private static PPBlockDB instance = null;

        public static PPBlockDB Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PPBlockDB();
                }
                return instance;
            }
        }
#endif

   
        private readonly Dictionary<string,PartType> mStartPre2PartTypes=new Dictionary<string, PartType>
        {
            {"pbs_|fig_",PartType.Small},
            {"sticker",PartType.Sticker}
        };

        public PartType GetLargeParticlesType(string name)
        {
            foreach (var prefix in mStartPre2PartTypes.Keys)
            {
                var pres = prefix.Split('|');
                if (pres.Any(name.StartsWith))
                {
                    return mStartPre2PartTypes[prefix];
                }
            }
            return PartType.Large;
        }

        #endregion
    }
}
