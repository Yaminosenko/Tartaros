﻿using Game.Entities;
using Game.Selection;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Game.Inputs
{    
    /// <summary>
    /// If mouse right click pressed, order attack or movement to Spartan selected groups.
    /// </summary>
    public class MouseActionListener : MonoBehaviour
    {
        #region Fields
        public static readonly string debugLogHeader = "Mouse Action Listener : ";

        [Required]
        [SerializeField] private GameObject _prefabMoveToOrderFeedback;
        [SerializeField] private Vector3 _orderMoveToInstanciatedOffset = Vector3.up;

        private GameObject _onclick;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Update()
        {
            ManageOrdersExecuter();
        }

        void OnEnable()
        {
            this.GetOrAddComponent<DoubleClickDetector>().OnDoubleClick += MouseActionListener_OnDoubleClick;
        }

        void OnDisable()
        {
            this.GetOrAddComponent<DoubleClickDetector>().OnDoubleClick -= MouseActionListener_OnDoubleClick;
        }
        #endregion

        #region Events Handlers
        private void MouseActionListener_OnDoubleClick()
        {
            if (MouseInput.GetEntityUnderMouse(out Entity entity) && entity.GetCharacterComponent<EntitySelectable>().IsSelected)
            {
                Entity[] entities = EntitiesGetter.GetEntitiesOfTypeInCamera(entity.EntityID);
                SelectionManager.Instance.AddEntities(entities);
                // REFACTOR NOTE:
                // Remove this method, that's not very good wallah
                SelectionManager.Instance.IgnoreNextMouseButtonUpInput();
            }
        }
        #endregion

        #region Private Methods
        void ManageOrdersExecuter()
        {
            if (SecondClickListener.Instance.ListenToClick)
                return;

            // over UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
                {
                    SelectedGroupsActionsCaller.OnEntityClick(hit.transform.GetComponent<Entity>());
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                {
                    SelectedGroupsActionsCaller.OrderMoveToPosition(hit.point);

                    if (SelectionManager.Instance.HasSelection)
                    {
                        DisplayMoveToOrderFeedback(hit.point);
                    }
                }
            }
        }

        void DisplayMoveToOrderFeedback(Vector3 position)
        {
            if (_onclick == null)
                InstanciateMoveToOrderFeedback();

            _onclick.SetActive(true);
            _onclick.transform.position = position + _orderMoveToInstanciatedOffset;
            _onclick.GetComponent<ParticleSystem>().Play(true);
        }

        void InstanciateMoveToOrderFeedback()
        {
            if (_onclick != null)
            {
                Debug.LogWarningFormat(debugLogHeader + "Can't instanciate prefab on click because it's already instanciated.");
                return;
            }

            Assert.IsNotNull(_prefabMoveToOrderFeedback);

            _onclick = Instantiate(_prefabMoveToOrderFeedback);
            _onclick.SetActive(false);
        }
        #endregion
        #endregion
    }
}
