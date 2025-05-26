namespace WoW
{
    // Class to store geosets set for customizaiton choice
    public class CustomizationGeoset
    {
        // Geoset related choice
        public int Related { get; private set; }
        // Geoset type
        public int Type { get; private set; }
        // Geoset ID
        public int ID { get; private set; }

        // Constructor
        public CustomizationGeoset(int related, int type, int id)
        {
            Related = related;
            Type = type;
            ID = id;
        }
    }
}