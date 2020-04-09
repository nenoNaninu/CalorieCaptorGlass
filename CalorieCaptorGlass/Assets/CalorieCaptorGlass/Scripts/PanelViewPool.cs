using System.Collections.Generic;
using UnityEngine;

namespace CalorieCaptorGlass
{
    public class PanelViewPool : GameObjectPool<GameObject>
    {
        private List<FoodDataPanel> _foodDataPanelList;
        private List<BoundingBox> _boundingBoxList;

        public IReadOnlyList<FoodDataPanel> FoodDataPanelList => _foodDataPanelList;
        public IReadOnlyList<BoundingBox> BoundingBoxList => _boundingBoxList;

        public PanelViewPool(int capacity = 20) : base(20)
        {
            _foodDataPanelList = new List<FoodDataPanel>(capacity);
            _boundingBoxList = new List<BoundingBox>(capacity);
        }

        public override void Add(GameObject obj)
        {
            base.Add(obj);
            var foodDataPanel = obj.GetComponentInChildren<FoodDataPanel>();
            var bbox = obj.GetComponentInChildren<BoundingBox>();

            _foodDataPanelList.Add(foodDataPanel);
            _boundingBoxList.Add(bbox);
        }

        public int RentIndex()
        {
            var rentObject = Rent();

            for (int i = 0; i < _gameObjectList.Count; i++)
            {
                if (ReferenceEquals(rentObject, _gameObjectList[i]))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}