using Dremu.Gameplay.Tool;
using UnityEngine;

namespace Dremu.Gameplay.Object {
    public abstract class NoteBase : MonoBehaviour, RecyclableObject {

        public float ArrivalTime;
        public JudgementLine JudgementLine { get; private set; }
        [Range(0.1f, 0.9f)] public float position;

        /// <summary>
        /// 将音符绑定到所属判定线上
        /// </summary>
        /// <param name="judgementLine">所属判定线</param>
        public void Bind( JudgementLine judgementLine ) {
            transform.SetParent(judgementLine.transform);
            this.JudgementLine = judgementLine;
        }

        /// <summary>
        /// 设置note在判定线上的（相对）位置
        /// </summary>
        /// <param name="position">位置</param>
        public void SetPosition( float position ) {
            this.position = position;
        }

        /// <summary>
        /// 设置到达时间
        /// </summary>
        /// <param name="ArrivalTime">到达时间</param>
        public void SetArrivalTime( float ArrivalTime ) {
            this.ArrivalTime = ArrivalTime;
        }

        public abstract void OnInitialize();

        public abstract void OnRecycle();

        /// <summary>
        /// 在音符的活动时期自动调用
        /// 音符管理器会自动调用，谁敢调用他我跟谁急奥！
        /// </summary>
        /// <param name="currentTime">当前时间</param>
        public abstract void OnActive( float currentTime );

    }
}
