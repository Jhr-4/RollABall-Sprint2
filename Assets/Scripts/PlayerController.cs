using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour {

    public float speed = 0;

    public TextMeshProUGUI countText;
    private int count = 0;
    public TextMeshProUGUI winTextObject;
    public TextMeshProUGUI jumpText;

    private Rigidbody rb;

    private float movementX;
    private float movementY;

    public GameObject door;

    private GameObject[] extended_enemies;

    public AudioClip collectSound, deathSound;
    
    public GameObject sparkle;

    private bool canJump = true;
    private float jumpCooldown = 3f;

    // Start is called before the first frame update
    void Start() 
    {
        extended_enemies = GameObject.FindGameObjectsWithTag("Extended_Enemy");
        for (int i=0; i<extended_enemies.Length; i++) {
            extended_enemies[i].SetActive(false);
        }

        sparkle.SetActive(false);
        
        rb = GetComponent<Rigidbody>();
        SetCountText();
        winTextObject.gameObject.SetActive(false);
    }

    void Update(){

        if (Input.GetKeyDown(KeyCode.Space) && canJump){
            Jump();
            canJump = false;
            jumpText.gameObject.SetActive(false);
        }

        if (canJump == false){
            jumpCooldown = jumpCooldown - Time.deltaTime;
            if (jumpCooldown <= 0) {
                jumpText.gameObject.SetActive(true);
                jumpCooldown = 3f;
                canJump = true;
            }
        }


    }

    private void Jump(){
        Vector3 jumpForce = new Vector3(0f, 4f, 0f);
        rb.AddForce(jumpForce, ForceMode.Impulse);
    }

    void OnMove(InputValue movementValue) 
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText() {
        countText.text = "Count: " + count.ToString() + "/12";

        if(count >= 4){
            door.SetActive(false);
            sparkle.SetActive(true);
        }

        if(count >= 5){
            GameObject[] gateBarriers = GameObject.FindGameObjectsWithTag("gateBarrier");
            for (int i=0; i<gateBarriers.Length; i++) {
                gateBarriers[i].SetActive(false);
            }
            for (int i=0; i<extended_enemies.Length; i++) {
                extended_enemies[i].SetActive(true);
            }
        }

        if(count >= 12){
            winTextObject.gameObject.SetActive(true);
            winTextObject.text = "You Won!";
            winTextObject.color = Color.green;

            //destroy emeny
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for(int i = 0; i<enemies.Length; i++){
                Destroy(enemies[i]);
            }
            //Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);

        if (transform.position.y <= -1){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Spike")){
            AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, 1.0f);
            Destroy(gameObject);
            winTextObject.gameObject.SetActive(true);
            winTextObject.text = "You Lost! \n Press 'R' To Try Again";
            winTextObject.color = Color.red;
        }

    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("PickUp")){
            AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position, 1.0f);
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }

        if (other.gameObject.CompareTag("Oil")){
            speed = 100f;
        }

        if (other.gameObject.CompareTag("Spring")){
            Vector3 jump = new Vector3(0f, 10f, 0f);
            rb.AddForce(jump, ForceMode.Impulse);
        }
    }



        private void OnTriggerExit(Collider other){
        if (other.gameObject.CompareTag("Oil")){
            speed = 10f;
        }
    }

}
