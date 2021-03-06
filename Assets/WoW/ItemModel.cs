using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace WoW
{
    //Class to store all item data
    public class ItemModel
    {
        //Item's id
        public int ID { get; private set; }
        //Item's version
        public int Version { get; private set; }
        //Slot this item goes into
        public int Slot { get; private set; }
        //Model for left side
        public int LeftModel { get; private set; }
        //Model for right side
        public int RightModel { get; private set; }
        //Texture for left model
        public int LeftTexture { get; private set; }
        //Texture for rigth model
        public int RightTexture { get; private set; }
        //Geosets enabled by this item
        public int Geoset1 { get; private set; }
        public int Geoset2 { get; private set; }
        public int Geoset3 { get; private set; }
        public int Geoset4 { get; private set; }
        public int Geoset5 { get; private set; }
        public int Geoset6 { get; private set; }
        //Geosets in collections enabled by this item
        public int Collection1 { get; private set; }
        public int Collection2 { get; private set; }
        public int Collection3 { get; private set; }
        public int Collection4 { get; private set; }
        public int Collection5 { get; private set; }
        public int Collection6 { get; private set; }
        //Textures to be painted on the body
        public int UpperArm { get; private set; }
        public int LowerArm { get; private set; }
        public int Hand { get; private set; }
        public int UpperTorso { get; private set; }
        public int LowerTorso { get; private set; }
        public int UpperLeg { get; private set; }
        public int LowerLeg { get; private set; }
        public int Foot { get; private set; }
        //Geosets that are hidden by the helmet
        public List<int> Helmet { get; private set; }
        //Geosets that are hidden by the helmet in Gilnean form
        public List<int> GilneanHelmet { get; private set; }
        //Particle colors for the item
        public ParticleColor[] ParticleColors { get; private set; }

        private SqliteConnection connection;

        //Constructor
        public ItemModel(int id, int version, int slot, int display, int race, bool gender)
        {
            ID = id;
            Version = version;
            Slot = slot;
            connection = new SqliteConnection($"URI=file:{Application.streamingAssetsPath}/database.sqlite");
            connection.Open();
            if (display == 0)
            {
                LeftModel = 0;
                RightModel = 0;
                LeftTexture = 0;
                RightTexture = 0;
                Geoset1 = 0;
                Geoset2 = 0;
                Geoset3 = 0;
                Geoset4 = 0;
                Geoset5 = 0;
                Geoset6 = 0;
                Collection1 = 0;
                Collection2 = 0;
                Collection3 = 0;
                Collection4 = 0;
                Collection5 = 0;
                Collection6 = 0;
                UpperArm = 0;
                LowerArm = 0;
                Hand = 0;
                UpperTorso = 0;
                LowerTorso = 0;
                UpperLeg = 0;
                LowerLeg = 0;
                Foot = 0;
                Helmet = HelmetGeosets(0, race);
                GilneanHelmet = HelmetGeosets(0, 1);
            }
            else
            {
                int maleHelmet;
                int femaleHelmet;
                int particleColor;
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT * FROM DisplayInfo WHERE ID = {display};";
                    SqliteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    LeftModel = reader.GetInt32(1);
                    RightModel = reader.GetInt32(2);
                    LeftTexture = reader.GetInt32(3);
                    RightTexture = reader.GetInt32(4);
                    Geoset1 = reader.GetInt32(5);
                    Geoset2 = reader.GetInt32(6);
                    Geoset3 = reader.GetInt32(7);
                    Geoset4 = reader.GetInt32(8);
                    Geoset5 = reader.GetInt32(9);
                    Geoset6 = reader.GetInt32(10);
                    Collection1 = reader.GetInt32(11);
                    Collection2 = reader.GetInt32(12);
                    Collection3 = reader.GetInt32(13);
                    Collection4 = reader.GetInt32(14);
                    Collection5 = reader.GetInt32(15);
                    Collection6 = reader.GetInt32(16);
                    UpperArm = reader.GetInt32(17);
                    LowerArm = reader.GetInt32(18);
                    Hand = reader.GetInt32(19);
                    UpperTorso = reader.GetInt32(20);
                    LowerTorso = reader.GetInt32(21);
                    UpperLeg = reader.GetInt32(22);
                    LowerLeg = reader.GetInt32(23);
                    Foot = reader.GetInt32(24);
                    maleHelmet = reader.GetInt32(25);
                    femaleHelmet = reader.GetInt32(26);
                    particleColor = reader.GetInt32(27);
                }
                Helmet = gender ? HelmetGeosets(maleHelmet, race) : HelmetGeosets(femaleHelmet, race);
                GilneanHelmet = gender ? HelmetGeosets(maleHelmet, 1) : HelmetGeosets(femaleHelmet, 1);
                ParticleColors = GetParticleColors(particleColor);
            }
            connection.Close();
        }

        //Get proper texture ID based on gender
        public int GetMaterial(int resource, int race, bool gender, int c)
        {
            int result = 0;
            int g = gender ? 0 : 1;
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT ID FROM ItemMaterials WHERE Material = {resource} AND Race = {race} AND Gender = 3;";
                SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
            }
            if (result == 0)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT ID FROM ItemMaterials WHERE Material = {resource} AND Race = {race} AND Gender = {g};";
                    SqliteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            if (result == 0)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT ID FROM ItemMaterials WHERE Material = {resource} AND Class = {c} AND Gender = 3;";
                    SqliteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            if (result == 0)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT ID FROM ItemMaterials WHERE Material = {resource} AND Class = {c} AND Gender = {g};";
                    SqliteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            if (result == 0)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT ID FROM ItemMaterials WHERE Material = {resource} AND Gender = 3;";
                    SqliteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            if (result == 0)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT ID FROM ItemMaterials WHERE Material = {resource} AND Gender = {g};";
                    SqliteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            connection.Close();
            return result;
        }

        //Get modelf file taht fits specific race, gender and class
        public int GetRaceSpecificModel(int resource, int race, bool gender, int c)
        {
            int result = 0;
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT Model FROM ItemModels JOIN ComponentModels ON Model = ComponentModels.ID WHERE \"Index\" = {resource} AND Race = {race} AND Gender = {gender} AND Class = {c};";
                SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
            }
            if (result == 0)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT Model FROM ItemModels JOIN ComponentModels ON Model = ComponentModels.ID WHERE \"Index\" = {resource} AND Race = {race} AND Gender = {gender};";
                    SqliteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            if (result == 0)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT Model FROM ItemModels JOIN ComponentModels ON Model = ComponentModels.ID WHERE \"Index\" = {resource} AND Race = {WoWHelper.RaceModel(race)} AND Gender = {gender};";
                    SqliteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            connection.Close();
            return result;
        }

        //Get left or right model file depending on chosen side
        public int GetSideSpecificModel(int resource, bool side, int c)
        {
            int result = 0;
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT Model FROM ItemModels JOIN ComponentModels ON Model = ComponentModels.ID WHERE \"Index\" = {resource} AND Position = {side} AND Class = {c};";
                SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
            }
            if (result == 0)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT Model FROM ItemModels JOIN ComponentModels ON Model = ComponentModels.ID WHERE \"Index\" = {resource} AND Position = {side};";
                    SqliteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            connection.Close();
            return result;
        }

        //Get model file
        public int GetModel(int resource, int c)
        {
            int result = 0;
            connection.Open();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT Model FROM ItemModels JOIN ComponentModels ON Model = ComponentModels.ID WHERE \"Index\" = {resource} AND Class = {c};";
                SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    result = reader.GetInt32(0);
                }
            }
            if (result == 0)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT Model FROM ItemModels WHERE \"Index\" = {resource};";
                    SqliteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            connection.Close();
            return result;
        }

        //Load geosets to be hidden with the helmet
        private List<int> HelmetGeosets(int index, int race)
        {
            List<int> helmet = new List<int>();
            if (index == 0)
            {
                return helmet;
            }
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT Geoset FROM HelmetGeosets WHERE [Index] = " + index + " AND Race = " + race + ";";
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    helmet.Add(reader.GetInt32(0));
                }
            }
            return helmet;
        }

        //Load particle colors for given item
        private ParticleColor[] GetParticleColors(int id)
        {
            ParticleColor[] particleColors = new ParticleColor[3];
            if (id == 0)
            {
                return particleColors;
            }
            using (SqliteCommand command = connection.CreateCommand())
            {
                int start, mid, end;
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * FROM ParticleColors WHERE ID = " + id + ";";
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    start = reader.GetInt32(1);
                    mid = reader.GetInt32(4);
                    end = reader.GetInt32(7);
                    particleColors[0] = new ParticleColor(start, mid, end);
                    start = reader.GetInt32(2);
                    mid = reader.GetInt32(5);
                    end = reader.GetInt32(8);
                    particleColors[1] = new ParticleColor(start, mid, end);
                    start = reader.GetInt32(3);
                    mid = reader.GetInt32(6);
                    end = reader.GetInt32(9);
                    particleColors[2] = new ParticleColor(start, mid, end);
                }
            }
            return particleColors;
        }
    }
}
