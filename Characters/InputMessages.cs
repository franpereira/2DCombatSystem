using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Konrad.Characters
{
    /// <summary>
    /// Receives messages from the InputSystem from a PlayerInput component and sends them to the Character Controller.
    /// </summary>
    public class InputMessages : MonoBehaviour
    {
        Character _character;
        bool _defending;
        
        void Awake() => _character = GetComponent<Character>();

        void OnAxis(InputValue value) => _character.Move(value.Get<Vector2>());

        void OnJump() => _character.Jump();

        void OnAttack() => _character.Attack();

        void OnDefense(InputValue value) => _defending = value.isPressed;

        void OnRestart() => SceneManager.LoadScene(0);

        void Update() => _character.Defend(_defending);
    }
}
