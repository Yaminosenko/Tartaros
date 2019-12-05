﻿using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using Registers;
using System;
using TMPro;
using UnityEngine;

namespace UI.Game
{
    [Serializable]
    public class PanelSelection : Panel
    {
        #region Fields
        [Space(order = 0)]
        [Header("Selected Groups", order = 1)]
        [SerializeField] private GameObject _prefabSelectedGroup;
        [SerializeField] private Transform _parentSelectedGroup;
        [Header("Orders")]
        [SerializeField] private OrdersWrapper _ordersWrapper;

        private Group[] _selectedGroupWrapper = new Group[5];
        #endregion

        #region Properties
        #endregion

        #region Methods
        public override void Initialize()
        {
            for (int i = 0; i < _selectedGroupWrapper.Length; i++)
            {
                _selectedGroupWrapper[i] = GameObject.Instantiate(_prefabSelectedGroup).GetComponent<Group>();
                _selectedGroupWrapper[i].transform.parent = _parentSelectedGroup.transform;
                _selectedGroupWrapper[i].gameObject.SetActive(false);
            }

            _ordersWrapper.HideOrders();
        }

        public override void OnValidate()
        {
            _ordersWrapper.ResizeArrayIfNeeded();
        }

        public void UpdateSelection(SelectionManager.Group[] selectedGroups, int highlightGroupIndex)
        {
            UpdateSelectedGroups(selectedGroups);
            UpdateHighlightGroup(highlightGroupIndex);

            if (highlightGroupIndex >= 0 && highlightGroupIndex < selectedGroups.Length)
            {
                UpdateOrdersWrapper(selectedGroups[highlightGroupIndex].selectedEntities[0].Entity.OrdersReceiver);
            }
        }

        void UpdateSelectedGroups(SelectionManager.Group[] selectedGroups)
        {
            for (int i = 0; i < _selectedGroupWrapper.Length; i++)
            {
                if (i >= selectedGroups.Length)
                {
                    _selectedGroupWrapper[i].gameObject.SetActive(false);
                    continue;
                }

                _selectedGroupWrapper[i].gameObject.SetActive(true);
                _selectedGroupWrapper[i].portrait.sprite = EntitiesRegister.GetRegisterData(selectedGroups[i].entityType).Portrait;
                _selectedGroupWrapper[i].unitsCount.text = selectedGroups[i].selectedEntities.Count.ToString();
            }
        }

        void UpdateHighlightGroup(int highlightGroupIndex)
        {
            for (int i = 0; i < _selectedGroupWrapper.Length; i++)
            {
                bool isHighlight = (i == highlightGroupIndex);

                _selectedGroupWrapper[i].SetHighlight(isHighlight);
            }
        }

        void UpdateOrdersWrapper(OrdersReceiver orderReceiver)
        {
            _ordersWrapper.UpdateOrders(orderReceiver);
        }
        #endregion
    }
}
