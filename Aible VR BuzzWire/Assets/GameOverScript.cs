using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

    public class GameOverScript : MonoBehaviour
    {
        public Text successText;
        public Text pointsText;
        public Text LevelofAssistnaceText;

    public void Setup(int score, float percent, float duration)
    {

        gameObject.SetActive(true);
        //pointsText.text = "Total Score:" + score.ToString() + " Points";
        //successText.text = "Overall Success Rate:" + percent.ToString("F2") + "%";

        gameObject.SetActive(true);
        pointsText.text = "Total Score:" + score.ToString() + " Points";

        successText.text = "Duration:" + duration.ToString("F2") + " mins"; ;

        LevelofAssistnaceText.text = "Level of Assistance:" + percent.ToString("F2") + "%";


    }

    public void RestartButton()
        {

            //SceneManager.LoadScene("VRHand_ExoInput_SnackBarDemoLeftL1_Test");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            
        }

        public void ExitButton()
        {
            //SceneManager.LoadScene("Menu");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Debug.Log("Quit Game");
            Application.Quit();
        }
    }
