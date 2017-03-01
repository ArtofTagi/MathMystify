using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Ververg;

#if NATIVE_PLUGINS_LITE_VERSION
using VoxelBusters.NativePlugins;
#endif

namespace Ververg
{
    public class GUIManager : Singleton<GUIManager>
    {

        public GameObject MainMenu;                     // Main menu panel
        public GameObject InGame;                       // In game panel
        public GameObject Lose;                         // Lose panel
        public GameObject Settings;                         // Settings panel
        public GameObject Pause;                        // Pause panel
        public GameObject Leaderboard;                     // Main menu panel

        public GameObject GridPanel;                    // Grid panel
        public GameObject GridButton;                   // Button for the grid
        public GameObject GridAnswerButton;             // Answer button for the grid

        public Text Value1;                             // Text to display our first value
        public Text Value2;                             // Text to display our second value
        public Text Operand;                            // Text to display our operand
        public Text Result;                             // Text to display our result
        public Text LevelName;                          // Text to display the level name
        public Text TimeString;                         // Text to display the countdown
        public Text LastLevel;                          // Text to display our last level

        public AudioSource audioSource;                 // Audio Source to play our win or retry clip
        public AudioClip WinAudio;                      // Clip when we succeed the level
        public AudioClip RetryAudio;                    // Clip when we hit the retry button

        private GridButton Selected1;                   // Variable to store the first number button
        private GridButton Selected2;                   // Variable to store the second number button
        private GridButton SelectedOperand;             // Variable to store operand button
        private GridButton SelectedResult;              // Variable to store result button

        private List<GridButton> buttons = new List<GridButton>();  // List of buttons in the level

        // Singleton setup
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            if (audioSource == null)
            {
                Debug.LogError("Please attach audio source!");
            }
        }

        void Update()
        {
            // Do nothing
        }

        public void PlayGame()
        {
            MainMenu.SetActive(false);
            InGame.SetActive(true);
            LevelManager.instance.LoadLevel();
            AdsManager.instance.ShowBanner();
        }

        public void SpawnButton(string s, bool isAnswer)
        {
            GameObject button = null;
            // Spawn between the answer button or not
            if (!isAnswer)
            {
                button = Instantiate(GridButton, Vector2.zero, Quaternion.identity) as GameObject;
            }
            else
            {
                button = Instantiate(GridAnswerButton, Vector2.zero, Quaternion.identity) as GameObject;
            }

            // Place the button in the grid and add into our buttons array
            button.transform.SetParent(GridPanel.transform);
            button.transform.localScale = new Vector3(1.0f, 1.0f);
            button.GetComponentInChildren<Text>().text = s;
            button.GetComponent<GridButton>().SetNumber(s);
            buttons.Add(button.GetComponent<GridButton>());

            // Set the answer if its the answer button
            if (isAnswer)
            {
                button.GetComponent<GridButton>().SetIsAnswer(true);
            }
        }

        public void SetGridConstraint(int i)
        {
            GridPanel.GetComponent<GridLayoutGroup>().constraintCount = i;
        }

        public void SetGridCellSize(int size)
        {
            GridPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(size, size);
        }

        public void Choose(GridButton button)
        {
            // Store if the operand is chosen
            if (button.GetNumber() == "+" || button.GetNumber() == "-" || button.GetNumber() == "*" || button.GetNumber() == "/")
            {
                Operand.text = button.GetNumber();
                SelectedOperand = button;
            }
            else
            {
                // Store the first number if its empty
                if (Selected1 == null)
                {
                    Value1.text = button.GetNumber();
                    Selected1 = button;
                }
                // Store the second number if its empty
                else if (Selected2 == null)
                {
                    Value2.text = button.GetNumber();
                    Selected2 = button;
                }
            }

            // If all required button is not empty, check the result
            if (Selected1 != null && Selected2 != null && SelectedOperand != null)
            {
                StartCoroutine(CheckResult());
            }
        }

        public void Cancel(GridButton button)
        {
            // Reset the text and remove from our stored button
            Result.text = "_";
            if (button == Selected1)
            {
                Selected1 = null;
                Value1.text = "_";
            }
            else if (button == Selected2)
            {
                Selected2 = null;
                Value2.text = "_";
            }
            else if (button = SelectedOperand)
            {
                SelectedOperand = null;
                Operand.text = "_";
            }
        }

        IEnumerator CheckResult()
        {
            int result = 0;
            switch (SelectedOperand.GetNumber())
            {
                case "+":
                    result = int.Parse(Selected1.GetNumber()) + int.Parse(Selected2.GetNumber());
                    break;
                case "-":
                    result = int.Parse(Selected1.GetNumber()) - int.Parse(Selected2.GetNumber());
                    break;
                case "*":
                    result = int.Parse(Selected1.GetNumber()) * int.Parse(Selected2.GetNumber());
                    break;
                case "/":
                    result = int.Parse(Selected1.GetNumber()) / int.Parse(Selected2.GetNumber());
                    break;
            }

            Result.text = result.ToString();
            //StatsManager.instance.SaveStats(StatsType.FailedCount);

            // Check if there is a corret answer for answer button in the grids and check for the win condition each time player answered correctly
            foreach (GridButton button in buttons)
            {
                int n;
                int.TryParse(button.GetNumber(), out n);
                if (n == result && button.GetIsAnswer() && !button.GetAnswered())
                {
                    //StatsManager.instance.SaveStats(StatsType.SuccessCount);
                    //StatsManager.instance.SaveStats(StatsType.FailedCount, 1, false);
                    SelectedResult = button;
                    Selected1.SetAnswered(true);
                    Selected2.SetAnswered(true);
                    SelectedOperand.SetAnswered(true);
                    SelectedResult.SetAnswered(true);
                    Selected1 = null;
                    Selected2 = null;
                    SelectedOperand = null;
                    SelectedResult = null;
                    Value1.text = "_";
                    Value2.text = "_";
                    Operand.text = "_";
                    Result.text = "_";
                    yield return new WaitForSeconds(1.0f);
                    CheckWin();
                    break;
                }
            }
        }

        void CheckWin()
        {
            // If all answer buttons in answered then win
            bool win = true;
            foreach (GridButton button in buttons)
            {
                if (button.GetAnswered() == false)
                {
                    win = false;
                }
            }

            if (win)
            {
                PlayAudio(WinAudio);
                ClearButtons();
                LevelManager.instance.IncreaseLevel();
                LevelManager.instance.LoadLevel();
            }
        }

        void ClearButtons()
        {
            buttons.Clear();
            foreach (Transform child in GridPanel.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void Retry()
        {
            // Set all buttons to unanswered
            //StatsManager.instance.SaveStats(StatsType.Retries);
            PlayAudio(RetryAudio);
            foreach (GridButton button in buttons)
            {
                button.SetAnswered(false);
            }
        }

        public void SetTime(string timeString)
        {
            TimeString.text = timeString;
        }

        void PlayAudio(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void SetLevelName(int lvl)
        {
            LevelName.text = "Level " + lvl;
        }

        public void SetLastLevel(int lvl)
        {
            LastLevel.text = lvl.ToString();
        }

        public GridButton GetSelected1()
        {
            return Selected1;
        }

        public GridButton GetSelected2()
        {
            return Selected2;
        }

        public GridButton GetSelectedOperand()
        {
            return SelectedOperand;
        }

        public GridButton GetSelectedResult()
        {
            return SelectedResult;
        }

        public void GameRetry()
        {
            ClearButtons();
            LevelManager.instance.SetLevel(1);
            Lose.SetActive(false);
            LevelManager.instance.LoadLevel();
        }

        public void RetryFromAds()
        {
            ClearButtons();
            Lose.SetActive(false);
            LevelManager.instance.LoadLevel();
        }

        public void GameHome()
        {
            Time.timeScale = 1.0f;
            LevelManager.instance.SetLevel(1);
            LevelManager.instance.StopAllCoroutines();
            ClearButtons();
            Leaderboard.SetActive(false);
            Settings.SetActive(false);
            Lose.SetActive(false);
            InGame.SetActive(false);
            MainMenu.SetActive(true);
            StatsManager.instance.UpdateStats();
            MainMenu.GetComponent<MenuManager>().Spawn();
            Pause.SetActive(false);
            AdsManager.instance.Hide_Banner();
        }

        public void GameLose()
        {
            Lose.SetActive(true);
            Lose.GetComponent<Animator>().SetTrigger("Lose");
            Lose.GetComponent<MenuManager>().Spawn();
            RateUsManager.instance.CheckIfPromptRateDialogue();
            AdsManager.instance.ShowAdsGameOver();
        }

        public void OpenLeaderboard()
        {
            Leaderboard.SetActive(true);
        }
        public void OpenSettings()
        {
            Settings.SetActive(true);
        }

        public void GamePause()
        {
            Pause.SetActive(true);
            Pause.GetComponent<Animator>().SetBool("Paused", true);
            Time.timeScale = 0;
        }

        public void GameResume()
        {
            Time.timeScale = 1.0f;
            Pause.GetComponent<Animator>().SetBool("Paused", false);
            StartCoroutine(Resume());
        }

        IEnumerator Resume()
        {
            yield return new WaitForSeconds(0.5f);
            Pause.SetActive(false);
        }

        public void ShareStats()
        {
#if NATIVE_PLUGINS_LITE_VERSION && USES_SHARING
            ShareSheet _shareSheet = new ShareSheet();

            _shareSheet.AttachScreenShot();

            NPBinding.UI.SetPopoverPointAtLastTouchPosition();
            NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
#endif
        }

#if NATIVE_PLUGINS_LITE_VERSION && USES_SHARING
        void FinishedSharing(eShareResult _result)
        {
            Debug.Log("Finished sharing");
            Debug.Log("Share Result = " + _result);
        }
#endif
    }
}