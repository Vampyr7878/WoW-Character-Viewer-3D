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
        public int Type { get; private set; }
        // Option choices
        public CustomizationChoice[] Choices { get; private set; }
        // Option choices
        public CustomizationChoice[] AllChoices { get; private set; }

        // Constructor
        public CustomizationOption(string name, int id, int model, int category, int type)
        {
            Name = name;
            ID = id;
            Model = model;
            Category = category;
            Type = type;
        }

        // Load all available choices
        public void LoadAllChoices(List<CustomizationChoice> choices)
        {
            AllChoices = choices.ToArray();
        }

        // Set choices that will be shown
        public void SetChoices(CustomizationChoice[] choices)
        {
            Choices = choices;
        }
    }
}
