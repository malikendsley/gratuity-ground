using UnityEngine;
using UnityEngine.UI;

namespace Endsley
{
    public class DebugUIController : MonoBehaviour
    {
        public Button button1;
        public Button button2;
        public Button button3;
        public Button button4;
        public Button button5;
        public Button button6;

        void Start()
        {
            button1.onClick.AddListener(Button1Clicked);
            button2.onClick.AddListener(Button2Clicked);
            button3.onClick.AddListener(Button3Clicked);
            button4.onClick.AddListener(Button4Clicked);
            button5.onClick.AddListener(Button5Clicked);
            button6.onClick.AddListener(Button6Clicked);
        }

        void Button1Clicked()
        {
            Debug.Log("Button 1 clicked");
            // Call your function here
        }

        void Button2Clicked()
        {
            Debug.Log("Button 2 clicked");
            // Call your function here
        }

        void Button3Clicked()
        {
            Debug.Log("Button 3 clicked");
            // Call your function here
        }

        void Button4Clicked()
        {
            Debug.Log("Button 4 clicked");
            // Call your function here
        }

        void Button5Clicked()
        {
            Debug.Log("Button 5 clicked");
            // Call your function here
        }

        void Button6Clicked()
        {
            Debug.Log("Button 6 clicked");
            // Call your function here
        }
    }
}
