using System.Collections.Generic;
using WoW;

namespace Assets.WoW
{
    public class Item
    {
        // Item ID
        public int ID { get; private set; }
        // Item class
        public WoWHelper.ItemClass ItemClass { get; private set; }
        // Item weapon type
        public WoWHelper.WeaponType WeaponType { get; private set; }
        // Item armor type
        public WoWHelper.ArmorType ArmorType { get; private set; }
        // Item slot
        public WoWHelper.ItemSlot ItemSlot { get; private set; }
        // Item sheathe type
        public int Sheathe { get; private set; }
        // Item icon
        public int Icon { get; private set; }
        // Item name
        public string Name { get; private set; }
        // Item flags
        public int Flags { get; private set; }
        // Item description ID
        public int DescriptionID { get; private set; }
        // Item quality
        public int Quality { get; private set; }
        // Item description
        public ItemDescription Description { get; set; }
        // Item appearances
        public Dictionary<int, ItemAppearance> Appearances { get; set; }

        public Item(int id, int itemClass, int itemType, int itemSlot, int sheathe, int icon, string name, int flags, int description, int quality)
        {
            ID = id;
            ItemClass = (WoWHelper.ItemClass)itemClass;
            if (ItemClass == WoWHelper.ItemClass.Weapon)
            {
                WeaponType = (WoWHelper.WeaponType)itemType;
                ArmorType = WoWHelper.ArmorType.None;
            }
            else
            {
                ArmorType = (WoWHelper.ArmorType)itemType;
                WeaponType = WoWHelper.WeaponType.None;
            }
            ItemSlot = (WoWHelper.ItemSlot)itemSlot;
            Sheathe = sheathe;
            Icon = icon;
            Name = name;
            Flags = flags;
            DescriptionID = description;
            Quality = quality;
        }
    }
}
