using UnityEngine;

namespace WoW
{
    //Class to store customization choice data
    public class CustomizationChoice
    {
        //ID from database
        public int Index { get; private set; }
        //Choice's name
        public string Name { get; private set; }
        //Choice's colors
        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
        //Druid form model
        public int Model { get; private set; }
        //Face boneset
        public int Bone { get; private set; }
        //ID used by Blizzard for armory import
        public int ID { get; private set; }

        //Geosets used by this choice
        public CustomizationGeosets[] Geosets { get; set; }
        //Textures used by this choice
        public CustomizationTextures[] Textures{ get; set; }
        
        //Constructor
        public CustomizationChoice(int index, string name, Color color1, Color color2, int model, int bone, int id)
        {
            Index = index;
            Name = name;
            Color1 = color1;
            Color2 = color2;
            Model = model;
            Bone = bone;
            ID = id;
        }
    }
}
