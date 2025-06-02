namespace Assets.WoW
{
    public class ItemAppearance
    {
        // Item appearance modifier
        public int Modifier { get; private set; }
        // Item particle color ID
        public int ParticleColor { get; private set; }
        // Item appearance display info ID
        public int DisplayInfoID { get; private set; }
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
        // Item icon
        public int Icon {  get; private set; }
        // Item particle color
        public ParticleColor[] ParticleColors { get; set; }
        // Item helmet geoset data
        public HelmetGeoset[][] HelmetGeosets { get; set; }
        // Item components
        public ItemComponent[] Components { get; set; }

        public ItemAppearance(int modifier, int particleColor, int displayInfoID, int[] models,
            int[] textures, int[] geosets, int[] skinnedGeosets, int[] helmetGeosetsID, int icon)
        {
            Modifier = modifier;
            ParticleColor = particleColor;
            DisplayInfoID = displayInfoID;
            Models = models;
            Textures = textures;
            Geosets = geosets;
            SkinnedGeosets = skinnedGeosets;
            HelmetGeosetsID = helmetGeosetsID;
            HelmetGeosets = new HelmetGeoset[HelmetGeosetsID.Length][];
            Icon = icon;
        }
    }
}
