using System.Collections.Generic;

namespace WoW
{
    // Class to store Customization option data from database
    public class CustomizationOption
    {
        // Option name
        public string Name { get; private set; }
        // Option ID
        public int ID { get; private set; }
        // Option model
        public int Model { get; private set; }
        // Option category
        public int Category { get; private set; }
        // Option type
        public WoWHelper.CustomizationType Type { get; private set; }
        // Option choices
        public Dictionary<int, CustomizationChoice> Choices { get; set; }
        // Option choices
        public Dictionary<int, CustomizationChoice> AllChoices { get; set; }

        // Constructor
        public CustomizationOption(string name, int id, int model, int category, int type)
        {
            Name = name;
            ID = id;
            Model = model;
            Category = category;
            Type = (WoWHelper.CustomizationType)type;
        }
    }
}
