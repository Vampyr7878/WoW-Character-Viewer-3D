using UnityEngine;

namespace Assets.WoW
{
    public class ItemDescription
    {
        // Description text
        public string Text { get; private set; }
        // Description Color
        public Color32 Color { get; private set; }

        public ItemDescription(string text, Color32 color)
        {
            Text = text;
            Color = color;
        }
    }
}
