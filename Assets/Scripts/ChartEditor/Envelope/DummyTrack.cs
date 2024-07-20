using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dremu.ChartEdit.Envelope
{

    public class DummyTrack : MonoBehaviour
    {
        [SerializeField] EnvelopeTrack realTrack;
        private RectTransform _realTrackTransform;
        private RectTransform _myRectTransform;
        public EnvelopeTrack RealTrack
        {
            get { return realTrack; }
            set { realTrack = value; 
                if (realTrack != null)
                    _realTrackTransform = value.GetComponent<RectTransform>();
            }
        }
        private void Start()
        {
            _myRectTransform = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        public void Update()
        {
            if (realTrack == null)
                return;
            realTrack.transform.position = this.transform.position;
            _realTrackTransform.sizeDelta = this._myRectTransform.sizeDelta;
        }


    }
}