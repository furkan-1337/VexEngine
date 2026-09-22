using Silk.NET.Input;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Vex.Debugging;

namespace Vex.Engine
{
    public static class Input
    {
        private static IInputContext? _inputContext;

        private static IKeyboard? _keyboard;
        private static readonly bool[] _currentKeys = new bool[512];
        private static readonly bool[] _previousKeys = new bool[512];

        private static IMouse? _mouse;
        private static readonly bool[] _currentMouseButtons = new bool[16];
        private static readonly bool[] _previousMouseButtons = new bool[16];
        private static Vector2 _currentMousePosition;
        private static Vector2 _previousMousePosition;
        private static Vector2 _scrollDelta;

        public static Vector2 MousePosition => _currentMousePosition;
        public static Vector2 MouseDelta => _currentMousePosition - _previousMousePosition;
        public static Vector2 ScrollDelta => _scrollDelta;

        internal static void Initialize(IWindow window)
        {
            _inputContext = window.CreateInput();
            if (_inputContext == null)
                throw new VexInvalidStateException("Failed to initialize Input: Input context could not be created from window.");

            _keyboard = _inputContext.Keyboards.Count > 0 ? _inputContext.Keyboards[0] : null;
            _mouse = _inputContext.Mice.Count > 0 ? _inputContext.Mice[0] : null;

            if (_keyboard != null)
            {
                _keyboard.KeyDown += (kb, key, scancode) => {
                    if ((int)key < 512) _currentKeys[(int)key] = true;
                };
                _keyboard.KeyUp += (kb, key, scancode) => {
                    if ((int)key < 512) _currentKeys[(int)key] = false;
                };
            }
            else
            {
                throw new VexInvalidStateException("Failed to initialize Input: No compatible keyboard device detected on the system.");
            }

            if (_mouse != null)
            {
                _mouse.MouseDown += (mouse, button) =>
                {
                    int index = (int)button;
                    if (index >= 0 && index < 16)
                        _currentMouseButtons[index] = true;
                };
                _mouse.MouseUp += (mouse, button) =>
                {
                    int index = (int)button;
                    if (index >= 0 && index < 16)
                        _currentMouseButtons[index] = false;
                };
                _mouse.MouseMove += (mouse, position) =>
                {
                    _currentMousePosition = new Vector2(position.X, position.Y);
                };
                _mouse.Scroll += (mouse, scrollWheel) =>
                {
                    _scrollDelta = new Vector2(scrollWheel.X, scrollWheel.Y);
                };
            }
            else
            {
                throw new VexInvalidStateException("Failed to initialize Input: No compatible mouse device detected on the system.");
            }
        }

        internal static void Update()
        {
            Array.Copy(_currentKeys, _previousKeys, _currentKeys.Length);
            Array.Copy(_currentMouseButtons, _previousMouseButtons, _currentMouseButtons.Length);
            _previousMousePosition = _currentMousePosition;
            _scrollDelta = Vector2.Zero;
        }

        public static bool IsKeyDown(Key key)
        {
            int index = (int)key;
            if (index < 0 || index >= 512) return false;
            return _currentKeys[index];
        }

        public static bool IsKeyPressed(Key key)
        {
            int index = (int)key;
            if (index < 0 || index >= 512) return false;
            return _currentKeys[index] && !_previousKeys[index];
        }

        public static bool IsKeyReleased(Key key)
        {
            int index = (int)key;
            if (index < 0 || index >= 512) return false;
            return !_currentKeys[index] && _previousKeys[index];
        }

        public static bool IsButtonDown(MouseButton button)
        {
            int index = (int)button;
            if (index < 0 || index >= 16) return false;
            return _currentMouseButtons[index];
        }

        public static bool IsButtonPressed(MouseButton button)
        {
            int index = (int)button;
            if (index < 0 || index >= 16) return false;
            return _currentMouseButtons[index] && !_previousMouseButtons[index];
        }

        public static bool IsButtonReleased(MouseButton button)
        {
            int index = (int)button;
            if (index < 0 || index >= 16) return false;
            return !_currentMouseButtons[index] && _previousMouseButtons[index];
        }

        internal static void Shutdown()
        {
            _inputContext?.Dispose();
        }
    }
}
