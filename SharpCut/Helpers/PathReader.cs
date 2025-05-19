namespace SharpCut.Helpers
{
    /// <summary>
    /// A reader for reading information from a path string from a SVG document.
    /// </summary>
    public class PathReader : StringReader
    {
        int[] buffer;

        /// <summary>
        /// Initializes a new instance of <see cref="PathReader"/>.
        /// </summary>
        /// <param name="path">The path string to read from.</param>
        /// <param name="bufferLength">The size to use for the buffer that individual values are read to before being parsed.</param>
        public PathReader(string path, int bufferLength = 32) : base(path)
        {
            buffer = new int[bufferLength];
        }

        /// <summary>
        /// Will read the next float in the path or return null if none exists.
        /// </summary>
        public float? ReadFloat()
        {
            int index = 0;
            int decimalPointIndex = 0;
            bool encounteredDecimalPoint = false;

            while (true)
            {
                int next = Peek();

                if (next == -1 || (next == 32 && index > 0)) // if next doesn't exist or if next is a space and we've read something
                    break;

                if (next == 46)
                {
                    decimalPointIndex = index;
                    encounteredDecimalPoint = true;
                    Read();
                    continue;
                }

                if (next < 48 || next > 57)
                    throw new InvalidDataException($"Invalid character encountered when reading float: '{(char)next}' (int value is {next}).");

                buffer[index] = next;
                index++;
                Read();
            }

            if (index == 0)
                return null;

            if (!encounteredDecimalPoint)
                decimalPointIndex = index;

            float result = 0;

            for (int i = 0; i < index; i++)
            {
                int integer = buffer[i] - 48;
                int position = decimalPointIndex - i - 1;
                int positionalMultiplier = GetPositionalMultiplier(position);

                if (position >= 0)
                    result += integer * positionalMultiplier;
                else
                    result += integer / (float)positionalMultiplier;
            }

            return result;
        }

        private int GetPositionalMultiplier(int position)
        {
            int result = 1;

            if (position == 0)
                return result;

            if (position < 0)
                position = position * -1;

            for (int i = 0; i < position; i++)
                result *= 10;

            return result;
        }
    }
}
