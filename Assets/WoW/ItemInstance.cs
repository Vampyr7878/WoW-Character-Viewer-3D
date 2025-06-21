using System.Linq;
using WoW;

namespace Assets.WoW
{
    public class ItemInstance
    {
        // Item ID
        public int ID { get; set; }
        // Item Appearance ID
        public int Appearance { get; set; }
        // Item data
        public Item Item { get; set; }
        // Item was changed
        public bool Changed { get; set; }

        public ItemInstance(int id, int appearance)
        {
            ID = id;
            Appearance = appearance;
        }

        public ItemInstance(int iD, int appearance, Item item) : this(iD, appearance)
        {
            Item = item;
        }

        // Get model matching proper conditions
        public int GetModel(int index, bool gender, WoWHelper.Race r, WoWHelper.Class c, int position)
        {
            WoWHelper.Race fallback = WoWHelper.RaceModel(r, gender);
            var model = Item.Appearances[Appearance].DisplayInfo.Models[index].FirstOrDefault
                (m => m.Class == c && m.Race == r && m.Gender == (gender ? 0 : 1) && m.Position == position);
            model ??= Item.Appearances[Appearance].DisplayInfo.Models[index].FirstOrDefault
                (m => m.Class == c && m.Race == fallback && m.Gender == (gender ? 0 : 1) && m.Position == position);
            model ??= Item.Appearances[Appearance].DisplayInfo.Models[index].FirstOrDefault
                (m => m.Race == r && m.Gender == (gender ? 0 : 1) && m.Position == position);
            model ??= Item.Appearances[Appearance].DisplayInfo.Models[index].FirstOrDefault
                (m => m.Race == fallback && m.Gender == (gender ? 0 : 1) && m.Position == position);
            model ??= Item.Appearances[Appearance].DisplayInfo.Models[index].FirstOrDefault
                (m => m.Class == c && m.Gender == 2 && m.Position == position);
            model ??= Item.Appearances[Appearance].DisplayInfo.Models[index].FirstOrDefault
                (m => (m.Gender == 2 || m.Gender == 3) && m.Position == position);
            return model.ID;
        }
    }
}
