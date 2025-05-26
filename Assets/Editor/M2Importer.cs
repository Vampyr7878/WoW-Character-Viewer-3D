using M2Lib;
using Newtonsoft.Json.Linq;
using SkelLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

//Importer for m2 files
[ScriptedImporter(1, "m2")]
public class M2Importer : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        //Load file contents
        M2 file = new();
        file.LoadFile(ctx.assetPath);
        //Prepare blank asset
        GameObject model = new();
        model.AddComponent<Animator>();
        model.AddComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer renderer = model.GetComponent<SkinnedMeshRenderer>();
        Mesh mesh = new Mesh();
        mesh.name = $"{file.Name}_mesh";
        //Fill vertex data
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
        //Add Attachment points
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
        //Add Animation Seqences
        string[] bonePaths = new string[file.Skeleton.Bones.Length];
        for (int i = 0; i < bonePaths.Length; i++)
        {
            bonePaths[i] = $"{GetBonePath(file.Skeleton.Bones[i].Parent, file.Skeleton.Bones)}Bone{i}";
        }
        AnimationClip loop = null;
        if (file.Skeleton.Loops.Length > 0)
        {
            loop = GetLoop(0, file.Skeleton, bonePaths, bones);
        }
        List<AnimationClip> clips = null;
        if (file.Skeleton.SequenceLookup.Length > 0)
        {
            clips = new()
            {
                GetSeqence(0, file.Skeleton, bonePaths, bones),
            };
        }
        //Load *.bytes files so data can be easly accessible at runtime
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
        //Populate the asset
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

    //Recursively get full path to the bone
    private string GetBonePath(short bone, Bone[] bones)
    {
        if (bone == -1)
        {
            return "Skeleton/";
        }
        if (bones[bone].Parent == -1)
        {
            return $"Skeleton/Bone{bone}/";
        }
        else
        {
            return $"{GetBonePath(bones[bone].Parent, bones)}Bone{bone}/";
        }
    }

    //Create animation seqence clip
    private AnimationClip GetSeqence(short sequence, Skel skeleton, string[] bonePaths, Transform[] bones)
    {
        AnimationClip clip = new()
        {
            name = $"Seqence{sequence}"
        };
        AnimationCurve curve;
        int index = Array.IndexOf(skeleton.Sequences, sequence);
        for (int i = 0; i < skeleton.Bones.Length; i++)
        {
            Bone bone = skeleton.Bones[i];
            if (bone.Translation.Values.Length == skeleton.Sequences.Length)
            {
                curve = TranslationCurve(bone.Translation.Interpolation, bone.Translation.Timestamps[index],
                    bone.Translation.Values[index].Select(v => v.X).ToArray(), bones[i].transform.localPosition.x);
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalPosition.x", curve);
                }
                curve = TranslationCurve(bone.Translation.Interpolation, bone.Translation.Timestamps[index],
                    bone.Translation.Values[index].Select(v => v.Y).ToArray(), bones[i].transform.localPosition.y);
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalPosition.y", curve);
                }
                curve = TranslationCurve(bone.Translation.Interpolation, bone.Translation.Timestamps[index],
                    bone.Translation.Values[index].Select(v => v.Z).ToArray(), bones[i].transform.localPosition.z);
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalPosition.z", curve);
                }
            }
            if (bone.Rotation.Values.Length == skeleton.Sequences.Length)
            {
                curve = RotationCurve(bone.Rotation.Interpolation, bone.Rotation.Timestamps[index], bone.Rotation.Values[index].Select(v => v.X).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalRotation.x", curve);
                }
                curve = RotationCurve(bone.Rotation.Interpolation, bone.Rotation.Timestamps[index], bone.Rotation.Values[index].Select(v => v.Y).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalRotation.y", curve);
                }
                curve = RotationCurve(bone.Rotation.Interpolation, bone.Rotation.Timestamps[index], bone.Rotation.Values[index].Select(v => v.Z).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalRotation.z", curve);
                }
                curve = RotationCurve(bone.Rotation.Interpolation, bone.Rotation.Timestamps[index], bone.Rotation.Values[index].Select(v => v.W).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalRotation.w", curve);
                }
            }
            if (bone.Scale.Values.Length == skeleton.Sequences.Length)
            {
                curve = ScaleCurve(bone.Scale.Interpolation, bone.Scale.Timestamps[index], bone.Scale.Values[index].Select(v => v.X).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalScale.x", curve);
                }
                curve = ScaleCurve(bone.Scale.Interpolation, bone.Scale.Timestamps[index], bone.Scale.Values[index].Select(v => v.Y).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalScale.y", curve);
                }
                curve = ScaleCurve(bone.Scale.Interpolation, bone.Scale.Timestamps[index], bone.Scale.Values[index].Select(v => v.Z).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalScale.z", curve);
                }
            }
        }
        return clip;
    }

    //Create global animation loop, mostly used to scale attachment points
    private AnimationClip GetLoop(int loop, Skel skeleton, string[] bonePaths, Transform[] bones)
    {
        AnimationClip clip = new()
        {
            name = $"Loop"
        };
        AnimationCurve curve;
        for (int i = 0; i < skeleton.Bones.Length; i++)
        {
            Bone bone = skeleton.Bones[i];
            if (bone.Translation.Sequence == loop)
            {
                curve = TranslationCurve(bone.Translation.Interpolation, bone.Translation.Timestamps[loop],
                    bone.Translation.Values[loop].Select(v => v.X).ToArray(), bones[i].transform.localPosition.x);
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalPosition.x", curve);
                }
                curve = TranslationCurve(bone.Translation.Interpolation, bone.Translation.Timestamps[loop],
                    bone.Translation.Values[loop].Select(v => v.Y).ToArray(), bones[i].transform.localPosition.y);
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalPosition.y", curve);
                }
                curve = TranslationCurve(bone.Translation.Interpolation, bone.Translation.Timestamps[loop],
                    bone.Translation.Values[loop].Select(v => v.Z).ToArray(), bones[i].transform.localPosition.z);
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalPosition.z", curve);
                }
            }
            if (bone.Rotation.Sequence == loop)
            {
                curve = RotationCurve(bone.Rotation.Interpolation, bone.Rotation.Timestamps[loop], bone.Rotation.Values[loop].Select(v => v.X).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalRotation.x", curve);
                }
                curve = RotationCurve(bone.Rotation.Interpolation, bone.Rotation.Timestamps[loop], bone.Rotation.Values[loop].Select(v => v.Y).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalRotation.y", curve);
                }
                curve = RotationCurve(bone.Rotation.Interpolation, bone.Rotation.Timestamps[loop], bone.Rotation.Values[loop].Select(v => v.Z).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalRotation.z", curve);
                }
                curve = RotationCurve(bone.Rotation.Interpolation, bone.Rotation.Timestamps[loop], bone.Rotation.Values[loop].Select(v => v.W).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalRotation.w", curve);
                }
            }
            if (bone.Scale.Sequence == loop)
            {
                curve = ScaleCurve(bone.Scale.Interpolation, bone.Scale.Timestamps[loop], bone.Scale.Values[loop].Select(v => v.X).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalScale.x", curve);
                }
                curve = ScaleCurve(bone.Scale.Interpolation, bone.Scale.Timestamps[loop], bone.Scale.Values[loop].Select(v => v.Y).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalScale.y", curve);
                }
                curve = ScaleCurve(bone.Scale.Interpolation, bone.Scale.Timestamps[loop], bone.Scale.Values[loop].Select(v => v.Z).ToArray());
                if (curve != null)
                {
                    clip.SetCurve(bonePaths[i], typeof(Transform), "m_LocalScale.z", curve);
                }
            }
        }
        return clip;
    }

    //Create curve for translation keys
    private AnimationCurve TranslationCurve(short interpolation, int[] timestamps, float[] values, float position)
    {
        AnimationCurve curve = null;
        List<Keyframe> keys;
        if (interpolation < 2)
        {
            keys = AddTranslationKeyframes(timestamps, values, position);
        }
        else
        {
            keys = AddTranslationKeyframes(timestamps, values, position, 2);
        }
        if (keys.Count > 0)
        {
            curve = new AnimationCurve(keys.ToArray());
            SetInterpolation(curve, interpolation);
        }
        return curve;
    }

    //Create curve for rotation keys
    private AnimationCurve RotationCurve(short interpolation, int[] timestamps, float[] values)
    {
        AnimationCurve curve = null;
        List<Keyframe> keys;
        if (interpolation < 2)
        {
            keys = AddRotationKeyframes(timestamps, values);
        }
        else
        {
            keys = AddRotationKeyframes(timestamps, values, 2);
        }
        if (keys.Count > 0)
        {
            curve = new AnimationCurve(keys.ToArray());
            SetInterpolation(curve, interpolation);
        }
        return curve;
    }

    //Create curve for scale keys
    private AnimationCurve ScaleCurve(short interpolation, int[] timestamps, float[] values)
    {
        AnimationCurve curve = null;
        List<Keyframe> keys;
        if (interpolation < 2)
        {
            keys = AddScaleKeyframes(timestamps, values);
        }
        else
        {
            keys = AddScaleKeyframes(timestamps, values, 2);
        }
        if (keys.Count > 0)
        {
            curve = new AnimationCurve(keys.ToArray());
            SetInterpolation(curve, interpolation);
        }
        return curve;
    }

    //Add translation keyframes
    private List<Keyframe> AddTranslationKeyframes(int[] timestamps, float[] values, float position, int step = 1)
    {
        List<Keyframe> keys = new();
        for (int j = 0; j < values.Length / step; j++)
        {
            if (j > 0 && timestamps[j * step] == 0)
            {
                continue;
            }
            if (values[j * step] == float.NaN)
            {
                values[j * step] = 0;
            }
            keys.Add(new Keyframe(timestamps[j] / 1000f, position + values[j * step]));
        }
        return keys;
    }

    //Add rotation keyframes
    private List<Keyframe> AddRotationKeyframes(int[] timestamps, float[] values, int step = 1)
    {
        List<Keyframe> keys = new();
        for (int j = 0; j < values.Length / step; j++)
        {
            if (j > 0 && timestamps[j * step] == 0)
            {
                continue;
            }
            if (values[j * step] == float.NaN)
            {
                values[j * step] = 0;
            }
            keys.Add(new Keyframe(timestamps[j] / 1000f, values[j * step]));
        }
        return keys;
    }

    //Add scale keyframes
    private List<Keyframe> AddScaleKeyframes(int[] timestamps, float[] values, int step = 1)
    {
        List<Keyframe> keys = new();
        for (int j = 0; j < values.Length / step; j++)
        {
            if (j > 0 && timestamps[j * step] == 0)
            {
                continue;
            }
            if (values[j * step] == float.NaN)
            {
                values[j * step] = 0;
            }
            keys.Add(new Keyframe(timestamps[j] / 1000f, values[j * step]));
        }
        return keys;
    }

    //Set proper interpolation mode for given curve
    private void SetInterpolation(AnimationCurve curve, short interpolation)
    {
        for (int j = 0; j < curve.length; j++)
        {
            switch (interpolation)
            {
                case 0:
                    AnimationUtility.SetKeyLeftTangentMode(curve, j, AnimationUtility.TangentMode.Constant);
                    AnimationUtility.SetKeyRightTangentMode(curve, j, AnimationUtility.TangentMode.Constant);
                    break;
                case 1:
                    AnimationUtility.SetKeyLeftTangentMode(curve, j, AnimationUtility.TangentMode.Linear);
                    AnimationUtility.SetKeyRightTangentMode(curve, j, AnimationUtility.TangentMode.Linear);
                    break;
                default:
                    AnimationUtility.SetKeyLeftTangentMode(curve, j, AnimationUtility.TangentMode.Auto);
                    AnimationUtility.SetKeyRightTangentMode(curve, j, AnimationUtility.TangentMode.Auto);
                    break;
            }
        }
    }
}
