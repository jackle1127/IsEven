using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Jack.IsEven
{
    public class ScrewColliderGenerator : MonoBehaviour
    {
        public float size = 0.001f;
        public float winds = 1f;
        public int windResolution = 30;
        public float windStartOffset = 0;
        public bool invertWindDirection = false;
        public AnimationCurve radiusCurve = AnimationCurve.EaseInOut(0, .1f, 1, .1f);
        public PhysicMaterial physicMaterial;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScrewColliderGenerator))]
    public class ScrewColliderGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ScrewColliderGenerator screwColliderGenerator = target as ScrewColliderGenerator;
            EditorGUI.BeginChangeCheck();

            screwColliderGenerator.size = Mathf.Max(.0001f, EditorGUILayout.FloatField("Size", screwColliderGenerator.size));
            screwColliderGenerator.winds = Mathf.Max(.5f, EditorGUILayout.FloatField("Winds", screwColliderGenerator.winds));
            screwColliderGenerator.windResolution = Mathf.Max(1, EditorGUILayout.IntField("Wind Resolution", screwColliderGenerator.windResolution));
            screwColliderGenerator.windStartOffset = EditorGUILayout.FloatField("Wind Start Offset", screwColliderGenerator.windStartOffset);
            screwColliderGenerator.invertWindDirection = EditorGUILayout.Toggle("Invert Wind Direction", screwColliderGenerator.invertWindDirection);
            screwColliderGenerator.radiusCurve = EditorGUILayout.CurveField("Radius", screwColliderGenerator.radiusCurve);
            screwColliderGenerator.physicMaterial = EditorGUILayout.ObjectField(screwColliderGenerator.physicMaterial, typeof(PhysicMaterial), false) as PhysicMaterial;

            if (GUILayout.Button("Clear Children"))
            {
                ClearChildren(screwColliderGenerator);
            }

            if (GUILayout.Button("Generate Colliders"))
            {
                GenerateColliders(screwColliderGenerator);
            }

            if (EditorGUI.EndChangeCheck())
            {
                GenerateColliders(screwColliderGenerator);
            }
        }

        private void ClearChildren(ScrewColliderGenerator screwColliderGenerator)
        {
            List<GameObject> children = new List<GameObject>();

            foreach (Transform child in screwColliderGenerator.transform)
            {
                children.Add(child.gameObject);
            }

            foreach (GameObject child in children)
            {
                DestroyImmediate(child);
            }
        }

        private void GenerateColliders(ScrewColliderGenerator screwColliderGenerator)
        {
            ClearChildren(screwColliderGenerator);

            int totalColliders = Mathf.FloorToInt(screwColliderGenerator.windResolution * screwColliderGenerator.winds);
            for (float i = 0; i < totalColliders; i++)
            {
                float progress = i / (totalColliders - 1);
                float angle = (screwColliderGenerator.invertWindDirection ? -1 : 1) * (-360 * (progress * screwColliderGenerator.winds + screwColliderGenerator.windStartOffset));
                float radius = screwColliderGenerator.radiusCurve.Evaluate(progress) / 1000f;

                CreateCollider(screwColliderGenerator, screwColliderGenerator.size, radius, angle, progress * screwColliderGenerator.winds, screwColliderGenerator.transform);
            }
        }

        private void CreateCollider(ScrewColliderGenerator screwColliderGenerator, float size, float radius, float angle, float currentWind, Transform parent)
        {
            GameObject colliderObject = new GameObject("Collider", typeof(BoxCollider));
            BoxCollider collider = colliderObject.GetComponent<BoxCollider>();
            collider.size = Vector3.one * size;
            collider.material = screwColliderGenerator.physicMaterial;

            float windHeight = 1.41421356237f * size;
            // sqrt(2) = 1.41421356237
            float height = windHeight * currentWind;

            Transform colliderTransform = colliderObject.transform;
            colliderTransform.SetParent(parent, false);

            Quaternion angleAroundScrew = Quaternion.AngleAxis(angle, Vector3.up);

            colliderTransform.localRotation =
                angleAroundScrew *
                // The higher the resolution, the less steep the cube collider pitches in the z axis
                Quaternion.AngleAxis((screwColliderGenerator.invertWindDirection ? -1 : 1) * -Mathf.Atan(windHeight / (2 * Mathf.PI * radius)) * Mathf.Rad2Deg, Vector3.forward) *
                Quaternion.AngleAxis(45, Vector3.right);

            colliderTransform.localPosition = height * Vector3.up
                + angleAroundScrew * (Vector3.forward * radius);
        }
    }
#endif
}