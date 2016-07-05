/*+ + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + *\
+                                                                              +
+     Starball - multiplayer ball sports game with spinning star drones.       +
+     Copyright (C) 2016  SuperIzzo                                            +
+                                                                              +
+     This file is part of Starball.                                           +
+                                                                              +
+     Mulsh is free software: you can redistribute it and/or modify            +
+     it under the terms of the GNU General Public License as published by     +
+     the Free Software Foundation, either version 3 of the License, or        +
+     ( at your option) any later version.                                     +
+                                                                              +
+     Mulsh is distributed in the hope that it will be useful,                 +
+     but WITHOUT ANY WARRANTY; without even the implied warranty of           +
+     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the            +
+     GNU General Public License for more details.                             +
+                                                                              +
+     You should have received a copy of the GNU General Public License        +
+     along with Mulsh.  If not, see<http://www.gnu.org/licenses/>.            +
+                                                                              +
\*+ + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + + */
namespace Izzo.Starball
{
    using UnityEngine;
    using UnityEngine.Networking;

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  A player controller of star objects.  </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%    
    [AddComponentMenu( "Starball/Player/Star Controller" )]

    public class PlayerStarController : NetworkBehaviour
    {
        //-------------------------------------------------------------
        [SyncVar, SerializeField, Tooltip
        ( "The star this controller controls."                       )]
        //----------------------------------
        private NetworkIdentity _star;

        //-------------------------------------------------------------
        [SerializeField, Range(0.1f,10.0f), Tooltip
        ( "The rate at which input events will be sent to the " +
          "server.\nIncrease this value to reduce network traffic."  )]
        //----------------------------------
        private float _inputSendRate = 0.5f;

        //-------------------------------------------------------------
        [SerializeField, Range(0.1f,10.0f), Tooltip
        ( "The minimal move delta that will be set to the server.\n" +
          "Increase this value to reduce network traffic."           )]
        //----------------------------------
        private float _movementThreshold = 0.1f;

        //-------------------------------------------------------------
        [SerializeField, Range(0.1f,10.0f), Tooltip
        ( "The minimal spin delta that will be set to the server.\n" +
          "Increase this value to reduce network traffic."           )]
        //----------------------------------
        private float _spinThreshold = 0.1f;

        //-------------------------------------------------------------
        /// <summary>  Internal movement buffer that keeps all 
        ///            accumulated movement input.           </summary>
        //----------------------------------
        private Vector2 _movementInputBuffer;

        //-------------------------------------------------------------
        /// <summary>  Internal spin buffer that keeps all 
        ///            accumulated spin input.               </summary>
        //----------------------------------
        private float _spinInputBuffer;

        //-------------------------------------------------------------
        /// <summary>  The number of times the movement input 
        ///            has been set since the last reset.    </summary>
        //----------------------------------
        private int _timesMovementSet;

        //-------------------------------------------------------------
        /// <summary>  The number of times the spin input 
        ///            has been set since the last reset.    </summary>
        //----------------------------------
        private int _timesSpinSet;

        //-------------------------------------------------------------
        /// <summary>  Internal dispach time counter.        </summary>
        //----------------------------------
        private float _dispatchTimer;

        //-------------------------------------------------------------	
        /// <summary>  Internal property value holder.       </summary>
        //----------------------------------
        private Star _starCache;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  A reference to the star game object.  </summary>
        //::::::::::::::::::::::::::::::::::
        public Star star
        {
            // TODO: Ugly and error-prone
            get
            {
                if( _star == null )
                {
                    _starCache = null;
                }
                else if( _starCache == null
                      || _starCache.gameObject == null )
                {
                    _starCache = new Star( _star.gameObject );
                }
                return _starCache;
            }

            set
            {
                if( value != _starCache )
                {
                    if( value == null )
                    {
                        _star = null;
                        _starCache = null;
                    }
                    else
                    {
                        _star = value.networkIdentity;
                        _starCache = value;
                    }
                }
            }
        }

        //.............................................................
        /// <summary>  Gets the averaged movement input.     </summary>
        //..................................
        private Vector2 movementInput
        {
            get
            {
                if( _timesMovementSet > 0 )
                {
                    return _movementInputBuffer / _timesMovementSet;
                }
                else
                {
                    return Vector2.zero;
                }
            }
        }

        //.............................................................
        /// <summary>  Gets the averaged spin input.         </summary>
        //..................................
        private float spinInput
        {
            get
            {
                if( _timesSpinSet > 0 )
                {
                    return _spinInputBuffer / _timesSpinSet;
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        //.............................................................
        /// <summary>  Gets whether the movement has been set
        ///            since the last reset.                 </summary>
        //..................................
        private bool isMovementInputSet
        {
            get { return _timesMovementSet > 0; }
        }

        //.............................................................
        /// <summary>  Gets whether the spin has been set
        ///            since the last reset.                 </summary>
        //..................................
        private bool isSpinInputSet
        {
            get { return _spinInputBuffer > 0; }
        }

        //.............................................................
        /// <summary>  Cache of the previous movement input. </summary>
        //..................................
        private Vector2 previousMovementInput { get; set; }

        //.............................................................
        /// <summary>  Cache of the previous spin input.     </summary>
        //..................................
        private float previousSpinInput { get; set; }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  UNITY CALLBACK                        </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [ClientCallback]
        protected void Update()
        {
            if( isLocalPlayer )
            {
                HandleLocalPlayerInput();
                DispatchInputToServer();
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Collects the local player input.      </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [Client]
        private void HandleLocalPlayerInput()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            AddToMovementInputBuffer( x, y );

            float s = Input.GetAxis("Spin");
            AddToSpinBuffer( s );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Dispatches the collected local input 
        ///            to the server.                        </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [Client]
        private void DispatchInputToServer()
        {
            if( isMovementInputSet )
            {
                float dispatchTime = 1/_inputSendRate;
                _dispatchTimer += Time.deltaTime;

                Vector2 currentMovementInput = movementInput;
                float currentSpinInput = spinInput;

                var moveDelta = currentMovementInput - previousMovementInput;
                var spinDelta = currentSpinInput - previousSpinInput;

                bool isTimeToDispatch =
                    (_dispatchTimer >= dispatchTime);
                bool hasEnoughDisplacement =
                    (moveDelta.magnitude > _movementThreshold) ||
                    (Mathf.Abs(spinDelta) > _spinThreshold);

                if( isTimeToDispatch && hasEnoughDisplacement )
                {
                    CmdControl( currentMovementInput, currentSpinInput );

                    previousMovementInput = currentMovementInput;
                    previousSpinInput = currentSpinInput;
                    _dispatchTimer = 0;
                }

                ResetMovementInputBuffer();
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        ///     Adds movement input to the internal buffer.  </summary>
        /// <param name="x"> The horizonta axes reading.       </param>
        /// <param name="y"> The vertical axes reading.        </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [Client]
        private void AddToMovementInputBuffer( float x, float y )
        {
            _movementInputBuffer.x += x;
            _movementInputBuffer.y += y;
            _timesMovementSet++;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Adds spin input to the internal buffer.</summary>
        /// <param name="s"> The spin axis reading.            </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [Client]
        private void AddToSpinBuffer( float s )
        {
            _spinInputBuffer += s;
            _timesSpinSet++;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Resets the movement buffer to 
        ///            accumulate a new input frame.         </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [Client]
        private void ResetMovementInputBuffer()
        {
            _movementInputBuffer = Vector2.zero;
            _spinInputBuffer = 0;

            _timesMovementSet = 0;
            _timesSpinSet = 0;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Executes the movement and spin input
        ///            on the server.                        </summary>
        /// <param name="movementInput"> 
        ///     The movement input vector.                     </param>
        /// <param name="spinInput"> 
        ///     The spin input scalar.                         </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [Command]
        private void CmdControl( Vector2 movementInput, float spinInput )
        {
            if( star != null && star.model )
            {
                star.model.Move( movementInput );
                star.model.Spin( spinInput );
            }
        }
    }
}