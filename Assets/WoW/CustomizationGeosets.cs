namespace WoW
{
    //Class to store geosets set for customizaiton choice
    public class CustomizationGeosets
    {
        //Geosets
        public int Geoset1 { get; private set; }
        public int Geoset2 { get; private set; }
        public int Geoset3 { get; private set; }

        //Constructor
        public CustomizationGeosets(int geoset1, int geoset2, int geoset3)
        {
            Geoset1 = geoset1;
            Geoset2 = geoset2;
            Geoset3 = geoset3;
        }
    }
}