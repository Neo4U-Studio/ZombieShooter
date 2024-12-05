using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZombieShooter
{
    public static class Utilities
    {
        public static string GROUND_TAG = "Ground";
        public static string PLAYER_TAG = "Player";
        public static string ITEM_TAG = "Item";
        public static string ZOMBIE_TAG = "Zombie";
        public static string RAGDOLL_TAG = "Ragdoll";
        public static string BULLET_TAG = "Bullet";

        public static string ZOMBIE_DETECT_RANGE_KEY = "Detect_Range";
        public static string ZOMBIE_ATTACK_RANGE_KEY = "Attack_Range";

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
    [Serializable]
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

    [Serializable]
    public enum eItemType
    {
        Empty = 0,
        MedKit,
        Ammo_Normal = 10,
    }

    [Serializable]
    public enum eZombieType
    { 
        Zombie_Normal, 
        Zombie_Archer, 
        Zombie_Boss,
    };

    [Serializable]
    public enum eZombieAction
    { 
        Idle, 
        Wander, 
        Attack, 
        Chase, 
        Dead 
    };
}