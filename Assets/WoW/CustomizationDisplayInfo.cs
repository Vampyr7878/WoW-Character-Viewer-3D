namespace WoW
{
    // Class to store customization choice data
    public class CustomizationDisplayInfo
    {
        // Texture related choice
        public int Related { get; private set; }
        // DisplayInfo ID
        public int ID { get; private set; }
        // DisplayInfo Model
        public string Model { get; private set; }
        // DisplayInfo Particle Color
        public int ParticleColor { get; private set; }
        // DisplayInfo Texture0
        public int Texture0 { get; private set; }
        // DisplayInfo Texture1
        public int Texture1 { get; private set; }
        // DisplayInfo Texture2
        public int Texture2 { get; private set; }
        // DisplayInfo Texture3
        public int Texture3 { get; private set; }
        // DisplayInfo Geoset
        public CustomizationGeoset[] Geosets { get; set; }
        // DisplayInfo Particle Colors
        public ParticleColor[] ParticleColors { get; set; }

        public CustomizationDisplayInfo(int related, int id, string model, int particle, int texture0, int texture1, int texture2, int texture3)
        {
            Related = related;
            ID = id;
            Model = model;
            ParticleColor = particle;
            Texture0 = texture0;
            Texture1 = texture1;
            Texture2 = texture2;
            Texture3 = texture3;
        }
    }
}
