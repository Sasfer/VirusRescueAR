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
        public GameObject RestartButton;
        public GameObject ExitButton;
        public GameObject MenuButton;
        public GameObject MenuPanel;

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
            MenuPanel.SetActive(isShow);
        }

        public void ShowInfo()
        {
            ShowInfoPanel(true);
        }
        
        public void HideInfo()
        {
            ShowInfoPanel(false);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene("PruebaARF");
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}