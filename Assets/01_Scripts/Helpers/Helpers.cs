using UnityEngine;

public static class Extend_TransformHelper
{
    public static Transform FindChildByName(this Transform transform, string childName)
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.name.Equals(childName))
            {
                return child;
            }
        }

        return null;
    }
}

#if UNITY_EDITOR
public static class DirectoryHelpers
{
    public static void ToRelativePath(ref string absolutePath)
    {
        int startIndex = absolutePath.IndexOf("/Assets/");

        Debug.Assert(startIndex > 0, "올바른 에셋 경로가 아닙니다.");

        absolutePath = absolutePath.Substring(startIndex + 1, absolutePath.Length - startIndex - 1);
    }
}

public static class FileHelpers
{
    public static string GetFileName(string assetPath)
    {
        Debug.Assert(assetPath.Length > 0, "올바른 에셋 경로가 아닙니다.");

        int endIndex = assetPath.LastIndexOf('/');

        return assetPath.Substring(endIndex + 1, assetPath.Length - endIndex - 1);
    }
}
#endif
