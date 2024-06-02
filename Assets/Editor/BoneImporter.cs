using M2Lib;
using SkelLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Unity.VersionControl.Git.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

//Importer for bone files
[ScriptedImporter(1, "bone")]
public class BoneImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        //Load file contents
        BoneLib.Bone file = new();
        file.LoadFile(ctx.assetPath);
        //Load skel file for reference
        M2 model = new();
        model.LoadFile($"{ctx.assetPath[..^8]}.m2");
        Skel skeleton = model.Skeleton;
        //Create empty animation clip
        AnimationClip clip = new();
        Matrix4x4 matrix;
        Vector4[] columns = new Vector4[4];
        //Generate temporary bones
        Transform[] bones = new Transform[skeleton.Bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i] = new GameObject($"Bone{i}").transform;
            bones[i].position = new Vector3(skeleton.Bones[i].Pivot.X, skeleton.Bones[i].Pivot.Y, skeleton.Bones[i].Pivot.Z);
        }
        GameObject rig = new("Skeleton");
        for (int i = 0; i < bones.Length; i++)
        {
            if (skeleton.Bones[i].Parent == -1)
            {
                bones[i].parent = rig.transform;
            }
            else
            {
                bones[i].parent = bones[skeleton.Bones[i].Parent];
            }
        }
        //Fill animation data for each bone
        List<short> blacklist = new();
        for (int i = 0; i < file.Bones.Length; i++)
        {
            string path = $"{GetBonePath(skeleton.Bones[file.Bones[i]].Parent, skeleton.Bones)}Bone{file.Bones[i]}";
            if (blacklist.Exists(b => path.Contains(b.ToString())))
            {
                continue;
            }
            for (int j = 0; j < 4; j++)
            {
                columns[j] = new Vector4(file.Transformations[i][j][0], file.Transformations[i][j][1], file.Transformations[i][j][2], file.Transformations[i][j][3]);
            }
            matrix = new Matrix4x4(columns[0], columns[1], columns[2], columns[3]);
            Vector3 translation = new(matrix.m03, matrix.m13, matrix.m23);
            if (translation.x == 0 && translation.y == 0 && translation.z == 0)
            {
                blacklist.Add(file.Bones[i]);
                continue;
            }
            translation.x += bones[file.Bones[i]].localPosition.x;
            translation.y += bones[file.Bones[i]].localPosition.y;
            translation.z += bones[file.Bones[i]].localPosition.z;
            AddCurve(clip, translation, "localPosition", path);
            UnityEngine.Quaternion rotation = matrix.rotation;
            Vector3 angles = rotation.eulerAngles;
            AddCurve(clip, angles, "localEulerAngles", path);
            Vector3 scale = matrix.lossyScale;
            AddCurve(clip, scale, "localScale", path);
        }
        //Clear temporary bones and add animation clip to the asset
        DestroyImmediate(rig);
        ctx.AddObjectToAsset(Path.GetFileNameWithoutExtension(ctx.assetPath.Replace("/", "\\")), clip);
    }

    //Recursively get full path to the bone
    private string GetBonePath(short bone, Bone[] bones)
    {
        if (bones[bone].Parent == -1)
        {
            return "Skeleton/Bone0/";
        }
        else
        {
            return $"{GetBonePath(bones[bone].Parent, bones)}Bone{bone}/";
        }
    }

    private void AddCurve(AnimationClip clip, Vector3 value, string name, string path)
    {
        AnimationCurve curve = AnimationCurve.Linear(0f, value.x, 0.1f, value.x);
        clip.SetCurve(path, typeof(Transform), $"{name}.x", curve);
        curve = AnimationCurve.Linear(0f, value.y, 0.1f, value.y);
        clip.SetCurve(path, typeof(Transform), $"{name}.y", curve);
        curve = AnimationCurve.Linear(0f, value.z, 0.1f, value.z);
        clip.SetCurve(path, typeof(Transform), $"{name}.z", curve);
    }
}