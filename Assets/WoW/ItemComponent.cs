using WoW;

namespace Assets.WoW
{
    public class ItemComponent
    {
        // Component secion
        public WoWHelper.ComponentSection ComponentSection { get; private set; }
        // Component material
        public int Material { get; private set; }
        // Component Textures
        public ComponentTexture[] Textures { get; set; }

        public ItemComponent(WoWHelper.ComponentSection componentSection, int material)
        {
            ComponentSection = componentSection;
            Material = material;
        }
    }
}
