using M2Lib;
using SkelLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using WoW;

// Importer for m2 files
[ScriptedImporter(1, "m2")]
public class M2Importer : ScriptedImporter
{
    // Set if it is a character model
    public bool character;
    // Shield attachment point
    public GameObject shield;
    // Right hand attachment point
    public GameObject handRight;
    // Left hand attachment point
    public GameObject handLeft;
    // Right Shoulder attachment point
    public GameObject shoulderRight;
    // Left Shoulder attachment point
    public GameObject shoulderLeft;
    // Helm attachment point
    public GameObject helm;
    // Quiver attachment point
    public GameObject quiver;
    // Buckle attachment point
    public GameObject buckle;
    // Book attachment point
    public GameObject book;
    // Backpack attachment point
    public GameObject backpack;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        // Load file contents
        M2 file = new();
        file.LoadFile(ctx.assetPath);
        // Prepare blank asset
        GameObject model = new();
        model.AddComponent<Animator>();
        model.AddComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer renderer = model.GetComponent<SkinnedMeshRenderer>();
        Mesh mesh = new()
        {
            name = $"{file.Name}_mesh"
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
        // Add Attachment points
        if (character)
        {
            AddAttachmentPoint(shield, bones[file.Skeleton.Attachments[file.Skeleton.
                AttachmentLookup[(int)WoWHelper.AttachmentPoint.Shield]].Bone]);
            AddAttachmentPoint(handRight, bones[file.Skeleton.Attachments[file.Skeleton.
                AttachmentLookup[(int)WoWHelper.AttachmentPoint.HandRight]].Bone]);
            AddAttachmentPoint(handLeft, bones[file.Skeleton.Attachments[file.Skeleton.
                AttachmentLookup[(int)WoWHelper.AttachmentPoint.HandLeft]].Bone]);
            AddAttachmentPoint(shoulderRight, bones[file.Skeleton.Attachments[file.Skeleton.
                AttachmentLookup[(int)WoWHelper.AttachmentPoint.ShoulderRight]].Bone]);
            AddAttachmentPoint(shoulderLeft, bones[file.Skeleton.Attachments[file.Skeleton.
                AttachmentLookup[(int)WoWHelper.AttachmentPoint.ShoulderLeft]].Bone]);
            AddAttachmentPoint(helm, bones[file.Skeleton.Attachments[file.Skeleton.
                AttachmentLookup[(int)WoWHelper.AttachmentPoint.Helm]].Bone]);
            AddAttachmentPoint(quiver, bones[file.Skeleton.Attachments[file.Skeleton.
                AttachmentLookup[(int)WoWHelper.AttachmentPoint.Quiver]].Bone]);
            AddAttachmentPoint(buckle, bones[file.Skeleton.Attachments[file.Skeleton.
                AttachmentLookup[(int)WoWHelper.AttachmentPoint.Buckle]].Bone]);
            AddAttachmentPoint(book, bones[file.Skeleton.Attachments[file.Skeleton.
                AttachmentLookup[(int)WoWHelper.AttachmentPoint.Book]].Bone]);
            AddAttachmentPoint(backpack, bones[file.Skeleton.Attachments[file.Skeleton.
                AttachmentLookup[(int)WoWHelper.AttachmentPoint.Backpack]].Bone]);
        }
        // Add Animation Seqences
        string[] bonePaths = new string[file.Skeleton.Bones.Length];
        for (int i = 0; i < bonePaths.Length; i++)
        {
            bonePaths[i] = $"{WoWHelper.GetBonePath(file.Skeleton.Bones[i].Parent, file.Skeleton.Bones)}Bone{i}";
        }
        AnimationClip loop = null;
        if (file.Skeleton.Loops.Length > 0)
        {
            loop = GetLoop(0, file.Skeleton, bonePaths, bones);
        }
        List<AnimationClip> clips = null;
        if (file.Skeleton.Sequences.Length > 0)
        {
            clips = new()
            {
                GetSeqence(0, file.Skeleton, bonePaths, bones),
            };
        }
        // Load *.bytes files so data can be easly accessible at runtime
        string path = Path.GetDirectoryName(ctx.assetPath);
        TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(path + "\\data.bytes");
        TextAsset skin = AssetDatabase.LoadAssetAtPath<TextAsset>(path + "\\skin.bytes");
        TextAsset skel = file.SkelFileID == 0 ? null : AssetDatabase.LoadAssetAtPath<TextAsset>(path + "\\skel.bytes");
        M2Model m2 = model.AddComponent<M2Model>();
        m2.data = data;
        m2.skin = skin;
        if (skel != null)
        {
            m2.skel = skel;
        }
        // Populate the asset
        skeleton.transform.localEulerAngles = new Vector3(-90, 0, 0);
        ctx.AddObjectToAsset(file.Name, model);
        ctx.AddObjectToAsset(mesh.name, mesh);
        ctx.AddObjectToAsset(skeleton.name, skeleton);
        ctx.AddObjectToAsset(data.name, data);
        ctx.AddObjectToAsset(skin.name, skin);
        if (skel != null)
        {
            ctx.AddObjectToAsset(skel.name, skel);
        }
        if (loop != null)
        {
            ctx.AddObjectToAsset(loop.name, loop);
        }
        if (clips != null)
        {
            foreach (var clip in clips)
            {
                ctx.AddObjectToAsset(clip.name, clip);
            }
        }
    }
    
    // Add attachment points for 3d elements
    private void AddAttachmentPoint(GameObject point, Transform bone)
    {
        GameObject attachment = Instantiate(point, bone);
        attachment.name = attachment.name.Replace("(Clone)", "");
    }

    // Create animation seqence clip
    private AnimationClip GetSeqence(short sequence, Skel skeleton, string[] bonePaths, Transform[] bones)
    {
        AnimationClip clip = new()
        {
            name = $"Seqence{sequence}"
        };
        int index = Array.IndexOf(skeleton.Sequences, sequence);
        for (int i = 0; i < skeleton.Bones.Length; i++)
        {
            Bone bone = skeleton.Bones[i];
            if (bone.Translation.Values.Length == skeleton.Sequences.Length)
            {
                WoWHelper.TranslationCurves(clip, index, i, bonePaths, bone, bones);
            }
            if (bone.Rotation.Values.Length == skeleton.Sequences.Length)
            {
                WoWHelper.RotationCurves(clip, index, i, bonePaths, bone, bones);
            }
            if (bone.Scale.Values.Length == skeleton.Sequences.Length)
            {
                WoWHelper.ScaleCurves(clip, index, i, bonePaths, bone, bones);
            }
        }
        return clip;
    }

    // Create global animation loop, mostly used to scale attachment points
    private AnimationClip GetLoop(int loop, Skel skeleton, string[] bonePaths, Transform[] bones)
    {
        AnimationClip clip = new()
        {
            name = $"Loop"
        };
        for (int i = 0; i < skeleton.Bones.Length; i++)
        {
            Bone bone = skeleton.Bones[i];
            if (bone.Translation.Sequence == loop)
            {
                WoWHelper.TranslationCurves(clip, loop, i, bonePaths, bone, bones);
            }
            if (bone.Rotation.Sequence == loop)
            {
                WoWHelper.RotationCurves(clip, loop, i, bonePaths, bone, bones);
            }
            if (bone.Scale.Sequence == loop)
            {
                WoWHelper.ScaleCurves(clip, loop, i, bonePaths, bone, bones);
            }
        }
        return clip;
    }
}
