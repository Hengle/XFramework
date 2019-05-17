#define ABTest
#if!UNITY_EDITOR || ABTest
#define AB
#endif

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XDEDZL
{
    public class ResourceManager : IGameModule
    {
        /// <summary>
        /// AB包的缓存
        /// </summary>
        private Dictionary<string, AssetBundle> m_ABDic;
        /// <summary>
        /// 用于获取AB包的依赖关系
        /// </summary>
        private AssetBundleManifest m_Mainfest;

        public int Priority { get { return 100; } }

        private readonly string ABPath = Application.streamingAssetsPath + "/AssetBundles";

        public ResourceManager()
        {
#if AB
            m_ABDic = new Dictionary<string, AssetBundle>();

            AssetBundle m_MainfestAB = AssetBundle.LoadFromFile(ABPath + "/AssetBundles");
            if (m_MainfestAB != null)
                m_Mainfest = m_MainfestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
#else

#endif
        }

        public T Load<T>(string path, string name) where T : Object
        {
            if (path.Substring(0, 4) == "Res/")
            {
                path = path.Substring(3, path.Length - 4);
                return Resources.Load<T>(path);
            }
#if AB
            return GetAssetBundle(path).LoadAsset<T>(name);
#else
            return LoadWithAssetDataBase<T>(path, name);
#endif
        }

        /// <summary>
        /// 获取一个AB包文件
        /// </summary>
        /// <param name="path">相对路径</param>
        /// <returns></returns>
        public AssetBundle GetAssetBundle(string path)
        {
            m_ABDic.TryGetValue(path, out AssetBundle ab);
            if (ab == null)
            {
                path = path.ToLower();
                string abName = string.IsNullOrEmpty(path) ? "" : "/" + path;

                ab = AssetBundle.LoadFromFile(ABPath + abName + ".ab");
                if (ab == null)
                    Debug.LogError(path + " 为空");
                m_ABDic.Add(path, ab);
            }

            //加载当前AB包的依赖包
            string[] dependencies = m_Mainfest.GetAllDependencies(ab.name);

            foreach (var item in dependencies)
            {
                string key = item.Substring(0, item.Length - 3);  // 对key去除.ab
                if (!m_ABDic.ContainsKey(key))
                {
                    AssetBundle dependAb = GetAssetBundle(key);
                    m_ABDic.Add(item, dependAb);
                }
            }

            return ab;
        }

        /// <summary>
        /// 卸载AB包
        /// </summary>
        /// <param name="path"></param>
        /// <param name="unLoadAllObjects"></param>
        public void UnLoad(string path,bool unLoadAllObjects = true)
        {
            GetAssetBundle(path).Unload(unLoadAllObjects);
            m_ABDic.Remove(path);
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        public void Shutdown()
        {
            m_ABDic.Clear();
            m_Mainfest = null;
        }


#if !AB

        private T LoadWithAssetDataBase<T>(string path, string name) where T : Object
        {
            string suffix = ""; // 后缀
            switch (typeof(T).Name)
            {
                case "GameObject":
                    suffix = ".prefab";
                    break;
                case "Material":
                    suffix = ".mat";
                    break;
                default:
                    break;
            }
            return AssetDatabase.LoadAssetAtPath<T>("Assets/ResourcesAB/" + path + "/" + name + suffix);
        }

#endif
    }
}