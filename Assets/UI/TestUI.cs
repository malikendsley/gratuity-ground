using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Endsley
{
    public class DebugUIController : MonoBehaviour
    {
        public Button button1;

        public GameObject AIBeacon;
        public GameObject testMech;
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
            button6.GetComponentInChildren<TextMeshProUGUI>().text = "Squads Can Spawn: " + (SquadManager.Instance.spawningAllowed ? "Yes" : "No");

        }

        void Button1Clicked()
        {
            if (AIBeacon == null)
            {
                Debug.LogWarning("No AIBeacon set on DebugUIController");
                return;
            }
            if (testMech.TryGetComponent(out MovementAI ai))
            {
                ai.MoveToTarget(AIBeacon.transform.position);
            }
            else
            {
                Debug.LogWarning("No MovementAI found on testMech");
            }
        }

        void Button2Clicked()
        {
            if (testMech.TryGetComponent(out MechController mech))
            {
                mech.RotateToHeading(90f);
            }
            else
            {
                Debug.LogWarning("No MechController found on testMech");
            }
        }

        void Button3Clicked()
        {
            if (testMech.TryGetComponent(out MechController mech))
            {
                mech.RotateToHeading(-90f);
            }
            else
            {
                Debug.LogWarning("No MechController found on testMech");
            }
        }

        void Button4Clicked()
        {
            if (testMech.TryGetComponent(out MechController mech))
            {
                mech.RotateToTarget(AIBeacon.transform.position);
            }
            else
            {
                Debug.LogWarning("No MechController found on testMech");
            }
        }

        void Button5Clicked()
        {
            // Spawn a new squad of enemies (the debug location is set in the SquadManager)
            // NOTE: If this suddenly starts behaving weirdly, it's probably because the debug location was removed
            Debug.Log("Button 5 clicked");
            SquadManager.Instance.SpawnNewSquad();
        }

        void Button6Clicked()
        {
            Debug.Log("Button 6 clicked");
            SquadManager.Instance.spawningAllowed = !SquadManager.Instance.spawningAllowed;
            Debug.Log("SquadManager.Instance.spawningAllowed = " + SquadManager.Instance.spawningAllowed);
            // Edit the text of the button to show current state
            button6.GetComponentInChildren<TextMeshProUGUI>().text = "Spawning Squads: " + (SquadManager.Instance.spawningAllowed ? "Yes" : "No");
        }
    }
}
