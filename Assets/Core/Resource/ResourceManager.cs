using System.Collections.Generic;
using UnityEngine;

namespace XDEDZL
{
    public class ResourceManager : IGameModule
{
        private Dictionary<string, AssetBundle> abDic;
        private readonly AssetBundleManifest manifest;
        private readonly AssetBundle mainfestAB;

        public int Priority => throw new System.NotImplementedException();

        public ResourceManager()
        {
            abDic = new Dictionary<string, AssetBundle>();
            mainfestAB = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundle/AssetBundle");
            if (mainfestAB != null)
                manifest = mainfestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        public T Load<T>(string path, string name) where T : Object
        {
            //return Resources.Load<T>(path + "/" + name);
            return GetAssetBundle(path).LoadAsset<T>(name);
        }

        private AssetBundle GetAssetBundle(string path)
        {
            abDic.TryGetValue(path, out AssetBundle ab);
            if (ab == null)
            {
                ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + path);
                if (ab == null)
                    Debug.LogError(path + " 为空");
                abDic.Add(path, ab);
            }

            //加载依赖
            string[] dependencies = manifest.GetAllDependencies(ab.name);
            foreach (var item in dependencies)
            {
                if (!abDic.ContainsKey(item))
                {
                    AssetBundle dependAb = GetAssetBundle(item);
                    abDic.Add(item, dependAb);
                }
            }

            return ab;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        public void Shutdown()
        {

        }
    }
}