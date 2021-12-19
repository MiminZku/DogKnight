using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Chracter player;
    public Transform playerSpawn;
    public GameObject spawnManager;
    public float playTime;
    public bool isBattle;
    public int enemyCount;

    public GameObject gameStartPanel;
    public GameObject inGamePanel;
    public GameObject gameEndPanel;
    public GameObject gamePausePanel;
    public Text bestRecordTxt;
    public Text playTimeTxt;
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text enemyCountTxt;
    public AudioSource audioSource;
    public AudioClip victorySound;
    public bool isPause;

    void Awake() {
        if(PlayerPrefs.HasKey("BestRecord")){
            int t = PlayerPrefs.GetInt("BestRecord");
            int h = t / 3600;
            int m = (t / 60) % 60;
            int s = t % 60;
            bestRecordTxt.text = string.Format("{0:D2}:{1,2:D2}:{2,2:D2}",h,m,s);
        }
    }
    void Update() {
        if(isBattle)    playTime += Time.deltaTime;
        if(isBattle && Input.GetKeyDown(KeyCode.Escape)){
            if(isPause){
                Time.timeScale = 1;
                isPause = false;
                gameCam.GetComponent<AudioSource>().volume=0.5f;
                gamePausePanel.SetActive(false);
            }
            else{
                Time.timeScale = 0;
                isPause = true;
                gameCam.GetComponent<AudioSource>().volume=0f;
                gamePausePanel.SetActive(true);
            }
        }
    }
    void LateUpdate() {
        int h = (int)playTime / 3600;
        int m = ((int)playTime / 60) % 60;
        int s = (int)playTime % 60;
        playTimeTxt.text = string.Format("{0:D2}:{1,2:D2}:{2,2:D2}",h,m,s);
        playerHealthTxt.text = player.curHealth + " / " + player.maxHealth;
        playerAmmoTxt.text = player.gun.GetComponent<Weapon>().currentBullet + " / " + player.ammo;
        enemyCountTxt.text = enemyCount.ToString() + " / " + spawnManager.GetComponent<SpawnManager>().maxEnemy.ToString();
        if(isBattle && spawnManager.GetComponent<SpawnManager>().maxEnemy <= enemyCount){
            whenPlayerWin();
        }
        if(isBattle && player.isDead)   whenPlayerDied();
    }

    public void onClickGameStart(){
        gameCam.GetComponent<CamMove>().currentZoom = 12f;
        gameCam.GetComponent<CamMove>().angle = 0f;
        gameCam.GetComponent<AudioSource>().volume=0.5f;
        player.transform.position = playerSpawn.position;
        player.isDead = false;
        player.curHealth = 100;
        player.gun.GetComponent<Weapon>().currentBullet = 15;
        player.ammo = 150;
        enemyCount = 0;
        spawnManager.GetComponent<SpawnManager>().curEnemy = 0;
        playTime = 0;
        isBattle = true;

        menuCam.SetActive(false);
        gameCam.SetActive(true);
        
        gameStartPanel.SetActive(false);
        gameStartPanel.SetActive(false);
        inGamePanel.SetActive(true);

        player.gameObject.SetActive(true);
        spawnManager.gameObject.SetActive(true);
    }
    void whenPlayerWin(){
        player.animator.SetBool("isWalk", false);
        player.animator.SetBool("isRun", false);
        gameCam.GetComponent<CamMove>().currentZoom = 6f;
        gameCam.GetComponent<AudioSource>().volume=0.1f;
        audioSource.PlayOneShot(victorySound);
        isBattle = false;
        spawnManager.gameObject.SetActive(false);
        PlayerPrefs.SetInt("BestRecord",(int)playTime);
        gameEndPanel.SetActive(true);
        gameEndPanel.transform.Find("GameOverText").gameObject.SetActive(false);
        gameEndPanel.transform.Find("VictoryText").gameObject.SetActive(true);
    }
    void whenPlayerDied(){
        gameCam.GetComponent<CamMove>().currentZoom = 6f;
        gameCam.GetComponent<AudioSource>().volume=0.1f;
        isBattle = false;
        spawnManager.gameObject.SetActive(false);
        gameEndPanel.SetActive(true);
        gameEndPanel.transform.Find("VictoryText").gameObject.SetActive(false);
        gameEndPanel.transform.Find("GameOverText").gameObject.SetActive(true);
    }
    public void onClickGoToMain(){
        Time.timeScale = 1;
        isPause = false;

        gameCam.SetActive(false);
        menuCam.SetActive(true);
        
        inGamePanel.SetActive(false);
        gamePausePanel.SetActive(false);
        gameEndPanel.SetActive(false);
        gameStartPanel.SetActive(true);
        
        player.gameObject.SetActive(false);
        spawnManager.gameObject.SetActive(false);

        int t = PlayerPrefs.GetInt("BestRecord");
        int h = t / 3600;
        int m = (t / 60) % 60;
        int s = t % 60;
        bestRecordTxt.text = string.Format("{0:D2}:{1,2:D2}:{2,2:D2}",h,m,s);
    }
    public void onClickHowToPlay(){
        gameStartPanel.transform.Find("HowToPlay").gameObject.SetActive(true);
    }
    public void onClickClose(){
        gameStartPanel.transform.Find("HowToPlay").gameObject.SetActive(false);
    }
    public void onClickExitGame(){
        Application.Quit();
    }
}