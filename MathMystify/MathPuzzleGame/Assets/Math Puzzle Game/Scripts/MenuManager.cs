using UnityEngine;
using System.Collections;

namespace Ververg
{
    public class MenuManager : MonoBehaviour
    {

        public GameObject[] MenuButtons;
        [SerializeField]
        private float SpawnDelay = 0.2f;
        private bool isMainMenu = true;

        void Start()
        {
            Spawn();
        }

        public void Spawn()
        {
            HiddenButtons();
            StartCoroutine(SpawnButton());
        }

        void HiddenButtons()
        {
            foreach (GameObject button in MenuButtons)
            {
                if (button.GetComponentInChildren<StatsDisplay>())
                    button.GetComponentInChildren<StatsDisplay>().UpdateText();
                button.SetActive(false);
            }
        }

        void Update()
        {
            if (isMainMenu && Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void SetIsMainMenu(bool b)
        {
            isMainMenu = b;
        }

        IEnumerator SpawnButton()
        {
            foreach (GameObject button in MenuButtons)
            {
                button.SetActive(true);
                button.GetComponent<GridButton>().ActiveAudio();
                yield return new WaitForSeconds(SpawnDelay);
            }
        }
    }
}