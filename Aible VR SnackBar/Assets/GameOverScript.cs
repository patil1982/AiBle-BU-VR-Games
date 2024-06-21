using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PW
{
    public class GameOverScript : MonoBehaviour
    {
        public Text successText;
        public Text pointsText;
        public Text LevelofAssistnaceText;

        public int gamescore = 0;
        public double gameduration = 0;
        public int gameLA = 0;

        public void Setup(int score, int percent, float duration)
        {

            gamescore = score;
            gameduration = duration;
            gameLA = percent;

            gameObject.SetActive(true);
            //pointsText.text = "Total Score:" + score.ToString() + " Points";
            //successText.text = "Overall Success Rate:" + percent.ToString("F2") + "%";

            gameObject.SetActive(true);
            pointsText.text = "Total Score:" + score.ToString() + " Points";

            successText.text = "Duration:" + duration.ToString("F2") + " mins"; ;

            LevelofAssistnaceText.text = "Level of Assistance:" + 100.ToString("F2") + "%";


        }

        public int GetGameScore()
        {
            return gamescore;
            
        }

        public int GetGameLA()
        {
            return gameLA;

        }

        public double GetGameDuration()
        {
            return gameduration;

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
}
