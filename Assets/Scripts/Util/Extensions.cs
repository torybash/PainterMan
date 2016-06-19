using UnityEngine;
using System;
using System.Collections;

public static class Extensions {

	public static Enum SetToValueByNumber(this Enum en, int number) {
		Type enumType = en.GetType(); // Enum.GetUnderlyingType(en.GetType());
		if (Enum.GetValues(enumType).Length >= number) {
			Array arr = Enum.GetValues(enumType);
			object val = arr.GetValue(number);
			return (Enum)val;
		}
		Debug.LogError("Invalid number " + number + " for enum " + en);
		return en;
	}
}


