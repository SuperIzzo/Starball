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

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  Generic access to the Input API.      </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public static class InputManager
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Gets or sets the currently active
        ///            input system.                         </summary>
        //::::::::::::::::::::::::::::::::::
        public static IInputManager active { get; set; }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  The current mouse position 
        ///            in pixel coordinates. (Read Only)     </summary>
        //::::::::::::::::::::::::::::::::::
        public static Vector2 mousePosition
        {
            get
            {
                if( active != null )
                {
                    return active.mousePosition;
                }
                else
                {
                    Debug.Log( "No active input manager set." );
                    return Vector2.zero;
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Returns whether a mouse device is 
        ///            currently present. (Read Only)        </summary>
        //::::::::::::::::::::::::::::::::::
        public static bool mousePresent
        {
            get
            {
                if( active != null )
                {
                    return active.mousePresent;
                }
                else
                {
                    Debug.Log( "No active input manager set." );
                    return false;
                }
            }
        }        

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>Returns whether the device on which application is
        ///          currently running supports touch input. </summary>
        //::::::::::::::::::::::::::::::::::
        public static bool touchSupported
        {
            get
            {
                if( active != null )
                {
                    return active.touchSupported;
                }
                else
                {
                    Debug.Log( "No active input manager set." );
                    return false;
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        /// <summary>  Number of touches. Guaranteed not to change 
        ///            throughout the frame. (Read Only)     </summary>
        //::::::::::::::::::::::::::::::::::
        public static int touchCount
        {
            get
            {
                if( active != null )
                {
                    return active.touchCount;
                }
                else
                {
                    Debug.Log( "No active input manager set." );
                    return 0;
                }
            }
        }

        //=============================================================
        /// <summary>  Returns the value of the virtual axis
        ///            identified by axisName and playerID.  </summary>
        //==================================
        public static float GetAxis( string axisName, short playerID=0 )
        {
            if( active != null )
            {
                return active.GetAxis( axisName, playerID );
            }
            else
            {
                Debug.Log( "No active input manager set." );
                return 0.0f;
            }
        }

        //=============================================================
        /// <summary>  Returns the value of the virtual axis 
        ///            identified by axisName and playerID 
        ///            with no smoothing filtering applied.  </summary>
        //==================================
        public static float GetAxisRaw( string axisName, short playerID=0 )
        {
            if( active != null )
            {
                return active.GetAxisRaw( axisName, playerID );
            }
            else
            {
                Debug.Log( "No active input manager set." );
                return 0.0f;
            }
        }

        //=============================================================
        /// <summary>  Returns true while the virtual button
        ///            identified by buttonName and playerID
        ///            is held down.                         </summary>
        //==================================        
        public static bool GetButton( string button, short playerID=0 )
        {
            if( active != null )
            {
                return active.GetButton( button, playerID );
            }
            else
            {
                Debug.Log( "No active input manager set." );
                return false;
            }
        }

        //=============================================================
        /// <summary>  Returns true during the frame the user 
        ///            pressed down the virtual button 
        ///            identified by buttonName and playerID.</summary>
        //==================================
        public static bool GetButtonDown( string button, short playerID=0 )
        {
            if( active != null )
            {
                return active.GetButtonDown( button, playerID );
            }
            else
            {
                Debug.Log( "No active input manager set." );
                return false;
            }
        }

        //=============================================================
        /// <summary>  Returns true during the frame the user 
        ///            releases the virtual button 
        ///            identified by buttonName and playerID. </summary>
        //==================================
        public static bool GetButtonUp( string button, short playerID=0 )
        {
            if( active != null )
            {
                return active.GetButtonUp( button, playerID );
            }
            else
            {
                Debug.Log( "No active input manager set." );
                return false;
            }
        }

        //=============================================================
        /// <summary>  Returns true during the frame the user 
        ///            pressed the given mouse button.        </summary>
        //==================================
        public static bool GetMouseButtonDown( int button )
        {
            if( active != null )
            {
                return active.GetMouseButtonDown( button );
            }
            else
            {
                Debug.Log( "No active input manager set." );
                return false;
            }
        }

        //=============================================================
        /// <summary>  Returns object representing status 
        ///            of a specific touch. (Does not allocate 
        ///            temporary variables).                  </summary>
        //==================================
        public static Touch GetTouch( int touchN )
        {
            if( active != null )
            {
                return active.GetTouch( touchN );
            }
            else
            {
                Debug.Log( "No active input manager set." );
                return new Touch();
            }
        }
    }
}