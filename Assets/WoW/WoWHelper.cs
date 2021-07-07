using M2Lib;
using UnityEngine;

namespace WoW
{
    //Helper class for some WoW related things.
    public class WoWHelper
    {
        //Color for each item quality
        public static Color QualityColor(int quality)
        {
            Color color;
            switch (quality)
            {
                //Poor
                case 0:
                    color = new Color(0.62f, 0.62f, 0.62f);
                    break;
                //Uncommon
                case 2:
                    color = new Color(0.12f, 1f, 0f);
                    break;
                //Rare
                case 3:
                    color = new Color(0f, 0.44f, 0.87f);
                    break;
                //Epic
                case 4:
                    color = new Color(0.64f, 0.21f, 0.93f);
                    break;
                //Legendary
                case 5:
                    color = new Color(1f, 0.5f, 0f);
                    break;
                //Artifact
                case 6:
                    color = new Color(0.9f, 0.8f, 0.5f);
                    break;
                //Heirloom
                case 7:
                    color = new Color(0f, 0.8f, 1f);
                    break;
                //Common
                default:
                    color = new Color(1f, 1f, 1f);
                    break;
            }
            return color;
        }

        //Return slot name
        public static string Slot(int slot)
        {
            string result;
            switch (slot)
            {
                case 1:
                    result = "head";
                    break;
                case 3:
                    result = "shoulder";
                    break;
                case 4:
                    result = "shirt";
                    break;
                case 5:
                    result = "chest";
                    break;
                case 6:
                    result = "waist";
                    break;
                case 7:
                    result = "legs";
                    break;
                case 8:
                    result = "feet";
                    break;
                case 9:
                    result = "wrist";
                    break;
                case 10:
                    result = "hands";
                    break;
                case 16:
                    result = "back";
                    break;
                case 19:
                    result = "tabard";
                    break;
                case 21:
                    result = "mainhand";
                    break;
                case 22:
                    result = "offhand";
                    break;
                default:
                    result = "";
                    break;
            }
            return result;
        }

        //Return slot ID
        public static int Slot(string slot)
        {
            int result;
            switch (slot.ToLower())
            {
                case "head":
                    result = 0;
                    break;
                case "shoulder":
                    result = 1;
                    break;
                case "back":
                    result = 2;
                    break;
                case "chest":
                    result = 3;
                    break;
                case "shirt":
                    result = 4;
                    break;
                case "tabard":
                    result = 5;
                    break;
                case "wrist":
                    result = 6;
                    break;
                case "mainhand":
                    result = 7;
                    break;
                case "hands":
                    result = 8;
                    break;
                case "waist":
                    result = 9;
                    break;
                case "legs":
                    result = 10;
                    break;
                case "feet":
                    result = 11;
                    break;
                case "offhand":
                    result = 12;
                    break;
                default:
                    result = -1;
                    break;
            }
            return result;
        }

        //Internal slot names
        public static string SlotName(int slot)
        {
            string result;
            switch (slot)
            {
                case 0:
                    result = "head";
                    break;
                case 1:
                    result = "shoulder";
                    break;
                case 2:
                    result = "back";
                    break;
                case 3:
                    result = "chest";
                    break;
                case 4:
                    result = "shirt";
                    break;
                case 5:
                    result = "tabard";
                    break;
                case 6:
                    result = "wrist";
                    break;
                case 7:
                    result = "mainhand";
                    break;
                case 8:
                    result = "hands";
                    break;
                case 9:
                    result = "waist";
                    break;
                case 10:
                    result = "legs";
                    break;
                case 11:
                    result = "feet";
                    break;
                case 12:
                    result = "offhand";
                    break;
                default:
                    result = "";
                    break;
            }
            return result;
        }

        //Translate race id to base race model id
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

        //Generate 3D Mesh gameobject from m2 file
        public static GameObject Generate3DMesh(M2 file)
        {
            GameObject model = new GameObject(file.Name);
            model.AddComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer renderer = model.GetComponent<SkinnedMeshRenderer>();
            Mesh mesh = new Mesh();
            mesh.name = file.Name + "_mesh";
            //Fill vertex data
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
            //Fill Submesh data
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
            //Generate bones
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

        //Particle shape
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

        //Generate particle effect system based on the data
        public static GameObject ParticleEffect(M2Particle particle)
        {
            //Create gameobject
            GameObject element = new GameObject();
            element.AddComponent<ParticleSystem>();
            ParticleSystem system = element.GetComponent<ParticleSystem>();
            //Setup lifetime and speed in main module
            ParticleSystem.MainModule main = system.main;
            float variation = particle.LifespanVariation * particle.Lifespan;
            main.startLifetime = new ParticleSystem.MinMaxCurve((particle.Lifespan - variation) / 2, (particle.Lifespan + variation) / 2);
            variation = particle.SpeedVariation * particle.Speed;
            main.startSpeed = new ParticleSystem.MinMaxCurve((particle.Speed - variation) / 2f, (particle.Speed + variation) / 2f);
            //Setup emission rate in emission module
            ParticleSystem.EmissionModule emission = system.emission;
            variation = particle.EmissionVariation * particle.EmissionRate;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve((particle.EmissionRate - variation) / 2, (particle.EmissionRate + variation) / 2);
            emission.rateOverDistance = new ParticleSystem.MinMaxCurve(particle.EmissionRate - variation, particle.EmissionRate + variation);
            //Setup shape and scale in shape module
            ParticleSystem.ShapeModule shape = system.shape;
            shape.shapeType = ParticleShape(particle.Type);
            shape.scale = new Vector3(particle.EmissionWidth, particle.EmissionLength, particle.EmissionWidth);
            //Setup color and alpha gradients in color over lifetime module
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
            //Setup size in size over lifetime module
            ParticleSystem.SizeOverLifetimeModule size = system.sizeOverLifetime;
            size.enabled = true;
            AnimationCurve curve = new AnimationCurve();
            for (int i = 0; i < particle.Scale.Values.Length; i++)
            {
                curve.AddKey(particle.Scale.Timestamps[i], particle.Scale.Values[i].X);
            }
            size.size = new ParticleSystem.MinMaxCurve(1f, curve);
            //Setup texture sheet in texture sheet animation module
            ParticleSystem.TextureSheetAnimationModule textureSheet = system.textureSheetAnimation;
            textureSheet.enabled = true;
            textureSheet.numTilesX = particle.TileColumns;
            textureSheet.numTilesY = particle.TileRows;
            return element;
        }
    }
}