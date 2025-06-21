using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using WoW;

namespace Assets.WoW
{
    public class DB
    {
        // Get races from DB
        public static Dictionary<int, string> GetRaces(SqliteConnection connection)
        {
            Dictionary<int, string> races = new();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Races;";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                races.Add(reader.GetInt32(0), reader.GetString(1));
            }
            return races;
        }

        // Get classes from DB
        public static Dictionary<int, string> GetClasses(SqliteConnection connection)
        {
            Dictionary<int, string> classes = new();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Classes;";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                classes.Add(reader.GetInt32(0), reader.GetString(1));
            }
            return classes;
        }

        // Get customization categories from DB
        public static CustomizationCategory[] GetCategories(SqliteConnection connection, int modelID)
        {
            using SqliteCommand command = connection.CreateCommand();
            List<CustomizationCategory> categories = new();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT ChrCustomizationCategory.ID, CategoryName_lang, CustomizeIcon, CustomizeIconSelected FROM " +
                $"ChrCustomizationCategory JOIN ChrCustomizationOption ON ChrCustomizationCategoryID = ChrCustomizationCategory.ID WHERE " +
                $"ChrModelID = {modelID} AND Requirement <> 10 AND Requirement <> 12 " +
                $"GROUP BY CategoryName_lang ORDER BY ChrCustomizationCategory.OrderIndex;";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3)));
            }
            return categories.ToArray();
        }

        // Get customization options from DB
        public static CustomizationOption[] GetOptions(SqliteConnection connection, int modelID, CreatureForm[] forms)
        {
            using SqliteCommand command = connection.CreateCommand();
            List<CustomizationOption> options = new();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT Name_lang, ID, ChrModelID, ChrCustomizationCategoryID, OptionType FROM ChrCustomizationOption WHERE " +
                $"ChrModelID = {modelID} AND Requirement <> 10 AND Requirement <> 12 ORDER BY SecondaryOrderIndex;";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    options.Add(new(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4)));
                }
            }
            foreach (var form in forms)
            {
                command.CommandText = $"SELECT Name_lang, ID, ChrModelID, ChrCustomizationCategoryID, OptionType FROM ChrCustomizationOption WHERE " +
                    $"ChrModelID = {form.ID} AND Requirement <> 10 AND Requirement <> 12 ORDER BY SecondaryOrderIndex;";
                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    options.Add(new(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4)));
                }
            }
            return options.ToArray();
        }

        // Get customization choices from DB
        public static Dictionary<int, CustomizationChoice> GetChoices(SqliteConnection connection, int optionID, int characterClass)
        {
            using SqliteCommand command = connection.CreateCommand();
            Dictionary<int, CustomizationChoice> choices = new();
            int id;
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT Name_lang, ChrCustomizationChoice.ID, ChrCustomizationReqID, SwatchColor_0, SwatchColor_1 FROM " +
                $"ChrCustomizationChoice JOIN ChrCustomizationReq ON ChrCustomizationReqID = ChrCustomizationReq.ID WHERE ChrCustomizationOptionID = " +
                $"{optionID} AND ReqType & 1 AND (ClassMask = 0 OR ClassMask & {1 << (characterClass - 1)}) " +
                $"AND ChrCustomizationReqID <> 3128 AND ChrCustomizationReqID <> 407 ORDER BY UiOrderIndex;";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    id = reader.GetInt32(1);
                    ColorUtility.TryParseHtmlString($"#{reader.GetInt32(3).ToString("X8").Substring(2)}", out Color color1);
                    ColorUtility.TryParseHtmlString($"#{reader.GetInt32(4).ToString("X8").Substring(2)}", out Color color2);
                    choices.Add(id, new(reader.GetString(0), id, reader.GetInt32(2), color1, color2));
                }
            }
            return choices;
        }

        // Get customization geosets from DB
        public static CustomizationGeoset[] GetGeosets(SqliteConnection connection, int choice)
        {
            using SqliteCommand command = connection.CreateCommand();
            List<CustomizationGeoset> geosets = new();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT RelatedChrCustomizationChoiceID, GeosetType, GeosetID, Modifier FROM ChrCustomizationElement JOIN " +
                $"ChrCustomizationGeoset ON ChrCustomizationGeosetID = ChrCustomizationGeoset.ID WHERE " +
                $"ChrCustomizationChoiceID = {choice} AND ChrCustomizationGeosetID > 0;";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                geosets.Add(new(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3)));
            }
            return geosets.ToArray();
        }

        // Get customization skinned geosets from DB
        public static CustomizationGeoset[] GetSkinnedGeosets(SqliteConnection connection, int choice)
        {
            using SqliteCommand command = connection.CreateCommand();
            List<CustomizationGeoset> geosets = new();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT RelatedChrCustomizationChoiceID, GeosetType, GeosetID, Modifier FROM ChrCustomizationElement JOIN " +
                $"ChrCustomizationSkinnedModel ON ChrCustomizationSkinnedModelID = ChrCustomizationSkinnedModel.ID WHERE " +
                $"ChrCustomizationChoiceID = {choice} AND ChrCustomizationSkinnedModelID > 0;";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                geosets.Add(new(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3)));
            }
            return geosets.ToArray();
        }

        // Get customization textures from DB
        public static CustomizationTexture[] GetCustomizationTextures(SqliteConnection connection, int choice)
        {
            using SqliteCommand command = connection.CreateCommand();
            List<CustomizationTexture> textures = new();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT RelatedChrCustomizationChoiceID, ChrModelTextureTargetID, FileDataID, UsageType FROM ChrCustomizationElement JOIN " +
                $"ChrCustomizationMaterial ON ChrCustomizationMaterialID = ChrCustomizationMAterial.ID JOIN TextureFileData ON " +
                $"ChrCustomizationMAterial.MaterialResourcesID = TextureFileData.MaterialResourcesID WHERE " +
                $"ChrCustomizationChoiceID = {choice} AND ChrCustomizationMAterialID > 0;";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                textures.Add(new(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3)));
            }
            return textures.ToArray();
        }

        // Get customization creature display info from DB
        public static CustomizationDisplayInfo[] GetCustomizationDisplayInfos(SqliteConnection connection, int choice)
        {
            using SqliteCommand command = connection.CreateCommand();
            List<CustomizationDisplayInfo> creatures = new();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT RelatedChrCustomizationChoiceID, CreatureDisplayInfo.ID, Path, CreatureDisplayInfo.ParticleColorID, " +
                $"TextureVariationFileDataID_0, TextureVariationFileDataID_1, TextureVariationFileDataID_2, TextureVariationFileDataID_3 FROM " +
                $"ChrCustomizationElement JOIN ChrCustomizationDisplayInfo ON ChrCustomizationDisplayInfoID = ChrCustomizationDisplayInfo.ID JOIN " +
                $"CreatureDisplayInfo ON CreatureDisplayInfoID = CreatureDisplayInfo.ID JOIN CreatureModels ON ModelID = CreatureModels.ID WHERE " +
                $"ChrCustomizationChoiceID = {choice} AND ChrCustomizationDisplayInfoID > 0;";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                creatures.Add(new(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3),
                    reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), reader.GetInt32(7)));
            }
            return creatures.ToArray();
        }

        // Get customization creature geosets from DB
        public static CustomizationGeoset[] GetCreatureGeosets(SqliteConnection connection, int creature)
        {
            using SqliteCommand command = connection.CreateCommand();
            List<CustomizationGeoset> geosets = new();
            int type;
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT GeosetIndex, GeosetValue FROM CreatureDisplayInfoGeosetData WHERE " +
                $"CreatureDisplayInfoID = {creature};";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                type = reader.GetInt32(0);
                geosets.Add(new(-1, type + 1, reader.GetInt32(1)));
            }
            return geosets.ToArray();
        }

        // Get particle colors from DB
        public static ParticleColor[] GetParticleColors(SqliteConnection connection, int id)
        {
            List<ParticleColor> colors = new();
            using SqliteCommand command = connection.CreateCommand();
            int start, mid, end;
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT Start_0, MID_0, End_0, Start_1, MID_1, End_1, Start_2, MID_2, End_2 " +
                $"FROM ParticleColor WHERE ID = {id};";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                start = reader.GetInt32(0);
                mid = reader.GetInt32(1);
                end = reader.GetInt32(2);
                colors.Add(new ParticleColor(start, mid, end));
                start = reader.GetInt32(3);
                mid = reader.GetInt32(4);
                end = reader.GetInt32(5);
                colors.Add(new ParticleColor(start, mid, end));
                start = reader.GetInt32(6);
                mid = reader.GetInt32(7);
                end = reader.GetInt32(8);
                colors.Add(new ParticleColor(start, mid, end));
            }
            return colors.ToArray();
        }

        // Get items for given slot from DB
        public static Dictionary<int, Item> GetItems(SqliteConnection connection, string condition)
        {
            using SqliteCommand command = connection.CreateCommand();
            Dictionary<int, Item> items = new();
            int id;
            string description;
            Color color = Color.black;
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT ID, ClassID, SubclassID, InventoryType, SheatheType, IconFileDataID, Display_lang, " +
                $"Flags_0, Description_lang, Color, OverallQualityID FROM Items WHERE {condition} ORDER BY ID;";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                id = reader.GetInt32(0);
                description = reader.IsDBNull(8) ? null : reader.GetString(8);
                if (!reader.IsDBNull(9))
                {
                    ColorUtility.TryParseHtmlString($"#{reader.GetInt32(9).ToString("X8")[2..]}", out color);
                }
                items.Add(id, new(id, reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5),
                    reader.GetString(6), reader.GetInt32(7), description, color, reader.GetInt32(10)));
            }
            return items;
        }

        // Get item appearance from DB
        public static void GetAppearances(SqliteConnection connection, Dictionary<int, Item> items)
        {
            using SqliteCommand command = connection.CreateCommand();
            Dictionary<int, ItemAppearance> appearances = new();
            int id, modifier;
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT ItemID, ItemAppearanceModifierID, ItemDisplayInfoID, DefaultIconFileDataID " +
                $"FROM ItemModifiedAppearance JOIN ItemAppearance ON ItemAppearanceID = ItemAppearance.ID " +
                $"WHERE ItemID IN ({string.Join(", ", items.Keys)});";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                id = reader.GetInt32(0);
                modifier = reader.GetInt32(1);
                items[id].Appearances ??= new();
                items[id].Appearances.Add(modifier, new(modifier, reader.GetInt32(2), reader.GetInt32(3)));
            }
        }

        // Get item appearance from DB
        public static void GetAppearances(SqliteConnection connection, Item item)
        {
            using SqliteCommand command = connection.CreateCommand();
            Dictionary<int, ItemAppearance> appearances = new();
            int modifier;
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT ItemAppearanceModifierID, ItemDisplayInfoID, DefaultIconFileDataID FROM " +
                $"ItemModifiedAppearance JOIN ItemAppearance ON ItemAppearanceID = ItemAppearance.ID WHERE ItemID = {item.ID};";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                modifier = reader.GetInt32(0);
                appearances.Add(modifier, new(modifier, reader.GetInt32(1), reader.GetInt32(2)));
            }
            item.Appearances = appearances;
        }

        // Get item display info from DB
        public static void GetDisplayInfo(SqliteConnection connection, ItemAppearance appearance)
        {
            using SqliteCommand command = connection.CreateCommand();
            int[] models, materials, geosets, skinnedGeosets, helmets;
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT GeosetGroupOverride, ParticleColorID, Flags, ModelResourcesID_0, ModelResourcesID_1, " +
                $"ModelMaterialResourcesID_0, ModelMaterialResourcesID_1, GeosetGroup_0, GeosetGroup_1, GeosetGroup_2, " +
                $"GeosetGroup_3, GeosetGroup_4, GeosetGroup_5, AttachmentGeosetGroup_0, AttachmentGeosetGroup_1, " +
                $"AttachmentGeosetGroup_2, AttachmentGeosetGroup_3, AttachmentGeosetGroup_4, AttachmentGeosetGroup_5, " +
                $"HelmetGeosetVis_0, HelmetGeosetVis_1 FROM ItemDisplayInfo WHERE ItemDisplayInfo.ID = {appearance.DisplayInfoID};";
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            models = new[] { reader.GetInt32(3), reader.GetInt32(4) };
            materials = new[] { reader.GetInt32(5), reader.GetInt32(6) };
            geosets = new[] { reader.GetInt32(7), reader.GetInt32(8), reader.GetInt32(9),
                        reader.GetInt32(10), reader.GetInt32(11), reader.GetInt32(12) };
            skinnedGeosets = new[] { reader.GetInt32(13), reader.GetInt32(14), reader.GetInt32(15),
                        reader.GetInt32(16), reader.GetInt32(17), reader.GetInt32(18) };
            helmets = new[] { reader.GetInt32(19), reader.GetInt32(20) };
            appearance.DisplayInfo = new(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), models, materials, geosets, skinnedGeosets, helmets);
        }

        // Get item textures from DB
        public static void GetItemModels(SqliteConnection connection, ItemDisplayInfo displayInfo)
        {
            for (int i = 0; i < displayInfo.Materials.Length; i++)
            {
                using SqliteCommand command = connection.CreateCommand();
                List<ItemModel> models = new();
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT FileDataID, GenderIndex, ClassID, RaceID, PositionIndex FROM ModelFileData LEFT JOIN ComponentModelFileData " +
                    $"ON ModelFileData.FileDataID = ComponentModelFileData.ID WHERE ModelResourcesID = {displayInfo.ModelID[i]};";
                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    models.Add(new(reader.GetInt32(0), reader.IsDBNull(1) ? 2 : reader.GetInt32(1),
                        reader.IsDBNull(2) ? WoWHelper.Class.All : (WoWHelper.Class)reader.GetInt32(2),
                        reader.IsDBNull(3) ? WoWHelper.Race.All : (WoWHelper.Race)reader.GetInt32(3),
                        reader.IsDBNull(4) ? -1 : reader.GetInt32(4)));
                }
                displayInfo.Models[i] = models.ToArray();
            }
        }

        // Get item textures from DB
        public static void GetItemTextures(SqliteConnection connection, ItemDisplayInfo displayInfo)
        {
            for(int i = 0; i < displayInfo.Materials.Length; i++)
            {
                using SqliteCommand command = connection.CreateCommand();
                List<ItemTexture> textures = new();
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT FileDataID, UsageType FROM TextureFileData WHERE MaterialResourcesID = {displayInfo.Materials[i]};";
                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    textures.Add(new(reader.GetInt32(0), reader.GetInt32(1)));
                }
                displayInfo.Textures[i] = textures.ToArray();
            }
        }

        // Get item appearance helmet geosets from DB
        public static void GetHelmetGeosets(SqliteConnection connection, ItemDisplayInfo displayInfo)
        {
            for (int i = 0; i < displayInfo.HelmetGeosets.Length; i++)
            {
                using SqliteCommand command = connection.CreateCommand();
                List<HelmetGeoset> helmetGeosets = new();
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT RaceID, HideGeosetGroup FROM HelmetGeosetData " +
                    $"WHERE HelmetGeosetVisDataID = {displayInfo.HelmetGeosetsID[i]};";
                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    helmetGeosets.Add(new((WoWHelper.Race)reader.GetInt32(0), reader.GetInt32(1)));
                }
                displayInfo.HelmetGeosets[i] = helmetGeosets.ToArray();
            }
        }

        // Get item appearance component from DB
        public static void GetComponents(SqliteConnection connection, ItemAppearance appearance)
        {
            using SqliteCommand command = connection.CreateCommand();
            List<ItemComponent> components = new();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT ComponentSection, MaterialResourcesID FROM ItemDisplayInfoMaterialRes " +
                $"WHERE ItemDisplayInfoID = {appearance.DisplayInfoID};";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                components.Add(new((WoWHelper.ComponentSection)reader.GetInt32(0), reader.GetInt32(1)));
            }
            appearance.DisplayInfo.Components = components.ToArray();
        }

        // Get item appearance component textures from DB
        public static void GetComponentTextures(SqliteConnection connection, ItemDisplayInfo displayInfo)
        {
            foreach (var component in displayInfo.Components)
            {
                using SqliteCommand command = connection.CreateCommand();
                List<ComponentTexture> textures = new();
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT FileDataID, GenderIndex, ClassID, RaceID FROM TextureFileData JOIN " +
                    $"ComponentTextureFileData ON FileDataID = ID WHERE MaterialResourcesID = {component.Material};";
                using SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    textures.Add(new(reader.GetInt32(0), reader.GetInt32(1), (WoWHelper.Class)reader.GetInt32(2),
                        (WoWHelper.Race)reader.GetInt32(3)));
                }
                component.Textures = textures.ToArray();
            }
        }

        // Get creature forms for given calss from DB
        public static CreatureForm[] GetCreatureForms(SqliteConnection connection, WoWHelper.Race race, bool gender, int id)
        {
            using SqliteCommand command = connection.CreateCommand();
            List<CreatureForm> forms = new();
            if (race == WoWHelper.Race.Worgen)
            {
                forms.Add(new CreatureForm(gender ? 1 : 2, "Gilnean", gender ? "race_humanmale" : "race_humanfemale", false));
            }
            else if (race == WoWHelper.Race.Dracthyr)
            {
                forms.Add(new CreatureForm(gender ? 127 : 128, "Visage", gender ? "race_visagemale" : "race_visagefemale", false));
            }
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT ID, Name, Icon FROM CreatureForms WHERE Class = {id} ORDER BY OrderIndex;";
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    forms.Add(new CreatureForm(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2).ToString()));
                }
            }
            return forms.ToArray();
        }

        // Get paths for race's models from DB
        public static int GetModel(SqliteConnection connection, WoWHelper.Race race, bool gender,
            out string path, out string model, out string extra, out string armor, out string collection)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceModels WHERE Race = {(int)race} AND Gender = {race == WoWHelper.Race.Dracthyr || gender};";
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            path = reader.GetString(6);
            model = reader.GetString(4);
            if (race == WoWHelper.Race.Dracthyr)
            {
                extra = null;
                armor = reader.GetString(5);
            }
            else
            {
                extra = reader.IsDBNull(5) ? null : reader.GetString(5);
                armor = null;
            }
            collection = reader.IsDBNull(7) ? null : reader.GetString(7);
            return reader.GetInt32(0);
        }

        // Get extra model from DB
        public static string GetModel(SqliteConnection connection, bool gender, out string extra, out string extraCollection)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceModels WHERE Race = 75 AND Gender = {gender};";
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            extra = reader.GetString(4);
            extraCollection = reader.GetString(7);
            return reader.GetString(6);
        }

        // Get classes available for given race from DB
        public static bool[] GetRaceClasses(SqliteConnection connection, int race, int length)
        {
            bool[] classes = new bool[length];
            using SqliteCommand command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = $"SELECT * FROM RaceClassCombos WHERE Race = {race};";
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            for (int i = 0; i < length; i++)
            {
                classes[i] = reader.GetBoolean(i + 1);
            }
            return classes;
        }
    }
}
