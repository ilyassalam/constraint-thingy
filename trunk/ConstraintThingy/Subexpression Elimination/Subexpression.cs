using System;
using System.Linq;

namespace ConstraintThingy
{
    sealed class Subexpression
    {
        private readonly Delegate _procedure;
        private readonly Variable[] _arguments;

        public Subexpression(Delegate procedure, params Variable[] arguments)
        {
            _procedure = procedure;
            _arguments = arguments.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj is Subexpression)
            {
                return Equals(obj as Subexpression);
            }

            return false;
        }

        public bool Equals(Subexpression other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (_procedure != other._procedure) return false;

            if (_arguments.Length == other._arguments.Length)
            {
                for (int i = 0; i < _arguments.Length; i++)
                {
                    if (!_arguments[i].Equals(other._arguments[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;

            hashCode ^= _procedure.GetHashCode();

            for (int i = 0; i < _arguments.Length; i++)
            {
                hashCode ^= _arguments[i].GetHashCode();
            }

            return hashCode;
        }
    }
}