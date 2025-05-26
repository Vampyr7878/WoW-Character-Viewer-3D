namespace Assets.WoW
{
    public class CreatureForm
    {
        // From ID
        public int ID { get; private set; }
        // Form name
        public string Name { get; private set; }
        // Form icon
        public string Icon { get; private set; }
        // Flip the button
        public bool Flip { get; private set; }

        // Constructor
        public CreatureForm(int id, string name, string icon, bool flip = true)
        {
            ID = id;
            Name = name;
            Icon = icon;
            Flip = flip;
        }
    }
}
