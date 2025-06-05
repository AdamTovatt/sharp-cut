using System.Globalization;

namespace SharpCut.Models
{
    internal readonly struct Scalar
    {
        internal readonly string Unit { get; init; }
        internal readonly float Value { get; init; }

        internal Scalar(float value, string unit)
        {
            Unit = unit;
            Value = value;
        }

        /// <summary>
        /// Will create a <see cref="Scalar"/> with the correct value and unit from a string that contains a value and unit.
        /// The format of the string should be for example 12.46mm where 12.46 is the value and mm is the unit.
        /// </summary>
        internal static Scalar FromString(string stringWithUnit)
        {
            int unitStartIndex = stringWithUnit.Length;

            for (int i = unitStartIndex - 1; i >= 0; i--)
            {
                if (!char.IsNumber(stringWithUnit[i]))
                    continue;

                unitStartIndex = i + 1;
                break;
            }

            float value = float.Parse(stringWithUnit.Substring(0, unitStartIndex), CultureInfo.InvariantCulture);
            string unit = stringWithUnit.Substring(unitStartIndex);

            return new Scalar(value, unit);
        }

        /// <summary>
        /// Will return the <see cref="Scalar"/> as a value followed by a unit, like this: 12.46mm
        /// </summary>
        public override string ToString()
        {
            return $"{GetValueAsString()}{Unit}";
        }

        /// <summary>
        /// Will return the <see cref="Scalar"/> as a value followed by a unit, like this: 12.462mm
        /// </summary>
        public string ToString(string? format = "0.00")
        {
            return $"{GetValueAsString()}{Unit}";
        }

        /// <summary>
        /// Will get only the value component as a string.
        /// </summary>
        internal string GetValueAsString(string? format = "0.00")
        {
            return Value.ToString("0.00", CultureInfo.InvariantCulture);
        }
    }
}
