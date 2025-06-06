﻿using SharpCut.Models;

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
        public float? ReadFloat(int allowedSpaceSkips = 0)
        {
            int performedSpaceSkips = 0;
            int index = 0;
            int decimalPointIndex = 0;
            bool encounteredDecimalPoint = false;

            while (true)
            {
                int next = Peek();

                if (next == -1 || // if next doesn't exist or 
                    (index > 0 && (next == ' ' || next == ',' || next == 'L' || next == 'Z' || next == 'C'))) // if we've read something and
                    break;                                                                                  // next is a space, a comma, an L, a Z or a C

                if (next == ' ' && performedSpaceSkips < allowedSpaceSkips)
                {
                    performedSpaceSkips++;
                    Read();
                    continue;
                }

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

        /// <summary>
        /// Will read the next point available in the stream. Throws an exception if none exists.
        /// </summary>
        /// <returns>The next point in the stream.</returns>
        public Point ReadPoint()
        {
            float? x = ReadFloat(2);

            if (x == null)
                throw new InvalidDataException($"Missing float value for X-coordinate of point when reading path.");

            int next = Peek();
            if (!char.IsNumber((char)next))
                Read(); // advance one character if the next character is not a number meaning it's some sort of separating character

            float? y = ReadFloat(2);

            if (y == null)
                throw new InvalidDataException($"X-coordinate ({x}) was found with missing Y-coordinate");

            return new Point(x.Value, y.Value);
        }

        /// <summary>
        /// Will read to advance the reader count amount of times. Doesn't return or save the value that is read.
        /// </summary>
        /// <param name="count">The amount of times to advance the reader one step forward.</param>
        public void Read(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Read();
            }
        }

        /// <summary>
        /// Will read the start of the path to move the current reader position to the right place when reading a new path.
        /// </summary>
        public void ReadStartOfPath()
        {
            int firstCharacter = Peek();

            if (char.IsNumber((char)firstCharacter))
                return;

            Read();

            int secondCharacter = Peek(); // check the second charcter

            if (char.IsNumber((char)secondCharacter))
                return; // return if we encountered a number here already

            Read(); // otherwise we read once again to move the reader one step forward
        }

        /// <summary>
        /// Will read a list of points from a path.
        /// </summary>
        /// <param name="didReadCloseCharacter">Wether or not a close character was read from the path.</param>
        /// <returns>A list of points from the path that was read.</returns>
        /// <exception cref="InvalidDataException">If an unexpected character is found in the path an exception is thrown.</exception>
        public List<Point> ReadPointListFromPath(out bool didReadCloseCharacter)
        {
            List<Point> points = new List<Point>();
            didReadCloseCharacter = false;

            const int maxSpaceCount = 5;
            int spaceCount = 0;

            while (true)
            {
                int next = Peek();

                if (next == 'C')
                {
                    Read(); // consume 'C'
                    if (Peek() == ' ') Read();

                    // Discard first 2 control points
                    ReadPoint();
                    ReadPoint();

                    // Read and keep the actual end point of the curve
                    Point point = ReadPoint();
                    points.Add(point);
                    spaceCount = 0;
                }
                else if (next == 'L' || next == 'M')
                {
                    Read(); // consume 'L' or 'M'
                    if (Peek() == ' ') Read();

                    Point point = ReadPoint();
                    points.Add(point);
                    spaceCount = 0;
                }
                else if (next == 'Z')
                {
                    Read(); // consume 'Z'
                    didReadCloseCharacter = true;
                    break;
                }
                else if (char.IsDigit((char)next) || next == '-' || next == '+')
                {
                    // Implicit point (continuation of previous command)
                    Point point = ReadPoint();
                    points.Add(point);
                    spaceCount = 0;
                }
                else if (next == -1)
                {
                    break;
                }
                else
                {
                    if(spaceCount < maxSpaceCount && next == ' ')
                    {
                        spaceCount++;
                        Read();
                        continue;
                    }
                    throw new InvalidDataException($"Unexpected character '{(char)next}' in path.");
                }
            }

            return points;
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
