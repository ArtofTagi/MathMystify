using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Ververg
{
    public class StatsDisplay : MonoBehaviour
    {

        public StatsType DisplayStats;

        void Start()
        {
            UpdateText();
        }

        void Update()
        {

        }

        public void UpdateText()
        {
            string txt = StatsManager.Instance.GetStats(DisplayStats).ToString();
            switch (DisplayStats)
            {
                case StatsType.Accuration:
                    txt += "%";
                    break;
                case StatsType.LevelAverage:
                    txt += "s";
                    break;
                case StatsType.TimeSpent:
                    txt += "s";
                    break;
            }
            GetComponent<Text>().text = txt;
        }
    }
}