using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelResetter : MonoBehaviour
{
    public GameObject boxPrefab;
    public GameObject[] m_LevelObjectss;
    public Dictionary<Vector3, GameObject> m_LevelObjectPositions;
    public GameObject[] m_LevelPuzzles;
    public ButtonSwitch[] m_Buttons;

    // Store positions of every moveable objects in the level
    void Start()
    {
        // Store box positions
        m_LevelObjectPositions = new Dictionary<Vector3, GameObject>();
        foreach (GameObject obj in m_LevelObjectss)
        {
            m_LevelObjectPositions.Add(obj.transform.position, obj);
        }
    }

    public void Reset()
    {
        Dictionary<Vector3, GameObject> newLevelObjectPositions = new Dictionary<Vector3, GameObject>();
        Debug.Log("Reset box position");
        foreach (KeyValuePair<Vector3, GameObject> objPositionPair in m_LevelObjectPositions)
        {
            /*
            if (objPositionPair.Key.transform.parent != null)
            {
                objPositionPair.Key.transform.parent = null;
            }
            PrefabUtility.RevertObjectOverride(objPositionPair.Key, InteractionMode.AutomatedAction);
            objPositionPair.Key.transform.position = objPositionPair.Value;
            */
            Debug.Log("destroy box");
            Destroy(objPositionPair.Value);
            GameObject newBox = Instantiate(boxPrefab);
            newBox.SetActive(false);
            //newBox.transform.position = objPositionPair.Key + new Vector3(0,0.1f,0);
            StartCoroutine(WaitAndSetPosition(newBox, objPositionPair.Key));
            // Store new position of the box
            newLevelObjectPositions[objPositionPair.Key] = newBox;
        }
        // Replace the old dictionary
        m_LevelObjectPositions = newLevelObjectPositions;

        Debug.Log("Reset button");
        foreach(ButtonSwitch button in m_Buttons)
        {
            button.ResetButton();
        }

        foreach (GameObject patternPuzzle in m_LevelPuzzles)
        {
            PatternPuzzleController controller = patternPuzzle.GetComponent<PatternPuzzleController>();
            if (controller)
            {
                controller.Reset();
            }
        }
    }

    private IEnumerator WaitAndSetPosition(GameObject obj, Vector3 pos)
    {
        yield return new WaitForEndOfFrame();
        obj.transform.position = pos;
        obj.SetActive(true);
        Debug.Log("position set");
    }
}
