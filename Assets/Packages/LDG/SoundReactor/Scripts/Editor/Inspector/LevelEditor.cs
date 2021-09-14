﻿// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;
using UnityEditor;

namespace LDG.SoundReactor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Level))]
    public class LevelEditor : Editor
    {
        static float[] fallingLevel = { 0 };
        SerializedProperty spectrumFilterProp;
        SerializedProperty linearizedFrequencyProp;
        SerializedProperty fallSpeedProp;
        SerializedProperty fallSpeedSourceProp;
        SerializedProperty inheritableProp;

        private void OnEnable()
        {
            spectrumFilterProp = serializedObject.FindProperty("spectrumFilter");
            linearizedFrequencyProp = serializedObject.FindProperty("linearizedFrequency");
            fallSpeedProp = serializedObject.FindProperty("fallSpeed");
            fallSpeedSourceProp = serializedObject.FindProperty("fallSpeedSource");
            inheritableProp = serializedObject.FindProperty("inheritable");
        }

        public override void OnInspectorGUI()
        {
            float newLinearizedFrequency;
            Level level = (Level)target;

            Frequency.SetBaseFrequency(level.frequencyMode);

            fallingLevel[0] = level.fallingLevel;
            newLinearizedFrequency = SpectrumGUILayout.SpectrumField(level.spectrumFilter, fallingLevel, level.linearizedFrequency, level.levelBeat);

            // temporarily convert to frequency. this way the serialized object will edit their real frequency instead of the normalized one.
            foreach (Level lvl in targets)
            {
                lvl.linearizedFrequency = Frequency.UnlinearizeFrequency(newLinearizedFrequency);
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(spectrumFilterProp, new GUIContent("Spectrum Filter", "SpectrumFilter to grab spectrum data from. If this is set to None, then it'll try to find a SpectrumFilter at runtime by looking up through the hierarchy."));
            EditorGUILayout.PropertyField(linearizedFrequencyProp, new GUIContent("Frequency (Hz)", "Frequency to track. Set the frequency directly here, or click in the frequency window above."));
            EditorGUILayout.PropertyField(fallSpeedSourceProp, new GUIContent("Fall Speed Source", "The fall speed source to use. Filter is global to all the levels that inherit Filter, and level is local to itself."));

            EditorGUI.indentLevel = 1;
            if (level.fallSpeedSource != FallSpeedSource.SpectrumFilter)
            {
                EditorGUILayout.PropertyField(fallSpeedProp, new GUIContent("Fall Speed", "The fall speed for this level."));
            }

            EditorGUI.indentLevel = 0;

            EditorGUILayout.PropertyField(inheritableProp, new GUIContent("Inheritable", "Tells a PropertyDriver if it can inherit this Level or not, unless the PropertyDriver is sharing the same GameObject as the Level, in which the Level will automatically be inherited."));

            serializedObject.ApplyModifiedProperties();

            float newFrequency = level.linearizedFrequency;

            // convert frequency back to linear
            foreach (Level lvl in targets)
            {
                lvl.linearizedFrequency = Mathf.Clamp(newFrequency, Frequency.lowerBaseFrequency, Frequency.upperBaseFrequency);
                lvl.linearizedFrequency = Frequency.LinearizeFrequency(lvl.linearizedFrequency);

                Undo.RecordObject(lvl, "Level");
            }

            if (Application.isPlaying || GUI.changed)
            {
                Repaint();
            }
        }
    }
}