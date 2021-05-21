using M2Lib;
using SkelLib;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

//Importer for bone files
[ScriptedImporter(1, "bone")]
public class BoneImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        //Load file contents
        BoneLib.Bone file = new BoneLib.Bone();
        file.LoadFile(ctx.assetPath);
        //Load skel file for reference
        M2 model = new M2();
        model.LoadFile(ctx.assetPath.Substring(0, ctx.assetPath.Length - 8) + ".m2");
        Skel skeleton = model.Skeleton;
        //Create empty animation clip
        AnimationClip clip = new AnimationClip();
        AnimationCurve curve;
        Matrix4x4 matrix;
        Vector4[] columns = new Vector4[4];
        float value;
        //Generate temporary bones
        Transform[] bones = new Transform[skeleton.Bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i] = new GameObject("Bone" + i).transform;
            bones[i].position = new Vector3(-skeleton.Bones[i].Pivot.X / 2, skeleton.Bones[i].Pivot.Z / 2, -skeleton.Bones[i].Pivot.Y / 2);
        }
        GameObject rig = new GameObject("Skeleton");
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
        for (int i = 0; i < file.Bones.Length; i++)
        {
            string path = GetBonePath(skeleton.Bones[file.Bones[i]].Parent, skeleton.Bones) + "Bone" + file.Bones[i];
            for (int j = 0; j < 4; j++)
            {
                columns[j] = new Vector4(file.Transformations[i][j][0], file.Transformations[i][j][1], file.Transformations[i][j][2], file.Transformations[i][j][3]);
            }
            matrix = new Matrix4x4(columns[0], columns[1], columns[2], columns[3]);
            value = -matrix.m03 / 2 + bones[file.Bones[i]].localPosition.x;
            curve = AnimationCurve.Linear(0f, value, 0.1f, value);
            clip.SetCurve(path, typeof(Transform), "localPosition.x", curve);
            value = matrix.m23 / 2 + bones[file.Bones[i]].localPosition.y;
            curve = AnimationCurve.Linear(0f, value, 0.1f, value);
            clip.SetCurve(path, typeof(Transform), "localPosition.y", curve);
            value = -matrix.m13 / 2 + bones[file.Bones[i]].localPosition.z;
            curve = AnimationCurve.Linear(0f, value, 0.1f, value);
            clip.SetCurve(path, typeof(Transform), "localPosition.z", curve);
            UnityEngine.Quaternion rotation = matrix.rotation;
            curve = AnimationCurve.Linear(0f, -rotation.x, 1f, rotation.x);
            clip.SetCurve(path, typeof(Transform), "localRotation.x", curve);
            curve = AnimationCurve.Linear(0f, -rotation.z, 0.1f, rotation.z);
            clip.SetCurve(path, typeof(Transform), "localRotation.y", curve);
            curve = AnimationCurve.Linear(0f, rotation.y, 0.1f, rotation.y);
            clip.SetCurve(path, typeof(Transform), "localRotation.z", curve);
            curve = AnimationCurve.Linear(0f, rotation.w, 0.1f, rotation.w);
            clip.SetCurve(path, typeof(Transform), "localRotation.w", curve);
            Vector3 scale = matrix.lossyScale;
            curve = AnimationCurve.Linear(0f, scale.x, 0.1f, scale.x);
            clip.SetCurve(path, typeof(Transform), "localScale.x", curve);
            curve = AnimationCurve.Linear(0f, scale.z, 0.1f, scale.z);
            clip.SetCurve(path, typeof(Transform), "localScale.y", curve);
            curve = AnimationCurve.Linear(0f, scale.y, 0.1f, scale.y);
            clip.SetCurve(path, typeof(Transform), "localScale.z", curve);
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
            return GetBonePath(bones[bone].Parent, bones) + "Bone" + bone + "/";
        }
    }
}