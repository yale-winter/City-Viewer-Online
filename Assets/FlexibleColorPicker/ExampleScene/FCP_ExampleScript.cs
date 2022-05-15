using UnityEngine;

public class FCP_ExampleScript : MonoBehaviour {

    public FlexibleColorPicker fcp;
    public Material material;

    private void Start() {
        fcp.onColorChange.AddListener(OnChangeColor);
    }

    private void OnChangeColor(Color co) {
        material.color = co;
    }
}
