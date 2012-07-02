using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConstraintThingy
{
    public class ScoreContraint<T> : Constraint<Variable>
    {
        public ScoreContraint(RealVariable score, FiniteDomainVariable<T> finiteDomainVariable, ScoreMapping<T> scoreMapping) : base(score, finiteDomainVariable)
        {
            _scoreMapping = scoreMapping;

            if (finiteDomainVariable.FiniteDomain != scoreMapping.FiniteDomain) 
                throw new InvalidOperationException("The finite domains of the variable and score mapping did not match.");
        }

        private readonly ScoreMapping<T> _scoreMapping; 

        private RealVariable Score
        {
            get { return Variables[0] as RealVariable; }
        }

        private FiniteDomainVariable<T> FiniteDomainVariable
        {
            get { return Variables[1] as FiniteDomainVariable<T>; }
        }

        private UInt64 FindPlausibleFiniteDomainValues()
        {
            MultiInterval score = Score.AllowableValues.First;

            int[] setBits = FiniteDomainVariable.AllowableValues.GetSetIndices();

            UInt64 mask = 0UL;
            for (int i = 0; i < setBits.Length; i++)
            {
                if (MultiInterval.Intersects(score, _scoreMapping[setBits[i]]))
                {
                    mask = mask.SetBit(setBits[i]);
                }
            }

            return mask;
        }

        /*
         * This version splits up the finite domain into several intervals and tries them all, while the other merges them into a single one.
         */

        private MultiInterval ScoreFiniteDomainVariable()
        {
            Debug.Assert(!FiniteDomainVariable.IsEmpty);

            List<Interval> intervals = new List<Interval>();

            ulong allowableValues = FiniteDomainVariable.AllowableValues;

            int[] setBits = allowableValues.GetSetIndices();

            for (int i = 0; i < setBits.Length; i++)
            {
                intervals.Add(_scoreMapping[setBits[i]]);
            }

            return new MultiInterval(intervals);
        }

        /*
        private Interval ScoreFiniteDomainVariable()
        {
            Debug.Assert(!FiniteDomainVariable.IsEmpty);

            ulong allowableValues = FiniteDomainVariable.AllowableValues;

            int[] setBits = allowableValues.GetSetIndices();

            Interval result = _scoreMapping[setBits[0]];

            for (int i = 1; i < setBits.Length; i++)
            {
                result = Interval.Union(result, _scoreMapping[setBits[i]]);
            }

            return result;
        }*/

        protected internal override void UpdateVariable(Variable variable, out bool success)
        {
            if (variable == Score)
            {
                // narrowedVariable == label
                var scores = ScoreFiniteDomainVariable();

                Score.NarrowTo(scores, out success);
            }
            else
            {
                UInt64 possibleFiniteDomainValues = FindPlausibleFiniteDomainValues();

                FiniteDomainVariable.NarrowTo(possibleFiniteDomainValues, out success);
            }
        }
    }
}