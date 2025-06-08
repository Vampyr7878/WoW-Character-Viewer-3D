namespace Assets.WoW
{
    public class ItemAppearance
    {
        // Item appearance modifier
        public int Modifier { get; private set; }
        // Item appearance display info ID
        public int DisplayInfoID { get; private set; }
        // Item icon
        public int Icon {  get; private set; }
        // Item display info
        public ItemDisplayInfo DisplayInfo { get; set; }

        public ItemAppearance(int modifier, int displayInfoID, int icon)
        {
            Modifier = modifier;
            DisplayInfoID = displayInfoID;
            Icon = icon;
        }
    }
}
