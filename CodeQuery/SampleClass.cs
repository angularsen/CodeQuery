using System;

namespace CodeQuery
{
    public class SampleClass
    {
        // Regular one
        public static void NormalDateTime(DateTime dt) { }
        // Nullable parameter
        public static void NullableDateTime1(DateTime? dt) { }
        // Nullable parameter, the symbol type will still read "System.DateTime?"
        public static void NullableDateTime2(Nullable<DateTime> dt) { }
    }
}