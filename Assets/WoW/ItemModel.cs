namespace WoW
{
    // Class to store model set for item
    public class ItemModel
    {
        // Model ID
        public int ID { get; private set; }
        // Model used for gender 
        public int Gender { get; private set; }
        // Model used for class
        public WoWHelper.Class Class { get; private set; }
        // Model used for race
        public WoWHelper.Race Race { get; private set; }
        // Model used for position
        public int Position { get; private set; }

        public ItemModel(int id, int gender, WoWHelper.Class c, WoWHelper.Race r, int position)
        {
            ID = id;
            Gender = gender;
            Class = c;
            Race = r;
            Position = position;
        }
    }
}