namespace JMResource
{
    using System.Collections;
    using UnityEngine;
    public class LoadingDelegateInterFace
    {

        public delegate void VoidDelegate();

        public delegate void LoadingStringFinishedDelegate(string fileName, string result);

        public delegate void LoadingArrayListFinishedDelegate(string fileName, ArrayList resultList);

        public delegate void LoadingObjectFinishedDelegate(string fileName, object resultObject);

        public delegate void LoadingFinishedDelegate(UnityEngine.Object obj, string name, params object[] args);

        public delegate void LoadingSceneFinishedDelegate(string sceneName, params object[] args);

        public delegate void LoadingObjectsFinishedDelegate(Object[] objs, string[] tagNames, params object[] args);

        public delegate void LoadingProgressDelegate(float progress, string name, params object[] args);

        public delegate void LoadingObjectsProgressDelegate(float progress, string[] tagNames);
    }

    public enum eRESOURCETYPE
    {

        RES_ERROR = 0,

        RES_GAMEOBJ = 1 << 0,

        RES_TEXTURE2D = 1 << 1,

        RES_AUDIO = 1 << 2,

        RES_UI_FONT = 1 << 3,

        RES_UI_ATLAS = 1 << 4,

        RES_TRUETYPE_FONT = 1 << 5,

        RES_TEXT_ASSET = 1 << 6,

        RES_SCRIPTABLEOBJ = 1 << 7,
    }

    public enum eSTORAGETYPE
    {

        STORAGE_ERROR = 0,

        STORAGE_SCENE = 1 << 0,

        STORAGE_PERMANENT = 1 << 1,
    }

    public class ResourceItemBase
    {
        public string resourceName;
        public eRESOURCETYPE resourceType;
        public eSTORAGETYPE storageType;
        public bool instantLoad; 	// Load asset into resource table right away after download AB.
        public bool loadAsync;		// use LoadAsync
        public ResourceItemBase(string _resourceName,
                            eRESOURCETYPE _resourceType = eRESOURCETYPE.RES_GAMEOBJ,
                            eSTORAGETYPE _storageType = eSTORAGETYPE.STORAGE_SCENE,
                            bool _instantLoad = true,
                            bool _loadAsync = false
                            )
        {
            resourceName = _resourceName;
            resourceType = _resourceType;
            storageType = _storageType;
            instantLoad = _instantLoad;
            loadAsync = _loadAsync;
        }
    }

    public class ResourcesHandle : MonoBehaviour
    {
        public bool m_bIsdeveloper = true;
        public virtual Texture2D GetTextureByName(string name)
        {
            return null;
        }

        public virtual bool TextureExists(string name)
        {
            return false;
        }

        //public virtual UIAtlas GetAtlasByName(string name)
        //{
        //    return null;
        //}

        public virtual void LoadFile(string fileName, LoadingDelegateInterFace.LoadingStringFinishedDelegate finishDelegate)
        {

        }
        public virtual bool loadSingleResource(ResourceItemBase resourceItem, LoadingDelegateInterFace.LoadingFinishedDelegate finishDelegate, LoadingDelegateInterFace.LoadingProgressDelegate progressDelegate, params object[] args)
        {
            return false;
        }
        public virtual bool LoadSingleAssetBundleResource(ResourceItemBase resourceItem,
                       LoadingDelegateInterFace.LoadingFinishedDelegate finishDelegate,
                       LoadingDelegateInterFace.LoadingProgressDelegate progressDelegate,
                       params object[] args)
        {

            return false;
        }

        public virtual UnityEngine.Object getResourceFromLocalFile(ResourceItemBase resourceItem)
        {
            return null;
        }

        public virtual void AddTexture(string name, Texture2D tex)
        {

        }


        //public virtual void AddAtlas(string name, UIAtlas atlas)
        //{

        //}
    }
}
