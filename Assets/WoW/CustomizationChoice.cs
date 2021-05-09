using UnityEngine;

namespace WoW
{
    public class CustomizationChoice
    {
        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
        public string Name { get; set; }
        public string Model { get; private set; }
        public string Texture1 { get; private set; }
        public string Texture2 { get; private set; }
        public string Texture3 { get; private set; }
        public int Geoset1 { get; private set; }
        public int Geoset2 { get; private set; }
        public int Geoset3 { get; private set; }
        public int Bone { get; private set; }
        public int Extra { get; private set; }
        public int ID { get; private set; }
        
        public CustomizationChoice(string name, Color color1, Color color2, string model, string texture1, string texture2, string texture3, int geoset1, int geoset2, int geoset3, int bone, int extra, int id)
        {
            Name = name;
            Color1 = color1;
            Color2 = color2;
            Model = model;
            Texture1 = texture1;
            Texture2 = texture2;
            Texture3 = texture3;
            Geoset1 = geoset1;
            Geoset2 = geoset2;
            Geoset3 = geoset3;
            Bone = bone;
            Extra = extra;
            ID = id;
        }
    }
}
