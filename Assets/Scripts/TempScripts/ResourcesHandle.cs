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
        [StringValue("RES_ERROR")]
        RES_ERROR = 0,
        [StringValue("RES_GAMEOBJ")]
        RES_GAMEOBJ = 1 << 0,
        [StringValue("RES_TEXTURE2D")]
        RES_TEXTURE2D = 1 << 1,
        [StringValue("RES_AUDIO")]
        RES_AUDIO = 1 << 2,
        [StringValue("RES_UI_FONT")]
        RES_UI_FONT = 1 << 3,
        [StringValue("RES_UI_ATLAS")]
        RES_UI_ATLAS = 1 << 4,
        [StringValue("RES_TRUETYPE_FONT")]
        RES_TRUETYPE_FONT = 1 << 5,
        [StringValue("RES_TEXT_ASSET")]
        RES_TEXT_ASSET = 1 << 6,
        [StringValue("RES_SCRIPTABLEOBJ")]
        RES_SCRIPTABLEOBJ = 1 << 7,
    }

    public enum eSTORAGETYPE
    {
        [StringValue("STORAGE_ERROR")]
        STORAGE_ERROR = 0,
        [StringValue("STORAGE_SCENE")]
        STORAGE_SCENE = 1 << 0,
        [StringValue("STORAGE_PERMANENT")]
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
