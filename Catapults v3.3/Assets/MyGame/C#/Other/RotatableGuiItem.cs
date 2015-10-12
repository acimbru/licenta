using UnityEngine;

public class RotatableGuiItem : MonoBehaviour {
 
    public Texture2D clockArm = null;
	public Texture2D clockBody = null;
    public float angle = 0;
	private Vector2 position = new Vector2(Screen.width/2,70);
    public Vector2 size = new Vector2(128, 128);
    Vector2 pos = new Vector2(0, 0);
    Rect rect;
    Vector2 pivot;
 
    void Start() {
		this.transform.localPosition = position;
        UpdateSettings();
    }
 
    void UpdateSettings() {
        pos = new Vector2(transform.localPosition.x, transform.localPosition.y);
        rect = new Rect(pos.x - size.x * 0.5f, pos.y - size.y * 0.5f, size.x, size.y);
        pivot = new Vector2(rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f);
    }
 
    void OnGUI() {
        if (Application.isEditor) { UpdateSettings(); }
        GUI.DrawTexture(rect, clockBody);
		Matrix4x4 matrixBackup = GUI.matrix;
        GUIUtility.RotateAroundPivot(angle, pivot);
		GUI.DrawTexture(rect, clockArm);
        GUI.matrix = matrixBackup;
    }
}