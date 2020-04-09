using UnityEngine;

namespace CalorieCaptorGlass
{
    public static class AreaCalculator
    {
        public static float Calculate(Vector3 first, Vector3 second, Vector3 third)
        {
            var first2Second = second - first;
            var first2Third = third - first;

            var dotValue = Vector3.Dot(first2Second, first2Third);

            return Mathf.Sqrt(first2Second.sqrMagnitude * first2Third.sqrMagnitude - dotValue * dotValue) / 2f;
        }
    }
}