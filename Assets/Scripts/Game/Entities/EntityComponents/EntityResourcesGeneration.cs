﻿namespace Game.Entities
{
    using Game.MapCellEditor;
    using UnityEngine;

    /// <summary>
    /// This script create resources each tick setted in EntityData.
    /// </summary>
    public partial class EntityResourcesGeneration : EntityComponent
    {
        #region Fields
        private float _nextTimeGenerationTick = 0; // Time.time + Entity.Data.GenerationTick
        private ResourcesWrapper _resourcesPerTick;

        private bool _onEnableFirstFrame = true;
        private bool _resourceProductionEnable = true;
        #endregion

        #region Events
        public static event EntityDelegate OnResourceGenerationEnable;
        public static event EntityDelegate OnResourceGenerationDisable;
        #endregion

        #region Properties
        public bool EnableResourceProduction
        {
            get => _resourceProductionEnable;
            set
            {
                // REFACTOR NOTE:
                // Properties should only do checks!

                if (value == _resourceProductionEnable)
                    return;

                var oldValue = _resourceProductionEnable;
                _resourceProductionEnable = value;

                if (_resourceProductionEnable)
                {
                    OnResourceGenerationEnable?.Invoke(Entity);
                }
                else
                {
                    OnResourceGenerationDisable?.Invoke(Entity);
                }

                CalculateNextTimeGenerationTick();
            }
        }
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Start()
        {
            // disable this component if entity can't create resources
            if (!Entity.Data.CanCreateResources)
            {
                enabled = false;
            }
            else
            {
                CalculateResourcesPerTick();
                CalculateNextTimeGenerationTick();

                // because _resourceProductionEnable is true,
                // invoke event
                OnResourceGenerationEnable?.Invoke(Entity);
            }
        }

        void Update()
        {
            TryCreateResources();
        }

        void OnEnable()
        {
            SubscribeToEvents();

            // don't throw warning the first frame
            if (!_onEnableFirstFrame && !Entity.Data.CanCreateResources)
            {
                Debug.LogWarningFormat("Entity Resources Generation : You shouldn't enable this component because this entity can't generate resources.");
            }

            _onEnableFirstFrame = true;
        }

        void OnDisable()
        {
            UnsubscribeToEvents();
        }

        void OnGUI()
        {
            DrawGUI_ResourcesPerTick();
        }
        #endregion

        #region Events subscription
        private void SubscribeToEvents()
        {
            if (TileSystem.Instance != null)
            {
                // listen to building game manager, to recalculate resoruces    
                TileSystem.Instance.OnTileTerrainChanged += OnTileTerrainChanged;
            }
            else
            {
                Debug.LogErrorFormat("Entity Resources Generation : TileSystem is missing. Can't resources generated by tick if tile terrain changed.");
            }
        }

        private void UnsubscribeToEvents()
        {
            if (TileSystem.Instance != null)
            {
                TileSystem.Instance.OnTileTerrainChanged -= OnTileTerrainChanged;
            }
        }
        #endregion

        #region Events handlers
        private void OnTileTerrainChanged(Vector2Int coords, GameObject gameObjectAtCoords)
        {
            CalculateResourcesPerTick();
        }
        #endregion

        #region Public Methods
        public void CalculateResourcesPerTick()
        {
            if (!Entity.Data.CanCreateResources)
                return;

            ResourcesWrapper resourcesPerTick = new ResourcesWrapper(0, 0, 0);

            switch (Entity.Data.GenerationType)
            {
                case GenerationType.Constant:
                    resourcesPerTick = Entity.Data.ConstantResourcesGeneration;
                    break;

                case GenerationType.PerCell:
                    foreach (var kvp in Entity.Data.ResourcesPerCell)
                    {
                        int cellCount = GetCellsInGenerationRadius(kvp.Key);
                        resourcesPerTick += kvp.Value * cellCount;
                    }
                    break;

                default:
                    throw new System.NotImplementedException();
            }

            _resourcesPerTick = resourcesPerTick;
        }
        #endregion

        #region Private Methods
        private void DrawGUI_ResourcesPerTick()
        {
            if (Camera.main == null)
            {
                Debug.LogErrorFormat("Entity Resources Generation : Camera.main is null. Can't draw resources per tick.");
                return;
            }

            if (Entity.Data == null)
                return;

            // only draw PerCell generation
            // otherwise, it's a bit useless
            if (Entity.Data.GenerationType == GenerationType.PerCell)
            {
                // Draw wood X food X stone X above building
                Vector2 guiPosition = Camera.main.WorldToScreenPoint(transform.position);

                // The WorldToScreenPoint functions return and integer starting from 0,0
                // at the BOTTOM LEFT of the screen.
                // Because of this, the y-value is flipped.
                // So to solve the problem, substract the screen height.
                guiPosition.y = Screen.height - guiPosition.y;

                Rect labelRect = new Rect(guiPosition.x, guiPosition.y, 300, 50);
                GUI.Label(labelRect, _resourcesPerTick.ToString(true));
            }
        }

        private void CalculateNextTimeGenerationTick()
        {
            _nextTimeGenerationTick = Time.time + Entity.Data.GenerationTick;
        }

        private void TryCreateResources()
        {
            if (_resourceProductionEnable && _nextTimeGenerationTick <= Time.time)
            {
                CalculateNextTimeGenerationTick();
                CreateResources();
            }
        }

        private void CreateResources()
        {
            GameManager.Instance.Resources += _resourcesPerTick;
        }

        private int GetCellsInGenerationRadius(CellType cellType)
        {
            return GameManager.Instance.MapCells.GetCells_WorldPosition(transform.position.x, transform.position.z, Entity.Data.RadiusToReachCells, cellType).Length;
        }
        #endregion
        #endregion
    }

#if UNITY_EDITOR
    public partial class EntityResourcesGeneration : EntityComponent
    {

        void OnDrawGizmos()
        {
            if (Entity == null)
                return;

            if (Entity.EntityID == string.Empty)
                return;

            if (Entity.Data == null)
                return;

            if (!Entity.Data.CanCreateResources)
                return;

            if (Entity.Data.GenerationType != GenerationType.PerCell)
            {
                UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, Entity.Data.RadiusToReachCells);
            }
        }

    }
#endif
}
