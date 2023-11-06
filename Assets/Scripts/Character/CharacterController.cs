using MMSGP.Selection;
using MMSGP.Units;
using UnityEngine;

namespace MMSGP.Movement
{
    public class CharacterController : MonoBehaviour
    {
        private Camera _camera;
        private SelectionManager _selectionManager;
        
        private void Awake()
        {
            _camera = Camera.main;
            _selectionManager = GetComponent<SelectionManager>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    var selectedObjects = _selectionManager.GetSelectedObjects();
                    foreach (var selectedObject in selectedObjects)
                    {
                        var selectableUnit = selectedObject.Value.GetComponent<SelectableUnit>();
                        if (selectableUnit != null)
                        {
                            selectableUnit.MoveTo(hitInfo.point);
                        }
                    }
                }
            }
        }
    }
}