using System.Collections;
using System.Collections.Generic;
using Dremu.Gameplay.Object;
using Dremu.Gameplay.Tool;
using UnityEngine;

namespace Dremu.Gameplay.Manager {
    public class NoteEffectManager : MonoBehaviour {

        [SerializeField] NoteEffect _NoteEffect;
        StandardObjectPool<NoteEffect> NoteEffectPool;

        List<NoteEffect> NoteEffects = new List<NoteEffect>();

        public static NoteEffectManager Instance { get; private set; }

        private void Awake() {
            Instance = this;

            NoteEffectPool = new StandardObjectPool<NoteEffect>(_NoteEffect);
        }

        /// <summary>
        /// 更新打击特效的状态
        /// 此方法只能由主控调用
        /// </summary>
        public static void UpdateNoteEffectState() {
            for (int i = 0; i < Instance.NoteEffects.Count; i++) {
                NoteEffect noteEffect = Instance.NoteEffects[i];
                noteEffect.OnActive();
                if (noteEffect.Time > 1) {
                    Instance.NoteEffectPool.ReturnObject(noteEffect);
                    Instance.NoteEffects.Remove(noteEffect);
                    i--;
                }
            }
        }

        /// <summary>
        /// 创建一个新的打击特效
        /// </summary>
        /// <returns>打击特效</returns>
        public static NoteEffect GetNewNoteEffect(Vector2 Position) {
            NoteEffect noteEffect = Instance.NoteEffectPool.GetObject();
            noteEffect.transform.position = Position;
            Instance.NoteEffects.Add(noteEffect);
            return noteEffect;
        }

    }

}
