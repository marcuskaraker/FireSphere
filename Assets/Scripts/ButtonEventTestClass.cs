using UnityEngine;
using UnityEngine.UI;

// Maybe this is a UIManager or something similar.
public class ButtonEventTestClass : MonoBehaviour
{
    public Button buttonPrefab;
    public LayoutGroup layoutParent;

    public void SpawnButtons(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Button spawnedButton = Instantiate(buttonPrefab, layoutParent.transform);

            int index = i;
            spawnedButton.onClick.AddListener(delegate { DoThisSpecificThing(spawnedButton, index); });
        }
    }

    public void DoThisSpecificThing(Button button, int index)
    {
        // Do stuff here, you can use the parameter(s) to create specific functionality for each button.
        // For example maybe you want each button to live a little longer than the last:
        Destroy(button.gameObject, index * 2);
    }
}
