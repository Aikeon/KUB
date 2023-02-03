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
    public PlayerControl Player;
    public GameObject KUB;
    public Quaternion KUBBaseRot = Quaternion.identity;
    private Coroutine transitLevel;
    [SerializeField] private GameObject cubeLetterPrefab;
    [SerializeField] private AnimationCurve cubFallCurve;
    [SerializeField] private AnimationCurve endLevelPlayerScale;
    private bool retrieveCubs;
    public bool changingLevel;

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
        changingLevel = false;

        Instance = this;
        StartCoroutine(LoadFirst());
        Cursor.lockState = CursorLockMode.Locked;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.N)) SceneManager.LoadScene("Footage");
        if (transitLevel != null) return;
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
                if (CameraControl.currentlyOrthographic) pause = false; 
                pauseTexts[pauseOption].color = Color.white;
                switch (pauseOption)
                {
                    case 0: break;
                    case 1: GameObject.Find("Player").GetComponent<PlayerControl>().Die(); break;
                    case 2: KUBBaseRot = Quaternion.identity; inGame = false; SceneManager.LoadScene("TitleScreen"); break; //TODO anim ?
                }
                pauseOption = -1;
                transform.GetChild(0).gameObject.SetActive(false);
            }
            if (Input.GetKeyDown(InputManager.Instance.cancel)) {if (CameraControl.currentlyOrthographic) pause = false; pauseTexts[pauseOption].color = Color.white; pauseOption = -1; transform.GetChild(0).gameObject.SetActive(false);}
        }
        else 
        {
            if (Input.GetKeyDown(InputManager.Instance.cancel) && inGame && pauseOption == -1 && DoorBehaviour.movement == null && CameraControl.movement == null)
            Application.Quit();
        }
    }

    public void goToNextLevel(bool isCustomLevel)
    {
        currentLevel++;
        PlaySound(3);
        pause = true;
        KUBBaseRot = Quaternion.identity;
        if (!isCustomLevel)
        {
            if (currentLevel == 6) discovered = true;
            levelsUnlocked.Add(currentLevel);
            GameManager.Instance.SaveGame();
        }
        changingLevel = true;
        Player.GetComponent<BoxCollider>().enabled = false;
        transitLevel = StartCoroutine(transitionToNextLevel((isCustomLevel ? "TitleScreen" : ("Level" + currentLevel.ToString()))));
    }

    IEnumerator transitionFromMenuToLevel()
    {
        yield return null;
    }

    IEnumerator transitionToNextLevel(string nextSceneName)
    {
        switch ((currentLevel-1) / 5)
        {
            case 0: fondu.color = new Color(1,1,1,0); break;
            case 1: fondu.color = new Color(1,0,0,0); break;
            case 2: fondu.color = new Color(0,1,0,0); break;
        }
        var timeEllapsed = 0f;
        while (timeEllapsed < 4f)
        {
            if (timeEllapsed < endLevelPlayerScale.keys[endLevelPlayerScale.length-1].time)
            {
                Player.transform.localScale = endLevelPlayerScale.Evaluate(timeEllapsed) * Vector3.one;
            }
            fondu.color = new Color(fondu.color.r,fondu.color.g,fondu.color.b,Mathf.Max(timeEllapsed - 2, 0)/2);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(nextSceneName);
        yield return new WaitForSeconds(1f);
        while (timeEllapsed < 6f)
        {
            fondu.color = new Color(fondu.color.r,fondu.color.g,fondu.color.b,Mathf.Max(6 - timeEllapsed, 0)/2);
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        transitLevel = null;
        pause = false;
        changingLevel = false;
    }

    public void showLevelName(string name)
    {
        StartCoroutine(showLevelNameEnum(name));
    }

    IEnumerator showLevelNameEnum(string name)
    {
        retrieveCubs = false;
        var scale = (name.Length == 0) ? 1f : Mathf.Clamp(10f/name.Length,0f,2f);
        for (int i = 0; i < name.Length; i++)
        {
            var rd = Random.Range(0,4);
            if (name[i].ToString() != " ")
            {
                var iLetter = CreateCubeLetter((i + 0.5f - name.Length/2f)* 3.5f * scale * Vector3.right - 25 * Vector3.forward, name[i], rd, scale);
                StartCoroutine(cubeLetterBehaviour(iLetter, rd, false, scale));
            }
            
        }
        yield return new WaitForSeconds(4f);
        retrieveCubs = true;
    }

    IEnumerator cubeLetterBehaviour(GameObject cub, int rdFace, bool reversed, float scale)
    {
        var rseed = Random.Range(0,3);
        yield return new WaitForSeconds(Random.Range(0f,1f));
        var timeEllapsed = 0f;
        switch (rseed)
        {
            case 0: cub.transform.localScale = (reversed) ? 0.15f * scale * Vector3.one : Vector3.zero; break;
            case 1: cub.transform.position = (reversed) ? new Vector3(cub.transform.position.x, 0, cub.transform.position.z) : cub.transform.position + 20 * Vector3.up; break;
            case 2: cub.transform.position = (reversed) ? new Vector3(cub.transform.position.x, 0, cub.transform.position.z) : cub.transform.position - 20 * Vector3.up; break;
        }
        cub.SetActive(true);
        var t2 = 0f;
        Quaternion newRot = Quaternion.identity;
        switch (rdFace)
        {
            case 0: newRot = cub.transform.rotation * Quaternion.Euler(-90,0,0); break;
            case 1: newRot = cub.transform.rotation * Quaternion.Euler(0,-90,0); break;
            case 2: newRot = cub.transform.rotation * Quaternion.Euler(0,90,0); break;
            case 3: newRot = cub.transform.rotation * Quaternion.Euler(90,0,0); break;
        }
        while (t2 < 1 && reversed)
        {
            cub.transform.rotation = Quaternion.Lerp(cub.transform.rotation, newRot, t2);
            t2 += Time.deltaTime;
            yield return null;
        }
        switch (rseed)
        {
            case 0: while (timeEllapsed < 0.5f) 
                            {
                                cub.transform.localScale = Vector3.Lerp(Vector3.zero, 0.15f * scale * Vector3.one, (reversed) ? 1 - timeEllapsed*2f : timeEllapsed*2f); 
                                timeEllapsed += Time.deltaTime; 
                                yield return null;
                            } 
                            break;
            case 1: while (timeEllapsed < 1f) 
                            {
                                cub.transform.position = new Vector3(cub.transform.position.x, cubFallCurve.Evaluate((reversed) ? 1 - timeEllapsed : timeEllapsed), cub.transform.position.z); 
                                timeEllapsed += Time.deltaTime; 
                                yield return null;
                            }
                            break;
            case 2: while (timeEllapsed < 1f) 
                            {
                                cub.transform.position = new Vector3(cub.transform.position.x, -cubFallCurve.Evaluate((reversed) ? 1 - timeEllapsed : timeEllapsed), cub.transform.position.z); 
                                timeEllapsed += Time.deltaTime; 
                                yield return null;
                            }
                            break;
        }
        switch (rseed)
        {
            case 0: cub.transform.localScale = (reversed) ? Vector3.zero : 0.15f * scale * Vector3.one; break;
            case 1: 
            case 2: cub.transform.position = (reversed) ? Vector3.up * 200 : new Vector3(cub.transform.position.x, 0, cub.transform.position.z); break;
        }
        var t3 = 0f;
        Quaternion newRot2 = Quaternion.identity;
        switch (rdFace)
        {
            case 0: newRot2 = cub.transform.rotation * Quaternion.Euler(-90,0,0); break;
            case 1: newRot2 = cub.transform.rotation * Quaternion.Euler(0,-90,0); break;
            case 2: newRot2 = cub.transform.rotation * Quaternion.Euler(0,90,0); break;
            case 3: newRot2 = cub.transform.rotation * Quaternion.Euler(90,0,0); break;
        }
        while (t3 < 1 && !reversed)
        {
            cub.transform.rotation = Quaternion.Lerp(cub.transform.rotation, newRot2, t3);
            t3 += Time.deltaTime;
            yield return null;
        }

        if (!reversed) 
        {
            while (!retrieveCubs) yield return null;
            StartCoroutine(cubeLetterBehaviour(cub, Random.Range(0,4), true, scale));
        }
        else 
        {
            Destroy(cub);
        }
    }

    GameObject CreateCubeLetter(Vector3 pos, char c, int rd, float scale)
    {
        var g = Instantiate(cubeLetterPrefab, pos, Quaternion.identity);
        g.transform.localScale = 0.15f * scale * Vector3.one;
        var txtcan = g.transform.GetChild(6 + rd).gameObject;
        txtcan.SetActive(true);
        txtcan.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = c.ToString();
        return g;
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
        // SceneManager.LoadScene("LevelEditor");
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
