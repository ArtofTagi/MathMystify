using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ververg
{
    // Class for Level Information
    [System.Serializable]
    public class LevelInfo
    {
        public int LevelStart;
        public int GridCount;
        public int levelDuration;
        public int minVal;
        public int maxVal;
    }

    // Class for Grid Information
    [System.Serializable]
    public class Grid
    {
        public string item;
        public bool isAnswer;
    }


    public class LevelManager : Singleton<LevelManager>
    {

        public LevelInfo[] LevelsDesign;                        // Array for our levels

        private int GridCount = 2;                              // Number of grids
        private int levelDuration = 60;                         // Duration for level
        [SerializeField]
        private int currentLevel = 1;                           // Current level
        private int minVal = 2;                                 // Minimum value for the grid number
        private int maxVal = 4;                                 // Maximum value for the grid number
        private List<Grid> grids = new List<Grid>();            // Our grids
        private List<int> holder = new List<int>();             // Holder to grid that already spawned
        public FacebookManager mFbManager;
        private System.TimeSpan time;
        private int timer;
        private string timeString;

        // Singleton setup
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            // Do nothing
        }

        void Update()
        {
            // Do nothing
        }

        public void LoadLevel()
        {
            // Setup to load our level
            StopAllCoroutines();
            SetLevel();
            StartCoroutine(StartCountdown());
            SetGrids();
            GUIManager.Instance.SetGridConstraint(GridCount);
            StartCoroutine(LoadingLevel());
     //       StatsManager.instance.SaveStats(StatsType.PlayCount);

            // Count to number played to rate us
            int count = PlayerPrefs.GetInt("NumberOfLevelPlayedToShowRateUs", 0);
            if (count != -1)
                count++;
			
            PlayerPrefs.SetInt("NumberOfLevelPlayedToShowRateUs", count);

            // Count to number played to show interstitials
            int countInterstitials = PlayerPrefs.GetInt("numberOfPlayToShowInterstitial", 0);
            countInterstitials++;
            PlayerPrefs.SetInt("numberOfPlayToShowInterstitial", countInterstitials);
        }

        void SetLevel()
        {
            // Set our level based on our level design information
            GUIManager.instance.SetLevelName(currentLevel);
            foreach (LevelInfo level in LevelsDesign)
            {
                if (currentLevel == level.LevelStart)
                {
                    GridCount = level.GridCount;
                    levelDuration = level.levelDuration;
                    minVal = level.minVal;
                    maxVal = level.maxVal;
                }
            }
        }

        IEnumerator StartCountdown()
        {
            timer = levelDuration;
            while (true)
            {
                time = new System.TimeSpan(0, 0, timer);
                timeString = time.Minutes.ToString() + ":" + time.Seconds.ToString();
                GUIManager.instance.SetTime(timeString);
                timer -= 1;
              //  StatsManager.instance.SaveStats(StatsType.TimeSpent);
                CheckLose();
                yield return new WaitForSeconds(1.0f);
            }
        }

        void CheckLose()
        {
            if (timer == -1)
            {
                Lose();
            }
        }

        public void Lose()
        {
            Debug.Log("LOST");
            GUIManager.instance.GameResume();
            StopAllCoroutines();
            GUIManager.instance.GameLose();
            GUIManager.instance.SetLastLevel(currentLevel);
            if (StatsManager.instance.GetStats(StatsType.BestLevel) < currentLevel)
            {
                StatsManager.instance.SaveStats(StatsType.BestLevel, currentLevel);
                mFbManager.SetScore(currentLevel);
            }
        }

        IEnumerator LoadingLevel()
        {
            // We spawn our grid one by one randomly
            for (int i = 0; i < grids.Count; i++)
            {
                int rand = Random.Range(0, grids.Count);
                while (holder.Contains(rand))
                {
                    rand = Random.Range(0, grids.Count);
                }
                GUIManager.Instance.SpawnButton(grids[rand].item, grids[rand].isAnswer);
                holder.Add(rand);
                yield return new WaitForSeconds(0.2f);
            }
        }

        void SetGrids()
        {
            grids.Clear();
            holder.Clear();
            // Make sure that 4 grids is an equation with a result
            for (int i = 0; i < GridCount * GridCount / 4; i++)
            {
                Grid val1 = new Grid();
                Grid val2 = new Grid();
                Grid val3 = new Grid();
                Grid val4 = new Grid();

                int v1;
                int v2;
                do
                {
                    v1 = Random.Range(minVal, maxVal);
                    v2 = Random.Range(minVal, maxVal);
                    val1.item = v1.ToString();
                    val2.item = v2.ToString();
                    val3.item = GetOperand();
                    switch (val3.item)
                    {
                        case "+":
                            val4.item = (v1 + v2).ToString();
                            break;
                        case "-":
                            val4.item = (v1 - v2).ToString();
                            break;
                        case "*":
                            val4.item = (v1 * v2).ToString();
                            break;
                        case "/":
                            val4.item = (v1 / v2).ToString();
                            break;
                    }
                    val4.isAnswer = true;
                } while (val3.item == "/" && v1 % v2 != 0);


                grids.Add(val1);
                grids.Add(val2);
                grids.Add(val3);
                grids.Add(val4);
            }
        }

        string GetOperand()
        {
            int rand = Random.Range(1, 5);
            switch (rand)
            {
                case 1:
                    return "+";
                case 2:
                    return "-";
                case 3:
                    return "*";
                case 4:
                    return "/";
            }
            return "+";
        }

        public List<Grid> GetGrids()
        {
            return grids;
        }

        public void IncreaseLevel()
        {
            currentLevel++;
        }

        public int GetLevel()
        {
            return currentLevel;
        }

        public void SetLevel(int lvl)
        {
            currentLevel = lvl;
        }
    }
}