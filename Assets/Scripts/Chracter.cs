using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chracter : MonoBehaviour
{
    public float speed;
    public float rotateSpeed;
    public int ammo;
    public Camera cam;
    public int curHealth;
    public int maxHealth;
    public AudioSource audioSource;
    public AudioClip gunSound;
    public AudioClip noBulletSound;
    public AudioClip reloadSound;
    public AudioClip swordSound;
    public AudioClip hitSound;
    public AudioClip dieSound;
    public Animator animator;

    Vector3 moveVec;
    Rigidbody rgdbody;
    GameObject sword;
    public GameObject gun;
    public GameManager gameManager;

    float hAxis;
    float vAxis;

    bool walkDown;
    bool jumpDown;
    bool runDown;
    bool meleeDown;
    bool rangeDown;
    bool reloadDown;

    bool isJumping;
    bool isMeleeReady = true;
    bool isRangeReady = true;
    bool isReloading;
    bool isHit;
    public bool isDead;

    float meleeDelay;
    float rangeDelay;

    // Start is called before the first frame update
    void Start()
    {
        rgdbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        sword = GameObject.Find("Weapon");
        gun = GameObject.Find("Gun");
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameManager.isBattle || gameManager.isPause || isDead) return;
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Reload();
    }

    void GetInput()
    {
        hAxis= Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkDown = hAxis!=0 || vAxis!=0;
        jumpDown = Input.GetButtonDown("Jump");
        runDown = Input.GetKey(KeyCode.LeftShift);
        rangeDown = Input.GetButtonDown("Fire1");
        meleeDown = Input.GetButtonDown("Fire2");
        reloadDown = Input.GetKeyDown(KeyCode.R);
    }
    void Move(){
        if(isDead) return;
        Vector3 front = transform.position - cam.transform.position;    front.y = 0;
        Vector3 camFront = transform.position - cam.transform.position;
        Vector3 right = Vector3.Cross(front,camFront);
        if(walkDown){
            moveVec = vAxis * front.normalized + hAxis * right.normalized;
            speed = 5; if (runDown  && !isReloading) speed = 7;
            if(!isMeleeReady || !isRangeReady){
                if(!isJumping)  moveVec = Vector3.zero;
            }
            transform.position += moveVec.normalized * speed * Time.deltaTime;
        }
        animator.SetBool("isWalk", walkDown);
        animator.SetBool("isRun", runDown);  
    }
    void Turn()
    {
        //1. of Mouse
        if (meleeDown || rangeDown)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                moveVec = rayHit.point - transform.position;
                moveVec.y = 0;
                transform.LookAt(transform.position + moveVec);
            }
        }
        //2. of KeyBoard
        if(walkDown)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveVec), Time.deltaTime * rotateSpeed);
    }
    void Jump(){
        if(jumpDown && !isJumping && !isReloading){
            rgdbody.AddForce(Vector3.up*6f,ForceMode.Impulse);
            animator.SetBool("isJumping",true);
            animator.SetTrigger("doJump");
            isJumping = true;
            // Invoke("JumpDone",0.5f);
        }
    }
    // void JumpDone(){
    //     animator.SetBool("isJumping",false);
    //     isJumping = false;
    // }
    void Attack()
    {
        meleeDelay += Time.deltaTime;
        rangeDelay += Time.deltaTime;
        isMeleeReady = sword.GetComponent<Weapon>().attackCycle < meleeDelay;
        isRangeReady = gun.GetComponent<Weapon>().attackCycle < rangeDelay;

        if(meleeDown && isMeleeReady && !isReloading)
        {
            audioSource.PlayOneShot(swordSound);
            sword.GetComponent<Weapon>().Use();
            animator.SetTrigger("doSwing");
            meleeDelay = 0;
        }
        else if(rangeDown && isRangeReady && !isReloading)
        {
            //효과음
            if(gun.GetComponent<Weapon>().currentBullet>0)
                audioSource.PlayOneShot(gunSound);
            else
                audioSource.PlayOneShot(noBulletSound);
            gun.GetComponent<Weapon>().Use();
            animator.SetTrigger("doShot");
            rangeDelay = 0;
        }
    }
    void Reload()
    {
        if (ammo == 0 || gun.GetComponent<Weapon>().currentBullet == gun.GetComponent<Weapon>().maxBullet) return;
        if(reloadDown && !isJumping && isRangeReady && !isReloading)
        {
            audioSource.PlayOneShot(reloadSound);
            animator.SetTrigger("doReload");
            isReloading = true;
            Invoke("ReloadOut", 2f);
        }
    }
    void ReloadOut()
    {
        int reAmmo = ammo < gun.GetComponent<Weapon>().maxBullet - gun.GetComponent<Weapon>().currentBullet
                        ? ammo : gun.GetComponent<Weapon>().maxBullet - gun.GetComponent<Weapon>().currentBullet;
        gun.GetComponent<Weapon>().currentBullet += reAmmo;
        ammo = ammo - reAmmo;
        isReloading = false;
    }
    void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Floor"){
            animator.SetBool("isJumping",false);
            isJumping = false;
        }
        if(other.gameObject.tag == "Enemy" && !isDead && !isHit){
            curHealth -= other.gameObject.GetComponent<Enemy>().damage;
            Vector3 reactVec = transform.position - other.gameObject.transform.position;
            if(curHealth > 0){
                StartCoroutine(OnDamage(reactVec));
            }
            else{
                OnDie(reactVec);
            }
        }
    }
    IEnumerator OnDamage(Vector3 reactVec){
        isHit = true;
        audioSource.PlayOneShot(hitSound);
        animator.SetTrigger("doHit");
        reactVec += Vector3.up;
        rgdbody.AddForce(reactVec*4, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
        isHit = false;
    }
    void OnDie(Vector3 reactVec){
        audioSource.PlayOneShot(dieSound);
        isDead = true;
        animator.SetTrigger("doDie");
        isHit = false;
    }
}
