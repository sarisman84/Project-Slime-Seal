using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Input : MonoBehaviour
    {
        public InputActionReference movementActionRef, jumpActionRef, interactionActionRef;

        // Start is called before the first frame update
        private void OnEnable()
        {
            SetReferencesActive(true, movementActionRef, jumpActionRef, interactionActionRef);
        }

        private void OnDisable()
        {
            SetReferencesActive(false, movementActionRef, jumpActionRef, interactionActionRef);
        }

        private void SetReferencesActive(bool state, params InputActionReference[] inputAction)
        {
            for (int i = 0; i < inputAction.Length; i++)
            {
                if (state)
                {
                    inputAction[i].action.Enable();
                    continue;
                    ;
                }

                inputAction[i].action.Disable();
            }
        }

        public enum InputType
        {
            Jump,
            Interact
        }


        public Vector3 GetInputMovementRaw(AxisType type)
        {
            switch (type)
            {
                case AxisType.Axis3D:
                    Vector2 dir = movementActionRef.action.ReadValue<Vector2>();
                    return new Vector3(dir.x, 0, dir.y);
                case AxisType.Axis2D:
                    return movementActionRef.action.ReadValue<Vector2>();
            }

            return default;
        }

        public bool GetButton(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Jump:
                    return jumpActionRef.action.ReadValue<float>() > 0;
                case InputType.Interact:
                    return interactionActionRef.action.ReadValue<float>() > 0;
            }

            return default;
        }

        public bool GetButtonDown(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Jump:
                    return jumpActionRef.action.ReadValue<float>() > 0 && jumpActionRef.action.triggered;
                case InputType.Interact:
                    return interactionActionRef.action.ReadValue<float>() > 0 && interactionActionRef.action.triggered;
            }

            return default;
        }
    }

    public enum AxisType
    {
        Axis3D,
        Axis2D
    }
}