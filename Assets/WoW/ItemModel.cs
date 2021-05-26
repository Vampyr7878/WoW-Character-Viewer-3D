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
        //Particle colors for the item
        public ParticleColor[] ParticleColors { get; private set; }

        //Constructor
        public ItemModel(int id, int version, int slot, int display, int race, bool gender)
        {
            ID = id;
            Version = version;
            Slot = slot;
            SqliteConnection connection = new SqliteConnection("URI=file:" + Application.streamingAssetsPath + "/database.sqlite");
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
                Helmet = HelmetGeosets(connection, 0, race);
            }
            else
            {
                int leftModel;
                int rightModel;
                int upperArm;
                int lowerArm;
                int hand;
                int upperTorso;
                int lowerTorso;
                int upperLeg;
                int lowerLeg;
                int foot;
                int maleHelmet;
                int femaleHelmet;
                int particleColor;
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM DisplayInfo WHERE ID = " + display + ";";
                    SqliteDataReader reader = command.ExecuteReader();
                    reader.Read();
                    leftModel = reader.GetInt32(1);
                    rightModel = reader.GetInt32(2);
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
                    upperArm = reader.GetInt32(17);
                    lowerArm = reader.GetInt32(18);
                    hand = reader.GetInt32(19);
                    upperTorso = reader.GetInt32(20);
                    lowerTorso = reader.GetInt32(21);
                    upperLeg = reader.GetInt32(22);
                    lowerLeg = reader.GetInt32(23);
                    foot = reader.GetInt32(24);
                    maleHelmet = reader.GetInt32(25);
                    femaleHelmet = reader.GetInt32(26);
                    particleColor = reader.GetInt32(27);
                }
                //LeftModel = GetFilePath(connection, leftModel).Replace("rshoulder", "lshoulder").Replace("_r.", "_l.");
                //RightModel = GetFilePath(connection, rightModel).Replace("lshoulder", "rshoulder").Replace("_l.", "_r.");
                //UpperArm = GetFilePath(connection, upperArm).Replace("_f.", "_u.").Replace("_m.", "_u.").Replace("_m.", "_u.");
                //LowerArm = GetFilePath(connection, lowerArm).Replace("_f.", "_u.").Replace("_m.", "_u.");
                //Hand = GetFilePath(connection, hand).Replace("_f.", "_u.").Replace("_m.", "_u.");
                //UpperTorso = GetFilePath(connection, upperTorso).Replace("_f.", "_u.").Replace("_m.", "_u.");
                //LowerTorso = GetFilePath(connection, lowerTorso).Replace("_f.", "_u.").Replace("_m.", "_u.");
                //UpperLeg = GetFilePath(connection, upperLeg).Replace("_f.", "_u.").Replace("_m.", "_u.");
                //LowerLeg = GetFilePath(connection, lowerLeg).Replace("_f.", "_u.").Replace("_m.", "_u.");
                //Foot = GetFilePath(connection, foot).Replace("_f.", "_u.").Replace("_m.", "_u.");
                if (gender)
                {
                    Helmet = HelmetGeosets(connection, maleHelmet, race);
                }
                else
                {
                    Helmet = HelmetGeosets(connection, femaleHelmet, race);
                }
                ParticleColors = GetParticleColors(connection, particleColor);
            }
            connection.Close();
        }

        string GetFilePath(SqliteConnection connection, int index)
        {
            if (index == 0)
            {
                return "";
            }
            string result;
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT File FROM Files WHERE ID = " + index + ";";
                SqliteDataReader reader = command.ExecuteReader();
                reader.Read();
                result = reader.GetString(0);
            }
            return result;
        }

        List<int> HelmetGeosets(SqliteConnection connection, int index, int race)
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

        ParticleColor[] GetParticleColors(SqliteConnection connection, int id)
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
