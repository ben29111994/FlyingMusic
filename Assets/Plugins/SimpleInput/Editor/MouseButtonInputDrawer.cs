﻿using UnityEditor;

namespace SonicBloom.Koreo
{
	[CustomPropertyDrawer( typeof( SimpleInput.MouseButtonInput ) )]
	public class MouseButtonInputDrawer : BaseInputDrawer
	{
		public override string ValueToString( SerializedProperty valueProperty )
		{
			return valueProperty.boolValue.ToString();
		}
	}
}