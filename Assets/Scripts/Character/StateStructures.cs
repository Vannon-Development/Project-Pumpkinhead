using UnityEngine;

namespace Character
{
    /**
     * <summary>Data structure that contains many MotionState specific variables.</summary>
     */
    public struct MotionStatus
    {
        /**
         * <summary>Flag for whether run is currently activated.</summary>
         */
        public bool Run;
        
        /**
         * <summary>Flag for whether the character is currently facing forward (right).</summary> 
         */
        public bool DirectionIsForward;
        
        /**
         * <summary>The last recorded motion vector, either from player input or AI controller.</summary>
         */
        public Vector2 CurrentMotion;
        
        /**
         * <summary>The current velocity caused by the motion controller. This is passed back to the character's
         * Fixed Update loop to get applied to the body.</summary>
         */
        public Vector2 MotionVelocity;
    }

    public class ActionStatus
    {
        
    }

    /**
     * <summary>Class holding all of the character animation information.</summary>
     */
    public class AnimationStatus
    {
        /**
         * <summary>Enumeration of motion animation states.</summary>
         */
        public enum MotionStates { Idle, Walking, Running }
        
        /**
         * <summary>Hash for the action animation state.</summary>
         */
        public static readonly int ActionHash = Animator.StringToHash("action");
        
        /**
         * <summary>Hash for the motion animation state.</summary>
         */
        public static readonly int MotionHash = Animator.StringToHash("motion");

        /**
         * <summary>Animator for the character.</summary>
         */
        public readonly Animator Animator;

        /**
         * <summary>Only constructor requires the animator to be set.</summary>
         */
        public AnimationStatus(Animator animator)
        {
            Animator = animator;
        }
    }
}