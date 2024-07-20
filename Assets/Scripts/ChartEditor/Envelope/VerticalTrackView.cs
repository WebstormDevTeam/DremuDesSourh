using System.Collections;
using System.Collections.Generic;
using Dremu.Gameplay.Tool;
using UnityEngine;

namespace Dremu.ChartEdit.Envelope
{

    public class VerticalTrackView : MonoBehaviour
    {
        [SerializeField] List<EnvelopeTrack> tracks;
        [SerializeField] EnvelopeTrack trackPrefab;
        [SerializeField] Transform trackContainer;
        [SerializeField] Transform selectedTrackContainer;
        [SerializeField] DummyTrack dummyTrack;
        private int _selectedTrackIndex;

        public float BeatX
        {
            get
            {
                return trackContainer.GetComponent<RectTransform>().sizeDelta[0] / UIManager.Instance.FullTime;
            }

        }
        public int Capacity
        {
            get
            {
                return tracks.Count;
            }
            set
            {
                for (int i = tracks.Count; i < value; i++)
                {
                    var track = Instantiate(trackPrefab, trackContainer.transform);
                    track.realParent = this;
                    track.ViewingParent = this.GetComponent<RectTransform>();
                    tracks.Add(track);
                }
            }
        }
        public void Update()
        {
            this.selectedTrackContainer.transform.localPosition = trackContainer.transform.localPosition;
        }
        public void InitEnvelopes(List<EnvelopeLine> envelopes)
        {
            this.Capacity = envelopes.Count;
            foreach (EnvelopeTrack track in tracks)
            {
                track.gameObject.SetActive(false);
            }
            for (int i = 0; i < envelopes.Count; i++)
            {
                var index = i;
                tracks[i].gameObject.SetActive(true);
                tracks[i].SetEnvelopeLine(envelopes[i]);
                tracks[i].SelectMe = () => { SelectTrack(index); };
            }
        }
        public void SetAllDirty()
        {
            foreach (EnvelopeTrack envelopeTrack in this.tracks)
            {
                envelopeTrack.SetAllDirty();
            }
        }

        public void DeselectAll()
        {
            dummyTrack.RealTrack = null;
            dummyTrack.transform.SetAsLastSibling();
            dummyTrack.gameObject.SetActive(false);
            this.tracks[_selectedTrackIndex].transform.parent = trackContainer;
            this.tracks[_selectedTrackIndex].transform.SetSiblingIndex(_selectedTrackIndex);
            foreach (EnvelopeTrack track in tracks)
            {
                track.SetSelected(false);
            }
            _selectedTrackIndex = -1;
        }
        public void SelectTrack(int index)
        {
            DeselectAll();
            _selectedTrackIndex = index;
            this.tracks[_selectedTrackIndex].transform.parent = selectedTrackContainer;
            dummyTrack.gameObject.SetActive(true);
            // 李鬼代替了李逵
            dummyTrack.transform.SetSiblingIndex(_selectedTrackIndex);
            dummyTrack.RealTrack = this.tracks[_selectedTrackIndex];
            if (tracks[index].gameObject.activeSelf)
            {
                tracks[index].SetSelected(true);
            }

        }

    }
}