using System;
using ConstraintThingy;

namespace ConstraintThingy
{
    /// <summary>
    /// A node that is represented as a circle
    /// </summary>
    public class CircleNode : Node
    {
        private Vector2 _position;

        /// <summary>
        /// The position of the node
        /// </summary>
        public override Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position != value && PositionChanged != null)
                {
                    _position = value;
                    PositionChanged(_position);
                }
                else _position = value;
            }
        }

        private double _radius;
        /// <summary>
        /// The radius of the node
        /// </summary>
        public double Radius
        {
            get { return _radius; }
            set
            {
                if (_radius != value && RadiusChanged != null)
                {
                    _radius = value;
                    RadiusChanged(_radius);
                }
                else _radius = value;
            }
        }

        /// <summary>
        /// Fired when the radius changes
        /// </summary>
        public event Action<double> RadiusChanged;


        /// <summary>
        /// Fired when the position changes
        /// </summary>
        public override event Action<Vector2> PositionChanged;
    }
}