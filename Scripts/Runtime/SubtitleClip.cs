using UnityEngine;
using UnityEngine.Playables;

namespace hrspecian.timelinesubtitle.runtime
{
    public class SubtitleClip : PlayableAsset
    {
        public string subtitleText;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);

            SubtitleBehaviour subtitleBehaviour = playable.GetBehaviour();
            subtitleBehaviour.subtitleText = subtitleText;

            return playable;
        }
    }
}
