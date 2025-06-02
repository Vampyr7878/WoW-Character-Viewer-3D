namespace Assets.WoW
{
    public class ItemInstance
    {
        // Item ID
        public int ID { get; set; }
        // Item Appearance ID
        public int Appearance { get; set; }
        // Item data
        public Item Item { get; set; }

        public ItemInstance(int id, int appearance)
        {
            ID = id;
            Appearance = appearance;
        }

        public ItemInstance(int iD, int appearance, Item item) : this(iD, appearance)
        {
            Item = item;
        }
    }
}
