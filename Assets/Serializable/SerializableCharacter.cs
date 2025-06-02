namespace Serializable
{
    public class SerializableCharacter
    {
        public int raceid;
        public bool gender;
        public int classid;
        public int[] customization;
        public SerializableItem[] items;
    }

    public class SerializableItem
    {
        public int id;
        public int appearance;
    }
}
