using UnityEngine;
using System.Collections;

public class VariantLoader : MonoBehaviour
{
    static string basePath = "file://" + System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/AllMyBundles/";

    // Use this for initialization
    IEnumerator Start()
    {
        if (Screen.height == 900  ||
            Screen.height == 1080 ||
            Screen.height == 1200)
        {
            var s = new WWW(basePath + "sprites.hd");
            yield return s;
            if (s.assetBundle == null)
                Debug.Log("load hd failed");
        }
        else
        {
            var s = new WWW(basePath + "sprites.sd");
            yield return s;
            if (s.assetBundle == null)
                Debug.Log("load sd failed");
        }

        var scenes = new WWW(basePath + "scenes");
        yield return scenes;
        if (scenes.assetBundle == null)
            Debug.Log("load scene failed");

        Application.LoadLevel("Demo");
    }
}
