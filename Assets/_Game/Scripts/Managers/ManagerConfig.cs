using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Configs/ManagerConfig")]
public class ManagerConfig : ScriptableObject
{
    [SerializeField]
    public List<GameObject> managerList;
}
