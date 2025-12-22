using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBox : MonoBehaviour
{

    [SerializeField, Range(0, 1f)] private float colliderSizeDownScale = 1f;
    [SerializeField] private GameObject dashedBox;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private Coroutine coroutine;

    private int clickInput;
    void Awake()
    {
        List<GameObject> selectedObjects = new List<GameObject>();
        spriteRenderer = dashedBox.GetComponent<SpriteRenderer>();
        boxCollider = dashedBox.GetComponent<BoxCollider2D>();

        InputManager.Instance.OnLeftClick += DashedBoxToLeftClick;
    }

    private void DashedBoxToLeftClick(Vector3 position)
    {
        dashedBox.transform.position = position;
        spriteRenderer.size = new Vector2(0f, 0f);
        dashedBox.SetActive(true);
        coroutine = StartCoroutine(BoxStretchToPointer());
    }

    // Conforms the dashed box always between dragging points.
    private IEnumerator BoxStretchToPointer()
    {
        float leftClick = InputManager.Instance.DetectLeftClickChange();
        float rightClick = InputManager.Instance.DetectRightClickChange();
        Vector3 followPointer;

        while (true) 
        {
            if (leftClick != InputManager.Instance.DetectLeftClickChange())
            {
                print("left click change!");
                break;
            }

            if (rightClick != InputManager.Instance.DetectRightClickChange())
            {
                print("right click change!");
                break; 
            }

            followPointer = InputManager.Instance.PointerWorldPosition();
            spriteRenderer.size = (followPointer - dashedBox.transform.position) / 2;

            // THEORY: the difference between bounds.size and .size is that bounds.size is the absolute value.
            boxCollider.size = (spriteRenderer.bounds.size / 2) * colliderSizeDownScale; 
            boxCollider.offset = new Vector2(spriteRenderer.size.x / 2, spriteRenderer.size.y / 2);
            


            yield return null;
        }
        
        // reset.
        dashedBox.SetActive(false);
    }
}
