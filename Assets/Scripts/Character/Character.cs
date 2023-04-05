using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Character : MonoBehaviour
    {
        #region Inspector Properties
        /**
         * <summary>Walk Speed of the character. This is set in the inspector.</summary>
         */
        [Tooltip("Character walk speed")]
        public Vector2 walkSpeed;
        
        /**
         * <summary>Run Speed of the character. This is set in the inspector.</summary>
         */
        [Tooltip("Character run speed")]
        public Vector2 runSpeed;
        #endregion

        #region Internal Components
        /**
         * <summary>Main rigid body for the character. This is loaded automatically at Start().</summary>
         */
        [HideInInspector]
        public Rigidbody2D mainBody;
        #endregion
        
        #region Motion State Machine
        /**
         * <summary>Enumeration for the different motion states.
         * States are changed by the enum rather than the object.</summary>
         */
        public enum MotionStates
        {
            /**
             * <summary>Idle state. Not in motion, but still animated.</summary>
             */
            Idle, 
            /**
             * <summary>Walk state. Moving in the controlled direction at character.walkSpeed.</summary>
             */
            Walk, 
            /**
             * <summary>Run state. Moving in the controlled direction at character.runSpeed</summary>
             */
            Run
        };

        /**
         * <summary>The current MotionState for the character.</summary>
         */
        private MotionState motionState;
        
        /**
         * <summary>Static list of possible motion states. Order conforms to the MotionStates enum.</summary>
         */
        private readonly MotionState[] motionStates = new MotionState[] { new IdleState(), new WalkState(), new RunState() };

        /**
         * <summary>MotionStatus structure attached to this character.</summary>
         */
        public MotionStatus MotionStatus;
        #endregion
        
        #region Unity Loops
        /**
         * <summary>Standard unity start point. Initializes state machines and
         * finds required components.</summary>
         */
        private void Start()
        {
            mainBody = gameObject.GetComponent<Rigidbody2D>();
            MotionStatus = new MotionStatus();
            motionState = motionStates[(int)MotionStates.Idle];
        }

        /**
         * <summary>Standard unity update loop.</summary>
         */
        private void Update()
        {
        
        }

        /**
         * <summary>Standard unity fixed update loop. Updates character motion.</summary>
         */
        private void FixedUpdate()
        {
            mainBody.velocity = MotionStatus.MotionVelocity;
        }
        #endregion

        #region Motion Controller
        /**
         * <summary>The state swap for the motion state. Called from the motion state machine.</summary>
         * <param name="value">The enumerated mode to switch to.</param>
         */
        public void ChangeMotionState(MotionStates value)
        {
            motionState = motionStates[(int)value];
            motionState.StateBegin(this);
        }

        /**
         * <summary>Callback from the motion state machine to make the character change direction.
         * This flips the gameObject by inverting the x Scale.</summary>
         * <param name="forward">Forward facing is to the right of the screen.</param>
         */
        public void ChangeDirection(bool forward)
        {
            var mult = forward ? 1.0f : -1.0f;
            var origScale = transform.localScale;
            var orig = Mathf.Abs(origScale.x);
            transform.localScale = new Vector3(mult * orig, origScale.y, origScale.z);
        }
        #endregion
        
        #region Inputs
        /**
         * <summary>Captures the player input that simulates the move stick when it changes.</summary>
         * <param name="value">The current value of the stick.</param>
         */
        private void OnMove(InputValue value)
        { 
            var stick = value.Get<Vector2>();
            motionState.MoveChange(this, stick);
        }

        /**
         * <summary>Captures the player input that simulates the run button when it changes.</summary>
         * <param name="value">The current value of the button as a float.</param>
         */
        private void OnRun(InputValue value)
        {
            var pressed = Mathf.Abs(value.Get<float>()) > 0.001;
            motionState.RunChange(this, pressed);
        }
        #endregion
    }
}
