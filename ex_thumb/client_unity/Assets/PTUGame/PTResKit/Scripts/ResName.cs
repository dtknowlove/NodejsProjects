using System;

namespace PTGame.ResKit
{
    public class ResName
    {
        public readonly string AssetName;
        public readonly string ABName;
        public readonly string Extension;
        public readonly string ABGroup;
        public readonly string FullName;

        public readonly eResType ResType;

        public static string RESOURCES_PREFIX = "resources/";
        public static string FILE_PREFIX = "singlefile/";

        public string AssetNameWithExtension
        {
            get { return AssetName + Extension; }
        }

        public ResName(string abName, string assetName, string extension)
        {
            AssetName = null;
            ABName = null;
            Extension = null;
            ABGroup = null;
            FullName = "";

            if (!string.IsNullOrEmpty(assetName))
            {
                if (assetName.StartsWith(RESOURCES_PREFIX))
                    ResType = eResType.Internal;
                else if (assetName.StartsWith(FILE_PREFIX))
                    ResType = eResType.SingleFile;
                else if (string.Equals(extension, ".unity", StringComparison.OrdinalIgnoreCase))
                    ResType = eResType.Scene;
                else
                    ResType = eResType.Asset;
            }
            else if (!string.IsNullOrEmpty(abName))
            {
                ResType = eResType.Assetbundle;
            }

            if (!string.IsNullOrEmpty(abName))
            {
                ABName = abName.ToLower();
                FullName += ABName + "#";
            }

            if (!string.IsNullOrEmpty(assetName))
            {
                AssetName = ResType != eResType.SingleFile ? assetName.ToLower() : assetName;
                FullName += AssetName;
            }

            if (!string.IsNullOrEmpty(extension))
            {
                Extension = ResType != eResType.SingleFile ? extension.ToLower() : extension;
                FullName += Extension;
            }
        }

        public ResName(string abGroup, string abName, string assetName, string extension)
            : this(abName, assetName, extension)
        {
            if (!string.IsNullOrEmpty(abGroup))
            {
                ABGroup = abGroup;
                FullName = ABGroup + "@" + FullName;
            }
        }

        public override string ToString()
        {
            return FullName;
        }

        public override bool Equals(object obj)
        {
            ResName target = obj as ResName;
            if (target == null) return false;
            return target.FullName.Equals(this.FullName);
        }

        public static bool NotABAsset(string assetName)
        {
            return assetName.StartsWith(RESOURCES_PREFIX) || assetName.StartsWith(FILE_PREFIX);
        }
    }
}