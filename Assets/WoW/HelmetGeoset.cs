using WoW;

namespace Assets.WoW
{
    public class HelmetGeoset
    {
        // Race Id
        public WoWHelper.Race Race { get; private set; }
        // Geoset group to hide
        public int Geoset { get; private set; }

        public HelmetGeoset(WoWHelper.Race race, int geoset)
        {
            Race = race;
            Geoset = geoset;
        }
    }
}
