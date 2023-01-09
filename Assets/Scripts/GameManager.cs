using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool pause = false;
    public bool inGame = false;
    public int currentLevel = 1;
    public List<int> levelsUnlocked;
    public AudioSource musicManager;
    public AudioSource soundManager;
    public int musicVolume = 10;
    public int soundVolume = 10;
    public int pauseOption = -1;
    [SerializeField] private AudioClip[] musics;
    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private Image fondu;
    private TextMeshProUGUI[] pauseTexts;
    private bool delay;
    public bool discovered;
    public bool konamiCoded;

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        if (Application.systemLanguage != SystemLanguage.French)
        {
            InputManager.Instance.up = KeyCode.W;
            InputManager.Instance.cam_up = KeyCode.W;
            InputManager.Instance.left = KeyCode.Q;
            InputManager.Instance.cam_left = KeyCode.Q;
            InputManager.Instance.cam_turn_left = KeyCode.A;
        }
        
        pauseTexts = new TextMeshProUGUI[3];
        pauseTexts[0] = transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        pauseTexts[1] = transform.GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
        pauseTexts[2] = transform.GetChild(0).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();
        
        Instance = this;
        StartCoroutine(LoadFirst());
        Cursor.lockState = CursorLockMode.Locked;
        // LoadGame();
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(InputManager.Instance.cancel) && inGame && pauseOption == -1 && discovered && DoorBehaviour.movement == null && CameraControl.movement == null)
        {
            pause = true;
            pauseOption = 0;
            pauseTexts[pauseOption].color = Color.yellow;
            transform.GetChild(0).gameObject.SetActive(true);
            delay = true;
        }
        if (delay) {delay = false; return;}
        if (pauseOption >= 0)
        {
            if (Input.GetKeyDown(InputManager.Instance.up)) {pauseTexts[pauseOption].color = Color.white; pauseOption = (pauseOption == 0) ? 2 : pauseOption - 1; pauseOption = pauseOption%3; pauseTexts[pauseOption].color = Color.yellow;}
            if (Input.GetKeyDown(InputManager.Instance.down)) {pauseTexts[pauseOption].color = Color.white; pauseOption++; pauseOption = pauseOption%3; pauseTexts[pauseOption].color = Color.yellow;}

            if (Input.GetKeyDown(InputManager.Instance.enter)) 
            {
                switch (pauseOption)
                {
                    case 0: if (CameraControl.currentlyOrthographic) pause = false; pauseTexts[pauseOption].color = Color.white; pauseOption = -1; transform.GetChild(0).gameObject.SetActive(false); break;
                    case 1: break; //TODO Options ?
                    case 2: if (CameraControl.currentlyOrthographic) pause = false; pauseTexts[pauseOption].color = Color.white; pauseOption = -1; transform.GetChild(0).gameObject.SetActive(false); GameManager.Instance.inGame = false; SceneManager.LoadScene("TitleScreen"); break;//TODO anim ?
                }
            }
            if (Input.GetKeyDown(InputManager.Instance.cancel)) {if (CameraControl.currentlyOrthographic) pause = false; pauseTexts[pauseOption].color = Color.white; pauseOption = -1; transform.GetChild(0).gameObject.SetActive(false);}
        }
        else 
        {
            if (Input.GetKeyDown(InputManager.Instance.cancel) && inGame && pauseOption == -1 && DoorBehaviour.movement == null && CameraControl.movement == null)
            Application.Quit();
        }
    }

    public void goToNextLevel()
    {
        //TODO : Fondu dépendant de la couleur de fond
        currentLevel++;
        PlaySound(3);
        pause = true;
        if (currentLevel == 6) discovered = true;
        levelsUnlocked.Add(currentLevel);
        GameManager.Instance.SaveGame();
        StartCoroutine(transition());
    }

    IEnumerator transition()
    {
        switch ((currentLevel-1) / 5)
        {
            case 0: fondu.color = new Color(1,1,1,0); break;
            case 1: fondu.color = new Color(1,0,0,0); break;
            case 2: fondu.color = new Color(0,1,0,0); break;
        }
        yield return new WaitForSeconds(2f);
        var timeEllapsed = 0f;
        while (timeEllapsed < 2f)
        {
            fondu.color = new Color(fondu.color.r,fondu.color.g,fondu.color.b,timeEllapsed/2);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene("Level" + currentLevel.ToString());
        yield return new WaitForSeconds(1f);
        while (timeEllapsed < 4f)
        {
            fondu.color = new Color(fondu.color.r,fondu.color.g,fondu.color.b,(4 - timeEllapsed)/2);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        pause = false;
    }

    private Save CreateSaveGameObject()
    {
        Save save = new Save();

        foreach (int b in levelsUnlocked)
        {
            save.levelsState.Add(b);
        }

        save.jump = InputManager.Instance.jump;
        save.left = InputManager.Instance.left;
        save.right = InputManager.Instance.right;
        save.up = InputManager.Instance.up;
        save.down = InputManager.Instance.down;
        save.cam_left = InputManager.Instance.cam_left;
        save.cam_right = InputManager.Instance.cam_right;
        save.cam_up = InputManager.Instance.cam_up;
        save.cam_down = InputManager.Instance.cam_down;
        save.cancel = InputManager.Instance.cancel;
        save.enter = InputManager.Instance.enter;
        save.zoom = InputManager.Instance.zoom;
        save.cam_turn_left = InputManager.Instance.cam_turn_left;
        save.cam_turn_right = InputManager.Instance.cam_turn_right;
        save.b_key = InputManager.Instance.b_key;
        save.a_key = InputManager.Instance.a_key;
        save.musicVolume = musicVolume;
        save.soundVolume = soundVolume;
        save.discovered = discovered;
        save.konamiCoded = konamiCoded;

        return save;
    }

    public void SaveGame()
    {
        Save save = CreateSaveGameObject();

        // 2
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        Debug.Log(Application.persistentDataPath);
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("Game Saved");
    }

    public void LoadGame()
    { 
        // 1
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            Debug.Log(save.jump);
            file.Close();

            levelsUnlocked = new List<int>();

            // 3
            foreach (int b in save.levelsState)
            {
                levelsUnlocked.Add(b);
            }

            InputManager.Instance.jump = save.jump;
            InputManager.Instance.left = save.left;
            InputManager.Instance.right = save.right;
            InputManager.Instance.up = save.up;
            InputManager.Instance.down = save.down;
            InputManager.Instance.cam_left = save.cam_left;
            InputManager.Instance.cam_right = save.cam_right;
            InputManager.Instance.cam_up = save.cam_up;
            InputManager.Instance.cam_down = save.cam_down;
            InputManager.Instance.cam_turn_left = save.cam_turn_left;
            InputManager.Instance.cam_turn_right = save.cam_turn_right;
            InputManager.Instance.cancel = save.cancel;
            InputManager.Instance.enter = save.enter;
            InputManager.Instance.zoom = save.zoom;
            musicVolume = save.musicVolume;
            soundVolume = save.soundVolume;
            discovered = save.discovered;
            konamiCoded = save.konamiCoded;

            musicManager.volume = musicVolume*0.05f;
            soundManager.volume = soundVolume*0.05f;

            // Resets menu display

            Debug.Log("Game Loaded");
        }
        else
        {
            levelsUnlocked.Add(1);
            Debug.Log("No game saved!");
        }
    }

    public void PlayMusic(int i)
    {
        musicManager.clip = musics[i];
        musicManager.Play();
    }

    public void PlayTouch(int i)
    {
        float pitch = Random.Range(0.8f,1.2f);
        soundManager.pitch = pitch;
        PlaySound(i);
        soundManager.pitch = 1f;
    }

    public void PlaySound(int i)
    {
        soundManager.PlayOneShot(sounds[i]);
    }

    public void changeVolume(bool music, int i)
    {
        if (music)
        {
            musicVolume = Mathf.Clamp(musicVolume + i, 0, 10);
            musicManager.volume = musicVolume * 0.05f;
        }
        else 
        {
            soundVolume = Mathf.Clamp(soundVolume + i, 0, 10);
            soundManager.volume = soundVolume * 0.05f;
        }
    }

    IEnumerator LoadFirst()
    {
        while (InputManager.Instance == null) yield return null;
        LoadGame();
        if (GameManager.Instance.levelsUnlocked.Contains(6))
        {
            SceneManager.LoadScene("TitleScreen");
        }
        else 
        {
            GameManager.Instance.inGame = true;
            SceneManager.LoadScene("Level1");
        }
    }
}
