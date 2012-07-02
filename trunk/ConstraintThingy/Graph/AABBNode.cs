using System;

namespace ConstraintThingy
{
    /// <summary>
    /// A node that is represented physically as an AABB
    /// </summary>
    public class AABBNode : Node
    {
        private AABB _aabb;
        /// <summary>
        /// The area spanned by this node
        /// </summary>
        public AABB AABB
        {
            get { return _aabb; }
            set
            {
                if (_aabb != value && PositionChanged != null)
                {
                    _aabb = value;
                    PositionChanged(_aabb.Center);
                }
                else _aabb = value;
            }
        }

        /// <summary>
        /// The position of the node
        /// </summary>
        public override Vector2 Position
        {
            get { return _aabb.Center; }
            set
            {
                AABB newAABB = new AABB(value, _aabb.Width, _aabb.Height);

                if (_aabb != newAABB)
                {
                    var oldAABB = _aabb;

                    _aabb = newAABB;
                    if (oldAABB.Center != newAABB.Center && PositionChanged != null) PositionChanged(newAABB.Center);
                    if ((oldAABB.Width != newAABB.Width || oldAABB.Height != newAABB.Height) && SizeChanged != null) SizeChanged(newAABB.Size);

                }
                else _aabb = newAABB;
            }
        }

        /// <summary>
        /// Fired when the position changes
        /// </summary>
        public override event Action<Vector2> PositionChanged;

        /// <summary>
        /// Fired when the size of the AABB changes
        /// </summary>
        public event Action<Vector2> SizeChanged;
    }
}