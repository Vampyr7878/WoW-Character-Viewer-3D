namespace WoW
{
    //Class to store item information
    public class Item
    {
        //Item's id
        public int ID { get; private set; }
        //Item's version
        public int Version { get; private set; }
        //Item's name
        public string Name { get; private set; }
        //Item's Display ID
        public int Display { get; private set; }
        //Item's icon
        public int Icon { get; set; }
        //Index to item's quality
        public int Quality { get; private set; }
        //Index of slot this item goes into
        public int Slot { get; private set; }

        //Constructor
        public Item(int id, int version, string name, int display, int icon, int quality, int slot)
        {
            ID = id;
            Version = version;
            Name = name;
            Display = display;
            Icon = icon;
            Quality = quality;
            Slot = slot;
        }
    }
}