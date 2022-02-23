using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;
using System;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.EventSystems;

public class AttackMoveController : MonoBehaviour
{
    GameObject enemyToKill;
    private Animator anim;
    private float distanceToQuickSlash = 6;
    private CameraController cameraController;
    private ClickDetection clickDetector;

    private bool isLocked;
    public bool canSwitchTarget;

    public CinemachineVirtualCamera cameraFreeLook;
    private CinemachineImpulseSource impulse;
    private PostProcessVolume postVolume;
    private PostProcessProfile postProfile;

    [Space]

    public List<Transform> screenTargets = new List<Transform>();
    private Transform oldTarget;
    public Transform target;
    public float warpDuration = .5f;

    [Space]

    public Transform sword;
    public Transform swordHand;
    private Vector3 swordOrigRot;
    private Vector3 swordOrigPos;
    private MeshRenderer swordMesh;

    [Space]
    public Material glowMaterial;

    [Space]

    [Header("Particles")]
    public ParticleSystem blueTrail;
    public ParticleSystem whiteTrail;
    public ParticleSystem swordParticle;

    [Space]

    [Header("Prefabs")]
    public GameObject hitParticle;

    [Space]

    [Header("Canvas")]
    public Vector2 uiOffset;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        clickDetector = GameObject.Find("ClickDetector").GetComponent<ClickDetection>();
        anim = GetComponent<Animator>();
        impulse = cameraFreeLook.GetComponent<CinemachineImpulseSource>();
        postVolume = Camera.main.GetComponent<PostProcessVolume>();
        swordOrigRot = sword.localEulerAngles;
        swordOrigPos = sword.localPosition;
        swordMesh = sword.GetComponentInChildren<MeshRenderer>();
        swordMesh.enabled = false;

        //Cursor.lockState = CursorLockMode.Locked;
    }
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }
    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }
    private void GameManagerOnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Aiming:
                transform.DOLookAt(target.transform.position, 0.02f);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = false;
        }

        target = screenTargets[targetIndex()];

        //only swithc target if user is controlling the aim
        if (Input.touchCount > 0 && GameManager.Instance.State == GameState.Aiming)
        {
            //Make sure the touch is not over a UI element
            foreach (Touch touch in Input.touches)
            {
                int id = touch.fingerId;
                if (!EventSystem.current.IsPointerOverGameObject(id))
                {
                     SwitchTarget(target);
                }
            }
        }

        if (GameManager.Instance.State == GameState.Killing)
            transform.DOLookAt(enemyToKill.transform.position, 0);

        if (GameManager.Instance.State == GameState.Aiming)
            return;

        //Melee attack
        if (IsTargetTooClose(2) && !isLocked)
            Kill();
    }

    public void Kill()
    {
        //dont switch between enemies
        isLocked = true;
        //Fix rotation
        enemyToKill = target.gameObject;

        swordParticle.Play();
        swordMesh.enabled = true;
        bool isTargetTooClose = IsTargetTooClose(distanceToQuickSlash);
        if (isTargetTooClose)
            anim.SetTrigger("quickSlash");
        else
            anim.SetTrigger("slash");

        //rotate toward starget
        transform.DOLookAt(new Vector3(enemyToKill.transform.position.x, transform.position.y, enemyToKill.transform.position.z), 0.2f);
    }

    private bool IsTargetVisible()
    {
        RaycastHit rayHit;
        Ray rayTest = new Ray(transform.position, transform.position - target.transform.position);

        if (Physics.Raycast(rayTest, out rayHit))
        {
            if (rayHit.collider.gameObject.tag == "Obstacle")
            {
                Debug.Log("wall");
                return false;
            }
            else
            {
                Debug.Log("no wall");
                return true;
            }
        }
        return true;
    }

    private void SwitchTarget(Transform currentTarget)
    {
        //if new target is selected
        if (currentTarget != oldTarget)
        {
            Debug.Log("new target selected");
            oldTarget = currentTarget;
            HighlightTarget();
        }
    }


    private void HighlightTarget()
    {
        foreach (Transform t in screenTargets)
        {
            t.GetComponent<TargetScript>().UnHighlight();
        }
        target.GetComponent<TargetScript>().HighLight();
    }


    public void Slash()
    {
        //rotate towards starget
        transform.DOLookAt(new Vector3(enemyToKill.transform.position.x, transform.position.y, enemyToKill.transform.position.z), 0.1f);

        sword.parent = null;
        sword.DOMove(enemyToKill.transform.position, warpDuration / 1.2f).OnComplete(()=> DoneQuickSlash());
        sword.DOLookAt(enemyToKill.transform.position, .2f, AxisConstraint.None);
        //Particles
        blueTrail.Play();
        whiteTrail.Play();

        //Lens Distortion
        DOVirtual.Float(0, -80, .2f, DistortionAmount);
        DOVirtual.Float(1, 2f, .2f, ScaleAmount);
    }

    public void Warp()
    {
        //rotate towards starget
        transform.DOLookAt(new Vector3(enemyToKill.transform.position.x, transform.position.y, enemyToKill.transform.position.z), 0.1f);

        ShowBody(false);
        anim.speed = 0;

        transform.DOMove(enemyToKill.transform.position, warpDuration).SetEase(Ease.InExpo).OnComplete(()=> DoneWarp());

        sword.parent = null;
        sword.DOMove(enemyToKill.transform.position, warpDuration/1.2f);
        sword.DOLookAt(enemyToKill.transform.position, .2f, AxisConstraint.None);

        //Particles
        blueTrail.Play();
        whiteTrail.Play();

        //Lens Distortion
        DOVirtual.Float(0, -80, .2f, DistortionAmount);
        DOVirtual.Float(1, 2f, .2f, ScaleAmount);
    }


    void DoneWarp()
    {
        sword.parent = swordHand;
        sword.localPosition = swordOrigPos;
        sword.localEulerAngles = swordOrigRot;
        FinishAttack();
        StartCoroutine(DeleteEnemy(0.7f));
        StartCoroutine(FixSword());
    }

    void DoneQuickSlash()
    {
        FinishAttack();
        StartCoroutine(DeleteEnemy(0.3f));

        swordMesh.enabled = false;

        StartCoroutine(FixSword());
    }


    void FinishAttack()
    {
        GameManager.Instance.UpdateGameState(GameState.Walking);
        ShowBody(true);

        SkinnedMeshRenderer[] skinMeshList = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinMeshList)
        {
            GlowAmount(30);
            DOVirtual.Float(30, 0, .5f, GlowAmount);
        }

        Instantiate(hitParticle, sword.position, Quaternion.identity);

        enemyToKill.GetComponentInParent<Animator>().SetTrigger("hit");
        enemyToKill.transform.parent.DOMove(target.position + transform.forward, .5f);

        //StartCoroutine(HideSword());
        StartCoroutine(StopParticles());

        isLocked = false;

        //Shake
        impulse.GenerateImpulse(Vector3.right);

        //Lens Distortion
        DOVirtual.Float(-80, 0, .2f, DistortionAmount);
        DOVirtual.Float(2f, 1, .1f, ScaleAmount);

    }

    bool IsTargetTooClose(float distance)
    {
        if (Vector3.Distance(target.transform.position, transform.position) < distance)
        {
            return true;
        }
        return false;
    }




    IEnumerator DeleteEnemy(float timer)
    {
        GameObject oldTarget = enemyToKill.gameObject;
        screenTargets.Remove(oldTarget.transform);
        yield return new WaitForSeconds(timer);
        Destroy(oldTarget.transform.parent.gameObject);
        yield return new WaitForSeconds(2);
        //Fix rotation
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    IEnumerator StopParticles()
    {
        yield return new WaitForSeconds(.2f);
        anim.speed = 1;
        blueTrail.Stop();
        whiteTrail.Stop();
    }

    IEnumerator FixSword()
    {
        yield return new WaitForSeconds(.8f);
        sword.parent = swordHand;
        sword.localPosition = swordOrigPos;
        sword.localEulerAngles = swordOrigRot;
    }


    void ShowBody(bool state)
    {
        SkinnedMeshRenderer[] skinMeshList = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinMeshList)
        {
            smr.enabled = state;
        }
    }

    void GlowAmount(float x)
    {
        SkinnedMeshRenderer[] skinMeshList = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinMeshList)
        {
            smr.material.SetVector("_FresnelAmount", new Vector4(x, x, x, x));
        }
    }

    void DistortionAmount(float x)
    {
        postProfile.GetSetting<LensDistortion>().intensity.value = x;
    }
    void ScaleAmount(float x)
    {
        postProfile.GetSetting<LensDistortion>().scale.value = x;
    }

    public int targetIndex()
    {
        float[] distances = new float[screenTargets.Count];

        for (int i = 0; i < screenTargets.Count; i++)
        {
            distances[i] = Vector2.Distance(Camera.main.WorldToScreenPoint(screenTargets[i].position), new Vector2(Screen.width / 2, Screen.height / 2));
        }

        float minDistance = Mathf.Min(distances);
        int index = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (minDistance == distances[i])
                index = i;
        }

        return index;

    }

}
