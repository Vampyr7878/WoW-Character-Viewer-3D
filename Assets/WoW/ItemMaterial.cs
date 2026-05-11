namespace WoW
{
    // Class to store material set for item
    public class ItemMaterial
    {
        // Texture ID
        public int ID { get; private set; }
        // Texture Usage Type 
        public int Model { get; private set; }

        public ItemMaterial(int id, int model)
        {
            ID = id;
            Model = model;
        }
    }
}