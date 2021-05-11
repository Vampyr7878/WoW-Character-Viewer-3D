namespace WoW
{
    //Class to store texture set for customizaiton choice
    public class CustomizationTextures
    {
        //texture ids
        public int Texture1 { get; private set; }
        public int Texture2 { get; private set; }
        public int Texture3 { get; private set; }
        public int Texture4 { get; private set; }

        //Constructor
        public CustomizationTextures(int texture1, int texture2, int texture3, int texture4)
        {
            Texture1 = texture1;
            Texture2 = texture2;
            Texture3 = texture3;
            Texture4 = texture4;
        }
    }
}