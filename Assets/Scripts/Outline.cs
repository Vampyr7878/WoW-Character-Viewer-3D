using UnityEngine;
using UnityEngine.UI;

//Script that generates white duplicate of an image to use as a base for Outline script
public class Outline : MonoBehaviour
{
    public Image image;

    void Update()
    {
        Texture2D texture = new Texture2D(image.sprite.texture.width, image.sprite.texture.height);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color color = image.sprite.texture.GetPixel(x, y);
                color = new Color(1f, 1f, 1f, color.a);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
    }
}
