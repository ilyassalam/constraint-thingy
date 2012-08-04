using System;
using Intervals;
using ConstraintThingy;

namespace ConstraintThingyGUI
{
    class ScoreLabeling : IntervalLabeling
    {
        public ScoreLabeling(string name, FiniteDomainLabeling labeling, float defScore, params object[] scores)
            : base(name, new Interval(0, 0))
        {
            baseLabeling = labeling;
            defaultScore = defScore;
            scoredLabels = new ulong[scores.Length / 2];
            labelScores = new float[scores.Length / 2];
            float minScore = defScore;
            float maxScore = defScore;
            int label = 0;
            UInt64 mask = 0;
            for (int i = 0; i < scores.Length; )
            {
                mask |= scoredLabels[label] = labeling.Domain.Bitmask((string)scores[i]);
                i++;
                var score = Convert.ToSingle(scores[i]);
                labelScores[label] = score;
                if (score < minScore)
                    minScore = score;
                if (score > maxScore)
                    maxScore = score;
                i++;
                label++;
            }
            InitialRange = new Interval(minScore, maxScore);
            unscoredLabelMask = ~mask & labeling.Domain.UniverseMask;
        }

        private readonly UInt64[] scoredLabels;
        private readonly UInt64 unscoredLabelMask;
        private readonly float[] labelScores;
        private readonly float defaultScore;
        private readonly FiniteDomainLabeling baseLabeling;

        /// <summary>
        /// Given a bitmask for the set of possible values for a variable, return the interval of possible scores for those values.
        /// </summary>
        Interval ScoreInterval(UInt64 mask)
        {
            Interval answer = new Interval(0, 0);
            bool gotOne = false;
            for (int i = 0; i < scoredLabels.Length; i++)
            {
                if ((scoredLabels[i] & mask) != 0)
                {
                    if (gotOne)
                        answer.Extend(labelScores[i]);
                    else
                    {
                        gotOne = true;
                        answer = new Interval(labelScores[i], labelScores[i]);
                    }
                }
            }
            if (gotOne)
            {
                if (HasUnscoredLabels(mask))
                    answer.Extend(defaultScore);
            }
            else
                answer = new Interval(defaultScore, defaultScore);
            return answer;
        }

        /// <summary>
        /// Given an interval of possible scores, return the bitmask for the set of labels whose scores are in that interval.
        /// </summary>
        UInt64 IntervalMask(Interval scoreRange)
        {
            UInt64 result = 0;
            if (scoreRange.Contains(defaultScore))
                result |= unscoredLabelMask;
            for (int i = 0; i < scoredLabels.Length; i++)
                if (scoreRange.Contains(labelScores[i]))
                    result |= scoredLabels[i];
            return result;
        }

        private bool HasUnscoredLabels(ulong mask)
        {
            return (mask & unscoredLabelMask) != 0;
        }

        protected override IntervalVariable MakeVariable(Node n)
        {
            var scoreVariable = base.MakeVariable(n);
            new ScoringConstraint(scoreVariable, baseLabeling.ValueVariable(n), this);
            return scoreVariable;
        }

        class ScoringConstraint : Constraint<Variable>
        {
            public ScoringConstraint(IntervalVariable score, FiniteDomainVariable label, ScoreLabeling sLabeling)
                : base(new Variable[] { score, label })
            {
                scoreLabeling = sLabeling;
                this.score = score;
                this.label = label;
            }

            private readonly ScoreLabeling scoreLabeling;
            private readonly IntervalVariable score;
            private readonly FiniteDomainVariable label;

            public override void Narrowed(Variable narrowedVariable, ref bool succeeded)
            {
                if (narrowedVariable == score)
                {
                    var newValue = label.Value & scoreLabeling.IntervalMask(score.Value);
                    if (newValue==FiniteDomain.EmptySet)
                    {
                        succeeded = false;
                        return;
                    }
                    label.TrySetValue(newValue, ref succeeded);
                }
                else
                {
                    // narrowedVariable == label
                    score.TrySetValue(Interval.Intersection(score.Value, scoreLabeling.ScoreInterval(label.Value)), ref succeeded);
                }
            }

            public override void UpdateVariable(Variable var, ref bool succeeded)
            {
                throw new NotImplementedException();
            }
        }
    }
}
