using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RDPolarity.Editor
{
    /// <summary>
    /// Selects all objects in the scene hierarchy with a missing script
    /// </summary>
    public class SelectEmpties : UnityEditor.Editor {    
        [MenuItem("MyMenu/SelectMissing")]
        static void SelectMissing(MenuCommand command)
        {
            Transform[] ts = FindObjectsOfType<Transform>();
            List<GameObject> selection = new List<GameObject>();
            foreach(Transform t in ts)
            {
                Component[] cs = t.gameObject.GetComponents<Component>();
                foreach(Component c in cs)
                {
                    if (c == null)
                    {
                        selection.Add(t.gameObject);
                    }
                }
            }
            Selection.objects = selection.ToArray();
        }
    }
} 