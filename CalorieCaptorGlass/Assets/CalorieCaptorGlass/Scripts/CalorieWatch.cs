using System.Collections;
using UnityEngine;

namespace CalorieCaptorGlass
{
    /// <summary>
    /// カロリー時計(腕のところに表示するよ)
    /// </summary>
    public class CalorieWatch : MonoBehaviour
    {
        private TextMesh _calorieWatchText;

        private bool _isCoroutineAlive;

        private float _totalCalorie;

        private float _currentViewCalorie;

        void OnEnable()
        {
            _currentViewCalorie = _totalCalorie;
            _isCoroutineAlive = false;
            _calorieWatchText.text = $"total:{_totalCalorie:F2} kcal";

        }

        // Use this for initialization
        void Awake()
        {
            _calorieWatchText = GetComponentInChildren<TextMesh>();
        }

        void OnTriggerEnter(Collider collider)
        {
            Debug.Log(collider.gameObject.name);
            Debug.Log(collider.gameObject.tag);
            if (collider.gameObject.CompareTag("FoodPanelTag"))
            {
                var foodPanel = collider.gameObject.GetComponent<FoodDataPanel>();
                if (!foodPanel.Touched)
                {
                    foodPanel.Touched = true;
                    _totalCalorie += foodPanel.WorldSpaceFoodData.Calorie;

                    if (!_isCoroutineAlive)
                    {
                        StartCoroutine(CountUpCalorie());
                    }
                }
            }
        }

        IEnumerator CountUpCalorie()
        {
            var wait = new WaitForEndOfFrame();
            _isCoroutineAlive = true;

            while (_currentViewCalorie < _totalCalorie)
            {
                if (_totalCalorie - _currentViewCalorie <= 2.0f)
                {
                    _currentViewCalorie = _totalCalorie;
                    _calorieWatchText.text = $"total:{_currentViewCalorie:F2} kcal";
                    break;
                }

                _currentViewCalorie += 1.11f;
                _calorieWatchText.text = $"total:{_currentViewCalorie:F2} kcal";

                yield return wait;

            }

            _isCoroutineAlive = false;
            yield return null;
        }
    }
}
