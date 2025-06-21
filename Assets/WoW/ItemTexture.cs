namespace WoW
{
    // Class to store texture set for item
    public class ItemTexture
    {
        // Texture ID
        public int ID { get; private set; }
        // Texture Usage Type 
        public int Usage { get; private set; }

        public ItemTexture(int id, int usage)
        {
            ID = id;
            Usage = usage;
        }
    }
}