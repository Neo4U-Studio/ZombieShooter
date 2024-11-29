using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZombieShooter
{
    public static class Utilities
    {
        public static IEnumerator DelayAction(float delayTime = 1f, Action onComplete = null)
        {
            yield return DelayUtils.Wait(delayTime);
            onComplete?.Invoke();
        }

        public static IEnumerator WaitEndOfFrame(Action onComplete = null)
        {
            yield return DelayUtils.WAIT_END_OF_FRAME;
            onComplete?.Invoke();
        }
    }

#if UNITY_EDITOR
    [System.Serializable]
    public class GameHeader
    {
        public string header;
    }
    [CustomPropertyDrawer(typeof(GameHeader))]
    public class GameHeaderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.LabelField(position, property.FindPropertyRelative("header").stringValue, new GUIStyle()
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color32(76, 201, 240, 255) }
            });
        }
    }
#endif
}