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
    using Networking;
    using UnityEngine;

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  An editor utility class to automatically
    ///            populate NetworkComponentSelector with 
    ///            components of specified types.        </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public class StarballNetworkComponentTypeSelector : MonoBehaviour
    {
#if UNITY_EDITOR
        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "A list of objects to be stripped off their components."   )]
        //----------------------------------
        private GameObject[] _objectsToStrip = null;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Populates the NetworkComponentSelector with
        ///            components of given type from the given 
        ///            objects.                              </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [ContextMenu( "Repopulate List" )]
        public void UpdateComponentSelector()
        {
            var componentSelector = GetComponent<NetworkComponentSelector>();

            componentSelector.ClearClientOnlyComponents();
            componentSelector.ClearServerOnlyComponents();
            
            System.Type[] clientOnlyTypes = GetClientOnlyTypes();
            foreach( System.Type clientOnlyType in clientOnlyTypes )
            {
                foreach( GameObject objectToStrip in _objectsToStrip )
                {
                    Component[] components =
                    objectToStrip.GetComponentsInChildren<Component>();

                    foreach( Component component in components )
                    {
                        if( clientOnlyType.IsInstanceOfType( component ) )
                        {
                            componentSelector.AddClientOnlyComponent( component );
                        }
                    }
                }
            }

            // TODO: This can be done in the loop above
            System.Type[] serverOnlyTypes = GetServerOnlyTypes();
            foreach( System.Type serverOnlyType in serverOnlyTypes )
            {
                foreach( GameObject objectToStrip in _objectsToStrip )
                {
                    Component[] components =
                    objectToStrip.GetComponentsInChildren<Component>();

                    foreach( Component component in components )
                    {
                        if( serverOnlyType.IsInstanceOfType( component ) )
                        {
                            componentSelector.AddServerOnlyComponent( component );
                        }
                    }
                }
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Creates and returns a list of component types
        ///            to be removed from servers.           </summary>        
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private static System.Type[] GetClientOnlyTypes()
        {
            return
                new System.Type[]
                {
                    typeof( ITargetOfInterest ),
                    typeof( MeshFilter ),
                    typeof( Renderer ),
                    typeof( Camera ),
                    typeof( Light ),
                };
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Creates and returns a list of component types
        ///            to be removed from clients.           </summary>     
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private static System.Type[] GetServerOnlyTypes()
        {
            return
                new System.Type[]
                {
                    typeof( Rigidbody ),
                    typeof( Collider ),
                };
        }
#endif
    }
}