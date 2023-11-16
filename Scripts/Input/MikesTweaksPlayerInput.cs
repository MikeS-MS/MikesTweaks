using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace MikesTweaks.Scripts.Input
{
    public partial class @MikesTweaksPlayerInput : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }

        public @MikesTweaksPlayerInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""MikesTweaksPlayerInput"",
    ""maps"": [
        {
            ""name"": ""Hotbar"",
            ""id"": ""acedaef2-b06f-4287-a2ed-00f0260b63da"",
            ""actions"": [
                {
                    ""name"": ""Hotbar1"",
                    ""type"": ""Button"",
                    ""id"": ""afdaf0de-d9cb-4835-93df-06ff8a407e18"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hotbar2"",
                    ""type"": ""Button"",
                    ""id"": ""1cf91964-9e69-4db5-90ae-65b813acbceb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hotbar3"",
                    ""type"": ""Button"",
                    ""id"": ""9787ab53-1dcb-439c-91cb-683e0e6e0fc2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hotbar4"",
                    ""type"": ""Button"",
                    ""id"": ""a63eb6ea-bced-4246-a625-439815fb3c86"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hotbar5"",
                    ""type"": ""Button"",
                    ""id"": ""8810962b-a9c4-4244-9202-05886a4dd217"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hotbar6"",
                    ""type"": ""Button"",
                    ""id"": ""4e4ccd85-b20d-422d-836a-6c30d64e8ada"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hotbar7"",
                    ""type"": ""Button"",
                    ""id"": ""cc6d7e8c-4c89-4a7f-8f69-07d40af41569"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hotbar8"",
                    ""type"": ""Button"",
                    ""id"": ""71e7f8d9-ddd6-45de-acdb-0e427b5cf883"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hotbar9"",
                    ""type"": ""Button"",
                    ""id"": ""9f3aee6a-4474-4e45-895f-4384d31e2651"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""FlashlightToggle"",
                    ""type"": ""Button"",
                    ""id"": ""8b3dca3f-50e1-4bb9-9d22-cb56d9486b78"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""WalkieTalkieSpeak"",
                    ""type"": ""PassThrough"",
                    ""id"": ""1f791eab-e066-4ffe-89cf-4ea0362c9cb8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""75fed245-d23a-4c28-842f-5f5c2e244087"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hotbar1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e3020d01-d049-4c17-98a4-c8e7b52e6f7e"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hotbar2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd0c3e9e-f967-4970-91eb-611774d404f3"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hotbar3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""201a61fa-e2d7-4c32-a293-89f48ce8b772"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hotbar4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0b0ce1b1-d25e-4e30-97d3-24b67a08b018"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hotbar5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""91c0c96d-9e51-424c-a6ae-150e99cacde5"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hotbar6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de949a83-e142-4d7e-96b4-885ec2abdd76"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hotbar7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b40a6f2b-d2a4-4db7-8eb1-9d585882ff0e"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hotbar8"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7089d993-a703-4246-aef3-2bfa18bf49f1"",
                    ""path"": ""<Keyboard>/9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Hotbar9"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""27f1b785-d544-447f-b0ec-29635c1ea7d4"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""FlashlightToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a2470b7-773e-4327-99f9-36648dae43ef"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WalkieTalkieSpeak"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Hotbar
            m_Hotbar = asset.FindActionMap("Hotbar", throwIfNotFound: true);
            m_Hotbar_Hotbar1 = m_Hotbar.FindAction("Hotbar1", throwIfNotFound: true);
            m_Hotbar_Hotbar2 = m_Hotbar.FindAction("Hotbar2", throwIfNotFound: true);
            m_Hotbar_Hotbar3 = m_Hotbar.FindAction("Hotbar3", throwIfNotFound: true);
            m_Hotbar_Hotbar4 = m_Hotbar.FindAction("Hotbar4", throwIfNotFound: true);
            m_Hotbar_Hotbar5 = m_Hotbar.FindAction("Hotbar5", throwIfNotFound: true);
            m_Hotbar_Hotbar6 = m_Hotbar.FindAction("Hotbar6", throwIfNotFound: true);
            m_Hotbar_Hotbar7 = m_Hotbar.FindAction("Hotbar7", throwIfNotFound: true);
            m_Hotbar_Hotbar8 = m_Hotbar.FindAction("Hotbar8", throwIfNotFound: true);
            m_Hotbar_Hotbar9 = m_Hotbar.FindAction("Hotbar9", throwIfNotFound: true);
            m_Hotbar_FlashlightToggle = m_Hotbar.FindAction("FlashlightToggle", throwIfNotFound: true);
            m_Hotbar_WalkieTalkieSpeak = m_Hotbar.FindAction("WalkieTalkieSpeak", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Hotbar
        private readonly InputActionMap m_Hotbar;
        private List<IHotbarActions> m_HotbarActionsCallbackInterfaces = new List<IHotbarActions>();
        private readonly InputAction m_Hotbar_Hotbar1;
        private readonly InputAction m_Hotbar_Hotbar2;
        private readonly InputAction m_Hotbar_Hotbar3;
        private readonly InputAction m_Hotbar_Hotbar4;
        private readonly InputAction m_Hotbar_Hotbar5;
        private readonly InputAction m_Hotbar_Hotbar6;
        private readonly InputAction m_Hotbar_Hotbar7;
        private readonly InputAction m_Hotbar_Hotbar8;
        private readonly InputAction m_Hotbar_Hotbar9;
        private readonly InputAction m_Hotbar_FlashlightToggle;
        private readonly InputAction m_Hotbar_WalkieTalkieSpeak;

        public struct HotbarActions
        {
            private @MikesTweaksPlayerInput m_Wrapper;

            public HotbarActions(@MikesTweaksPlayerInput wrapper)
            {
                m_Wrapper = wrapper;
            }

            public InputAction @Hotbar1 => m_Wrapper.m_Hotbar_Hotbar1;
            public InputAction @Hotbar2 => m_Wrapper.m_Hotbar_Hotbar2;
            public InputAction @Hotbar3 => m_Wrapper.m_Hotbar_Hotbar3;
            public InputAction @Hotbar4 => m_Wrapper.m_Hotbar_Hotbar4;
            public InputAction @Hotbar5 => m_Wrapper.m_Hotbar_Hotbar5;
            public InputAction @Hotbar6 => m_Wrapper.m_Hotbar_Hotbar6;
            public InputAction @Hotbar7 => m_Wrapper.m_Hotbar_Hotbar7;
            public InputAction @Hotbar8 => m_Wrapper.m_Hotbar_Hotbar8;
            public InputAction @Hotbar9 => m_Wrapper.m_Hotbar_Hotbar9;
            public InputAction @FlashlightToggle => m_Wrapper.m_Hotbar_FlashlightToggle;
            public InputAction @WalkieTalkieSpeak => m_Wrapper.m_Hotbar_WalkieTalkieSpeak;

            public InputActionMap Get()
            {
                return m_Wrapper.m_Hotbar;
            }

            public void Enable()
            {
                Get().Enable();
            }

            public void Disable()
            {
                Get().Disable();
            }

            public bool enabled => Get().enabled;

            public static implicit operator InputActionMap(HotbarActions set)
            {
                return set.Get();
            }

            public void AddCallbacks(IHotbarActions instance)
            {
                if (instance == null || m_Wrapper.m_HotbarActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_HotbarActionsCallbackInterfaces.Add(instance);
                @Hotbar1.started += instance.OnHotbar1;
                @Hotbar1.performed += instance.OnHotbar1;
                @Hotbar1.canceled += instance.OnHotbar1;
                @Hotbar2.started += instance.OnHotbar2;
                @Hotbar2.performed += instance.OnHotbar2;
                @Hotbar2.canceled += instance.OnHotbar2;
                @Hotbar3.started += instance.OnHotbar3;
                @Hotbar3.performed += instance.OnHotbar3;
                @Hotbar3.canceled += instance.OnHotbar3;
                @Hotbar4.started += instance.OnHotbar4;
                @Hotbar4.performed += instance.OnHotbar4;
                @Hotbar4.canceled += instance.OnHotbar4;
                @Hotbar5.started += instance.OnHotbar5;
                @Hotbar5.performed += instance.OnHotbar5;
                @Hotbar5.canceled += instance.OnHotbar5;
                @Hotbar6.started += instance.OnHotbar6;
                @Hotbar6.performed += instance.OnHotbar6;
                @Hotbar6.canceled += instance.OnHotbar6;
                @Hotbar7.started += instance.OnHotbar7;
                @Hotbar7.performed += instance.OnHotbar7;
                @Hotbar7.canceled += instance.OnHotbar7;
                @Hotbar8.started += instance.OnHotbar8;
                @Hotbar8.performed += instance.OnHotbar8;
                @Hotbar8.canceled += instance.OnHotbar8;
                @Hotbar9.started += instance.OnHotbar9;
                @Hotbar9.performed += instance.OnHotbar9;
                @Hotbar9.canceled += instance.OnHotbar9;
                @FlashlightToggle.started += instance.OnFlashlightToggle;
                @FlashlightToggle.performed += instance.OnFlashlightToggle;
                @FlashlightToggle.canceled += instance.OnFlashlightToggle;
                @WalkieTalkieSpeak.started += instance.OnWalkieTalkieSpeak;
                @WalkieTalkieSpeak.performed += instance.OnWalkieTalkieSpeak;
                @WalkieTalkieSpeak.canceled += instance.OnWalkieTalkieSpeak;
            }

            private void UnregisterCallbacks(IHotbarActions instance)
            {
                @Hotbar1.started -= instance.OnHotbar1;
                @Hotbar1.performed -= instance.OnHotbar1;
                @Hotbar1.canceled -= instance.OnHotbar1;
                @Hotbar2.started -= instance.OnHotbar2;
                @Hotbar2.performed -= instance.OnHotbar2;
                @Hotbar2.canceled -= instance.OnHotbar2;
                @Hotbar3.started -= instance.OnHotbar3;
                @Hotbar3.performed -= instance.OnHotbar3;
                @Hotbar3.canceled -= instance.OnHotbar3;
                @Hotbar4.started -= instance.OnHotbar4;
                @Hotbar4.performed -= instance.OnHotbar4;
                @Hotbar4.canceled -= instance.OnHotbar4;
                @Hotbar5.started -= instance.OnHotbar5;
                @Hotbar5.performed -= instance.OnHotbar5;
                @Hotbar5.canceled -= instance.OnHotbar5;
                @Hotbar6.started -= instance.OnHotbar6;
                @Hotbar6.performed -= instance.OnHotbar6;
                @Hotbar6.canceled -= instance.OnHotbar6;
                @Hotbar7.started -= instance.OnHotbar7;
                @Hotbar7.performed -= instance.OnHotbar7;
                @Hotbar7.canceled -= instance.OnHotbar7;
                @Hotbar8.started -= instance.OnHotbar8;
                @Hotbar8.performed -= instance.OnHotbar8;
                @Hotbar8.canceled -= instance.OnHotbar8;
                @Hotbar9.started -= instance.OnHotbar9;
                @Hotbar9.performed -= instance.OnHotbar9;
                @Hotbar9.canceled -= instance.OnHotbar9;
                @FlashlightToggle.started -= instance.OnFlashlightToggle;
                @FlashlightToggle.performed -= instance.OnFlashlightToggle;
                @FlashlightToggle.canceled -= instance.OnFlashlightToggle;
                @WalkieTalkieSpeak.started -= instance.OnWalkieTalkieSpeak;
                @WalkieTalkieSpeak.performed -= instance.OnWalkieTalkieSpeak;
                @WalkieTalkieSpeak.canceled -= instance.OnWalkieTalkieSpeak;
            }

            public void RemoveCallbacks(IHotbarActions instance)
            {
                if (m_Wrapper.m_HotbarActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IHotbarActions instance)
            {
                foreach (var item in m_Wrapper.m_HotbarActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_HotbarActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }

        public HotbarActions @Hotbar => new HotbarActions(this);

        public interface IHotbarActions
        {
            void OnHotbar1(InputAction.CallbackContext context);
            void OnHotbar2(InputAction.CallbackContext context);
            void OnHotbar3(InputAction.CallbackContext context);
            void OnHotbar4(InputAction.CallbackContext context);
            void OnHotbar5(InputAction.CallbackContext context);
            void OnHotbar6(InputAction.CallbackContext context);
            void OnHotbar7(InputAction.CallbackContext context);
            void OnHotbar8(InputAction.CallbackContext context);
            void OnHotbar9(InputAction.CallbackContext context);
            void OnFlashlightToggle(InputAction.CallbackContext context);
            void OnWalkieTalkieSpeak(InputAction.CallbackContext context);
        }
    }
}