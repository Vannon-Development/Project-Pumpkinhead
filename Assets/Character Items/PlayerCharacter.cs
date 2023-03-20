using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerCharacter : MonoBehaviour
{
    [Serializable]
    public class PlayerAction
    {
        [Flags]
        public enum InitialStates { None = 0, Still = 1, Walking = 2, Running = 4 }
        public enum ActionCommand { FastAttack, SlowAttack, SpecialAttack, Dodge, Jump }
        
        [Serializable]
        public class PlayerActionItem
        {
            [FormerlySerializedAs("action")] [Tooltip("The button pushed to initiate this action")]
            public ActionCommand actionCommand;
            [Tooltip("The number of the animation triggered by this action")]
            public int animationIndex;
            [Tooltip("Whether this action has to hit an enemy or object to allow the next action to be triggered")]
            public bool requiresHit;
        }

        [Tooltip("The name of this set of actions")]
        public string name;
        [Tooltip("The initial states this character must be in to trigger this action sequence ")]
        public InitialStates initialState;
        [Tooltip("The list of individual actions that make up this chain")]
        public PlayerActionItem[] chain;
    }

    private class ActionController
    {
        private readonly PlayerAction[] playerActions;
        private List<PlayerAction> possibleActions;
        private PlayerAction.PlayerActionItem current;
        private bool comboLocked;
        private int currentChain;
        private bool nextLocked;
        private bool didHit;
        private bool actionQueued;

        public ActionController(PlayerAction[] actions)
        {
            playerActions = actions;
        }

        public bool StartAction(PlayerAction.ActionCommand actionCommand, bool walking, bool running)
        {
            PlayerAction.InitialStates i = PlayerAction.InitialStates.Still;
            if (running) i = PlayerAction.InitialStates.Running;
            else if (walking) i = PlayerAction.InitialStates.Walking;

            var filter = from item in playerActions
                where item.initialState.HasFlag(i) && item.chain[0].actionCommand == actionCommand
                select item;
            possibleActions = new List<PlayerAction>(filter);

            if (possibleActions.Count == 0) return false;
            
            current = possibleActions[0].chain[0];
            comboLocked = false;
            currentChain = 1;
            nextLocked = false;
            didHit = false;
            actionQueued = true;
            return true;
        }
        public void AddAction(PlayerAction.ActionCommand actionCommand)
        {
            if (!comboLocked && current != null)
            {
                if (!nextLocked)
                {
                    var filter = from item in possibleActions
                        where item.chain.Length > currentChain && item.chain[currentChain].actionCommand == actionCommand
                        select item;
                    possibleActions = new List<PlayerAction>(filter);
                    if (possibleActions.Count == 0)
                    {
                        comboLocked = false;
                    }
                    else
                    {
                        nextLocked = true;
                    }
                }
                else
                {
                    comboLocked = true;
                }
            }
        }

        public void RegisterHit()
        {
            didHit = true;
        }

        public int CurrentIndex => current?.animationIndex ?? 0;

        public void NextAction()
        {
            if (!nextLocked) return;
            var filter = from item in possibleActions
                where !item.chain[currentChain].requiresHit || item.chain[currentChain].requiresHit == didHit
                select item;
            possibleActions = new List<PlayerAction>(filter);
            if (possibleActions.Count == 0)
            {
                current = null;
                nextLocked = false;
            }
            else
            {
                current = possibleActions[0].chain[currentChain];
                currentChain += 1;
                nextLocked = false;
                didHit = false;
                actionQueued = true;
            }
        }

        public bool Ready
        {
            get
            {
                bool ret = actionQueued;
                actionQueued = false;
                return ret;
            }
        }
    }
    
    private class MotionController
    {
        public bool DirectionForward { get; private set; } = true;
        public Vector2 MotionVector { get; private set; } = Vector2.zero;
        private bool actionMode;

        private bool forwardChanged;
        private bool running;
        private bool motionChanged;

        public void MoveChanged(Vector2 val)
        {
            MotionVector = val;
            UpdateDirection(false);
            motionChanged = true;
        }

        public bool InActionMode
        {
            get => actionMode;
            set
            {
                actionMode = value;
                if (!value) UpdateDirection(true);
            }
        }

        private void UpdateDirection(bool force)
        {
            if (!actionMode && (Mathf.Abs(MotionVector.x) > 0.001 || force))
            {
                var newForward = MotionVector.x > 0;
                if (DirectionForward != newForward)
                {
                    DirectionForward = newForward;
                    forwardChanged = true;
                }
            }
        }

        public void RunChanged(bool val)
        {
            running = val;
            motionChanged = true;
        }
        
        public bool DirectionChanged
        {
            get
            {
                var ret = forwardChanged;
                forwardChanged = false;
                return ret;
            }
        }

        public bool MotionChanged
        {
            get
            {
                var ret = motionChanged || forwardChanged;
                motionChanged = false;
                return ret;
            }
        }

        public bool IsWalking => MotionVector.magnitude > 0.001;
        public bool IsRunning => running && IsWalking;
    }

    [Tooltip("The speed this character moves while in walk mode.")]
    public Vector2 walkSpeed;
    [Tooltip("The speed this character moves while in run mode")]
    public Vector2 runSpeed;
    [Tooltip("The actions registered to this character")]
    public PlayerAction[] actions;

    private Rigidbody2D body;
    private Animator ani;
    private MotionController motion;
    private bool resumeMove;
    private ActionController action;
    
    private static readonly int Action = Animator.StringToHash("action");
    private static readonly int Motion = Animator.StringToHash("motion");

    public void Start()
    {
        body = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        motion = new MotionController();
        action = new ActionController(actions);
    }

    public void Update()
    {
        if (motion.InActionMode)
        {
            if (action.Ready)
            {
                ani.SetInteger(Action, action.CurrentIndex);
            }
        }
        else if (motion.MotionChanged || resumeMove)
        {
            resumeMove = false;
            if (motion.DirectionChanged)
            {
                Flip(motion.DirectionForward);
            }
            if (motion.IsRunning)
            {
                ani.SetInteger(Motion, 2);
                body.velocity = runSpeed * motion.MotionVector;
            }
            else if (motion.IsWalking)
            {
                ani.SetInteger(Motion, 1);
                body.velocity = walkSpeed * motion.MotionVector;
            }
            else
            {
                ani.SetInteger(Motion, 0);
                body.velocity = Vector2.zero;
            }
        }
    }

    public void OnMove(InputValue value)
    { 
        var stick = value.Get<Vector2>();
        motion.MoveChanged(stick);
    }

    public void OnFastAttack()
    {
        DoAction(PlayerAction.ActionCommand.FastAttack);
    }

    public void OnSlowAttack()
    {
        DoAction(PlayerAction.ActionCommand.SlowAttack);
    }

    public void OnRun(InputValue value)
    {
        motion.RunChanged(Mathf.Abs(value.Get<float>()) > 0.001);
    }

    private void DoAction(PlayerAction.ActionCommand button)
    {
        if(motion.InActionMode)
            action.AddAction(button);
        else
        {
            motion.InActionMode = action.StartAction(button, motion.IsWalking, motion.IsRunning);
            if(motion.InActionMode) body.velocity = Vector2.zero;
        }
    }

    public void ActionPerformed()
    {
        action.NextAction();
    }

    public void ActionEnded()
    {
        motion.InActionMode = false;
        ani.SetInteger(Action, 0);
        resumeMove = true;
    }

    public void VelocityNudge(float val)
    {
        body.velocity = new Vector2(val * (motion.DirectionForward ? 1 : -1), 0);
    }
    
    private void VelocityCancel()
    {
        body.velocity = Vector2.zero;
    }

    private void Flip(bool forward)
    {
        Vector2 current = transform.localScale;
        transform.localScale = new Vector2(Mathf.Abs(current.x) * (forward ? 1 : -1) , current.y);
    }
}
