using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SpecGlossCombiner : EditorWindow
{
    private Texture2D _specMap;
    private Texture2D _glossMap;
    private bool _roughMap;


    [MenuItem("Tools/Specular Gloss Map combiner")]
    public static void GetWindow()
    {
        GetWindow<SpecGlossCombiner>("Specular Gloss Map combiner");
    }

    private void OnGUI()
    {
        _specMap = EditorGUILayout.ObjectField("Specular Map", _specMap, typeof(Texture2D), false) as Texture2D;
        _glossMap = EditorGUILayout.ObjectField((_roughMap ? "Roughness" : "Gloss") + " Map", _glossMap, typeof(Texture2D), false) as Texture2D;
        _roughMap = EditorGUILayout.Toggle("Roughness Map?", _roughMap);

        if (GUILayout.Button("Combine"))
        {
            if (_specMap && _glossMap)
            {
                CombineSpecGlossMaps();
            }
        }
    }

    private void CombineSpecGlossMaps()
    {
        Material combiningMaterial = new Material(Shader.Find("Hidden/SpecGlossCombiner"));
        if (combiningMaterial.shader)
        {
            combiningMaterial.SetTexture("_SpecMap", _specMap);
            combiningMaterial.SetTexture("_GlossMap", _glossMap);
            combiningMaterial.SetFloat("_RoughMode", _roughMap ? 1 : 0);

            RenderTexture temp = RenderTexture.GetTemporary(_specMap.width, _specMap.height, 24, RenderTextureFormat.ARGB32);
            Graphics.Blit(null, temp, combiningMaterial);
            Texture2D combinedMap = new Texture2D(_specMap.width, _specMap.height, TextureFormat.ARGB32, true);
            RenderTexture.active = temp;
            combinedMap.ReadPixels(new Rect(0, 0, _specMap.width, _specMap.height), 0, 0, true);

            // Use the same path as the spec map file.
            // Name the new file <spec_map_name>_GLOSS.png
            string fullPath = AssetDatabase.GetAssetPath(_specMap);
            string parentPath = Path.GetDirectoryName(fullPath);
            string combinedFileName = parentPath + "/" +
                Path.GetFileNameWithoutExtension(fullPath) + "_GLOSS.png";

            File.WriteAllBytes(combinedFileName, combinedMap.EncodeToPNG());

            AssetDatabase.ImportAsset(combinedFileName);
        }
        else
        {
            Debug.LogError("SpecGlossCombiner shader not found!");
        }
    }
}
