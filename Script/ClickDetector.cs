using UnityEngine;
using System.Collections;

public class ClickDetector : MonoBehaviour
{
	public bool requireDownHit = false;
	public string onMouseDownMethodName = "down";

	public bool requireDragHit = false;
	public bool allowDragDuplicate = true;
	public string onMouseDragMethodName = "drag";

	public bool requireUpHit = false;
	public string onMouseUpMethodName = "up";

	public bool requireMouseHit = false;
	public string onMouseClickMethodName = "click";
	public LayerMask layerMask;

	private GameObject clickingElement = null;
	private GameObject draggedElement = null;

	public void z_down() {
		GameObject gameObject = GetClickedGameObject ();
		if (!requireDownHit || gameObject) {
			this.gameObject.SendMessage (onMouseDownMethodName, gameObject, SendMessageOptions.DontRequireReceiver);
			clickingElement = gameObject;
		}
		draggedElement = null;
	}

	public void z_drag() {
		GameObject gameObject = GetClickedGameObject ();
		if (!requireDragHit || gameObject && (allowDragDuplicate || draggedElement != gameObject)) {
			this.gameObject.SendMessage (onMouseDragMethodName, gameObject, SendMessageOptions.DontRequireReceiver);
			if (clickingElement != gameObject) {
				clickingElement = null;
			}
		}
		draggedElement = gameObject;
	}

	public void z_up() {
		GameObject gameObject = GetClickedGameObject ();
		if (!requireUpHit || gameObject) {
			this.gameObject.SendMessage (onMouseUpMethodName, gameObject, SendMessageOptions.DontRequireReceiver);
			if (clickingElement == gameObject) {
				this.z_click();
			}
		}
	}

	public void z_click() {
		GameObject gameObject = clickingElement;
		if (!requireMouseHit || gameObject) {
			this.gameObject.SendMessage (onMouseClickMethodName, gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	GameObject GetClickedGameObject()
    {
        // Builds a ray from camera point of view to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Casts the ray and get the first game object hit
        if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
			return hit.transform.gameObject;
		} else {
			return null;
		}
    }
}
