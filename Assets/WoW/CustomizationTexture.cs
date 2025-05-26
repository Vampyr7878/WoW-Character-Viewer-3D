namespace WoW
{
    // Class to store texture set for customizaiton choice
    public class CustomizationTexture
    {
        // Texture related choice
        public int Related { get; private set; }
        // Texture target
        public int Target { get; private set; }
        // Texture ID
        public int ID { get; private set; }
        // Texture Emission
        public int Usage { get; private set; }

        // Constructor
        public CustomizationTexture(int related, int target, int id, int usage)
        {
            Related = related;
            Target = target;
            ID = id;
            Usage = usage;
        }
    }
}