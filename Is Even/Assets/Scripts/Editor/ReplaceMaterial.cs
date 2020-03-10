using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReplaceMaterial : EditorWindow
{
    private Material _materialToReplace;
    private Material _replacementMaterial;

    [MenuItem("F45/Tools/Replace Material")]
    public static void GetWindow()
    {
        GetWindow<ReplaceMaterial>("Replace Material");
    }

    private void OnGUI()
    {
        GUILayout.Label("This utility looks for a specific material in the selected MeshRenderer and replace it with another.");
        _materialToReplace = EditorGUILayout.ObjectField("Material to replace", _materialToReplace,
            typeof(Material), false) as Material;
        _replacementMaterial = EditorGUILayout.ObjectField("Replacement material", _replacementMaterial,
            typeof(Material), false) as Material;

        if (GUILayout.Button("Replace"))
        {
            foreach (Object selected in Selection.objects)
            {
                if (selected is GameObject)
                {
                    MeshRenderer meshRenderer = ((GameObject)selected).GetComponent<MeshRenderer>();
                    if (meshRenderer)
                    {
                        Material[] materials = meshRenderer.sharedMaterials;
                        for (int i = 0; i < materials.Length; i++)
                        {
                            if (materials[i] == _materialToReplace)
                            {
                                Undo.RecordObject(meshRenderer, "Material swap");
                                Debug.Log(selected + ": Replacing " + meshRenderer.sharedMaterials[i]
                                    + " with " + _replacementMaterial);
                                materials[i] = _replacementMaterial;
                                EditorUtility.SetDirty(meshRenderer);
                            }
                        }
                        meshRenderer.sharedMaterials = materials;
                    }
                }
            }
        }
    }
}
