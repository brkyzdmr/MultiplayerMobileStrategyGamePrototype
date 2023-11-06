    using System;
    using UnityEngine;
    using Sirenix.OdinInspector;

    namespace MMSGP.CameraMovement
    {
        public class CameraController : MonoBehaviour
        {
            private Transform cameraTransform;

            [BoxGroup("Horizontal Translation")] [SerializeField] private float maxSpeed = 5f;
            [BoxGroup("Horizontal Translation")] [SerializeField] private float acceleration = 10f;
            [BoxGroup("Horizontal Translation")] [SerializeField] private float damping = 15f;

            [BoxGroup("Vertical Translation")] [SerializeField] private float stepSize = 2f;
            [BoxGroup("Vertical Translation")] [SerializeField] private float zoomDampening = 7.5f;
            [BoxGroup("Vertical Translation")] [SerializeField] private float minHeight = 5f;
            [BoxGroup("Vertical Translation")] [SerializeField] private float maxHeight = 50f;
            [BoxGroup("Vertical Translation")] [SerializeField] private float zoomSpeed = 2f;
            
            [BoxGroup("Edge Movement")] [SerializeField] [Range(0f, 0.1f)] private float edgeTolerance = 0.05f;

            private Vector3 _targetPosition;
            private float _zoomHeight;
            private Vector3 _horizontalVelocity;
            private Vector3 _lastPosition;
            private Vector3 _startDrag;
            private float _speed;
            private Camera _camera;

            private void Awake()
            {
                _camera = GetComponentInChildren<Camera>();
                cameraTransform = _camera.transform;
            }

            private void OnEnable()
            {
                _zoomHeight = cameraTransform.localPosition.y;
                cameraTransform.LookAt(transform);

                _lastPosition = transform.position;
            }
            

            private void Update()
            {
                //inputs
                GetKeyboardMovement();
                // CheckMouseAtScreenEdge();
                DragCamera();
                ZoomCamera();

                //move base and camera objects
                UpdateVelocity();
                UpdateBasePosition();
                UpdateCameraPosition();
            }

            private void UpdateVelocity()
            {
                _horizontalVelocity = (transform.position - _lastPosition) / Time.deltaTime;
                _horizontalVelocity.y = 0f;
                _lastPosition = transform.position;
            }

            private void GetKeyboardMovement()
            {
                Vector3 inputValue = Input.GetAxis("Horizontal") * GetCameraRight()
                                     + Input.GetAxis("Vertical") * GetCameraForward();

                inputValue = inputValue.normalized;

                if (inputValue.sqrMagnitude > 0.1f)
                    _targetPosition += inputValue;
            }

            private void CheckMouseAtScreenEdge()
            {
                //mouse position is in pixels
                Vector2 mousePosition = Input.mousePosition;
                Vector3 moveDirection = Vector3.zero;

                //horizontal scrolling
                if (mousePosition.x < edgeTolerance * Screen.width)
                    moveDirection += -GetCameraRight();
                else if (mousePosition.x > (1f - edgeTolerance) * Screen.width)
                    moveDirection += GetCameraRight();

                //vertical scrolling
                if (mousePosition.y < edgeTolerance * Screen.height)
                    moveDirection += -GetCameraForward();
                else if (mousePosition.y > (1f - edgeTolerance) * Screen.height)
                    moveDirection += GetCameraForward();

                _targetPosition += moveDirection;
            }

            private void UpdateBasePosition()
            {
                if (_targetPosition.sqrMagnitude > 0.1f)
                {
                    //create a ramp up or acceleration
                    _speed = Mathf.Lerp(_speed, maxSpeed, Time.deltaTime * acceleration);
                    transform.position += _targetPosition * _speed * Time.deltaTime;
                }
                else
                {
                    //create smooth slow down
                    _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
                    transform.position += _horizontalVelocity * Time.deltaTime;
                }

                //reset for next frame
                _targetPosition = Vector3.zero;
            }

            private void ZoomCamera()
            {
                float inputValue = Input.mouseScrollDelta.y;

                if (Mathf.Abs(inputValue) > 0.1f)
                {
                    _zoomHeight = cameraTransform.localPosition.y + inputValue * stepSize;

                    if (_zoomHeight < minHeight)
                        _zoomHeight = minHeight;
                    else if (_zoomHeight > maxHeight)
                        _zoomHeight = maxHeight;
                }
            }
            
            private void DragCamera()
            {
                if (!Input.GetMouseButton(2))
                    return;

                //create plane to raycast to
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        
                if(plane.Raycast(ray, out float distance))
                {
                    if (Input.GetMouseButtonDown(2))
                        _startDrag = ray.GetPoint(distance);
                    else
                        _targetPosition += _startDrag - ray.GetPoint(distance);
                }
            }

            private void UpdateCameraPosition()
            {
                //set zoom target
                Vector3 zoomTarget = new Vector3(cameraTransform.localPosition.x, _zoomHeight,
                    cameraTransform.localPosition.z);
                //add vector for forward/backward zoom
                zoomTarget -= zoomSpeed * (_zoomHeight - cameraTransform.localPosition.y) * Vector3.forward;

                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomTarget,
                    Time.deltaTime * zoomDampening);
                cameraTransform.LookAt(this.transform);
            }

            //gets the horizontal forward vector of the camera
            private Vector3 GetCameraForward()
            {
                Vector3 forward = cameraTransform.forward;
                forward.y = 0f;
                return forward;
            }

            //gets the horizontal right vector of the camera
            private Vector3 GetCameraRight()
            {
                Vector3 right = cameraTransform.right;
                right.y = 0f;
                return right;
            }
        }
    }