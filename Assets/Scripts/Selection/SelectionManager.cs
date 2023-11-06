using System.Collections.Generic;
using Fusion;
using MMSGP.Managers;
using MMSGP.Network;
using MMSGP.Units;
using UnityEngine;
using NetworkPlayer = MMSGP.Network.NetworkPlayer;

namespace MMSGP.Selection
{
    [RequireComponent(typeof(Rigidbody))]
    public class SelectionManager : NetworkBehaviour, ISelection
    {

        [SerializeField] private LayerMask selectableLayer;

        private Dictionary<int, GameObject> selectedTable = new Dictionary<int, GameObject>();
        private RaycastHit _hit;
        private Camera _camera;
        private bool _dragSelect;
        private MeshCollider _selectionBox;
        private Mesh _selectionMesh;
        private Vector3 _p1;
        private Vector3 _p2;
        private Vector2[] _corners;
        private Vector3[] _verts;
        private Vector3[] _vecs;
        private NetworkPlayer _networkPlayer;
        
        private void Start()
        {
            _camera = Camera.main;
            _dragSelect = false;
            _networkPlayer = GetComponent<NetworkPlayer>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _p1 = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                if ((_p1 - Input.mousePosition).magnitude > 40)
                {
                    _dragSelect = true;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_dragSelect == false) //single select
                {
                    SingleSelect();
                }
                else
                {
                    MarqueeSelect();
                }

                _dragSelect = false;
            }
        }

        private void SingleSelect()
        {
            Ray ray = _camera.ScreenPointToRay(_p1);

            if (Physics.Raycast(ray, out _hit, 50000.0f, selectableLayer))
            {
                if (Input.GetKey(KeyCode.LeftShift)) //inclusive select
                {
                    AddSelected(_hit.transform.gameObject);
                }
                else //exclusive selected
                {
                    DeselectAll();
                    AddSelected(_hit.transform.gameObject);
                }
            }
            else //if we didnt hit something
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    DeselectAll();
                }
            }
        }

        private void MarqueeSelect()
        {
            _verts = new Vector3[4];
            _vecs = new Vector3[4];
            int i = 0;
            _p2 = Input.mousePosition;
            _corners = SelectionHelpers.GetBoundingBox(_p1, _p2);

            foreach (Vector2 corner in _corners)
            {
                Ray ray = _camera.ScreenPointToRay(corner);

                if (Physics.Raycast(ray, out _hit, 50000.0f))
                {
                    _verts[i] = new Vector3(_hit.point.x, _hit.point.y, _hit.point.z);
                    _vecs[i] = ray.origin - _hit.point;
                    Debug.DrawLine(_camera.ScreenToWorldPoint(corner), _hit.point, Color.red, 1.0f);
                }

                i++;
            }
            
            _selectionMesh = SelectionHelpers.GenerateSelectionMesh(_verts, _vecs);
            _selectionBox = SelectionHelpers.AddMeshCollider(gameObject, _selectionMesh);

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                DeselectAll();
            }

            Destroy(_selectionBox, 0.1f);
        }

        public void AddSelected(GameObject go)
        {
            if (go == null) return;
            var unit = go.GetComponent<Unit>();
            if (unit.OwnerPlayerId != _networkPlayer.PlayerId) return;

            int id = go.GetInstanceID();

            if (!(selectedTable.ContainsKey(id)))
            {
                if (go.GetComponent<Unit>() != null)
                {
                    selectedTable.Add(id, go);
                    go.GetComponent<Unit>().SetIsSelected(true);
                    Debug.Log("Added " + id + " to selected dict");
                }
            }
        }

        public void DeselectAll()
        {
            foreach (KeyValuePair<int, GameObject> entry in selectedTable)
            {
                entry.Value.GetComponent<Unit>().SetIsSelected(false);
            }

            selectedTable.Clear();        
        }
        
        public Dictionary<int, GameObject> GetSelectedObjects()
        {
            return selectedTable;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (HasInputAuthority == false) return;

            AddSelected(other.gameObject);
        }

        private void OnGUI()
        {
            if (!_dragSelect) return;
            var rect = Utils.GetScreenRect(_p1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }
}