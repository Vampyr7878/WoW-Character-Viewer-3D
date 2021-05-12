using M2Lib;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

//Importer for m2 files
[ScriptedImporter(1, "m2")]
public class M2Importer : ScriptedImporter
{
    //Set if it is a character model
    public bool character;
    //Shield attachment point
    public GameObject shield;
    //Right hand attachment point
    public GameObject handRight;
    //Left hand attachment point
    public GameObject handLeft;
    //Right Shoulder attachment point
    public GameObject shoulderRight;
    //Left Shoulder attachment point
    public GameObject shoulderLeft;
    //Helm attachment point
    public GameObject helm;
    //Quiver attachment point
    public GameObject quiver;
    //Buckle attachment point
    public GameObject buckle;
    //Book attachment point
    public GameObject book;
    //Backpack attachment point
    public GameObject backpack;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        //Load file contents
        M2 file = new M2();
        file.LoadFile(ctx.assetPath);
        //Prepare blank asset
        GameObject model = new GameObject();
        model.AddComponent<Animator>();
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
            vertices[i] = new Vector3(-file.Vertices[i].Position.X / 2, file.Vertices[i].Position.Z / 2, -file.Vertices[i].Position.Y / 2);
            normals[i] = new Vector3(-file.Vertices[i].Normal.X, file.Vertices[i].Normal.Z, -file.Vertices[i].Normal.Y);
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
            bones[i].position = new Vector3(-file.Skeleton.Bones[i].Pivot.X / 2, file.Skeleton.Bones[i].Pivot.Z / 2, -file.Skeleton.Bones[i].Pivot.Y / 2);
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
        //Add Attachment points
        if (character)
        {
            AddAttachmentPoint(shield, bones[file.Skeleton.Attachments[file.Skeleton.AttachmentLookup[0]].Bone]);
            AddAttachmentPoint(handRight, bones[file.Skeleton.Attachments[file.Skeleton.AttachmentLookup[1]].Bone]);
            AddAttachmentPoint(handLeft, bones[file.Skeleton.Attachments[file.Skeleton.AttachmentLookup[2]].Bone]);
            AddAttachmentPoint(shoulderRight, bones[file.Skeleton.Attachments[file.Skeleton.AttachmentLookup[5]].Bone]);
            AddAttachmentPoint(shoulderLeft, bones[file.Skeleton.Attachments[file.Skeleton.AttachmentLookup[6]].Bone]);
            AddAttachmentPoint(helm, bones[file.Skeleton.Attachments[file.Skeleton.AttachmentLookup[11]].Bone]);
            AddAttachmentPoint(quiver, bones[file.Skeleton.Attachments[file.Skeleton.AttachmentLookup[26]].Bone]);
            AddAttachmentPoint(buckle, bones[file.Skeleton.Attachments[file.Skeleton.AttachmentLookup[53]].Bone]); ;
            AddAttachmentPoint(book, bones[file.Skeleton.Attachments[file.Skeleton.AttachmentLookup[53]].Bone]);
            AddAttachmentPoint(backpack, bones[file.Skeleton.Attachments[file.Skeleton.AttachmentLookup[57]].Bone]);
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
        //Fill particle effect data
        //if (file.Particles.Length > 0)
        //{
        //    GameObject[] particles = new GameObject[file.Particles.Length];
        //    for (int i = 0; i < particles.Length; i++)
        //    {
        //        particles[i] = ParticleEffect(file.Particles[i]);
        //        particles[i].transform.parent = bones[file.Particles[i].Bone];
        //        particles[i].transform.localPosition = Vector3.zero;
        //        particles[i].name = "Particle" + i;
        //        ctx.AddObjectToAsset(particles[i].name, particles[i]);
        //    }
        //}
        //Load *.bytes files so data can be easly accessible at runtime
        string path = Path.GetDirectoryName(ctx.assetPath);
        TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(path + "\\data.bytes");
        TextAsset skin = AssetDatabase.LoadAssetAtPath<TextAsset>(path + "\\skin.bytes");
        TextAsset skel = new TextAsset("");
        skel.name = "skel";
        if (file.SkelFileID != 0)
        {
            skel = AssetDatabase.LoadAssetAtPath<TextAsset>(path + "\\skel.bytes");
        }
        M2Model m2 = model.AddComponent<M2Model>();
        m2.data = data;
        m2.skin = skin;
        m2.skel = skel;
        //Populate the asset
        ctx.AddObjectToAsset(file.Name, model);
        ctx.AddObjectToAsset(mesh.name, mesh);
        ctx.AddObjectToAsset(skeleton.name, skeleton);
        ctx.AddObjectToAsset(data.name, data);
        ctx.AddObjectToAsset(skin.name, skin);
        ctx.AddObjectToAsset(skel.name, skel);
    }

    //Instantiate empty GameObject as an attachment point for armor and weapons
    private void AddAttachmentPoint(GameObject point, Transform bone)
    {
        GameObject attachment = Instantiate(point, bone);
        attachment.name = attachment.name.Replace("(Clone)", "");
    }

    //Particle shape
    private ParticleSystemShapeType ParticleShape(byte value)
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

    //Generate particle effect system based on the data
    private GameObject ParticleEffect(M2Particle particle)
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