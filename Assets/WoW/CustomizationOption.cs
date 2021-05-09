namespace WoW
{
    //Class to store Customization option data from database
    public class CustomizationOption
    {
        //Option ID
        public int ID { get; private set; }
        //Option category
        public int Category { get; private set; }
        //Option name
        public string Name { get; private set; }
        //Form that option is available to
        public int Form { get; private set; }
        //ID used by Blizzard, value used for imprting from armory
        public int Blizzard { get; private set; }

        //Constructor
        public CustomizationOption(int id, int category, string name, int form, int blizzard)
        {
            ID = id;
            Category = category;
            Name = name;
            Form = form;
            Blizzard = blizzard;
        }
    }
}
