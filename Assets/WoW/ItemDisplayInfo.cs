namespace Assets.WoW
{
    public class ItemDisplayInfo
    {
        // Item particle color ID
        public int ParticleColor { get; private set; }
        // Item models
        public int[] Models { get; private set; }
        // Item model textures
        public int[] Textures { get; private set; }
        // Item geosets
        public int[] Geosets { get; private set; }
        // Item skinned geosets
        public int[] SkinnedGeosets { get; private set; }
        // Item helmet geosets IDs
        public int[] HelmetGeosetsID { get; private set; }
        // Item particle color
        public ParticleColor[] ParticleColors { get; set; }
        // Item helmet geoset data
        public HelmetGeoset[][] HelmetGeosets { get; set; }
        // Item components
        public ItemComponent[] Components { get; set; }

        public ItemDisplayInfo(int particleColor, int[] models, int[] textures, 
            int[] geosets, int[] skinnedGeosets, int[] helmetGeosetsID)
        {
            ParticleColor = particleColor;
            Models = models;
            Textures = textures;
            Geosets = geosets;
            SkinnedGeosets = skinnedGeosets;
            HelmetGeosetsID = helmetGeosetsID;
            HelmetGeosets = new HelmetGeoset[HelmetGeosetsID.Length][];
        }
    }
}
