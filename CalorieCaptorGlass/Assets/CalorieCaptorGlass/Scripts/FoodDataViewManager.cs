using System.Collections.Generic;
using UnityEngine;

namespace CalorieCaptorGlass
{
    /// <summary>
    /// 投げ込まれた食事と世界座標の対応をとって、
    /// 食事の上にfoodDataを描画する
    /// </summary>
    public class FoodDataViewManager : MonoBehaviour
    {
        [SerializeField, Tooltip("食事の上に表示するためのpanelのprefab")]
        private GameObject _foodPropertyPanelObj;

        [SerializeField, Tooltip("食事領域を表示するバウンディングボックスのprefab")]
        private GameObject _boundingBoxObj;

        private PanelViewPool _panelViewPool;
        private List<GameObject> _currentUsedPanel;

        public void Awake()
        {
            _panelViewPool = new PanelViewPool();
            _currentUsedPanel = new List<GameObject>(_panelViewPool.Capacity);

            for (int i = 0; i < _panelViewPool.Capacity; i++)
            {

                var propertyView = Instantiate(_foodPropertyPanelObj);

                var boundingBoxObj = Instantiate(_boundingBoxObj);

                var emptyGameObject = new GameObject();
                propertyView.transform.parent = emptyGameObject.transform;
                boundingBoxObj.transform.parent = emptyGameObject.transform;

                _panelViewPool.Add(emptyGameObject);

                emptyGameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 同じものがあったら上書きして描画する実装。とりあえず没で。
        /// </summary>
        /// <param name="worldSpaceFoodList"></param>
        //private void OverWrite(IReadOnlyCollection<WorldSpaceFoodData> worldSpaceFoodList)
        //{
        //    var keys = _drawingFoodDataObjDic.Select(x => x.Key);

        //    foreach (var foodData in worldSpaceFoodList)
        //    {
        //        //新しいworldSpaceFoodListの中に、前のフレームと同じ食事があるかを調べる。
        //        var worldSpaceFoodData = keys.Where(x =>
        //            {
        //                var vec = x.CenterWorldPosition - foodData.CenterWorldPosition;
        //                if (vec.sqrMagnitude < 0.1f)
        //                {
        //                    return true;
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            })
        //            .OrderBy(x =>
        //            {
        //                var vec = x.CenterWorldPosition - foodData.CenterWorldPosition;
        //                return vec.sqrMagnitude;
        //            })
        //            .FirstOrDefault();

        //        //同一の食事があった場合
        //        if (worldSpaceFoodData != null)
        //        {
        //            var panel = _drawingFoodDataObjDic[worldSpaceFoodData];
        //            panel.WorldSpaceFoodData = foodData;
        //        }
        //        else//新しい食事の場合。
        //        {
        //            GameObject newPropertyView = Instantiate(_foodPropertyPanelObj);
        //            FoodDataPanel panel = newPropertyView.GetComponent<FoodDataPanel>();
        //            panel.WorldSpaceFoodData = foodData;
        //            _drawingFoodDataObjDic.Add(foodData, panel);
        //        }
        //    }
        //}

        public void ReDrawFoodData(IReadOnlyCollection<WorldSpaceFoodData> worldSpaceFoodList)
        {
            foreach (var panel in _currentUsedPanel)
            {
                _panelViewPool.Return(panel);
                panel.SetActive(false);
            }

            _currentUsedPanel.Clear();

            foreach (var worldSpaceFoodData in worldSpaceFoodList)
            {
                var index       = _panelViewPool.RentIndex();
                var rootObject  =  _panelViewPool.GameObjectList[index];
                var panel       = _panelViewPool.FoodDataPanelList[index];
                var boundingBox = _panelViewPool.BoundingBoxList[index];

                rootObject.SetActive(true);

                boundingBox.Initialize(worldSpaceFoodData.TopLeftWorldPosition,
                                       worldSpaceFoodData.TopRightWorldPosition,
                                       worldSpaceFoodData.BottomRightWorldPosition,
                                       worldSpaceFoodData.BottomLeftWorldPosition);

                panel.Initialize();
                panel.WorldSpaceFoodData = worldSpaceFoodData;
                _currentUsedPanel.Add(rootObject);
            }
        }
    }
}