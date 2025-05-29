namespace Serializable
{
    public class SerializableCharacter
    {
        public int raceid;
        public bool gender;
        public int classid;
        public int[] customization;
        public SerializableItems[] items;
    }

    public class SerializableItems
    {
        public int id;
        public int version;
    }
}
