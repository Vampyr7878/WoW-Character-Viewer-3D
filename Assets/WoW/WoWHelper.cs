using UnityEngine;

namespace WoW
{
    //Helper class for some WoW related things.
    public class WoWHelper
    {
        //Color for each item quality
        public static UnityEngine.Color QualityColor(int quality)
        {
            UnityEngine.Color color;
            switch (quality)
            {
                //Poor
                case 0:
                    color = new UnityEngine.Color(0.62f, 0.62f, 0.62f);
                    break;
                //Uncommon
                case 2:
                    color = new UnityEngine.Color(0.12f, 1f, 0f);
                    break;
                //Rare
                case 3:
                    color = new UnityEngine.Color(0f, 0.44f, 0.87f);
                    break;
                //Epic
                case 4:
                    color = new UnityEngine.Color(0.64f, 0.21f, 0.93f);
                    break;
                //Legendary
                case 5:
                    color = new UnityEngine.Color(1f, 0.5f, 0f);
                    break;
                //Artifact
                case 6:
                    color = new UnityEngine.Color(0.9f, 0.8f, 0.5f);
                    break;
                //Heirloom
                case 7:
                    color = new UnityEngine.Color(0f, 0.8f, 1f);
                    break;
                //Common
                default:
                    color = new UnityEngine.Color(1f, 1f, 1f);
                    break;
            }
            return color;
        }

        //Load icon
        public static Sprite GetIcon(string icon)
        {
            Texture2D texture = Resources.Load<Texture2D>(icon.Replace(".blp", ""));
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
        }

        //Return slot name
        public static string Slot(int slot)
        {
            string result;
            switch (slot)
            {
                case 1:
                    result = "head";
                    break;
                case 3:
                    result = "shoulder";
                    break;
                case 4:
                    result = "shirt";
                    break;
                case 5:
                    result = "chest";
                    break;
                case 6:
                    result = "waist";
                    break;
                case 7:
                    result = "legs";
                    break;
                case 8:
                    result = "feet";
                    break;
                case 9:
                    result = "wrist";
                    break;
                case 10:
                    result = "hands";
                    break;
                case 16:
                    result = "back";
                    break;
                case 19:
                    result = "tabard";
                    break;
                case 21:
                    result = "mainhand";
                    break;
                case 22:
                    result = "offhand";
                    break;
                default:
                    result = "";
                    break;
            }
            return result;
        }

        //Return slot ID
        public static int Slot(string slot)
        {
            int result;
            switch (slot.ToLower())
            {
                case "head":
                    result = 0;
                    break;
                case "shoulder":
                    result = 1;
                    break;
                case "back":
                    result = 2;
                    break;
                case "chest":
                    result = 3;
                    break;
                case "shirt":
                    result = 4;
                    break;
                case "tabard":
                    result = 5;
                    break;
                case "wrist":
                    result = 6;
                    break;
                case "mainhand":
                    result = 7;
                    break;
                case "hands":
                    result = 8;
                    break;
                case "waist":
                    result = 9;
                    break;
                case "legs":
                    result = 10;
                    break;
                case "feet":
                    result = 11;
                    break;
                case "offhand":
                    result = 12;
                    break;
                default:
                    result = -1;
                    break;
            }
            return result;
        }

        //Internal slot names
        public static string SlotName(int slot)
        {
            string result;
            switch (slot)
            {
                case 0:
                    result = "head";
                    break;
                case 1:
                    result = "shoulder";
                    break;
                case 2:
                    result = "back";
                    break;
                case 3:
                    result = "chest";
                    break;
                case 4:
                    result = "shirt";
                    break;
                case 5:
                    result = "tabard";
                    break;
                case 6:
                    result = "wrist";
                    break;
                case 7:
                    result = "mainhand";
                    break;
                case 8:
                    result = "hands";
                    break;
                case 9:
                    result = "waist";
                    break;
                case 10:
                    result = "legs";
                    break;
                case 11:
                    result = "feet";
                    break;
                case 12:
                    result = "offhand";
                    break;
                default:
                    result = "";
                    break;
            }
            return result;
        }
    }
}
