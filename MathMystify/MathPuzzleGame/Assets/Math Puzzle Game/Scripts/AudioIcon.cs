using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Ververg
{
    public class AudioIcon : MonoBehaviour
    {
        [Tooltip("True if BGM, False if SFX")]
        public bool BgmOrSfx = true;   // True if BGM, false if SFX

        private int isMute;         // Variables for checking if its in mute or not
        private Image img;          // Image icon that will changed between muted or not

        void Start()
        {
            img = GetComponent<Image>();
            if (!img)
            {
                Debug.LogError("Please attach this script to Audio Icon that has Image component in it!");
            }


            // Change the BGM image icon between muted or not
            if (BgmOrSfx)
            {
                isMute = PlayerPrefs.GetInt("BGMMute", 1);

                if (isMute == 1)
                {
                    img.sprite = AudioManager.Instance.BGMIcon[1];
                }
                else
                {
                    img.sprite = AudioManager.Instance.BGMIcon[0];
                }
            }
            // Chage the SFX image icon between muted or not
            else
            {
                isMute = PlayerPrefs.GetInt("SFXMute", 1);

                if (isMute == 1)
                {
                    img.sprite = AudioManager.Instance.SFXIcon[1];
                }
                else
                {
                    img.sprite = AudioManager.Instance.SFXIcon[0];
                }
            }
        }

        void Update()
        {
            // Do nothing
        }
    }
}