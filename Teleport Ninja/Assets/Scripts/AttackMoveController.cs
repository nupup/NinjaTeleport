using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cinemachine;
using System;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.EventSystems;
using IndieMarc.EnemyVision;

public class AttackMoveController : MonoBehaviour
{
    GameObject enemyToKill;
    private Animator anim;
    private float distanceToQuickSlash = 6;
    private CameraController cameraController;
    private ClickDetection clickDetector;

    private bool isLocked; //for rotation towards enemy, this stops once teleport happens
    private bool killing; //for the quick slash, to make sure the attack does not hapen twice
    public bool canSwitchTarget; //Is locked if attack is happening

    public CinemachineVirtualCamera cameraFreeLook;
    public CinemachineVirtualCamera cameraWalk;
    private CinemachineImpulseSource impulse;
    private CinemachineImpulseSource impulseWalk;
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
        impulseWalk = cameraWalk.GetComponent<CinemachineImpulseSource>();
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
            case GameState.Lose:
                anim.SetTrigger("die");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == GameState.Victory)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = false;
        }

        //Switch target in case new one is aimed at
        ConstantlySwitchTarget();

        //Rotate towards target if in process of warping
        if (isLocked)
            transform.DOLookAt(enemyToKill.transform.position, 0);

        if (GameManager.Instance.State == GameState.Aiming)
            return;

        //Melee attack if not killing already
        if (IsTargetTooClose(2) && !isLocked && GameManager.Instance.State == GameState.Walking) {
            GameManager.Instance.UpdateGameState(GameState.QuickKilling);
            Kill();
        }

            
    }

    public void Kill()
    {
        if (target == null)
        {
            GameManager.Instance.UpdateGameState(GameState.Walking);
            return;
        }

        enemyToKill = target.gameObject;
        enemyToKill.transform.parent.tag = "KilledTarget";

        killing = true;
        enemyToKill.GetComponentInParent<Enemy>().isDead = true;

        swordParticle.Play();
        swordMesh.enabled = true;
        bool isTargetTooClose = IsTargetTooClose(distanceToQuickSlash);
        if (isTargetTooClose)
            anim.SetTrigger("quickSlash");
        else
        {
            isLocked = true;
            anim.SetTrigger("slash");
        }

        //rotate toward starget
        transform.DOLookAt(new Vector3(enemyToKill.transform.position.x, transform.position.y, enemyToKill.transform.position.z), 0.2f);
    }

    private void ConstantlySwitchTarget()
    {
        if (GameManager.Instance.State == GameState.Walking)
        {
            if (screenTargets.Count != 0)
                SwitchTarget(screenTargets[targetIndex()]);
        }

        if (Input.touchCount > 0)
        {
            if (GameManager.Instance.State == GameState.Aiming)
            {
                //Make sure the touch is not over a UI element
                foreach (Touch touch in Input.touches)
                {
                    int id = touch.fingerId;
                    if (!EventSystem.current.IsPointerOverGameObject(id))
                    {
                        SwitchTarget(screenTargets[targetIndex()]);
                    }
                }
            }
        }
    }

    private bool IsTargetVisible(Transform targetPos)
    { 
        if (targetPos == null)
            return false;

        Debug.DrawRay(transform.position, (targetPos.transform.parent.position - transform.position), Color.blue, 5f);
        var ray = new Ray(transform.position, (targetPos.transform.parent.position - transform.position));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)){
            //obstacel is 9
            if (hit.transform.gameObject.layer != 9)
            {
                return true;
            }
            
        }
        return false;
    }

    private void SwitchTarget(Transform proposedTarget)
    {
        //if new target is selected
        if (proposedTarget != oldTarget)
        {
            if (IsTargetVisible(proposedTarget))
            {
                Debug.Log("target visible and assigned");
                target = proposedTarget;
                oldTarget = target;
                if (GameManager.Instance.State == GameState.Aiming)
                    HighlightTarget();
            }
            else
            {
                Debug.Log("target not visible");
                target = null;
                foreach (Transform t in screenTargets)
                {
                    t.GetComponent<TargetScript>().UnHighlight();
                }
            }
                
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
        isLocked = false;
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

    public void ProposeAim()
    {
        //find the closest available target to aim towards, if none are in the view then decline aim and shake cam
        float closestDistance = 99999;
        Transform closestTarget = null;
        for (int i = 0;i < screenTargets.Count;i++)
        {
            if (IsTargetVisible(screenTargets[i]) && !screenTargets[i].GetComponentInParent<Enemy>().isDead)
            {
                float distance = Vector3.Distance(screenTargets[i].transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = screenTargets[i];
                }
            }
        }
        if (closestTarget != null)
        {
            Debug.Log("propose aim adhered");
            target = closestTarget;
            HighlightTarget();
            GameManager.Instance.UpdateGameState(GameState.Aiming);

        }
        else
        {
            impulseWalk.GenerateImpulse(Vector3.right);
        }
    }


    void DoneWarp()
    {
        sword.parent = swordHand;
        sword.localPosition = swordOrigPos;
        sword.localEulerAngles = swordOrigRot;
        FinishAttack();
        StartCoroutine(DeleteEnemy(1.5f));
        StartCoroutine(FixSword());
    }

    void DoneQuickSlash()
    {
        FinishAttack();
        StartCoroutine(DeleteEnemy(1.5f));

        swordMesh.enabled = false;

        StartCoroutine(FixSword());
    }


    void FinishAttack()
    {
        if (!GameManager.Instance.AreEnemiesDead())
            GameManager.Instance.UpdateGameState(GameState.Walking);

        ShowBody(true);

        SkinnedMeshRenderer[] skinMeshList = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinMeshList)
        {
            GlowAmount(30);
            DOVirtual.Float(30, 0, .5f, GlowAmount);
        }

        Instantiate(hitParticle, sword.position, Quaternion.identity);

        foreach (Transform child in enemyToKill.transform.parent)
        {
            if (child.tag == "Cone")
                Destroy(child.gameObject);
        }

        enemyToKill.GetComponent<TargetScript>().DeadHighlight();
        enemyToKill.GetComponentInParent<Animator>().SetInteger("state",5);
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
        if (target!= null && Vector3.Distance(target.transform.position, transform.position) < distance)
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
        killing = false;
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
