using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

/* State Pattern base classes
 * by Mikhail Naumov
 * 
 * The state pattern, which closely resembles Strategy Pattern, 
 * is a behavioral software design pattern, also known as the objects for states pattern. 
 * This pattern is used in computer programming to represent the state of an object. 
 * This is a clean way for an object to partially change its type at runtime
 * 
 * The goal of the State Pattern is to create a cleaner way of changing object's functionality at runtime
 * without having to use multiple "switch" or "if" loops. This pattern is handy when object can enter
 * different "states" that modify its behavior. To easier understand this, imagine a state diagram,
 * where there is only one active (current) state that can transition into neearby states based on some
 * conditions.
 * 
 * For example imagine a multi-stage boss fight in WoW:
 * At 1st stage the boss will perform basic attack. 
 * Once its life is below 75% it enters stage 2 and performs magic attacks. 
 * After 33% it enters its "enrage" state, etc.
 * One way of implementing this would be using "switch" or "if" loops, e.g. 
 * 
 * Update()
 * {
 *     if (life > 75%)
 *         //stage 1 behavior
 *     if (life > 33%)
 *         //stage 2 behavior
 *     else
 *         /stage 3 behavior
 * }
 * 
 * But this gets messy and hard to follow if there are numerous stages, which significantly alter
 * its behavior. This also becomes very error prone.
 * 
 * A state pattern allows to use a component-based design to implement state changes.
 * The way it works is it wraps every behavior in a "state" object. These objects are then added
 * to the StateContext one at a time as specified behavior is expected. State transitions are performed 
 * within states themselves. Following the above example, the code would look something like this:
 * 
 * Update()
 * {
 *     activeState.Update();
 * }
 * 
 * Each state then would need to be implemented in a separate class, inheriting from the State base:
 * 
 * public class Stage1Behavior : State
 * {
 *     public override void Update(GameTime time)
 *     {
 *         //do stage 1 behavior
 *         if (life < 75%)
 *             _context.SetState(Stage2Behavior); //transition into stage2
 *     }
 * }
 * 
 * For the state transtion to work, every State needs to be aware of its StateContext, or the class
 * that keeps track of active state and updates it. 
 * In example above boss will need to either: 
 * 1) inherit from StateContext or 
 * 2) include an instance of StateContext.
 * 
 * See: http://en.wikipedia.org/wiki/State_pattern
*/

namespace ModelViewer
{
    /// <summary>
    /// A container class that keeps track of active state and updates it. Inheriting this in your custom
    /// StateContext is a good idea, since you will probably need to extend it with variables that
    /// need to be visible to all states
    /// </summary>
    public class StateContext
    {
        /// <summary>
        /// Current state instance to update
        /// </summary>
        State _currentState = null;

        /// <summary>
        /// Current state instance to update, can not be set to, since state transtions are handled internally
        /// </summary>
        public State CurrentState { get { return _currentState; } }

        /// <summary>
        /// Constructor with no initial state
        /// </summary>
        public StateContext()
        {
        }

        /// <summary>
        /// Constructor that defines initial state
        /// </summary>
        /// <param name="initialState">initial state to set</param>
        public StateContext(State initialState)
        {
            SetState(initialState);
        }

        /// <summary>
        /// Sets the current state, use with empty ctor
        /// </summary>
        /// <param name="state">state to set</param>
        public void SetState(State state)
        {
            if (state != null) state.Initialize();
            _currentState = state;
        }

        /// <summary>
        /// Updates the current state
        /// </summary>
        /// <param name="time">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime time)
        {
            if (_currentState != null) _currentState.Update(time);
        }
    }

    /// <summary>
    /// State class defines a certain functionality or behavior as well as what state transtions
    /// to take based on different conditions. Create a state for every different behavior an object
    /// may perform
    /// </summary>
    public abstract class State
    {
        /// <summary>
        /// State needs to be aware of its container, StateContext to be able to access variables
        /// and perform state transsitions
        /// </summary>
        protected StateContext _context;

        /// <summary>
        /// Creates a new state
        /// </summary>
        /// <param name="context">StateContext instance owning this state</param>
        public State(StateContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Iniitializes the state in case you want to add some fancy object pooling, this is not necessary
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Updating state behavior, watching for conditions to initiate state transitions - all go here
        /// </summary>
        /// <param name="time">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime time)
        {
        }
    }
}
