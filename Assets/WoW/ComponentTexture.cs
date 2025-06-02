using WoW;

namespace Assets.WoW
{
    public class ComponentTexture
    {
        // Texture ID
        public int ID { get; private set; }
        // Texture gender
        public int Gender { get; private set; }
        // Texture class
        public WoWHelper.Class Class { get; private set; }
        // Texture race
        public WoWHelper.Race Race { get; private set; }

        public ComponentTexture(int id, int gender, WoWHelper.Class c , WoWHelper.Race r)
        {
            ID = id;
            Gender = gender;
            Class = c;
            Race = r;
        }
    }
}
