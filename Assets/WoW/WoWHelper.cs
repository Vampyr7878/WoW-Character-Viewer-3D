using M2Lib;
using UnityEngine;

namespace WoW
{
    // Helper class for some WoW related things.
    public class WoWHelper
    {
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
                5 => new Color32(122, 128, 0, 255),
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
        public static string Slot(int slot)
        {
            string result = slot switch
            {
                1 => "head",
                3 => "shoulder",
                4 => "shirt",
                5 => "chest",
                6 => "waist",
                7 => "legs",
                8 => "feet",
                9 => "wrist",
                10 => "hands",
                16 => "back",
                19 => "tabard",
                21 => "mainhand",
                22 => "offhand",
                _ => "",
            };
            return result;
        }

        // Return slot ID
        public static int Slot(string slot)
        {
            var result = slot.ToLower() switch
            {
                "head" => 0,
                "shoulder" => 1,
                "back" => 2,
                "chest" => 3,
                "shirt" => 4,
                "tabard" => 5,
                "wrist" => 6,
                "mainhand" => 7,
                "hands" => 8,
                "waist" => 9,
                "legs" => 10,
                "feet" => 11,
                "offhand" => 12,
                _ => -1,
            };
            return result;
        }

        // Internal slot names
        public static string SlotName(int slot)
        {
            string result = slot switch
            {
                0 => "head",
                1 => "shoulder",
                2 => "back",
                3 => "chest",
                4 => "shirt",
                5 => "tabard",
                6 => "wrist",
                7 => "mainhand",
                8 => "hands",
                9 => "waist",
                10 => "legs",
                11 => "feet",
                12 => "offhand",
                _ => "",
            };
            return result;
        }

        // Translate race id to base race model id
        public static int RaceModel(int race)
        {
            int result = 0;
            switch (race)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 22:
                case 24:
                case 32:
                case 35:
                    result = race;
                    break;
                case 23:
                    result = 1;
                    break;
                case 27:
                    result = 4;
                    break;
                case 28:
                    result = 6;
                    break;
                case 29:
                    result = 10;
                    break;
                case 30:
                    result = 11;
                    break;
                case 31:
                    result = 8;
                    break;
                case 34:
                    result = 3;
                    break;
                case 36:
                    result = 2;
                    break;
                case 37:
                    result = 7;
                    break;
            }
            return result;
        }

        // Return text describing the item version
        public static string ItemVersion(int version)
        {
            string result = "";
            switch(version)
            {
                case 1:
                    result = "(Heroic)";
                    break;
                case 2:
                    result = "(Raid Finder)";
                    break;
                case 3:
                    result = "(Mythic)";
                    break;
            }
            return result;
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
                vertices[i] = new Vector3(-file.Vertices[i].Position.X / 2, file.Vertices[i].Position.Y / 2, file.Vertices[i].Position.Z / 2);
                normals[i] = new Vector3(-file.Vertices[i].Normal.X, file.Vertices[i].Normal.Y, file.Vertices[i].Normal.Z);
                BoneWeight weight = new BoneWeight
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
            mesh.subMeshCount = file.Skin.Submeshes.Length;
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] triangles = new int[file.Skin.Submeshes[i].Count];
                for (int j = 0; j < triangles.Length; j++)
                {
                    triangles[j] = file.Skin.Indices[file.Skin.Submeshes[i].Start + j];
                }
                mesh.SetTriangles(triangles, i);
            }
            // Generate bones
            Transform[] bones = new Transform[file.Skeleton.Bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i] = new GameObject("Bone" + i).transform;
                bones[i].position = new Vector3(-file.Skeleton.Bones[i].Pivot.X / 2, file.Skeleton.Bones[i].Pivot.Y / 2, file.Skeleton.Bones[i].Pivot.Z / 2);
            }
            GameObject skeleton = new GameObject("Skeleton");
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
                    shape = ParticleSystemShapeType.Circle;
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
            emission.rateOverTime = new ParticleSystem.MinMaxCurve((particle.EmissionRate - variation) / 2f, (particle.EmissionRate + variation) / 2f);
            emission.rateOverDistance = new ParticleSystem.MinMaxCurve(particle.EmissionRate - variation, particle.EmissionRate + variation);
            // Setup shape and scale in shape module
            ParticleSystem.ShapeModule shape = system.shape;
            shape.shapeType = ParticleShape(particle.Type);
            shape.scale = new Vector3(particle.EmissionWidth, particle.EmissionWidth, particle.EmissionLength);
            // Setup color and alpha gradients in color over lifetime module
            ParticleSystem.ColorOverLifetimeModule color = system.colorOverLifetime;
            color.enabled = true;
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[particle.Colors.Values.Length];
            for (int i = 0; i < colorKeys.Length; i++)
            {
                colorKeys[i] = new GradientColorKey(new Color(particle.Colors.Values[i].X / 255f, particle.Colors.Values[i].Y / 255f, particle.Colors.Values[i].Z / 255f), particle.Colors.Timestamps[i]);
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
            AnimationCurve curve = new AnimationCurve();
            for (int i = 0; i < particle.Scale.Values.Length; i++)
            {
                curve.AddKey(particle.Scale.Timestamps[i], particle.Scale.Values[i].X);
            }
            size.size = new ParticleSystem.MinMaxCurve(1f, curve);
            // Setup texture sheet in texture sheet animation module
            ParticleSystem.TextureSheetAnimationModule textureSheet = system.textureSheetAnimation;
            textureSheet.enabled = true;
            textureSheet.numTilesX = particle.TileColumns;
            textureSheet.numTilesY = particle.TileRows;
            return element;
        }
    }
}