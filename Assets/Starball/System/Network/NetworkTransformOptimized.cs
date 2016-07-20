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
namespace Izzo.Networking
{
    using System;
    using UnityEngine;
    using UnityEngine.Networking;

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary>  Synchronizes the game objects movement 
    ///            and rotation over the network.        </summary>
    /// <remarks>
    ///         The component can synchronise both phyisics and
    ///     normal transforms, however the physics simulation is
    ///     only run on the server. Clients receive precalculated
    ///     results from the server and interpolate them, without
    ///     doing any physics simulation on their own.
    ///                                                  </remarks>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    [DisallowMultipleComponent]
    [AddComponentMenu( "Network/NetworkTransform (Optimized)", 1 )]

    public class NetworkTransformOptimized : NetworkBehaviour
    {
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        /// <summary>Level of controlession over the network.</summary>
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        public enum CompressionLevel
        {
            None = 0,
            Low,
            High
        }

        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        /// <summary> Synchronization variable flags.        </summary>
        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        [Flags]
        public enum SyncMask
        {
            position_x = 1 << 0,
            position_y = 1 << 1,
            position_z = 1 << 2,
            velocity_x = 1 << 3,
            velocity_y = 1 << 4,
            velocity_z = 1 << 5,
            rotation_x = 1 << 6,
            rotation_y = 1 << 7,
            rotation_z = 1 << 8,
            angVelocity_x = 1 << 9,
            angVelocity_y = 1 << 10,
            angVelocity_z = 1 << 11
        }

        //-------------------------------------------------------------
        /// <summary>  </summary>
        //----------------------------------
        private const SyncMask syncAll = SyncMask.position_x |
                                         SyncMask.position_y |
                                         SyncMask.position_z |
                                         SyncMask.velocity_x |
                                         SyncMask.velocity_y |
                                         SyncMask.velocity_z |
                                         SyncMask.rotation_x |
                                         SyncMask.rotation_y |
                                         SyncMask.rotation_z |
                                         SyncMask.angVelocity_x |
                                         SyncMask.angVelocity_y |
                                         SyncMask.angVelocity_z;

        //-------------------------------------------------------------
        [SerializeField, Range(0, 30), Tooltip
        ( "The send rate in number of dispatches per second. "       )]
        //----------------------------------        
        float _sendRate = 8;

        //-------------------------------------------------------------
        [SerializeField, EnumFlags, Tooltip
        ( "What variables to synchronize."                           )]
        //----------------------------------
        SyncMask _synchronize = syncAll;


        [Header("Linear")]
        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "The minimal change in position to be reported."           )]
        //----------------------------------
        float _positionThreshold = 0.1f;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "The minimal change in velocity to be reported."           )]
        //----------------------------------
        float _velocityThreshold = 0.05f;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "Compression level of the position vector."                )]
        //----------------------------------
        CompressionLevel _positionCompression = CompressionLevel.Low;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "Compression level of the velocity vector."                )]
        //----------------------------------
        CompressionLevel _velocityCompression = CompressionLevel.Low;


        [Header("Angular")]
        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "The minimal change in rotation to be reported (degrees)." )]
        //----------------------------------
        float _rotationThreshold = 0.1f;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "The minimal change in angular velocity to be reported " +
          " (degrees per second)."                                   )]
        //----------------------------------
        float _angVelocityThreshold = 0.05f;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "Compression level of the rotation."                       )]
        //----------------------------------
        CompressionLevel _rotationCompression = CompressionLevel.Low;

        //-------------------------------------------------------------
        [SerializeField, Tooltip
        ( "Compression level of the angular velocity."               )]
        //----------------------------------
        CompressionLevel _angVelocityCompression = CompressionLevel.Low;


        //-------------------------------------------------------------
        /// <summary>  The cached position on the network. 
        ///            Represents what the client knows.     </summary>
        //----------------------------------
        private Vector3 netPosition;

        //-------------------------------------------------------------
        /// <summary>  The cached velocity on the network. 
        ///            Represents what the client knows.     </summary>
        //----------------------------------
        private Vector3 netVelocity;

        //-------------------------------------------------------------
        /// <summary>  The cached rotation on the network. 
        ///            Represents what the client knows.     </summary>
        //----------------------------------
        private Vector3 netRotation;

        //-------------------------------------------------------------
        /// <summary>  The cached angular velocity on the network.
        ///            Represents what the client knows.     </summary>
        //----------------------------------
        private Vector3 netAngVelocity;

        //-------------------------------------------------------------
        /// <summary>  Internal send timer.                  </summary>
        //----------------------------------
        private float sendTimer;

        //-------------------------------------------------------------
        /// <summary>  Reference to the RigidBody component. </summary>
        //----------------------------------
        private Rigidbody rigidBody;

        //-------------------------------------------------------------
        /// <summary>  Smooth damp velocity,
        ///            used for interpolation on the client. </summary>
        //----------------------------------
        private Vector3 dampVelocity;


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  UNITY CALLBACK                        </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  UNITY CALLBACK                        </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void Update()
        {
            netPosition += netVelocity * Time.deltaTime;
            netRotation += netAngVelocity * Time.deltaTime;
            netRotation.x = Mathf.Repeat( netRotation.x, 360 );
            netRotation.y = Mathf.Repeat( netRotation.y, 360 );
            netRotation.z = Mathf.Repeat( netRotation.z, 360 );

            if( isServer )
            {
                sendTimer -= Time.deltaTime;
                if( sendTimer <= 0 )
                {
                    SetDirtyBit( 1 );
                }
            }
            else if( isClient )
            {
                const float posInterpolationTime = 0.2f;
                const int rotInterpolationSpeed = 5;

                transform.position = Vector3.SmoothDamp( transform.position,
                                                         netPosition,
                                                         ref dampVelocity,
                                                         posInterpolationTime );
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.Euler( netRotation ),
                    Time.deltaTime * rotInterpolationSpeed );
            }
        }

        //=============================================================
        /// <summary>  Prepares and sends the state 
        ///            of this component over the network.   </summary>
        /// <param name="writer">
        ///     The UNET network writer.                       </param>
        /// <param name="initialState"> 
        ///     Whether this is the first dispatch
        ///     on the network.                                </param>
        /// <returns>  True if data was written.             </returns>
        //==================================        
        public override bool OnSerialize( NetworkWriter writer,
                                          bool initialState )
        {
            if( initialState || sendTimer <= 0 )
            {
                sendTimer = 1 / _sendRate;

                SyncMask dirtyBits = 0;
                if( initialState )
                {
                    dirtyBits = ResetNetValues();
                }
                else
                {
                    dirtyBits = UpdateNetValues();
                    writer.Write( (ushort) dirtyBits );
                }

                WriteNetValues( writer, dirtyBits );
                return true;
            }
            else
            {
                writer.Write( (ushort) 0 );
                return false;
            }
        }

        //=============================================================
        /// <summary>  Reads data from the network and synchronizes
        ///            the state of this component.          </summary>
        /// <param name="reader"> 
        ///     The UNET network reader.                       </param>
        /// <param name="initialState">
        ///     Whether this is the first dispatch
        ///     from the network.                              </param>
        //==================================
        public override void OnDeserialize( NetworkReader reader,
                                            bool initialState )
        {
            if( !isServer )
            {
                SyncMask dirtyBits;

                if( initialState )
                {
                    dirtyBits = syncAll;
                }
                else
                {
                    dirtyBits = (SyncMask) reader.ReadUInt16();
                }

                ReadNetValues( reader, dirtyBits );
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Forces an update on all cached network values 
        ///            and returns a bitmask with all values that 
        ///            actually changed (which should be all of them).
        ///                                                  </summary>
        /// <returns> A bitmask of all changed net values.   </returns>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private SyncMask ResetNetValues()
        {
            Vector3 position = transform.position;
            Vector3 velocity = rigidBody.velocity;
            Vector3 rotation = transform.eulerAngles;
            Vector3 angVelocity = rigidBody.angularVelocity;

            netPosition = position;
            netVelocity = velocity;
            netRotation = rotation;
            netAngVelocity = angVelocity;

            return syncAll;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Updates the local cache of network values on 
        ///            the server and returns a bitmask with all
        ///            values that actually changed.         </summary>
        /// <returns> A bitmask of all changed net values.   </returns>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private SyncMask UpdateNetValues()
        {
            SyncMask syncBitMask = _synchronize;
            Vector3 position = transform.position;
            Vector3 velocity = rigidBody.velocity;
            Vector3 rotation = transform.eulerAngles;
            Vector3 angVelocity = rigidBody.angularVelocity;

            // Position
            UpdateNetScalar( ref netPosition.x, position.x,
                                 _positionThreshold,
                             ref syncBitMask, SyncMask.position_x );

            UpdateNetScalar( ref netPosition.y, position.y,
                                 _positionThreshold,
                             ref syncBitMask, SyncMask.position_y );

            UpdateNetScalar( ref netPosition.z, position.z,
                                 _positionThreshold,
                             ref syncBitMask, SyncMask.position_z );

            // Velocity
            UpdateNetScalar( ref netVelocity.x, velocity.x,
                                 _velocityThreshold,
                             ref syncBitMask, SyncMask.velocity_x );

            UpdateNetScalar( ref netVelocity.y, velocity.y,
                                 _velocityThreshold,
                             ref syncBitMask, SyncMask.velocity_y );

            UpdateNetScalar( ref netVelocity.z, velocity.z,
                                 _velocityThreshold,
                             ref syncBitMask, SyncMask.velocity_z );

            // Rotation
            UpdateNetAngle( ref netRotation.x, rotation.x,
                                 _rotationThreshold,
                             ref syncBitMask, SyncMask.rotation_x );

            UpdateNetAngle( ref netRotation.y, rotation.y,
                                 _rotationThreshold,
                             ref syncBitMask, SyncMask.rotation_y );

            UpdateNetAngle( ref netRotation.z, rotation.z,
                                 _rotationThreshold,
                             ref syncBitMask, SyncMask.rotation_z );

            // Angular Velocity
            UpdateNetScalar( ref netAngVelocity.x, angVelocity.x,
                                 _angVelocityThreshold,
                             ref syncBitMask, SyncMask.angVelocity_x );

            UpdateNetScalar( ref netAngVelocity.y, angVelocity.y,
                                 _angVelocityThreshold,
                             ref syncBitMask, SyncMask.angVelocity_y );

            UpdateNetScalar( ref netAngVelocity.z, angVelocity.z,
                                 _angVelocityThreshold,
                             ref syncBitMask, SyncMask.angVelocity_z );

            return syncBitMask;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Writes data to the network writer.    </summary>
        /// <remarks>  
        ///     This method must be kept consistent with 
        ///     ReadNetValues() at all times.                </remarks>
        /// <param name="writer">
        ///     The UNET network writer.                       </param>
        /// <param name="dirtyBits">
        ///     Bitmask showing what values to send.           </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void WriteNetValues( NetworkWriter writer, SyncMask dirtyBits )
        {
            // Position
            if( (dirtyBits & SyncMask.position_x) != 0 )
            {
                WriteNetScalar( writer,
                                ref netPosition.x,
                                _positionCompression );
            }
            if( (dirtyBits & SyncMask.position_y) != 0 )
            {
                WriteNetScalar( writer,
                                ref netPosition.y,
                                _positionCompression );
            }
            if( (dirtyBits & SyncMask.position_z) != 0 )
            {
                WriteNetScalar( writer,
                                ref netPosition.z,
                                _positionCompression );
            }

            // Velocity
            if( (dirtyBits & SyncMask.velocity_x) != 0 )
            {
                WriteNetScalar( writer,
                                ref netVelocity.x,
                                _velocityCompression );
            }
            if( (dirtyBits & SyncMask.velocity_y) != 0 )
            {
                WriteNetScalar( writer,
                                ref netVelocity.y,
                                _velocityCompression );
            }
            if( (dirtyBits & SyncMask.velocity_z) != 0 )
            {
                WriteNetScalar( writer,
                                ref netVelocity.z,
                                _velocityCompression );
            }

            // Rotation
            if( (dirtyBits & SyncMask.rotation_x) != 0 )
            {
                WriteNetAngle( writer,
                               ref netRotation.x,
                               _rotationCompression );
            }
            if( (dirtyBits & SyncMask.rotation_y) != 0 )
            {
                WriteNetAngle( writer,
                               ref netRotation.y,
                               _rotationCompression );
            }
            if( (dirtyBits & SyncMask.rotation_z) != 0 )
            {
                WriteNetAngle( writer,
                               ref netRotation.z,
                               _rotationCompression );
            }

            // Angular velocity
            if( (dirtyBits & SyncMask.angVelocity_x) != 0 )
            {
                WriteNetScalar( writer,
                             ref netAngVelocity.x,
                             _angVelocityCompression );
            }
            if( (dirtyBits & SyncMask.angVelocity_y) != 0 )
            {
                WriteNetScalar( writer,
                             ref netAngVelocity.y,
                             _angVelocityCompression );
            }
            if( (dirtyBits & SyncMask.angVelocity_z) != 0 )
            {
                WriteNetScalar( writer,
                             ref netAngVelocity.z,
                             _angVelocityCompression );
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Reads data from the network reader.   </summary>
        /// <remarks>  
        ///     This method must be kept consistent with 
        ///     WriteNetValues() at all times.               </remarks>
        /// <param name="reader">
        ///     The UNET network reader.                       </param>
        /// <param name="dirtyBits">
        ///     Bitmask showing what values were sent.         </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void ReadNetValues( NetworkReader reader, SyncMask dirtyBits )
        {
            // Position
            if( (dirtyBits & SyncMask.position_x) != 0 )
            {
                ReadNetScalar( reader,
                            ref netPosition.x,
                            _positionCompression );
            }
            if( (dirtyBits & SyncMask.position_y) != 0 )
            {
                ReadNetScalar( reader,
                            ref netPosition.y,
                            _positionCompression );
            }
            if( (dirtyBits & SyncMask.position_z) != 0 )
            {
                ReadNetScalar( reader,
                            ref netPosition.z,
                            _positionCompression );
            }

            // Velocity
            if( (dirtyBits & SyncMask.velocity_x) != 0 )
            {
                ReadNetScalar( reader,
                            ref netVelocity.x,
                            _velocityCompression );
            }
            if( (dirtyBits & SyncMask.velocity_y) != 0 )
            {
                ReadNetScalar( reader,
                            ref netVelocity.y,
                            _velocityCompression );
            }
            if( (dirtyBits & SyncMask.velocity_z) != 0 )
            {
                ReadNetScalar( reader,
                            ref netVelocity.z,
                            _velocityCompression );
            }

            // Rotation
            if( (dirtyBits & SyncMask.rotation_x) != 0 )
            {
                ReadNetAngle( reader,
                           ref netRotation.x,
                           _rotationCompression );
            }
            if( (dirtyBits & SyncMask.rotation_y) != 0 )
            {
                ReadNetAngle( reader,
                           ref netRotation.y,
                           _rotationCompression );
            }
            if( (dirtyBits & SyncMask.rotation_z) != 0 )
            {
                ReadNetAngle( reader,
                           ref netRotation.z,
                           _rotationCompression );
            }

            // Angular velocity
            if( (dirtyBits & SyncMask.angVelocity_x) != 0 )
            {
                ReadNetScalar( reader,
                            ref netAngVelocity.x,
                            _angVelocityCompression );
            }
            if( (dirtyBits & SyncMask.angVelocity_y) != 0 )
            {
                ReadNetScalar( reader,
                            ref netAngVelocity.y,
                            _angVelocityCompression );
            }
            if( (dirtyBits & SyncMask.angVelocity_z) != 0 )
            {
                ReadNetScalar( reader,
                            ref netAngVelocity.z,
                            _angVelocityCompression );
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Updates a general network value from a real 
        ///           value or excludes it from dispatching. </summary>
        /// <param name="netValue"> 
        ///     The current network value to be updated.       </param>
        /// <param name="realValue">
        ///     The real value that netValue is mapped to.     </param>
        /// <param name="threshold">
        ///     The update threshold for difference between
        ///     netValue and realValue.                        </param>
        /// <param name="bitMask">
        ///     The bit mask to be updated based on whether 
        ///     netValue is updated or not.                    </param>
        /// <param name="currentBit">
        ///     The SyncMask bit representing the netValue.    </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void UpdateNetScalar( ref float netValue,
                                          float realValue,
                                          float threshold,
                                      ref SyncMask bitMask,
                                          SyncMask currentBit )
        {
            if( (bitMask & currentBit) != 0 )
            {
                if( Mathf.Abs( netValue - realValue ) > threshold )
                {
                    netValue = realValue;
                    bitMask |= currentBit;
                }
                else
                {
                    bitMask &= ~currentBit;
                }
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Compresses a general scalar value and
        ///            writes it on the network.             </summary>
        /// <param name="writer">
        ///     The UNET network writer.                       </param>
        /// <param name="value"> 
        ///     The networked value to be updated and sent.    </param>
        /// <param name="compression"> 
        ///     The compression level.                         </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void WriteNetScalar( NetworkWriter writer,
                                  ref float value,
                                  CompressionLevel compression )
        {
            switch( compression )
            {
                case CompressionLevel.None:
                    {
                        writer.Write( value );
                        break;
                    }
                case CompressionLevel.Low:
                    {
                        ushort halfValue = Mathf.FloatToHalf(value);
                        writer.Write( halfValue );
                        value = Mathf.HalfToFloat( halfValue );
                        break;
                    }
                case CompressionLevel.High:
                    {
                        sbyte byteValue = (sbyte) Mathf.Clamp( value*4,
                                                               sbyte.MinValue,
                                                               sbyte.MaxValue );
                        writer.Write( byteValue );
                        value = byteValue / 4.0f;
                        break;
                    }
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Reads a general scalar value from the
        ///            network and decompresses it.          </summary>
        /// <param name="reader">
        ///     The UNET network reader.                       </param>
        /// <param name="value"> 
        ///     The networked value to be read.                </param>
        /// <param name="compression"> 
        ///     The compression level.                         </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void ReadNetScalar( NetworkReader reader,
                                 ref float value,
                                 CompressionLevel compression )
        {
            switch( compression )
            {
                case CompressionLevel.None:
                    {
                        value = reader.ReadSingle();
                        break;
                    }
                case CompressionLevel.Low:
                    {
                        value = Mathf.HalfToFloat( reader.ReadUInt16() );
                        break;
                    }
                case CompressionLevel.High:
                    {
                        value = reader.ReadSByte() / 4.0f;
                        break;
                    }
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary> Updates an angle network value from a real 
        ///           value or excludes it from dispatching. </summary>
        /// <param name="netValue"> 
        ///     The current network value to be updated.       </param>
        /// <param name="realValue">
        ///     The real value that netValue is mapped to.     </param>
        /// <param name="threshold">
        ///     The update threshold for difference between
        ///     netValue and realValue.                        </param>
        /// <param name="bitMask">
        ///     The bit mask to be updated based on whether 
        ///     netValue is updated or not.                    </param>
        /// <param name="currentBit">
        ///     The SyncMask bit representing the netValue.    </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void UpdateNetAngle( ref float netValue,
                                         float realValue,
                                         float threshold,
                                     ref SyncMask bitMask,
                                         SyncMask currentBit )
        {
            if( (bitMask & currentBit) != 0 )
            {
                if( Mathf.Abs( Mathf.DeltaAngle( netValue, realValue ) ) > threshold )
                {
                    netValue = realValue;
                    bitMask |= currentBit;
                }
                else
                {
                    bitMask &= ~currentBit;
                }
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Compresses an angle value and
        ///            writes it on the network.             </summary>
        /// <remarks>
        ///     Angles are always positive and mapped in the range 
        ///     0-360. With low compression they are mapped to the
        ///     range of ushort and with high compression to the range
        ///     of a byte.                                   </remarks>
        /// <param name="writer">
        ///     The UNET network writer.                       </param>
        /// <param name="angle"> 
        ///     The networked value to be updated and sent.    </param>
        /// <param name="compression"> 
        ///     The compression level.                         </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void WriteNetAngle( NetworkWriter writer,
                                 ref float angle,
                                 CompressionLevel compression )
        {
            switch( compression )
            {
                case CompressionLevel.None:
                    {
                        writer.Write( angle );
                        break;
                    }
                case CompressionLevel.Low:
                    {
                        ushort shortAngle = (ushort) ((angle / 360) * ushort.MaxValue);
                        writer.Write( shortAngle );
                        angle = shortAngle * 360.0f / ushort.MaxValue;
                        break;
                    }
                case CompressionLevel.High:
                    {
                        byte byteAngle = (byte) ((angle / 360) * byte.MaxValue);
                        writer.Write( byteAngle );
                        angle = byteAngle * 360.0f / byte.MaxValue;
                        break;
                    }
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Reads an angle value from the network 
        ///            and decompresses it.                  </summary>
        /// <param name="reader">
        ///     The UNET network reader.                       </param>
        /// <param name="angle"> 
        ///     The networked value to be read.                </param>
        /// <param name="compression"> 
        ///     The compression level.                         </param>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private void ReadNetAngle( NetworkReader reader,
                                ref float angle,
                                CompressionLevel compression )
        {
            switch( compression )
            {
                case CompressionLevel.None:
                    {
                        angle = reader.ReadSingle();
                        break;
                    }
                case CompressionLevel.Low:
                    {
                        angle = reader.ReadUInt16() * 360.0f / ushort.MaxValue; ;
                        break;
                    }
                case CompressionLevel.High:
                    {
                        angle = reader.ReadByte() * 360.0f / byte.MaxValue;
                        break;
                    }
            }
        }
    }
}