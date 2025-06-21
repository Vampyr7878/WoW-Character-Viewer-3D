using M2Lib;
using UnityEngine;

namespace WoW
{
    // Helper class for some WoW related things.
    public class WoWHelper
    {
        public static readonly int Heroic = 0x8;

        // Enum for various layered texture types
        public enum LayeredTexture
        {
            None = 0,
            Skin = 1,
            Eye = 2,
            Extra1 = 3,
            Extra2 = 4,
            Hair = 5
        }

        // Enum for forms used for druid and warlock extra customization
        public enum CreatureForm
        {
            Imp = 148,
            Felhunter = 176,
            Voidwalker = 180,
            Felguard = 181,
            Doomguard = 182,
            Sayaad = 183,
            Infernal = 184,
            BearForm = 189,
            CatForm = 190,
            AquaticForm = 191,
            TravelForm = 192,
            FlightForm = 193,
            MoonkinForm = 194,
            Darkglare = 197,
            Tyrant = 198
        }

        // Enum for character races
        public enum Race
        {
            All = 0,
            Human = 1,
            Orc = 2,
            Dwarf = 3,
            NightElf = 4,
            Undead = 5,
            Tauren = 6,
            Gnome = 7,
            Troll = 8,
            Goblin = 9,
            BloodElf = 10,
            Draenei = 11,
            Worgen = 22,
            Gilnean = 23,
            Pandaren = 24,
            Nightborne = 27,
            Highmountain = 28,
            VoidElf = 29,
            Lightforged = 30,
            Zandalari = 31,
            KulTiran = 32,
            DarkIron = 34,
            Vulpera = 35,
            Maghar = 36,
            Mechagnome = 37,
            Dracthyr = 52,
            Visage = 75,
            Earthen = 84
        }

        // Enum for character classes
        public enum Class
        {
            All = 0,
            Warrior = 1,
            Paladin = 2,
            Hunter = 3,
            Rogue = 4,
            Priest = 5,
            DeathKnight = 6,
            Shaman = 7,
            Mage = 8,
            Warlock = 9,
            Monk = 10,
            Druid = 11,
            DemonHunter = 12,
            Evoker = 13
        }

        // Enum for customization UI type
        public enum CustomizationType
        {
            Dropdown = 0,
            Toggle = 1
        }

        // Enum for equipment slots
        public enum ItemSlot
        {
            Head = 1,
            Shoulder = 3,
            Shirt = 4,
            Chest = 5,
            Waist = 6,
            Legs = 7,
            Feet = 8,
            Wrist = 9,
            Hands = 10,
            OneHand = 13,
            Shield = 14,
            Bow = 15,
            Back = 16,
            TwoHand = 17,
            Tabard = 19,
            Robe = 20,
            MainHand = 21,
            OffHand = 22,
            HeldInOffHand = 23,
            Ranged = 26
        }

        // Enum for slot indices
        public enum SlotIndex
        {
            None = -1,
            Head = 0,
            RightShoulder = 1,
            LeftShoulder = 2,
            Back = 3,
            Chest = 4,
            Shirt = 5,
            Tabard = 6,
            Wrist = 7,
            Hands = 8,
            Waist = 9,
            Legs = 10,
            Feet = 11,
            MainHand = 12,
            OffHand = 13
        }

        // Enum for equipment Class
        public enum ItemClass
        {
            Weapon = 2,
            Armor = 4
        }

        // Enum for weapon types
        public enum WeaponType
        {
            OneHandedAxe = 0,
            TwoHandedAxe = 1,
            Bow = 2,
            Gun = 3,
            OneHandedMace = 4,
            TwoHandedMace = 5,
            Polearm = 6,
            OneHandedSword = 7,
            TwoHandedSword = 8,
            Warglaive = 9,
            Staff = 10,
            FistWeapon = 13,
            Dagger = 15,
            Crossbow = 18,
            Wand = 19,
            None = 100
        }

        // Enum for armor types
        public enum ArmorType
        {
            Misc = 0,
            Cloth = 1,
            Leather = 2,
            Mail = 3,
            Plate = 4,
            Cosmetic = 5,
            Shield = 6,
            None = 100
        }

        // Enum for component sections
        public enum ComponentSection
        {
            ArmUpper = 0,
            ArmLower = 1,
            Hand = 2,
            TorsoUpper = 3,
            TorsoLower = 4,
            LegUpper = 5,
            LegLower = 6,
            Foot = 7,
            Accessory = 8,
            Face = 100,
            Body = 200,
            Full = 300
        }

        // Enum for equipment attachment points
        public enum AttachmentPoint
        {
            Shield = 0,
            HandRight = 1,
            HandLeft = 2,
            ShoulderRight = 5,
            ShoulderLeft = 6,
            Helm = 11,
            Quiver = 26,
            Buckle = 53,
            Book = 53,
            Backpack = 57
        }

        // Color for each item quality
        public static Color QualityColor(int quality)
        {
            Color32 color = quality switch
            {
                // Poor
                0 => new Color32(157, 157, 157, 255),
                // Uncommon
                2 => new Color32(30, 255, 0, 255),
                // Rare
                3 => new Color32(0, 112, 221, 255),
                // Epic
                4 => new Color32(163, 53, 238, 255),
                // Legendary
                5 => new Color32(255, 128, 0, 255),
                // Artifact
                6 => new Color32(230, 204, 128, 255),
                // Heirloom
                7 => new Color32(0, 204, 255, 255),
                // Common
                _ => new Color32(255, 255, 255, 255),
            };
            return color;
        }

        // Return slot name
        public static string SlotName(ItemSlot slot)
        {
            return slot switch
            {
                ItemSlot.Head => "Head",
                ItemSlot.Shoulder => "Shoulder",
                ItemSlot.Shirt => "Shirt",
                ItemSlot.Chest => "Chest",
                ItemSlot.Waist => "Waist",
                ItemSlot.Legs => "Legs",
                ItemSlot.Feet => "Feet",
                ItemSlot.Wrist => "Wrist",
                ItemSlot.Hands => "Hands",
                ItemSlot.OneHand => "One-Hand",
                ItemSlot.Shield => "Off Hand",
                ItemSlot.Bow => "Ranged",
                ItemSlot.Back => "Back",
                ItemSlot.TwoHand => "Two-Hand",
                ItemSlot.Tabard => "Tabard",
                ItemSlot.Robe => "Chest",
                ItemSlot.MainHand => "Main Hand",
                ItemSlot.OffHand => "Off Hand",
                ItemSlot.HeldInOffHand => "Held in Off-Hand",
                ItemSlot.Ranged => "Ranged",
                _ => "Unknown",
            };
        }

        // Return mapped slot indices
        public static SlotIndex MapSlots(string slot)
        {
            return slot switch
            {
                "head" => SlotIndex.Head,
                "rightshoulder" => SlotIndex.RightShoulder,
                "leftshoulder" => SlotIndex.LeftShoulder,
                "back" => SlotIndex.Back,
                "chest" => SlotIndex.Chest,
                "shirt" => SlotIndex.Shirt,
                "tabard" => SlotIndex.Tabard,
                "wrist" => SlotIndex.Wrist,
                "hands" => SlotIndex.Hands,
                "waist" => SlotIndex.Waist,
                "legs" => SlotIndex.Legs,
                "feet" => SlotIndex.Feet,
                "mainhand" => SlotIndex.MainHand,
                "offhand" => SlotIndex.OffHand,
                _ => SlotIndex.None,
            };
        }

        // Return mapped slot names
        public static string MapSlots(SlotIndex slot)
        {
            return slot switch
            {
                SlotIndex.Head => "head",
                SlotIndex.RightShoulder => "rightshoulder",
                SlotIndex.LeftShoulder => "leftshoulder",
                SlotIndex.Back => "back",
                SlotIndex.Chest => "chest",
                SlotIndex.Shirt => "shirt",
                SlotIndex.Tabard => "tabard",
                SlotIndex.Wrist => "wrist",
                SlotIndex.Hands => "hands",
                SlotIndex.Waist => "waist",
                SlotIndex.Legs => "legs",
                SlotIndex.Feet => "feet",
                SlotIndex.MainHand => "mainhand",
                SlotIndex.OffHand => "offhand",
                _ => "",
            };
        }

        // Return Blizzard internal slot IDs to slot indices
        public static SlotIndex MapSlot(int slot)
        {
            return slot switch
            {
                0 => SlotIndex.Head,
                2 => SlotIndex.RightShoulder,
                3 => SlotIndex.Shirt,
                4 => SlotIndex.Chest,
                5 => SlotIndex.Waist,
                6 => SlotIndex.Legs,
                7 => SlotIndex.Feet,
                8 => SlotIndex.Wrist,
                9 => SlotIndex.Hands,
                14 => SlotIndex.Back,
                15 => SlotIndex.MainHand,
                16 => SlotIndex.OffHand,
                18 => SlotIndex.Tabard,
                _ => SlotIndex.None
            };
        }

        // Return weapon type name
        public static string WeaponTypeName(WeaponType type)
        {
            return type switch
            {
                WeaponType.OneHandedAxe or WeaponType.TwoHandedAxe => "Axe",
                WeaponType.OneHandedMace or WeaponType.TwoHandedMace => "Mace",
                WeaponType.OneHandedSword or WeaponType.TwoHandedSword => "Sword",
                WeaponType.Bow => "Bow",
                WeaponType.Gun => "Gun",
                WeaponType.Polearm => "Polearm",
                WeaponType.Warglaive => "Warglaive",
                WeaponType.Staff => "Staff",
                WeaponType.FistWeapon => "Fist Weapon",
                WeaponType.Dagger => "Dagger",
                WeaponType.Crossbow => "Crossbow",
                WeaponType.Wand => "Wand",
                _ => "Unknown",
            };
        }

        // Return armor type name
        public static string ArmorTypeName(ArmorType type)
        {
            return type switch
            {
                ArmorType.Misc => "",
                ArmorType.Cloth => "Cloth",
                ArmorType.Leather => "Leather",
                ArmorType.Mail => "Mail",
                ArmorType.Plate => "Plate",
                ArmorType.Cosmetic => "Cosmetic",
                ArmorType.Shield => "Shield",
                _ => "Unknown",
            };
        }

        // Return appearance modifier name
        public static string AppearanceModifierName(int modifier)
        {
            return modifier switch
            {
                1 => "Heroic",
                3 => "Mythic",
                4 => "Raid Finder",
                5 => "Mythic",
                _ => "",
            };
        }

        // Return rectangle for component section
        public static RectInt ComponentRect(ComponentSection section)
        {
            return section switch
            {
                ComponentSection.ArmUpper => new RectInt(0, 384, 256, 128),
                ComponentSection.ArmLower => new RectInt(0, 256, 256, 128),
                ComponentSection.Hand => new RectInt(0, 192, 256, 64),
                ComponentSection.TorsoUpper => new RectInt(256, 384, 256, 128),
                ComponentSection.TorsoLower => new RectInt(256, 320, 256, 64),
                ComponentSection.LegUpper => new RectInt(256, 192, 256, 128),
                ComponentSection.LegLower => new RectInt(256, 64, 256, 128),
                ComponentSection.Foot => new RectInt(256, 0, 256, 64),
                ComponentSection.Accessory => new RectInt(768, 0, 256, 128),
                ComponentSection.Face => new RectInt(512, 0, 512, 512),
                ComponentSection.Body => new RectInt(0, 0, 512, 512),
                ComponentSection.Full => new RectInt(768, 0, 256, 128),
                _ => new RectInt(0, 0, 256, 128),
            };
        }

        // Translate race id to base race model id
        public static Race RaceModel(WoWHelper.Race race, bool gender)
        {
            Race result = 0; switch (race)
            {
                case Race.Human:
                case Race.Orc:
                case Race.Dwarf:
                case Race.NightElf:
                case Race.Undead:
                case Race.Tauren:
                case Race.Gnome:
                case Race.Troll:
                case Race.Goblin:
                case Race.BloodElf:
                case Race.Draenei:
                case Race.Worgen:
                case Race.Pandaren:
                case Race.KulTiran:
                case Race.Vulpera:
                case Race.Dracthyr:
                    result = race;
                    break;
                case Race.Gilnean:
                    result = Race.Human;
                    break;
                case Race.Nightborne:
                    result = Race.NightElf;
                    break;
                case Race.Highmountain:
                    result = Race.Tauren;
                    break;
                case Race.VoidElf:
                    result = Race.BloodElf;
                    break;
                case Race.Lightforged:
                    result = Race.Draenei;
                    break;
                case Race.Zandalari:
                    result = gender ? Race.Zandalari : Race.Troll;
                    break;
                case Race.DarkIron:
                case Race.Earthen:
                    result = Race.Dwarf;
                    break;
                case Race.Maghar:
                    result = Race.Orc;
                    break;
                case Race.Mechagnome:
                    result = Race.Gnome;
                    break;
                case Race.Visage:
                    result = gender ? Race.BloodElf : Race.Human;
                    break;
            }
            return result;
        }

        // Get region two letter symbol
        public static string WoWRegion(int value)
        {
            string region = value switch
            {
                0 => "us",
                1 => "eu",
                2 => "kr",
                3 => "tw",
                4 => "cn",
                _ => "",
            };
            return region;
        }

        // Generate 3D Mesh gameobject from m2 file
        public static GameObject Generate3DMesh(M2 file)
        {
            GameObject model = new(file.Name);
            model.AddComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer renderer = model.GetComponent<SkinnedMeshRenderer>();
            Mesh mesh = new()
            {
                name = file.Name + "_mesh"
            };
            // Fill vertex data
            Vector3[] vertices = new Vector3[file.Vertices.Length];
            Vector3[] normals = new Vector3[file.Vertices.Length];
            BoneWeight[] weights = new BoneWeight[file.Vertices.Length];
            Vector2[] uv = new Vector2[file.Vertices.Length];
            Vector2[] uv2 = new Vector2[file.Vertices.Length];
            for (int i = 0; i < file.Vertices.Length; i++)
            {
                vertices[i] = new Vector3(file.Vertices[i].Position.X, file.Vertices[i].Position.Y, file.Vertices[i].Position.Z);
                normals[i] = new Vector3(file.Vertices[i].Normal.X, file.Vertices[i].Normal.Y, file.Vertices[i].Normal.Z);
                BoneWeight weight = new()
                {
                    boneIndex0 = file.Vertices[i].Bones[0],
                    boneIndex1 = file.Vertices[i].Bones[1],
                    boneIndex2 = file.Vertices[i].Bones[2],
                    boneIndex3 = file.Vertices[i].Bones[3],
                    weight0 = file.Vertices[i].Weights[0] / 255f,
                    weight1 = file.Vertices[i].Weights[1] / 255f,
                    weight2 = file.Vertices[i].Weights[2] / 255f,
                    weight3 = file.Vertices[i].Weights[3] / 255f
                };
                weights[i] = weight;
                uv[i] = new Vector2(file.Vertices[i].UV[0].X, 1 - file.Vertices[i].UV[0].Y);
                uv2[i] = new Vector2(file.Vertices[i].UV[1].X, 1 - file.Vertices[i].UV[1].Y);
            }
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.boneWeights = weights;
            mesh.uv = uv;
            mesh.uv2 = uv2;
            // Fill Submesh data
            mesh.subMeshCount = file.Skin.Textures.Length;
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] triangles = new int[file.Skin.Submeshes[file.Skin.Textures[i].Id].Count];
                for (int j = 0; j < triangles.Length; j++)
                {
                    triangles[j] = file.Skin.Indices[file.Skin.Submeshes[file.Skin.Textures[i].Id].Start + j];
                }
                mesh.SetTriangles(triangles, i);
            }
            // Generate bones
            Transform[] bones = new Transform[file.Skeleton.Bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i] = new GameObject($"Bone{i}").transform;
                bones[i].position = new Vector3(file.Skeleton.Bones[i].Pivot.X, file.Skeleton.Bones[i].Pivot.Y, file.Skeleton.Bones[i].Pivot.Z);
            }
            GameObject skeleton = new("Skeleton");
            for (int i = 0; i < bones.Length; i++)
            {
                if (file.Skeleton.Bones[i].Parent == -1)
                {
                    bones[i].parent = skeleton.transform;
                }
                else
                {
                    bones[i].parent = bones[file.Skeleton.Bones[i].Parent];
                }
            }
            Matrix4x4[] bind = new Matrix4x4[bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                bind[i] = bones[i].worldToLocalMatrix * model.transform.localToWorldMatrix;
            }
            skeleton.transform.parent = model.transform;
            renderer.materials = new Material[mesh.subMeshCount];
            renderer.sharedMesh = mesh;
            renderer.bones = bones;
            renderer.rootBone = bones[0];
            mesh.bindposes = bind;
            return model;
        }

        // Particle shape
        private static ParticleSystemShapeType ParticleShape(byte value)
        {
            ParticleSystemShapeType shape = ParticleSystemShapeType.Cone;
            switch (value)
            {
                case 1:
                    shape = ParticleSystemShapeType.Rectangle;
                    break;
                case 2:
                    shape = ParticleSystemShapeType.Sphere;
                    break;
                case 3:
                    shape = ParticleSystemShapeType.Circle;
                    break;
            }
            return shape;
        }

        // Generate particle effect system based on the data
        public static GameObject ParticleEffect(M2Particle particle)
        {
            // Create gameobject
            GameObject element = new();
            element.AddComponent<ParticleSystem>();
            ParticleSystem system = element.GetComponent<ParticleSystem>();
            // Setup lifetime and speed in main module
            ParticleSystem.MainModule main = system.main;
            float variation = particle.LifespanVariation * particle.Lifespan;
            main.startLifetime = new ParticleSystem.MinMaxCurve((particle.Lifespan - variation) / 2f, (particle.Lifespan + variation) / 2f);
            variation = particle.SpeedVariation * particle.Speed;
            main.startSpeed = new ParticleSystem.MinMaxCurve((particle.Speed - variation) / 2f, (particle.Speed + variation) / 2f);
            // Setup emission rate in emission module
            ParticleSystem.EmissionModule emission = system.emission;
            variation = particle.EmissionVariation * particle.EmissionRate;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(particle.EmissionRate - variation, particle.EmissionRate + variation);
            // Setup shape and scale in shape module
            ParticleSystem.ShapeModule shape = system.shape;
            shape.shapeType = ParticleShape(particle.Type);
            shape.scale = new Vector3(particle.EmissionWidth, particle.EmissionLength, particle.EmissionWidth);
            // Setup color and alpha gradients in color over lifetime module
            ParticleSystem.ColorOverLifetimeModule color = system.colorOverLifetime;
            color.enabled = true;
            Gradient gradient = new();
            GradientColorKey[] colorKeys = new GradientColorKey[particle.Colors.Values.Length];
            for (int i = 0; i < colorKeys.Length; i++)
            {
                colorKeys[i] = new GradientColorKey(new Color(particle.Colors.Values[i].X / 255f, particle.Colors.Values[i].Y / 255f,
                    particle.Colors.Values[i].Z / 255f), particle.Colors.Timestamps[i]);
            }
            GradientAlphaKey[] alphaKeys;
            if (particle.Alpha.Values.Length > 8)
            {
                alphaKeys = new GradientAlphaKey[particle.Alpha.Values.Length / 2];
            }
            else
            {
                alphaKeys = new GradientAlphaKey[particle.Alpha.Values.Length];
            }
            for (int i = 0, j = 0; i < alphaKeys.Length; i++, j++)
            {
                if (particle.Alpha.Values.Length > 8)
                {
                    j++;
                }
                alphaKeys[i] = new GradientAlphaKey(particle.Alpha.Values[j], particle.Alpha.Timestamps[j]);
            }
            gradient.SetKeys(colorKeys, alphaKeys);
            color.color = gradient;
            // Setup size in size over lifetime module
            ParticleSystem.SizeOverLifetimeModule size = system.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve curve = new();
            for (int i = 0; i < particle.Scale.Values.Length; i++)
            {
                curve.AddKey(particle.Scale.Timestamps[i], particle.Scale.Values[i].X - particle.Scale.Values[i].X * particle.ScaleVariation.X);
            }
            size.size = new ParticleSystem.MinMaxCurve(1f, curve);
            // Setup texture sheet in texture sheet animation module
            ParticleSystem.TextureSheetAnimationModule textureSheet = system.textureSheetAnimation;
            textureSheet.enabled = true;
            textureSheet.numTilesX = particle.TileColumns;
            textureSheet.numTilesY = particle.TileRows;
            curve = new();
            for (int i = 0; i < particle.TextureUV.Timestamps.Length; i++)
            {
                curve.AddKey(particle.TextureUV.Timestamps[i], particle.TextureUV.Values[i] / (float)(particle.TileColumns * particle.TileRows - 1));
            }
            textureSheet.frameOverTime = new ParticleSystem.MinMaxCurve(1f, curve);
            return element;
        }
    }
}