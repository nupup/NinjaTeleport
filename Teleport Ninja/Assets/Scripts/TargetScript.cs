using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{

    AttackMoveController warp;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    public Material highlightedColor;
    private Material usualColor;

    void Start()
    {
        warp = FindObjectOfType<AttackMoveController>();
        skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        usualColor = skinnedMeshRenderer.material;
    }

    private void OnBecameVisible()
    {
        if (!warp.screenTargets.Contains(transform))
            warp.screenTargets.Add(transform);
    }

    private void OnBecameInvisible()
    {
        if(warp.screenTargets.Contains(transform))
            warp.screenTargets.Remove(transform);
    }

    public void HighLight()
    {
        if (GameManager.Instance.State != GameState.Killing)
            skinnedMeshRenderer.material = highlightedColor;
    }
    public void UnHighlight()
    {
        if (GameManager.Instance.State != GameState.Killing)
            skinnedMeshRenderer.material = usualColor;
    }
}
