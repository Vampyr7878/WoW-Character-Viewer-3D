namespace WoW
{
    // Class to store Customization category data from database
    public class CustomizationCategory
    {
        // Category ID
        public int ID { get; private set; }
        // Category name
        public string Name { get; private set; }
        // Category icon
        public int Icon { get; private set; }
        // Category icon when selected
        public int Selected { get; private set; }

        // Constructor
        public CustomizationCategory(int id, string name, int icon, int selected)
        {
            ID = id;
            Name = name;
            Icon = icon;
            Selected = selected;
        }
    }
}
