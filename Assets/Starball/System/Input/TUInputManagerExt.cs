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
namespace Izzo.Input
{
    using UnityEngine;
    using System;
    
    using TUInputManager = TeamUtility.IO.InputManager;
    using TUPlayerID = TeamUtility.IO.PlayerID;
    using TeamUtility.IO;
    using System.Collections.Generic;
    

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  A wrapper around TeamUtility.IO.InputManager 
    ///            with extended functionality.          </summary>
    /// <remarks>  The class provides extensions to support
    ///            multiple alternative mappings to uniquely
    ///            named controls and automatic control
    ///            scheme management for four players with
    ///            joystick and keyboard setup.           
    /// 
    ///            Multiple mappings for the same virtual axes and
    ///            buttons can be defined in the configuration
    ///            editor provided with InputManager by postfixing
    ///            their names with ":X" where X is a umber between
    ///            2 and 10. For instances: "Horizontal",
    ///            "Horizontal:2", "Horizontal:3" etc will all
    ///            trigger the "Horizontal" axis.
    ///            
    ///            Note that there axis definitions must be
    ///            sequential and there cannot be gaps. In other
    ///            word "Horizontal:5" and "Horizontal:6" will
    ///            not be checked unless all numbers prior to
    ///            that have been defined in the configurations
    ///            manager.                              </remarks>
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
    //  TODO:  This class can be split into a couple of smaller ones
    //         as it has a number of different responsibilities.
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class TUInputManagerExt : MonoBehaviour, IInputManager
    {
        public const int maxAlternativeConfigs = 10;

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        /// <summary>  A data structure to hold keyboard and joystick
        ///            configurations for players.           </summary>
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        [Serializable]
        private struct PlayerConfig
        {
            public string keyboardConfig;
            public string joysticConfig;
            public int    joystic;
        }
        
        //-------------------------------------------------------------
        [SerializeField, Tooltip
        (   "The default input configuations for player one."        )]
        //--------------------------------------
        PlayerConfig _defaultPlayer1;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        (   "The default input configuations for player two."        )]
        //--------------------------------------
        PlayerConfig _defaultPlayer2;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        (   "The default input configuations for player three."      )]
        //--------------------------------------
        PlayerConfig _defaultPlayer3;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        (   "The default input configuations for player four."       )]
        //--------------------------------------
        PlayerConfig _defaultPlayer4;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <inheritDoc/>
        //::::::::::::::::::::::::::::::::::
        public Vector2 mousePosition
        {
            get
            {
                return TUInputManager.mousePosition;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <inheritDoc/>
        //::::::::::::::::::::::::::::::::::
        public bool mousePresent
        {
            get
            {
                return TUInputManager.mousePresent;
            }
        }
        
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <inheritDoc/>
        //::::::::::::::::::::::::::::::::::
        public int touchCount
        {
            get
            {
                return TUInputManager.touchCount;
            }
        }
        
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <inheritDoc/>
        //::::::::::::::::::::::::::::::::::
        public bool touchSupported
        {
            get
            {
                return TUInputManager.touchSupported;
            }
        }
        
        //=============================================================
        /// <summary>  Returns the value of the virtual axis obtained 
        ///            from multiple AxisConfiguration-s.    </summary>
        /// <remarks>  Returns the maximal signal strength from all 
        ///            defined virtual axis mappings.        </remarks>
        //==================================
        public float GetAxis( string axis, short playerID )
        {
            float axisSignal = 0;

            foreach( var axisConfig in AxisConfigurations( playerID, axis ) )
            {
                float newAxisSignal = axisConfig.GetAxis();

                if( Mathf.Abs( newAxisSignal ) > Mathf.Abs( axisSignal ) )
                {
                    axisSignal = newAxisSignal;
                }
            }

            return axisSignal;
        }

        //=============================================================
        /// <summary>  Returns the unsmoothed value 
        ///            of the virtual axis obtained from multiple 
        ///            AxisConfiguration-s.                  </summary>
        /// <remarks>  Returns the maximal signal strength from
        ///            all defined virtual axis mappings.    </remarks>
        //==================================
        public float GetAxisRaw( string axis, short playerID )
        {
            float axisSignal = 0;

            foreach( var axisConfig in AxisConfigurations( playerID, axis ) )
            {
                float newAxisSignal = axisConfig.GetAxisRaw();

                if( Mathf.Abs( newAxisSignal ) > Mathf.Abs( axisSignal ) )
                {
                    axisSignal = newAxisSignal;
                }
            }

            return axisSignal;
        }

        //=============================================================
        /// <summary>  Returns the value of the virtual button obtained 
        ///            from multiple AxisConfiguration-s.    </summary>
        /// <remarks>  Returns true if any of the defined virtual 
        ///            button mappings is being held down.   </remarks>
        //==================================
        public bool GetButton( string button, short playerID )
        {
            bool buttonSignal = false;

            foreach( var axisConfig in AxisConfigurations( playerID, button ) )
            {
                buttonSignal |= axisConfig.GetButton();
            }

            return buttonSignal;
        }

        //=============================================================
        /// <summary>  Returns the whether at least one of the virtual
        ///            buttons for the configurations has been pressed
        ///            this frame.                           </summary>
        /// <remarks>  Returns true if any of the defined virtual 
        ///            button mappings was pressed. Because of this 
        ///            it is possible to receive two or more ButtonDown
        ///            events before a ButtonUp.             </remarks>
        //==================================
        public bool GetButtonDown( string button, short playerID )
        {
            bool buttonSignal = false;

            foreach( var axisConfig in AxisConfigurations( playerID, button ) )
            {
                buttonSignal |= axisConfig.GetButtonDown();
            }

            return buttonSignal;
        }

        //=============================================================
        /// <summary>  
        ///     Returns the whether at least one of the virtual buttons
        ///     for the configurations has been released this frame.                           
        ///                                                  </summary>
        /// <remarks>  
        ///     Returns true if any of the defined virtual 
        ///     button mappings was released. Because of this it is
        ///     possible to receive two or more ButtonDown events
        ///     before a ButtonUp.                           </remarks>
        //==================================
        public bool GetButtonUp( string button, short playerID )
        {
            bool buttonSignal = false;

            foreach( var axisConfig in AxisConfigurations( playerID, button ) )
            {
                buttonSignal |= axisConfig.GetButtonUp();
            }

            return buttonSignal;
        }

        //=============================================================
        /// <summary>  Returns whether the specified mouse button was
        ///            pressed this frame.                   </summary>
        //==================================
        public bool GetMouseButtonDown( int button )
        {
            return TUInputManager.GetMouseButtonDown( button );
        }

        //=============================================================
        /// <summary>  Returns the touch at the given index.  </summary>
        //==================================
        public Touch GetTouch( int index )
        {
            return TUInputManager.GetTouch( index );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> CALLBACK. Sets up the input manager.   </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected void Awake()
        {
            if( InputManager.active == null )
            {
                InputManager.active = this;
                LoadInputConfigs();
            }
            else
            {
                Debug.LogError( "An InputManager instance is already loaded in the scene." );
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Loads the default input configurations. </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void LoadInputConfigs()
        {
            string player1Config = MergeKeyboardJoyConfigs(
            _defaultPlayer1.keyboardConfig,
            _defaultPlayer1.joysticConfig,
            _defaultPlayer1.joystic );

            string player2Config = MergeKeyboardJoyConfigs(
            _defaultPlayer2.keyboardConfig,
            _defaultPlayer2.joysticConfig,
            _defaultPlayer2.joystic );

            string player3Config = MergeKeyboardJoyConfigs(
            _defaultPlayer3.keyboardConfig,
            _defaultPlayer3.joysticConfig,
            _defaultPlayer3.joystic );

            string player4Config = MergeKeyboardJoyConfigs(
            _defaultPlayer4.keyboardConfig,
            _defaultPlayer4.joysticConfig,
            _defaultPlayer4.joystic );

            TUInputManager.SetInputConfiguration( player1Config, TUPlayerID.One );
            TUInputManager.SetInputConfiguration( player2Config, TUPlayerID.Two );
            TUInputManager.SetInputConfiguration( player3Config, TUPlayerID.Three );
            TUInputManager.SetInputConfiguration( player4Config, TUPlayerID.Four );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  AxisConfiguration enumerator, returns all axis
        ///            configurations that correspond to the the given
        ///            name fore playerID.                   </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private static IEnumerable<AxisConfiguration> 
            AxisConfigurations( int playerID, string name )
        {
            AxisConfiguration axisConfig = GetAxisConfiguration( playerID, name);

            if( axisConfig != null )
            {
                yield return axisConfig;
            }
            else
            {
                Debug.LogError( string.Format( "\'{0}\' does not exist in the active input configuration for player {1}", name, playerID ) );
                yield break;
            }

            for( int i = 2; i <= maxAlternativeConfigs; i++ )
            {
                axisConfig = GetAxisConfiguration( playerID, name + ":" + i );

                if( axisConfig != null )
                {
                    yield return axisConfig;
                }
                else
                {
                    yield break;
                }
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  A shorthand for method for 
        /// TeamUtility.IO.InputManager.GetAxisConfiguration.</summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private static AxisConfiguration 
            GetAxisConfiguration( int playerID, string name )
        {
            return TUInputManager.GetAxisConfiguration( 
                (TUPlayerID) playerID,  name );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> 
        ///     Merges and retargets a keyboard and a joystick
        ///     InputConfiguration-s into a single one.      </summary>
        /// <remarks> 
        ///     The joystick is always the second configuration. 
        ///     Retargeting (a joystick) means changing all button
        ///     KeyCode-s so that they correspond to the given
        ///     joystick id.                                 </remarks>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private static string 
            MergeKeyboardJoyConfigs( string kbd, string joy, int joyID )
        {
            var kbdConfig = TUInputManager.GetInputConfiguration(kbd);
            var joyConfig = TUInputManager.GetInputConfiguration(joy);

            if( kbdConfig != null || joyConfig != null )
            {
                InputConfiguration config =
                MergeKeyboardJoyConfigs( kbdConfig, joyConfig, joyID );

                return config.name;
            }

            return null;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Merges and retargets a keyboard and a joystick
        ///           InputConfiguration-s into a single one.</summary>
        /// <remarks> The joystick is always the second configuration. 
        ///           Retargeting (a joystick) means changing all 
        ///           button KeyCode-s so that they correspond 
        ///           to the given joystick id.              </remarks>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private static InputConfiguration 
            MergeKeyboardJoyConfigs( InputConfiguration kbd,
                                     InputConfiguration joy,
                                     int joyID )
        {
            if( joy == null )
                return kbd;

            string combinedConfigName = "";

            if( kbd != null )
            {
                combinedConfigName = kbd.name + " & ";
            }

            combinedConfigName += joy.name + "(" + joyID + ")";

            InputConfiguration combinedConfig =
            TUInputManager.CreateInputConfiguration( combinedConfigName );


            var axes = new List<AxisConfiguration>();
            var axesMap = new Dictionary<string,int>();

            if( kbd != null )
            {
                foreach( AxisConfiguration axis in kbd.axes )
                {
                    var newAxis = new AxisConfiguration();
                    newAxis.Copy( axis );
                    axes.Add( newAxis );
                    axesMap[axis.name] = 1;
                }
            }

            if( joy != null )
            {
                foreach( AxisConfiguration axis in joy.axes )
                {
                    var newAxis = new AxisConfiguration();
                    newAxis.Copy( axis );
                    newAxis.joystick = joyID;

                    newAxis.positive =
                        RetargetJoysticButton( newAxis.positive, joyID );

                    newAxis.negative =
                        RetargetJoysticButton( newAxis.negative, joyID );

                    newAxis.altPositive =
                        RetargetJoysticButton( newAxis.altPositive, joyID );

                    newAxis.altNegative =
                        RetargetJoysticButton( newAxis.altNegative, joyID );

                    if( axesMap.ContainsKey( newAxis.name ) )
                    {
                        axesMap[newAxis.name]++;
                        newAxis.name += ":" + axesMap[newAxis.name];
                    }

                    axes.Add( newAxis );
                }
            }

            combinedConfig.axes = axes;
            combinedConfig.UpdateAxes();

            return combinedConfig;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Changes a joystick button KeyCode to the correct
        ///           KeyCode for a specific joystic id.      </remarks>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        static KeyCode RetargetJoysticButton( KeyCode joyButton, int joyID )
        {
            // HACK: This is using a little bit of hackery and 
            // some arcane knowledge about keycode ordering in Unity.
            // Joystic keycodes are (at the time of writing) ordered
            // sequentially starting at JoystickButton0 (=330) and
            // going through all joystic ids from 1 to 8 and all
            // buttons from 0 to 19 it ends at Joystick8Button19 (=509)
            // Non of this is guaranteed to stay the same in the future.
            // In the following code we make the assumptions that:
            //  a) JoystickButton0 is the first joysic button entry in the 
            //     enum and Joystick8Button19 is the last
            //  b) The numbering of buttons and joystics is sequential 
            //     (no bit masks or anything) 
            //  c) Every joystic has the same maximum of possible buttons

            KeyCode firstJoyButton = KeyCode.JoystickButton0;
            KeyCode lastJoyButton = KeyCode.Joystick8Button19;

            if( joyButton >= firstJoyButton && joyButton <= lastJoyButton )
            {
                // The max number of buttons (20, but we don't know that)
                int maxJoyButtonNumber = 
                    KeyCode.Joystick2Button0 - KeyCode.Joystick1Button0;

                // This will give us the plain button id without the joystic id
                int joyButtonID = 
                    (joyButton - firstJoyButton) % maxJoyButtonNumber;

                // Joystick id and button id (offset), the +1 is because the
                // first set is global KeyCodes, rather than the first joystick
                int offset = joyButtonID + maxJoyButtonNumber * (joyID+1);

                // The correct KeyCode (hopefully)
                return firstJoyButton + offset;
            }

            return joyButton;
        }
    }
}