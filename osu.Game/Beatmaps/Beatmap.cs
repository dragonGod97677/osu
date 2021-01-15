// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps.Timing;
using osu.Game.Rulesets.Objects;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps.ControlPoints;
using Newtonsoft.Json;
using osu.Game.IO.Serialization.Converters;

namespace osu.Game.Beatmaps
{
    /// <summary>
    /// A Beatmap containing converted HitObjects.
    /// </summary>
    public class Beatmap<T> : IBeatmap<T>
        where T : HitObject
    {
        public BeatmapInfo BeatmapInfo { get; set; } = new BeatmapInfo
        {
            Metadata = new BeatmapMetadata
            {
                Artist = @"Unknown",
                Title = @"Unknown",
                AuthorString = @"Unknown Creator",
            },
            Version = @"Normal",
            BaseDifficulty = new BeatmapDifficulty()
        };

        [JsonIgnore]
        public BeatmapMetadata Metadata => BeatmapInfo?.Metadata ?? BeatmapInfo?.BeatmapSet?.Metadata;

        public ControlPointInfo ControlPointInfo { get; set; } = new ControlPointInfo();

        public List<BreakPeriod> Breaks { get; set; } = new List<BreakPeriod>();

        [JsonIgnore]
        public double TotalBreakTime => Breaks.Sum(b => b.Duration);

        [JsonConverter(typeof(TypedListConverter<HitObject>))]
        public List<T> HitObjects { get; set; } = new List<T>();

        IReadOnlyList<T> IBeatmap<T>.HitObjects => HitObjects;

        IReadOnlyList<HitObject> IBeatmap.HitObjects => HitObjects;

        public virtual IEnumerable<BeatmapStatistic> GetStatistics() => Enumerable.Empty<BeatmapStatistic>();

        public double GetMostCommonBeatLength()
        {
            // The last playable time in the beatmap - the last timing point extends to this time.
            // Note: This is more accurate and may present different results because osu-stable didn't have the ability to calculate slider durations in this context.
            double lastTime = HitObjects.LastOrDefault()?.GetEndTime() ?? ControlPointInfo.TimingPoints.LastOrDefault()?.Time ?? 0;

            var beatLengthsAndDurations =
                // Construct a set of (beatLength, duration) tuples for each individual timing point.
                ControlPointInfo.TimingPoints.Select((t, i) =>
                                {
                                    if (t.Time > lastTime)
                                        return (beatLength: t.BeatLength, 0);

                                    var nextTime = i == ControlPointInfo.TimingPoints.Count - 1 ? lastTime : ControlPointInfo.TimingPoints[i + 1].Time;
                                    return (beatLength: t.BeatLength, duration: nextTime - t.Time);
                                })
                                // Aggregate durations into a set of (beatLength, duration) tuples for each beat length
                                .GroupBy(t => t.beatLength)
                                .Select(g => (beatLength: g.Key, duration: g.Sum(t => t.duration)))
                                // And if there are no timing points, use a default.
                                .DefaultIfEmpty((TimingControlPoint.DEFAULT_BEAT_LENGTH, 0));

            // Find the single beat length with the maximum aggregate duration.
            double maxDurationBeatLength = double.NegativeInfinity;
            double maxDuration = double.NegativeInfinity;

            foreach (var (beatLength, duration) in beatLengthsAndDurations)
            {
                if (duration > maxDuration)
                {
                    maxDuration = duration;
                    maxDurationBeatLength = beatLength;
                }
            }

            return 60000 / maxDurationBeatLength;
        }

        IBeatmap IBeatmap.Clone() => Clone();

        public Beatmap<T> Clone()
        {
            var clone = (Beatmap<T>)MemberwiseClone();

            clone.ControlPointInfo = ControlPointInfo.CreateCopy();
            // todo: deep clone other elements as required.

            return clone;
        }
    }

    public class Beatmap : Beatmap<HitObject>
    {
        public new Beatmap Clone() => (Beatmap)base.Clone();

        public override string ToString() => BeatmapInfo?.ToString() ?? base.ToString();
    }
}
