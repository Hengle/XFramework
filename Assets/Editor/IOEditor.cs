using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class IOEditor {

    [MenuItem("MyTools/DiminishTex")]
	public static void ChangeTextureMaxSize()
    {
        // 编辑器下弹出警告窗口
        string[] paths = Directory.GetFiles(Application.dataPath + "/TestFolder","*",SearchOption.AllDirectories);
        List<Texture> textures = new List<Texture>();
        foreach (var path in paths)
        {
            string tempPath = path.Replace(@"\", "/");
            tempPath = tempPath.Substring(tempPath.IndexOf("Assets"));
            TextureImporter tex = AssetImporter.GetAtPath(tempPath) as TextureImporter;
            
            if (tex != null)
            {
                tex.maxTextureSize = 256;
            }
        }
    }
}

public class AutoSetImpotor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        //自动设置类型;
        TextureImporter textureImporter = (TextureImporter)assetImporter;

        textureImporter.textureType = TextureImporterType.Sprite;
    }
}
