// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CustomEditor(typeof(SpectrumBuilder))]
    public class SpectrumBuilderEditor : Editor
    {
        static int[] frequencyRangeOptions =
        {
            (int)FrequencyRangeOption.Custom,
            (int)FrequencyRangeOption.FullRange,
            (int)FrequencyRangeOption.Bass,
            (int)FrequencyRangeOption.LowMidrange,
            (int)FrequencyRangeOption.Midrange,
            (int)FrequencyRangeOption.HighMidrange,
            (int)FrequencyRangeOption.Presence,
            (int)FrequencyRangeOption.Brilliance,
        };

        static GUIContent[] frequencyRangeOptionStrings = new GUIContent[]
        {
            new GUIContent("Custom"),
            new GUIContent("Full Range"),
            new GUIContent("Bass"),
            new GUIContent("Low Midrange"),
            new GUIContent("Midrange"),
            new GUIContent("High Midrange"),
            new GUIContent("Presence"),
            new GUIContent("Brilliance"),
        };
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            float lower = 20;
            float upper = 20000;
            bool buildEnabled = true;

            SpectrumBuilder builder = (SpectrumBuilder)target;

            EditorGUILayout.Space();

#if UNITY_2018_2_OR_NEWER
            if (PrefabUtility.GetPrefabAssetType(builder) != PrefabAssetType.NotAPrefab)
#else
            if (PrefabUtility.GetPrefabType(builder) != PrefabType.None)
#endif
            {
                EditorGUILayout.HelpBox("Building disabled on prefabs.", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField("Level", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            builder.segmentMode = (SegmentMode)EditorGUILayout.EnumPopup(new GUIContent("Mode", "Creates a shape that is made up of game objects, or a vector."), builder.segmentMode);

            if (builder.segmentMode == SegmentMode.Vector)
            {
                builder.shape = (ShapeMode)Mathf.Min((int)builder.shape, (int)VectorShapeMode.Circle);
            }

            builder.levelSize = EditorGUILayout.Vector3Field("Size", builder.levelSize);

            switch (builder.segmentMode)
            {
                case SegmentMode.Object:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("levelInstanceList"), new GUIContent("Levels", ""), true);

                    buildEnabled = (builder.levelInstanceList.Count > 0);

                    foreach(GameObject level in builder.levelInstanceList)
                    {
                        if(level == null)
                        {
                            buildEnabled = false;
                            break;
                        }
                    }

                    builder.shareDriver = EditorGUILayout.Toggle(new GUIContent("Share Driver", "Tells the builder to attach the PropertyDrivers from the Level to the instanced Levels."), builder.shareDriver);
                    break;

                case SegmentMode.Vector:
                    //if (builder.vectorShape != null)
                    {
                        builder.colorDriver = (ColorDriver)EditorGUILayout.ObjectField(new GUIContent("Color Driver", ""), builder.colorDriver, typeof(ColorDriver), true);

                        if (!builder.colorDriver)
                        {
                            EditorGUILayout.HelpBox("A ColorDriver must be attached.", MessageType.Warning);
                            buildEnabled = false;
                        }

                        builder.vectorMaterial = (Material)EditorGUILayout.ObjectField("Material", builder.vectorMaterial, typeof(Material), true);

                        if (!builder.vectorMaterial)
                        {
                            EditorGUILayout.HelpBox("A Material must be attached.", MessageType.Warning);
                            buildEnabled = false;
                        }

                        builder.travel = EditorGUILayout.FloatField(new GUIContent("Travel", ""), builder.travel);
                        //builder.closeCurve = EditorGUILayout.Toggle(new GUIContent("Close Curve", ""), builder.closeCurve);
                        builder.vectorAnchored = EditorGUILayout.Toggle(new GUIContent("Anchored", "Anchor the bottom or inside edge of the shape."), builder.vectorAnchored);

                        if (builder.vectorAnchored && builder.shape == ShapeMode.Circle)
                        {
                            builder.vectorAnchoredDiameter = EditorGUILayout.FloatField(new GUIContent("Anchored Diam.", "Diameter of the inside edge of the vector."), builder.vectorAnchoredDiameter);
                        }
                    }
                    break;
            }
            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("Layout", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;

            if (builder.segmentMode == SegmentMode.Object)
            {
                builder.shape = (ShapeMode)EditorGUILayout.EnumPopup(new GUIContent("Shape", "The shape to arrange the Levels into."), builder.shape);
            }
            else
            {
                builder.shape = (ShapeMode)EditorGUILayout.EnumPopup(new GUIContent("Shape", "The shape to arrange the Levels into."), (VectorShapeMode)builder.shape);
            }

            //if (builder.spacingFoldout)
            {
                builder.spacingMode = (SpacingMode)EditorGUILayout.EnumPopup(new GUIContent("Spacing Mode", "How the Levels should be spaced. Levels can either be spaced evenly apart from each other, or the Layout Size can be divided up into levels."), builder.spacingMode);

                switch (builder.shape)
                {
                    case ShapeMode.SegmentedLevels:
                        builder.numColumns = Mathf.Max(1, EditorGUILayout.IntField("Columns", builder.numColumns));
                        builder.numRows = Mathf.Max(1, EditorGUILayout.IntField("Rows", builder.numRows));

                        if (builder.spacingMode == SpacingMode.Divided)
                        {
                            builder.layoutSize = EditorGUILayout.Vector2Field("Layout Size", builder.layoutSize);
                        }

                        if (builder.spacingMode == SpacingMode.Spaced)
                        {
                            builder.levelSpacing = EditorGUILayout.Vector2Field("Spacing", builder.levelSpacing);
                        }

                        break;
                    case ShapeMode.Rectangle:
                        builder.fromTexture = EditorGUILayout.Toggle(new GUIContent("From Texture", "Use the size of the texture, and the frequency from the red channel."), builder.fromTexture);

                        if (builder.fromTexture)
                        {
                            builder.texture = (Texture2D)EditorGUILayout.ObjectField("Texture", builder.texture, typeof(Texture2D), true);

                            if (builder.spacingMode == SpacingMode.Divided)
                            {
                                builder.layoutSize = EditorGUILayout.Vector2Field("Layout Size", builder.layoutSize);
                            }

                            if (builder.spacingMode == SpacingMode.Spaced)
                            {
                                builder.levelSpacing = EditorGUILayout.Vector2Field("Spacing", builder.levelSpacing);
                            }
                        }
                        else
                        {
                            builder.numColumns = Mathf.Max(1, EditorGUILayout.IntField("Columns", builder.numColumns));
                            builder.numRows = Mathf.Max(1, EditorGUILayout.IntField("Rows", builder.numRows));

                            if (builder.spacingMode == SpacingMode.Divided)
                            {
                                builder.layoutSize = EditorGUILayout.Vector2Field("Layout Size", builder.layoutSize);
                            }

                            if (builder.spacingMode == SpacingMode.Spaced)
                            {
                                builder.levelSpacing = EditorGUILayout.Vector2Field("Spacing", builder.levelSpacing);
                            }
                        }

                        break;

                    default:
                        builder.numColumns = Mathf.Max(2, EditorGUILayout.IntField("Levels", builder.numColumns));

                        if (builder.spacingMode == SpacingMode.Divided)
                        {
                            builder.layoutSize.x = EditorGUILayout.FloatField("Layout Size", builder.layoutSize.x);
                        }

                        if (builder.spacingMode == SpacingMode.Spaced)
                        {
                            builder.levelSpacing.x = EditorGUILayout.FloatField("Spacing", builder.levelSpacing.x);
                        }
                        break;
                }
            }

            if (builder.spacingMode == SpacingMode.Divided)
            {
                builder.fit = EditorGUILayout.Toggle(new GUIContent("Fit Inside", "Divide the layout so that the Level edges are tangent to the Layout Size. Otherwise just divide the layout as if the Level Size were zero."), builder.fit);
            }
            else
            {
                builder.betweenEdges = EditorGUILayout.Toggle(new GUIContent("Between Edges", "Add space between Level edges. Otherwise add space between Level centers."), builder.betweenEdges);
            }

            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("Frequency Range", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;

            //if (builder.bandwidthFoldout)
            {
                builder.frequencyRangeOption = (FrequencyRangeOption)EditorGUILayout.IntPopup(new GUIContent("Preset"), (int)builder.frequencyRangeOption, frequencyRangeOptionStrings, frequencyRangeOptions);

                if (builder.frequencyRangeOption == FrequencyRangeOption.Custom)
                {
                    lower = builder.frequencyLower;
                    upper = builder.frequencyUpper;
                }
                else
                {
                    Frequency.GetRangePreset(out lower, out upper, (int)builder.frequencyRangeOption, FrequencyBase.Audio);
                }

                EditorGUI.BeginChangeCheck();

                float lowerFrequency;
                float upperFrequency;

                Frequency.GetRange(FrequencyBase.Audio, out lowerFrequency, out upperFrequency);

                builder.frequencyLower = Mathf.Min(EditorGUILayout.Slider("Lower (Hz)", lower, lowerFrequency, upperFrequency), upper);
                builder.frequencyUpper = Mathf.Max(EditorGUILayout.Slider("Upper (Hz)", upper, lowerFrequency, upperFrequency), lower);
                
                if (EditorGUI.EndChangeCheck())
                {
                    builder.frequencyRangeOption = FrequencyRangeOption.Custom;
                }
            }

            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("Transformation", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;

            //if (builder.transformFoldout)
            {
                builder.clamp = EditorGUILayout.Toggle(new GUIContent("Clamp", "Keeps last level from becoming the first level."), builder.clamp);
                GUI.enabled = !builder.clamp;
                builder.transformRepeat = EditorGUILayout.FloatField(new GUIContent("Repeat", "The number of times to repeat the frequency range along the shape."), builder.transformRepeat);
                GUI.enabled = true;
                builder.transformAlternate = EditorGUILayout.Toggle(new GUIContent("Alternate", "Tells the frequency to reverse the frequency every time it repeats."), builder.transformAlternate);
                builder.transformReverse = EditorGUILayout.Toggle(new GUIContent("Reverse", "Causes the levels to be assigned frequencies starting with the highest frequency first."), builder.transformReverse);

                if (builder.shape == ShapeMode.SegmentedLevels)
                {
                    builder.transformFlipLevel = EditorGUILayout.Toggle(new GUIContent("Flip Level", "Causes the levels to display upside down. Only works with segmented levels."), builder.transformFlipLevel);
                }
            }

            EditorGUI.indentLevel = 0;

            EditorGUILayout.LabelField("Build", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;

            GUI.enabled = buildEnabled;

            builder.autoBuild = EditorGUILayout.Toggle("Auto Build", builder.autoBuild);

            if (builder.autoBuild && GUI.changed)
            {
                builder.Build();
                EditorUtility.SetDirty(target);
            }
            else if (GUILayout.Button("Build"))
            {
                builder.Build();
                EditorUtility.SetDirty(target);
            }
            EditorGUI.indentLevel = 0;

            serializedObject.ApplyModifiedProperties();

            Undo.RecordObject(builder, "Spectrum Builder");
        }
    }
}