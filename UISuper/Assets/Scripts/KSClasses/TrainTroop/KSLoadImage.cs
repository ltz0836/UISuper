using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class KSLoadImage
{
    public static IEnumerator LoadImage(Image image, string path)
    {
        //string path = @"file://" + Application.dataPath + @"/Resources/Army/" + name + ".png";
        using (WWW www = new WWW(path))
        {
            yield return www;
            if (www.isDone && www.error == null)
            {
                Texture2D texture = www.texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                image.sprite = sprite;
                //image.sprite = Sprite.Create(www.texture, new Rect(0, 0, 512f, 512f), new Vector2(0.5f, 0.5f));
            }
            else
            {
                KSDebug.LogError(www.error);
            }
        }
    }
}
