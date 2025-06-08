using System.Collections.Generic;
using UnityEngine;
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
        // Description text
        public string Description { get; private set; }
        // Description Color
        public Color32 Color { get; private set; }
        // Item quality
        public int Quality { get; private set; }
        // Item appearances
        public Dictionary<int, ItemAppearance> Appearances { get; set; }

        public Item(int id, int itemClass, int itemType, int itemSlot, int sheathe, int icon,
            string name, int flags, string description, Color32 color, int quality)
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
            Description = description;
            Color = color;
            Quality = quality;
        }
    }
}
