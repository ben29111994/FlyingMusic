using UnityEditor;

namespace SonicBloom.Koreo
{
	[CustomPropertyDrawer( typeof( SimpleInput.AxisInput ) )]
	public class AxisInputDrawer : BaseInputDrawer
	{
		public override string ValueToString( SerializedProperty valueProperty )
		{
			return valueProperty.floatValue.ToString();
		}
	}
}