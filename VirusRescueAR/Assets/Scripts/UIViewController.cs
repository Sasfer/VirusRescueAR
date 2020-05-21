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
        public GameObject moveCel;

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
            RestartButton.SetActive(false);
            ExitButton.SetActive(false);
            MenuButton.SetActive(false);
            JuegoObjetos.SetActive(false);
            MenuPanel.SetActive(true);
        }
        
        public void HideInfo()
        {
            if(Inicio == true)
            {
                RestartButton.SetActive(true);
                ExitButton.SetActive(true);
                MenuButton.SetActive(false);
                JuegoObjetos.SetActive(true);
                MenuPanel.SetActive(false);
                moveCel.SetActive(true);
                Inicio = false;
            }
            else
            {
                RestartButton.SetActive(true);
                ExitButton.SetActive(true);
                MenuButton.SetActive(true);
                JuegoObjetos.SetActive(true);
                MenuPanel.SetActive(false);
                Data.SetActive(true);
                moveCel.SetActive(false);
            }
           
        }

        public void PauseGame()
        {
            RestartButton.SetActive(false);
            ExitButton.SetActive(false);
            MenuButton.SetActive(false);
            JuegoObjetos.SetActive(true);
            MenuPanel.SetActive(true);
            Data.SetActive(false);
        }

        public void NextLevel(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void RestartGame(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}