using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZSCompassUI : MonoBehaviour
{
    [SerializeField] GameObject pointer;
    [SerializeField] RectTransform compassLine;
    RectTransform rect;

    private GameObject player;
    private GameObject target;

    private bool isScanning = false;

    void Awake()
    {
        rect = pointer.GetComponent<RectTransform>();
        isScanning = false;
        this.gameObject.SetActive(false);
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    void Update()
    {
        if (player == null || target == null || !isScanning)
        {
            ToggleCompass(false);
            return;
        }

        Vector3[] corners = new Vector3[4];
        compassLine.GetLocalCorners(corners);
        float pointerScale = Vector3.Distance(corners[1], corners[2]);
        Vector3 direction = target.transform.position - player.transform.position;
        float angleToTarget = Vector3.SignedAngle(player.transform.forward, direction, player.transform.up);
        angleToTarget = Mathf.Clamp(angleToTarget, -90, 90) / 180.0f * pointerScale;
        rect.localPosition = new Vector3(angleToTarget, rect.localPosition.y, rect.localPosition.z);
    }

    public void ToggleCompass(bool toggle)
    {
        isScanning = toggle;
        this.gameObject.SetActive(toggle);
    }
}
