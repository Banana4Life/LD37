using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class Utils {

	public static string ConvertToRoman(this int value)
	{
		// Validate the range.
		if (value < 1 || value > 3999999)
			return string.Empty;

		// Create a StringBuilder of a suitable size. The longest Roman
		// numeral supported is 27 characters long (3,888,888).
		StringBuilder sb = new StringBuilder(27);

		while (value - 1000000 >= 0) { sb.Append("m");  value -= 1000000; }
		while (value -  900000 >= 0) { sb.Append("cm"); value -=  900000; }
		while (value -  500000 >= 0) { sb.Append("d");  value -=  500000; }
		while (value -  400000 >= 0) { sb.Append("cd"); value -=  400000; }
		while (value -  100000 >= 0) { sb.Append("c");  value -=  100000; }
		while (value -   90000 >= 0) { sb.Append("xc"); value -=   90000; }
		while (value -   50000 >= 0) { sb.Append("l");  value -=   50000; }
		while (value -   40000 >= 0) { sb.Append("xl"); value -=   40000; }
		while (value -   10000 >= 0) { sb.Append("x");  value -=   10000; }
		while (value -    9000 >= 0) { sb.Append("Mx"); value -=    9000; }
		while (value -    5000 >= 0) { sb.Append("v");  value -=    5000; }
		while (value -    4000 >= 0) { sb.Append("Mv"); value -=    4000; }
		while (value -    1000 >= 0) { sb.Append("M");  value -=    1000; }
		while (value -     900 >= 0) { sb.Append("CM"); value -=     900; }
		while (value -     500 >= 0) { sb.Append("D");  value -=     500; }
		while (value -     400 >= 0) { sb.Append("CD"); value -=     400; }
		while (value -     100 >= 0) { sb.Append("C");  value -=     100; }
		while (value -      90 >= 0) { sb.Append("XC"); value -=      90; }
		while (value -      50 >= 0) { sb.Append("L");  value -=      50; }
		while (value -      40 >= 0) { sb.Append("XL"); value -=      40; }
		while (value -      10 >= 0) { sb.Append("X");  value -=      10; }
		while (value -       9 >= 0) { sb.Append("IX"); value -=       9; }
		while (value -       5 >= 0) { sb.Append("V");  value -=       5; }
		while (value -       4 >= 0) { sb.Append("IV"); value -=       4; }
		while (value -       1 >= 0) { sb.Append("I");  value -=       1; }
		return sb.ToString();
	}
}
