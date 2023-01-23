using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleFileBrowser;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Transform buttons;
    [SerializeField] private Transform musicBar;
    [SerializeField] private Transform soundBar;
    [SerializeField] private Transform[] optionsTexts;
    [SerializeField] private Material notSelected;
    [SerializeField] private Material selected;
    [SerializeField] private Material filledBar;
    [SerializeField] private Material unfilledBar;
    [SerializeField] private TextMeshProUGUI[] inputsTexts;
    [SerializeField] private TextMeshProUGUI[] inputsDisplays;
    [SerializeField] private TextMeshProUGUI[] customLevelTexts;
    [SerializeField] private GameObject inputCanvas;
    [SerializeField] private GameObject inputChronoBoard;
    [SerializeField] private TextMeshProUGUI inputChronoText;
    [SerializeField] private Scrollbar inputScrollBar;
    [SerializeField] private GameObject konamiDoor;
    public int numberOfLevels;
    private Transform[] levelButtons;
    private int state;
    private int konamiState;
    public bool inMenu = false;
    private int selectedLevel;
    private int selectedOption;
    private int selectedInput;
    private bool editorSelected;
    private bool changingInputs;
    private Coroutine selectingKey;
    private CameraControl cameraControl;

    [SerializeField] Image background;


    [SerializeField] private GameObject exitPanel;
    [SerializeField] private TextMeshProUGUI yesText;
    [SerializeField] private TextMeshProUGUI noText;
    bool exitShown;
    bool quit;
    // Start is called before the first frame update
    void Start()
    {
        exitShown = false;
        quit = false;
        selectedLevel = 1;
        selectedOption = -1;
        editorSelected = false;
        levelButtons = new Transform[numberOfLevels+1];
        changingInputs = false;
        for (int i = 1; i <= numberOfLevels; i++)
        {
            levelButtons[i] = buttons.GetChild(i);
        }
        foreach (int i in GameManager.Instance.levelsUnlocked)
        {
            levelButtons[i].GetChild(0).gameObject.SetActive(true);
            levelButtons[i].GetChild(1).gameObject.SetActive(false);
        }
        levelButtons[selectedLevel].GetChild(0).GetComponent<TextMeshPro>().color = Color.yellow;
        state = 0;
        cameraControl = GetComponent<CameraControl>();
        for (int i = 0; i < GameManager.Instance.musicVolume; i++)
        {
            musicBar.GetChild(i).GetComponent<Renderer>().material = filledBar;
        }
        for (int i = 0; i < GameManager.Instance.soundVolume; i++)
        {
            soundBar.GetChild(i).GetComponent<Renderer>().material = filledBar;
        }
        inputsTexts[0].text = InputManager.Instance.enter.ToString();
        inputsTexts[1].text = InputManager.Instance.cancel.ToString();
        inputsTexts[2].text = InputManager.Instance.a_key.ToString();
        inputsTexts[3].text = InputManager.Instance.b_key.ToString();
        inputsTexts[4].text = InputManager.Instance.up.ToString();
        inputsTexts[5].text = InputManager.Instance.down.ToString();
        inputsTexts[6].text = InputManager.Instance.left.ToString();
        inputsTexts[7].text = InputManager.Instance.right.ToString();
        inputsTexts[8].text = InputManager.Instance.jump.ToString();
        inputsTexts[9].text = InputManager.Instance.zoom.ToString();
        inputsTexts[10].text = InputManager.Instance.cam_up.ToString();
        inputsTexts[11].text = InputManager.Instance.cam_down.ToString();
        inputsTexts[12].text = InputManager.Instance.cam_left.ToString();
        inputsTexts[13].text = InputManager.Instance.cam_right.ToString();
        inputsTexts[14].text = InputManager.Instance.cam_turn_left.ToString();
        inputsTexts[15].text = InputManager.Instance.cam_turn_right.ToString();
        konamiDoor.SetActive(!GameManager.Instance.konamiCoded);
    }

    // Update is called once per frame
    void Update()
    {
        if (selectingKey != null || FileBrowser.IsOpen) return;
        if (inMenu)
        {
            if (changingInputs)
            {
                if (Input.GetKeyDown(InputManager.Instance.up)) {inputsTexts[selectedInput].color = Color.white; inputsDisplays[selectedInput].color = Color.white; selectedInput += (selectedInput == 0) ? 15 : -1; inputsTexts[selectedInput].color = Color.yellow; inputsDisplays[selectedInput].color = Color.yellow;}
                if (Input.GetKeyDown(InputManager.Instance.down)) {inputsTexts[selectedInput].color = Color.white; inputsDisplays[selectedInput].color = Color.white; selectedInput += (selectedInput == 15) ? -15 : 1; inputsTexts[selectedInput].color = Color.yellow; inputsDisplays[selectedInput].color = Color.yellow;}

                switch (selectedInput)
                {
                    case 0:while (!(inputScrollBar.value <= 1f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 1f)) {inputScrollBar.value += 0.1f;} break;
                    case 1: while (!(inputScrollBar.value <= 1f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0.86f)) {inputScrollBar.value += 0.1f;} break;
                    case 2: while (!(inputScrollBar.value <= 1f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0.75f)) {inputScrollBar.value += 0.1f;} break;
                    case 3: while (!(inputScrollBar.value <= 0.94f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0.75f)) {inputScrollBar.value += 0.1f;} break;
                    case 4: while (!(inputScrollBar.value <= 0.85f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0.55f)) {inputScrollBar.value += 0.1f;} break;
                    case 5: while (!(inputScrollBar.value <= 0.74f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0.46f)) {inputScrollBar.value += 0.1f;} break;
                    case 6: while (!(inputScrollBar.value <= 0.65f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0.35f)) {inputScrollBar.value += 0.1f;} break;
                    case 7: while (!(inputScrollBar.value <= 0.65f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0.35f)) {inputScrollBar.value += 0.1f;} break;
                    case 8: while (!(inputScrollBar.value <= 0.54f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0.26f)) {inputScrollBar.value += 0.1f;} break;
                    case 9: while (!(inputScrollBar.value <= 0.34f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0.06f)) {inputScrollBar.value += 0.1f;} break;
                    case 10: while (!(inputScrollBar.value <= 0.34f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0.06f)) {inputScrollBar.value += 0.1f;} break;
                    case 11: while (!(inputScrollBar.value <= 0.25f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0)) {inputScrollBar.value += 0.1f;} break;
                    case 12: while (!(inputScrollBar.value <= 0.14f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0)) {inputScrollBar.value += 0.1f;} break;
                    case 13: while (!(inputScrollBar.value <= 0.14f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0)) {inputScrollBar.value += 0.1f;} break;
                    case 14: while (!(inputScrollBar.value <= 0.05f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0)) {inputScrollBar.value += 0.1f;} break;
                    case 15: while (!(inputScrollBar.value <= 0.05f)) {inputScrollBar.value -= 0.1f;} while (!(inputScrollBar.value >= 0)) {inputScrollBar.value += 0.1f;} break;
                }

                if (Input.GetKeyDown(InputManager.Instance.enter)) {selectingKey = StartCoroutine(changeKey(selectedInput));}
                if (Input.GetKeyDown(InputManager.Instance.cancel)) {inputsTexts[selectedInput].color = Color.white; inputsDisplays[selectedInput].color = Color.white; inputCanvas.SetActive(false); changingInputs = false;}
            }
            else 
            {
                switch (state)
                {
                    case 0: break;
                    case 1:
                        var latestSelected = selectedLevel;
                        if (Input.GetKeyDown(InputManager.Instance.left)) {selectedLevel += (selectedLevel%5 == 1) ? 4 : -1; while (!GameManager.Instance.levelsUnlocked.Contains(selectedLevel)) selectedLevel += (selectedLevel%5 == 1) ? 4 : -1;}
                        if (Input.GetKeyDown(InputManager.Instance.right)) {selectedLevel += (selectedLevel%5 == 0) ? -4 : 1; while (!GameManager.Instance.levelsUnlocked.Contains(selectedLevel)) selectedLevel += (selectedLevel%5 == 0) ? -4 : 1;}
                        if (Input.GetKeyDown(InputManager.Instance.up)) {selectedLevel += (selectedLevel < 6) ? 20 : -5; while (!GameManager.Instance.levelsUnlocked.Contains(selectedLevel)) selectedLevel += (selectedLevel < 6) ? 20 : -5;}
                        if (Input.GetKeyDown(InputManager.Instance.down)) {selectedLevel += (selectedLevel > 20) ? -20 : 5; while (!GameManager.Instance.levelsUnlocked.Contains(selectedLevel)) selectedLevel += (selectedLevel > 20) ? -20 : 5;}
                        if (selectedLevel != latestSelected) 
                        {
                            levelButtons[latestSelected].GetChild(0).GetComponent<TextMeshPro>().color = Color.white;
                            levelButtons[selectedLevel].GetChild(0).GetComponent<TextMeshPro>().color = Color.yellow;
                        }
                        if (Input.GetKeyDown(InputManager.Instance.enter) || Input.GetKeyDown(InputManager.Instance.jump)) {GameManager.Instance.currentLevel = selectedLevel; GameManager.Instance.inGame = true; SceneManager.LoadScene("Level"+selectedLevel.ToString());} //TODO Animation + sound
                    break;
                    case 2: break;
                    case 3: 
                        if (selectedOption == -1)
                        {
                            selectedOption = 0;
                            foreach (Renderer r in optionsTexts[selectedOption].GetComponentsInChildren<Renderer>()) r.material = selected;
                        }
                        var latestOption = selectedOption;
                        if (Input.GetKeyDown(InputManager.Instance.up)) {selectedOption = (selectedOption-1)%3;}
                        if (Input.GetKeyDown(InputManager.Instance.down)) {selectedOption = (selectedOption+1)%3;}
                        if (Input.GetKeyDown(InputManager.Instance.left)) {if (selectedOption != 0) {if (selectedOption == 1) 
                                                                                                    {
                                                                                                        GameManager.Instance.changeVolume(true, -1); musicBar.GetChild(GameManager.Instance.musicVolume).GetComponent<Renderer>().material = unfilledBar;    
                                                                                                    }
                                                                                                    else 
                                                                                                    {
                                                                                                        GameManager.Instance.changeVolume(false, -1); soundBar.GetChild(GameManager.Instance.soundVolume).GetComponent<Renderer>().material = unfilledBar;
                                                                                                    }}}
                        if (Input.GetKeyDown(InputManager.Instance.right)) {if (selectedOption != 0) {if (selectedOption == 1) 
                                                                                                    {
                                                                                                        GameManager.Instance.changeVolume(true, 1); musicBar.GetChild(GameManager.Instance.musicVolume - 1).GetComponent<Renderer>().material = filledBar;
                                                                                                    }
                                                                                                    else 
                                                                                                    {
                                                                                                        GameManager.Instance.changeVolume(false, 1); soundBar.GetChild(GameManager.Instance.soundVolume - 1).GetComponent<Renderer>().material = filledBar;
                                                                                                    }}}

                        if (Input.GetKeyDown(InputManager.Instance.enter) && selectedOption == 0) 
                        {
                            selectedInput = 0;
                            inputsTexts[selectedInput].color = Color.yellow;
                            inputsDisplays[selectedInput].color = Color.yellow;
                            inputScrollBar.value = 1;
                            inputCanvas.SetActive(true);
                            changingInputs = true;
                        }

                        if (selectedOption != latestOption)
                        {
                            foreach (Renderer r in optionsTexts[latestOption].GetComponentsInChildren<Renderer>()) r.material = notSelected;
                            foreach (Renderer r in optionsTexts[selectedOption].GetComponentsInChildren<Renderer>()) r.material = selected;
                        }

                        if (Input.GetKeyDown(InputManager.Instance.cancel)) {foreach (Renderer r in optionsTexts[selectedOption].GetComponentsInChildren<Renderer>()) r.material = notSelected; selectedOption = -1;}
                    break;
                    case 16:
                        if (Input.GetKeyDown(InputManager.Instance.enter))
                        {
                            if (editorSelected) SceneManager.LoadScene("LevelEditor");
                            else StartCoroutine(CustomLevelFileManager.WaitForSelectionInExplorer(false,false));
                        }
                        if (Input.GetKeyDown(InputManager.Instance.left) || Input.GetKeyDown(InputManager.Instance.right))
                        {
                            editorSelected = !editorSelected;
                            customLevelTexts[0].color = (editorSelected ? Color.white : Color.yellow);
                            customLevelTexts[1].color = (editorSelected ? Color.yellow : Color.white);
                        }
                        if (Input.GetKeyDown(InputManager.Instance.cancel)) {if (editorSelected) customLevelTexts[1].color = Color.white; else customLevelTexts[0].color = Color.white; editorSelected = false;}
                        break;
                    case 12: break;
                }
                
                if (Input.GetKeyDown(InputManager.Instance.cancel) && CameraControl.movement == null) {inMenu = false; cameraControl.changePerspective();}
            }
        }
        else 
        {
            if (exitShown) 
            {
                if (Input.GetKeyDown(InputManager.Instance.cancel))
                {
                    exitShown = false;
                    exitPanel.SetActive(false);
                }

                if (Input.GetKeyDown(InputManager.Instance.enter))
                {
                    if (quit) 
                    {
                        Application.Quit(); //TODO Anim ? Sound ?
                    }
                    else 
                    {
                        exitShown = false;
                        exitPanel.SetActive(false);
                    }
                }

                if (Input.GetKeyDown(InputManager.Instance.left) || Input.GetKeyDown(InputManager.Instance.right)) {quit = !quit; if (quit) {yesText.color = Color.yellow; noText.color = Color.white;} else {yesText.color = Color.white; noText.color = Color.yellow;} }
            }
            else 
            {
                if (Input.GetKeyDown(InputManager.Instance.cancel))
                {
                    exitShown = true;
                    quit = false;
                    yesText.color = Color.white; noText.color = Color.yellow;
                    exitPanel.SetActive(true);
                }
                
                if (Input.GetKeyDown(InputManager.Instance.up) && state == 0) state = 16;
                if (Input.GetKeyDown(InputManager.Instance.down) && state == 0 && GameManager.Instance.konamiCoded) state = 12;

                if (Input.GetKeyDown(InputManager.Instance.right) && state < 11) state++;
                if (Input.GetKeyDown(InputManager.Instance.left) && state < 11) state--;

                if (state < 10) {state = state%4; if (state == -1) state = 3;}
                else 
                {
                    if (state == 12)
                    {
                        if (Input.GetKeyDown(InputManager.Instance.up)) state = 0;
                    }
                    else 
                    {
                        if (state == 16 && Input.GetKeyDown(InputManager.Instance.down)) state = 0;
                    }
                }

                if ((Input.GetKeyDown(InputManager.Instance.enter) || Input.GetKeyDown(InputManager.Instance.jump)) && CameraControl.movement == null) 
                {
                    inMenu = true; 
                    cameraControl.changePerspective();
                    if (state == 16) customLevelTexts[0].color = Color.yellow;
                };
            }
        }

        switch (konamiState)
        {
            case 0 :
            case 1 : if (Input.anyKeyDown) {if (Input.GetKeyDown(InputManager.Instance.up)) konamiState++; else konamiState = 0;} break;
            case 2 :
            case 3 : if (Input.anyKeyDown) {if (Input.GetKeyDown(InputManager.Instance.down)) konamiState++; else konamiState = 0;} break;
            case 4 :
            case 6 : if (Input.anyKeyDown) {if (Input.GetKeyDown(InputManager.Instance.left)) konamiState++; else konamiState = 0;} break;
            case 5 :
            case 7 : if (Input.anyKeyDown) {if (Input.GetKeyDown(InputManager.Instance.right)) konamiState++; else konamiState = 0;} break;
            case 8 : if (Input.anyKeyDown) {if (Input.GetKeyDown(InputManager.Instance.b_key)) konamiState++; else konamiState = 0;} break;
            case 9 : if (Input.anyKeyDown) {if (Input.GetKeyDown(InputManager.Instance.a_key)) {GameManager.Instance.konamiCoded = true; konamiDoor.SetActive(false); GameManager.Instance.SaveGame(); Debug.Log("KonamiCoded !");} else konamiState = 0;} break;
        }

        Color.RGBToHSV(background.color, out float hue, out float sat, out float v);
        hue += 0.01f * Time.deltaTime;
        background.color = Color.HSVToRGB(hue,sat,v);

        if (Input.GetKeyDown(KeyCode.K)) Debug.Log(selectedOption);
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(state == 12 ? 90f : state == 16 ? -90f : 0f, state * 90f, 0f), 0.05f);
    }

    IEnumerator changeKey(int keyId)
    {
        var timeEllapsed = 0f;
        yield return null;
        inputChronoBoard.SetActive(true);
        while (timeEllapsed < 5f)
        {
            foreach(KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                inputChronoText.text = Mathf.CeilToInt(5 - timeEllapsed).ToString();
                if (Input.GetKeyDown(k))
                {
                    switch (keyId)
                    {
                        case 0: if (InputManager.Instance.cancel != k) InputManager.Instance.enter = k; break;
                        case 1: if (InputManager.Instance.enter != k) InputManager.Instance.cancel = k; break;
                        case 2: InputManager.Instance.a_key = k; break;
                        case 3: InputManager.Instance.b_key = k; break;
                        case 4: InputManager.Instance.up = k; break; 
                        case 5: InputManager.Instance.down = k; break;
                        case 6: InputManager.Instance.left = k; break;
                        case 7: InputManager.Instance.right = k; break;
                        case 8: InputManager.Instance.jump = k; break;
                        case 9: InputManager.Instance.zoom = k; break;
                        case 10: InputManager.Instance.cam_up = k; break;
                        case 11: InputManager.Instance.cam_down = k; break;
                        case 12: InputManager.Instance.cam_left = k; break;
                        case 13: InputManager.Instance.cam_right = k; break;
                        case 14: InputManager.Instance.cam_turn_left = k; break;
                        case 15: InputManager.Instance.cam_turn_right = k; break;
                    }
                    inputsTexts[keyId].text = k.ToString();
                    GameManager.Instance.SaveGame();
                    timeEllapsed = 6f;
                }
            }
            timeEllapsed += Time.deltaTime;
            yield return null;
        }
        inputChronoBoard.SetActive(false);
        selectingKey = null;
    }
}
