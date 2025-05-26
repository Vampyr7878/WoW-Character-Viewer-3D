using System.Collections.Generic;
using UnityEngine;

namespace WoW
{
    // Class to store customization choice data
    public class CustomizationChoice
    {
        // Choice name
        public string Name { get; private set; }
        // Choice ID
        public int ID { get; private set; }
        // Choice Requirement
        public int Requirement { get; private set; }
        // Choice first color
        public Color32 Color1 { get; set; }
        // Choice second color
        public Color32 Color2 { get; set; }
        // Choice geosets
        public CustomizationGeoset[] Geosets { get; private set; }
        // Choice skinned geosets
        public CustomizationGeoset[] SkinnedGeosets { get;private set; }
        // Choice textures
        public CustomizationTexture[] Textures { get; private set; }
        // Choice creature
        public CustomizationDisplayInfo[] Creatures { get; private set; }

        // Constructor
        public CustomizationChoice(string name, int id, int requirement, Color32 color1, Color32 color2)
        {
            Name = name;
            ID = id;
            Requirement = requirement;
            Color1 = color1;
            Color2 = color2;
        }

        // Load geosets for that choice
        public void LoadGeosets(List<CustomizationGeoset> geosets)
        {
            Geosets = geosets.ToArray();
        }

        // Load goestes on skinned model for that choice
        public void LoadSkinnedGeosets(List<CustomizationGeoset> geosets)
        {
            SkinnedGeosets = geosets.ToArray();
        }

        // Load textures for that choice
        public void LoadTextures(List<CustomizationTexture> textures)
        {
            Textures = textures.ToArray();
        }

        // Load creature displayinfo for that choice
        public void LoadCreature(List<CustomizationDisplayInfo> creatures)
        {
            Creatures = creatures.ToArray();
        }
    }
}
