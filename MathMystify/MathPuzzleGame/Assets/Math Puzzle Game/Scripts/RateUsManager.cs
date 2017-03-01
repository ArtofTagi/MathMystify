using UnityEngine;
using System.Collections;
using Ververg;

namespace Ververg
{
    public class RateUsManager : Singleton<RateUsManager>
    {

        public GameObject RateUsUI;
        public int NumberOfLevelPlayedToShowRateUs = 10;
        public string AndroidURL = "http://ververg.com";

        void Awake()
        {
            instance = this;
        }

        void Start()
        {

        }

        void Update()
        {

        }

        public void Yes()
        {
#if UNITY_ANDROID
            Application.OpenURL(AndroidURL);
#endif

            PlayerPrefs.SetInt("NumberOfLevelPlayedToShowRateUs", -1);
            PlayerPrefs.Save();
            HideRateUs();
        }

        public void Later()
        {
            PlayerPrefs.SetInt("NumberOfLevelPlayedToShowRateUs", 0);
            PlayerPrefs.Save();
            HideRateUs();
        }

        public void Never()
        {
            PlayerPrefs.SetInt("NumberOfLevelPlayedToShowRateUs", -1);
            PlayerPrefs.Save();
            HideRateUs();
        }

        public void CheckIfPromptRateDialogue()
        {
            int count = PlayerPrefs.GetInt("NumberOfLevelPlayedToShowRateUs", 0);

            if (count > NumberOfLevelPlayedToShowRateUs)
            {
                ShowRateUs();
            }
        }

        public void ShowRateUs()
        {
            RateUsUI.SetActive(true);
            RateUsUI.GetComponent<Animator>().SetBool("Show", true);
        }

        void HideRateUs()
        {
            RateUsUI.GetComponent<Animator>().SetBool("Show", false);
            StartCoroutine(HideRateUsCoroutine());
        }

        IEnumerator HideRateUsCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            RateUsUI.SetActive(false);
        }
    }
}