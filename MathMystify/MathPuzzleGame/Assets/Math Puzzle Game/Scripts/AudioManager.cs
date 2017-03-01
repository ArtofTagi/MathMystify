using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

namespace Ververg
{
    public class AudioManager : Singleton<AudioManager>
    {

        public AudioMixer mixer;                    // Audio Mixer that's gonna blend our BGM and SFX nicely
        public float MuteValue = -80.0f;            // Value in mixer to remove the volume completely
        public float BGMValue = -10.0f;             // Value in mixer how loud our BGM is master mixer
        public float SFXValue = -5.0f;              // Value in mixer how loud our SFX is from master mixer
        public Image BGMImage;                      // BGM Image that will toggled between muted or not
        [Tooltip("Attach 2 icons, index 0 for off icon, index 1 for on icon")]
        public Sprite[] BGMIcon;                    // Mute/Unmute Icon for BGM
        public Image SFXImage;                      // SFX Image that will toggled between muted or not
        [Tooltip("Attach 2 icons, index 0 for off icon, index 1 for on icon")]
        public Sprite[] SFXIcon;                    // Mute/Unmute Icon for SFX

        private int bgm;                            // Variables to store if the BGM is muted or not
        private int sfx;                            // Variables to store if the SFX is muted or not

        // Singleton setup
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            // Get info for the last time user is in mute or not
            bgm = PlayerPrefs.GetInt("BGMMute", 1);
            sfx = PlayerPrefs.GetInt("SFXMute", 1);

            // Set the BGM volume
            if (bgm == 1)
            {
                SetVolume("BGMvol", BGMValue);
            }
            else
            {
                SetVolume("BGMvol", MuteValue);
            }

            // Set the SFX volume
            if (sfx == 1)
            {
                SetVolume("SFXvol", SFXValue);
            }
            else
            {
                SetVolume("SFXvol", MuteValue);
            }
        }

        void Update()
        {
            // Do nothing
        }

        public void ToggleBGM(Image theImage)
        {
            // Mute if BGM is on
            if (bgm == 1)
            {
                SetVolume("BGMvol", MuteValue);
                bgm = 0;
                theImage.sprite = BGMIcon[0];
                PlayerPrefs.SetInt("BGMMute", 0);
            }
            // Unmute if SFX is off
            else
            {
                SetVolume("BGMvol", BGMValue);
                bgm = 1;
                theImage.sprite = BGMIcon[1];
                PlayerPrefs.SetInt("BGMMute", 1);
            }
        }

        public void ToggleSFX(Image theImage)
        {
            // Mute if SFX is on
            if (sfx == 1)
            {
                SetVolume("SFXvol", MuteValue);
                sfx = 0;
                theImage.sprite = SFXIcon[0];
                PlayerPrefs.SetInt("SFXMute", 0);
            }
            // Unmute if SFX is off
            else
            {
                SetVolume("SFXvol", SFXValue);
                sfx = 1;
                theImage.sprite = SFXIcon[1];
                PlayerPrefs.SetInt("SFXMute", 1);
            }
        }

        void SetVolume(string name, float vol)
        {
            // Set the volume in mixer
            // Make sure you expose your audio mixer volume in BGM or SFX
            // And match the expose parameter's name with the name parameter
            mixer.SetFloat(name, vol);
        }
    }
}