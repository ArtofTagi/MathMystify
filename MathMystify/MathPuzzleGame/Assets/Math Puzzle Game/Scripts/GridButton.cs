using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Ververg {
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Button))]
    public class GridButton : MonoBehaviour {

        public AudioClip[] ButtonPop;           // Clips when button show.
        public AudioClip ButtonPressed;         // Clip when button pressed.
        public AudioClip ButtonAnswered;        // Clip when player succedd answered

        public AudioSource audioSource;         // Audio source that will play the clip

        private string number;                  // Variable to store the button number
        private bool isAnswer = false;          // Variable to store if its the answer button or not
        private bool isSelected = false;        // Variable to store if its currently selected or not
        private bool isAnswered = false;        // Variable to store if its answered or not

        private Animator anim;
        private Button button;

        void Start() {
            // Make sure this button has audio source
            if (audioSource == null)
            {
                Debug.LogError("Please attach audio source!");
            }

            anim = GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogError("Please attach animator in this Game Object!");
            }

            button = GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("Please attach button in this Game Object!");
            }

            // Play the pops clip randomly
            int rand = Random.Range(0, ButtonPop.Length);
            PlayAudio(ButtonPop[rand]);
        }

        void Update() {
            // Do Nothing
        }

        public void ActiveAudio()
        {
            int rand = Random.Range(0, 2);
            PlayAudio(ButtonPop[rand]);
        }

        void PlayAudio(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void Choose()
        {
            // If its already selected then cancel
            if (isSelected)
            {
                Cancel();
            }
            // If the button is operand
            else if (GetNumber() == "+" || GetNumber() == "-" || GetNumber() == "*" || GetNumber() == "/")
            {
                // Choose if operand is not selected and no other operand currently selected
                if (!GUIManager.Instance.GetSelectedOperand())
                {
                    if (!isSelected)
                    {
                        PlayAudio(ButtonPressed);
                        GUIManager.Instance.Choose(this);
                        anim.SetBool("Chosen", true);
                        isSelected = true;
                    }
                    else
                    {
                        Cancel();
                    }
                }
            }
            // If there is slot to the number grid
            else if (!GUIManager.Instance.GetSelected1() || !GUIManager.Instance.GetSelected2())
            {
                PlayAudio(ButtonPressed);
                if (!isSelected)
                {
                    GUIManager.Instance.Choose(this);
                    anim.SetBool("Chosen", true);
                    isSelected = true;
                }
            }
        }

        void Cancel()
        {
            // Cancel this button from the selected
            PlayAudio(ButtonPressed);
            GUIManager.Instance.Cancel(this);
            anim.SetBool("Chosen", false);
            isSelected = false;
        }

        public void SetNumber(string num)
        {
            number = num;
        }

        public string GetNumber()
        {
            return number;
        }

        public bool GetIsAnswer()
        {
            return isAnswer;
        }

        public void SetIsAnswer(bool b)
        {
            isAnswer = b;
        }

        public void SetAnswered(bool b)
        {
            isAnswered = b;
            // Button that answered is not interactable
            if (isAnswered)
            {
                PlayAudio(ButtonAnswered);
                anim.SetBool("Answered", true);
                button.interactable = false;
            }
            // Button thats not answered is interactable
            else
            {
                anim.SetBool("Answered", false);
                anim.SetBool("Chosen", false);
                button.interactable = true;
                GUIManager.Instance.Cancel(this);
                isSelected = false;
            }
        }

        public bool GetAnswered()
        {
            return isAnswered;
        }
    }
}