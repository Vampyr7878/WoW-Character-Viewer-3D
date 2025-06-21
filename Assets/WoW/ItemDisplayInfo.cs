using WoW;

namespace Assets.WoW
{
    public class ItemDisplayInfo
    {
        // Item geosets to be enabled
        public int ItemGeoset { get; private set; }
        // Item particle color ID
        public int ParticleColor { get; private set; }
        // Item Flags
        public int Flags { get; private set; }
        // Item models
        public int[] ModelID { get; private set; }
        // Item model textures
        public int[] Materials { get; private set; }
        // Item geosets
        public int[] Geosets { get; private set; }
        // Item skinned geosets
        public int[] SkinnedGeosets { get; private set; }
        // Item helmet geosets IDs
        public int[] HelmetGeosetsID { get; private set; }
        // Item particle color
        public ParticleColor[] ParticleColors { get; set; }
        // Sets of item models
        public ItemModel[][] Models { get; set; }
        // Sets of item textures for each model
        public ItemTexture[][] Textures { get; set; }
        // Item helmet geoset data
        public HelmetGeoset[][] HelmetGeosets { get; set; }
        // Item components
        public ItemComponent[] Components { get; set; }

        public ItemDisplayInfo(int itemGeoset, int particleColor, int flags, int[] models,
            int[] materials, int[] geosets, int[] skinnedGeosets, int[] helmetGeosetsID)
        {
            ItemGeoset = itemGeoset;
            ParticleColor = particleColor;
            Flags = flags;
            ModelID = models;
            Models = new ItemModel[models.Length][];
            Materials = materials;
            Textures = new ItemTexture[materials.Length][];
            Geosets = geosets;
            SkinnedGeosets = skinnedGeosets;
            HelmetGeosetsID = helmetGeosetsID;
            HelmetGeosets = new HelmetGeoset[HelmetGeosetsID.Length][];
        }
    }
}
