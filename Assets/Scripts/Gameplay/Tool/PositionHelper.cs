using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dremu.Gameplay.Tool {
    public sealed class PositionHelper {
        public static float width, height;

        static PositionHelper() {
            if (Screen.width / Screen.height < 16 / 9f) {
                width = Screen.width;
                height = (Screen.width * 9 / 16f + Screen.height) / 2f;
            }
            else {
                width = (Screen.height * 16 / 9f + Screen.width) / 2f;
                height = Screen.height;
            }
        }

        /// <summary>
        /// 将相对于屏幕宽高的坐标转换为绝对坐标
        /// （相对坐标的xy在-1~1内时，坐标在屏幕内）
        /// </summary>
        /// <param name="Position">相对坐标</param>
        /// <param name="Camera">相机，如果为null则转换为屏幕坐标</param>
        /// <returns>绝对坐标</returns>
        public static Vector2 RelativeCoordToAbsoluteCoord( Vector2 Position, Camera Camera = null ) {
            if (Camera != null) {
                Vector2 vec = Camera.ScreenToWorldPoint(new Vector2(width, height));
                Position.x *= vec.x;
                Position.y *= vec.y;
            }
            else {
                Position.x = (Position.x + 1) / 2f * width;
                Position.y = (Position.y + 1) / 2f * height;
            }
            return Position;
        }

        /// <summary>
        /// 将绝对坐标转换为相对于屏幕宽高的坐标
        /// </summary>
        /// <param name="Position">绝对坐标</param>
        /// <param name="Camera">相机，如果坐标为屏幕坐标则为null</param>
        /// <returns>相对坐标</returns>
        public static Vector2 AbsoluteCoordToRelativeCoord( Vector2 Position, Camera Camera = null ) {
            if (Camera != null) {
                Vector2 vec = Camera.ScreenToWorldPoint(new Vector2(width, height));
                Position.x /= vec.x;
                Position.y /= vec.y;
            }
            else {
                Position.x = Position.x / width * 2 - 1;
                Position.y = Position.y / height * 2 - 1;
            }
            return Position;
        }

    }
}
