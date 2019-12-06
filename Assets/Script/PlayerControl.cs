﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    /* Initialize */
    public Vector2 jumpForce = new Vector2(0, 350);
    public AudioSource JumpSound;
    public AudioSource SwordPlusSound;
    public AudioSource SwordMinSound;
    public AudioSource DogSound;
    public AudioSource FishSound;
    public AudioSource CatFoodSound;
    public AudioSource BoneSound;

    public Text NotifText;

    /* Data si player*/
    int life = 5;
    int attackPower = 0;
    bool isStun = false;

    /* Game Object untuk nyawa player */

    public GameObject[] arrLifeBar;
    public GameObject[] arrAttackBar;


    /* Untuk Ganti Animasi player*/
    public int animCounter = 0;

    Animator animator;


    IEnumerator WaitForStunToEnd()
    {
        yield return new WaitForSeconds(1f);
        isStun = false;
        NotifText.text = "";
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (PlayerPrefs.HasKey("isResume"))
        {
            attackPower = PlayerPrefs.GetInt("attackPower");
            life = PlayerPrefs.GetInt("playerLife");
            PlayerPrefs.DeleteKey("isResume");
        }
        whatAttack();
        whatLife();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStun)
        {
            if (Input.GetKeyDown("space") || Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.9f);
                // Vector3 thePosition = transform.localPosition;
                // if(thePosition.y < -2.7){
                    GetComponent<Rigidbody2D>().AddForce(jumpForce);
                // }
                // transform.position += new Vector3(0,2,0);
                animCounter = 3;
                animator.SetInteger("catAnim", animCounter);
                playMusic("jump");
            }

            if (Input.GetKey(KeyCode.S))
            {
                transform.position += new Vector3(0, -2, 0) * Time.deltaTime;
                animCounter = 3;
                animator.SetInteger("catAnim", animCounter);
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position += new Vector3(-4, 0, 0) * Time.deltaTime * 2;
                animCounter = 1;
                animator.SetInteger("catAnim", animCounter);
                GetComponent<SpriteRenderer>().flipX = false;

                Vector3 theScale = transform.localScale;
                
                if(theScale.x < 0 ){
                    // transform.localScale.x *= new Vector3(-1, 0, 0);
                    theScale.x *= -1;
                }
                transform.localScale = theScale;
                
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.position += new Vector3(4, 0, 0) * Time.deltaTime * 2;
                animCounter = 1;
                animator.SetInteger("catAnim", animCounter);
                GetComponent<SpriteRenderer>().flipX = true;
            
                Vector3 theScale = transform.localScale;
                
                if(theScale.x > 0 ){
                    // transform.localScale.x *= new Vector3(-1, 0, 0);
                    theScale.x *= -1;
                }
                transform.localScale = theScale;
            }

            if (life <= 0)
            {
                SceneManager.LoadScene("PlayerScore");
            }

            if (attackPower > 7)
            {
                SceneManager.LoadScene("Win");
            }
            // animCounter=0;
            // animator.SetInteger("catAnim",animCounter);

            PlayerPrefs.SetInt("attackPower", attackPower);
            PlayerPrefs.SetInt("playerLife", life);

            Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
            if (screenPosition.y > Screen.height || screenPosition.y < 0)
            {
                isStun = true;
                NotifText.text = "Can't fly higher";
                StartCoroutine(WaitForStunToEnd());
            }
        }
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        /* Mulai nabrak */
        if (coll.gameObject.tag == "boneTag")
        {
            // Debug.Log("Bone");
            playMusic("boneTag");
            isStun = true;
            NotifText.text = "Stunned ..";
            StartCoroutine(WaitForStunToEnd());
        }
        if (coll.gameObject.tag == "fishTag")
        {
            // Debug.Log("Fish");
            tambahLife();
            playMusic("fishTag");
        }
        if (coll.gameObject.tag == "catFoodTag")
        {
            // Debug.Log("Cat Food");
            tambahLife();
            playMusic("catFoodTag");
        }
        if (coll.gameObject.tag == "swordMinTag")
        {
            // Debug.Log("Sword Min");
            kurangiAttack();
            playMusic("swordMinTag");
        }
        if (coll.gameObject.tag == "swordPlusTag")
        {
            // Debug.Log("Sword Plus");
            attackPower ++;
            whatAttack();
            playMusic("swordPlusTag");
        }
        if (coll.gameObject.tag == "dogTag")
        {
            // Debug.Log("Dog");
            kurangiLife();
            playMusic("dogTag");
        }
    }

    void playMusic(string mode)
    {   
        if (!PlayerPrefs.HasKey("isUseMusic") || PlayerPrefs.GetInt("isUseMusic") == 1)
        {
            if (mode == "jump")
            {
                JumpSound.Play();
            }
            else if (mode == "boneTag")
            {
                BoneSound.Play();
            }
            else if (mode == "fishTag")
            {
                FishSound.Play();
            }
            else if (mode == "catFoodTag")
            {
                CatFoodSound.Play();
            }
            else if (mode == "swordMinTag")
            {
                SwordMinSound.Play();
            }
            else if (mode == "swordPlusTag")
            {
                SwordPlusSound.Play();
            }
            else if (mode == "dogTag")
            {
                DogSound.Play();
            }
        }
    }


    void tambahLife()
    {
        life++;
        if (life > 5)
        {
            life = 5;
        }
        whatLife();
    }

    void kurangiLife()
    {
        life--;
        if (life < 0)
        {
            life = 0;
        }
        whatLife();
    }

    void kurangiAttack(){
        attackPower --;
        if(attackPower<0){
            attackPower=0;
        }
        whatAttack();
    }

    void whatAttack()
    {
        int i = 0;
        for (i = 0; i < 7; i++){   
            if(i<attackPower){
                arrAttackBar[i].SetActive(true);
            }else{
                arrAttackBar[i].SetActive(false);
            }
        }
    }

    void whatLife()
    {
        int i = 0;
        for (i = 0; i < 5; i++){
            if(i<life){
                arrLifeBar[i].SetActive(true);
            }else{
                arrLifeBar[i].SetActive(false);
            }
        }
    }
}
