﻿#region [Copyright (c) 2015 Cristian Alexandru Geambasu]
//	Distributed under the terms of an MIT-style license:
//
//	The MIT License
//
//	Copyright (c) 2015 Cristian Alexandru Geambasu
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
//	and associated documentation files (the "Software"), to deal in the Software without restriction, 
//	including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//	and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all copies or substantial 
//	portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TeamUtility.IO
{
	[Serializable]
	public sealed class InputConfiguration
	{
		/// <summary>
		/// Do not change the name of an input configuration at runtime because it will invalidate the lookup tables.
		/// </summary>
		public string name;

        /// <summary>
		/// All configured axes. Call <c>UpdateAxes()</c> if you change this list.
		/// </summary>
		public List<AxisConfiguration> axes;


		public bool isExpanded;

        /// <summary>
        /// Maps axis names to axes.
        /// </summary>
        internal Dictionary<string, AxisConfiguration> _axisTable
        {
            get; private set;
        }

        public InputConfiguration() :
			this("New Configuration") { }
		
		public InputConfiguration(string name)
		{
			axes = new List<AxisConfiguration>();
			this.name = name;
			isExpanded = false;
            _axisTable = new Dictionary<string, AxisConfiguration>();
        }
		
		public static InputConfiguration Duplicate(InputConfiguration source)
		{
			InputConfiguration inputConfig = new InputConfiguration();
			inputConfig.name = source.name;
			
			inputConfig.axes = new List<AxisConfiguration>(source.axes.Count);
			for(int i = 0; i < source.axes.Count; i++)
			{
				inputConfig.axes.Add(AxisConfiguration.Duplicate(source.axes[i]));
			}

            inputConfig.UpdateAxes();

            return inputConfig;
		}

        /// <summary>
		/// Updates internal axes data.
        /// Call when you add new <c>AxisConfiguration</c> to <c>axes</c> or
        /// change their names.
		/// </summary>
        public void UpdateAxes()
        {
            _axisTable.Clear();

            foreach( AxisConfiguration axisConfig in axes )
            {
                if( !_axisTable.ContainsKey( axisConfig.name ) )
                {
                    _axisTable.Add( axisConfig.name, axisConfig );
                }
                else
                {
                    Debug.LogWarning( string.Format( "Input configuration \'{0}\' already contains an axis named \'{1}\'", name, axisConfig.name ) );
                }
            }
        }
    }
}