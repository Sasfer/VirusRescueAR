using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace NewHelicopter
{
    public class UIViewController : MonoBehaviour
    {
        public GameObject PlayerUI;

        [Header("FlightView")]
        public Text HeigthView;
        public Text EngineForceView;
        public Text Damage;
        public Text Person;
        public Text Time;
        public Text Level;
        public Text Capacity;
        public GameObject RestartButton;
        public GameObject ExitButton;
        public GameObject MenuButton;
        public GameObject MenuPanel;
        public GameObject JuegoObjetos;
        public GameObject Data;

        private bool Inicio = true;

        public static UIViewController runtime;

        void Awake()
        {
            runtime = this;
        }

        public void SetPlayer(Transform player)
        {
            PlayerUI.SetActive(true);
        }

      
        public void DeletePlayer(Transform player)
        {
            
        }

        void Start()
        {
            ShowInfo();
        }


        private void ShowInfoPanel(bool isShow)
        {
            RestartButton.SetActive(!isShow);
            ExitButton.SetActive(!isShow);
            MenuButton.SetActive(!isShow);
            JuegoObjetos.SetActive(!isShow);
            MenuPanel.SetActive(isShow);

            if(Inicio == false)
            {
                Data.SetActive(!isShow);
            }
        }

        public void ShowInfo()
        {
            ShowInfoPanel(true);
        }
        
        public void HideInfo()
        {
            ShowInfoPanel(false);

            if(Inicio == true)
            {
                Inicio = false;
            }
        }

        public void PauseGame()
        {
            ShowInfoPanel(true);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene("GameVirusRescueAR");
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}