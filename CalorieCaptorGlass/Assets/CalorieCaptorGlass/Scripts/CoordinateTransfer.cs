using UnityEngine;

namespace CalorieCaptorGlass
{
    public static class CoordinateTransfer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">画像上の座標。左上原点x軸右向き。</param>
        /// <param name="y">画像上の座標。左上原点y軸下向き。</param>
        /// <param name="imageHeight"></param>
        /// <param name="imageWidth"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraToWorldMatrix"></param>
        /// <param name="positionOnWorldCordinate"></param>
        /// <returns></returns>
        public static bool ImagePos2WorldPos(float x, float y, float imageHeight, float imageWidth, Matrix4x4 projectionMatrix, Matrix4x4 cameraToWorldMatrix, Vector3 headPosition, out Vector3 positionOnWorldCordinate)
        {
            Vector2 imagePosZeroToOne = new Vector2(x / imageWidth, 1 - y / imageHeight);
            Vector2 imagePosProjected2D = imagePosZeroToOne * 2 - new Vector2(1, 1); // -1 ~ 1空間に。
            Vector3 imagePosProjected = new Vector3(imagePosProjected2D.x, imagePosProjected2D.y, 1);

            Vector3 cameraSpacePos = UnProjectVector(projectionMatrix, imagePosProjected);

            Vector3 worldSpacePos = cameraToWorldMatrix.MultiplyPoint(cameraSpacePos);

            RaycastHit hit;

            if (Physics.Raycast(headPosition, worldSpacePos - headPosition, out hit, 20f, 1 << 31))
            {
                positionOnWorldCordinate = hit.point;

                return true;
            }

            positionOnWorldCordinate = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Unityの世界座標から画像上の座標に変換する。
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraToWorldMatrix"></param>
        /// <param name="imageHeight"></param>
        /// <param name="imageWidth"></param>
        /// <param name="x">画像上の座標。左上原点x軸右向き。</param>
        /// <param name="y">画像上の座標。左上原点y軸下向き。</param>
        /// <returns>worldPosが画像内に移っていればtrue,移っていなければfalse</returns>
        public static bool WorldPos2ImagePos(Vector3 worldPos, Matrix4x4 projectionMatrix, Matrix4x4 cameraToWorldMatrix, int imageHeight, int imageWidth, ref int x, ref int y)
        {
            var world2CameraMatrix = cameraToWorldMatrix.inverse;
            var cameraSpacePos = world2CameraMatrix.MultiplyPoint(worldPos);

            if (cameraSpacePos.z >= 0)
            {
                return false;
            }

            //プロジェクション空間は左下原点
            var projectionPosition = projectionMatrix.MultiplyPoint(cameraSpacePos);
            var imagePosProjected2D = new Vector2(projectionPosition.x, projectionPosition.y);

            if (Mathf.Abs(imagePosProjected2D.x) >= 1.0f || Mathf.Abs(imagePosProjected2D.y) >= 1.0f)
            {
                return false;
            }

            //-1~1空間を0~1空間に
            var normalized = 0.5f * imagePosProjected2D + 0.5f * Vector2.one;
            x = (int)(imageWidth * normalized.x);
            y = (int)(imageHeight - imageHeight * normalized.y);
            return true;
        }

        private static Vector3 UnProjectVector(Matrix4x4 proj, Vector3 to)
        {
            /*
             * Source: https://developer.microsoft.com/en-us/windows/holographic/locatable_camera
             */
            Vector3 from = new Vector3(0, 0, 0);
            var axsX = proj.GetRow(0);
            var axsY = proj.GetRow(1);
            var axsZ = proj.GetRow(2);
            from.z = to.z / axsZ.z;
            from.y = (to.y - (from.z * axsY.z)) / axsY.y;
            from.x = (to.x - (from.z * axsX.z)) / axsX.x;
            return from;
        }
    }
}