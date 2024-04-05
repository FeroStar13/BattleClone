#region Namespaces/Directives

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class CameraMov : MonoBehaviour
{
    #region Declarations

    [Header("Camera Movement Settings")]
    [SerializeField] float _mouseSensitivity;
    [SerializeField] float _cameraCurrentX = 0;
    Vector2 mouseDelta;
    [SerializeField] private Vector3 offset;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void LateUpdate()
    {
        UpdateMouseLook();
    }


    #endregion

    void UpdateMouseLook()
    {
        mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * _mouseSensitivity;

        _cameraCurrentX -= mouseDelta.y;

        _cameraCurrentX = Mathf.Clamp(_cameraCurrentX, -90, 90);

        Camera.main.transform.localEulerAngles = Vector3.right * _cameraCurrentX;
        transform.Rotate(Vector3.up * mouseDelta.x);

       
    }
}
