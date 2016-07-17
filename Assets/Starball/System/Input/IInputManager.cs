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
    /// <summary> A general InputManager interface.      </summary>
    /// <remarks>
    ///     Can be implemented by various input generators,
    ///     decorators or filters to collect and manipulate 
    ///     data from input provides.                    </remarks>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public interface IInputManager
    {
        Vector2 mousePosition { get; }
        bool mousePresent { get; }
        bool touchSupported { get; }
        int touchCount { get; }

        float GetAxis( string axis, short playerID = 0 );
        bool GetButton( string button, short playerID = 0 );
        bool GetButtonUp( string button, short playerID = 0 );
        bool GetButtonDown( string button, short playerID = 0 );
        float GetAxisRaw( string axis, short playerID = 0 );
        bool GetMouseButtonDown( int button );
        Touch GetTouch( int index );
    }
}