using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace WPLGSS.Utilities
{
    /// <summary>
    /// A text reader that can read primative types in string format from a stream and output them.
    /// </summary>
    public sealed class TypeTextReader : IDisposable
    {
        private readonly TextReader underlyingReader;

        private bool disposed;

        /// <summary>
        /// Initializes a new <see cref="JKor.IO.TypeTextReader" /> object using the given <see cref="System.IO.TextReader"/> as the base reader.
        /// </summary>
        /// <param name="reader">The base reader.</param>
        public TypeTextReader(TextReader reader)
        {
            underlyingReader = reader;
        }

        /// <summary>
        /// Initalizes a new <see cref="JKor.IO.TypeTextReader" /> object to read from the given stream.
        /// </summary>
        /// <param name="stream">The base stream.</param>
        public TypeTextReader(Stream stream)
            :this(new StreamReader(stream))
        {

        }

        /// <summary>
        /// Initalizes a new <see cref="JKor.IO.TypeTextReader"/> object to read from the given string.
        /// </summary>
        /// <param name="s">The base string.</param>
        public TypeTextReader(string s)
            :this(new StringReader(s))
        {

        }

        /// <summary>
        /// Releases all resources used by the <see cref="JKor.IO.TypeTextReader"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleans up the TypeTextReader object.
        /// </summary>
        ~TypeTextReader()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    BaseReader.Dispose(); 
                }
            }
            disposed = true;
        }

        /// <summary>
        /// The underlying reader.
        /// </summary>
        /// <value>The <see cref="System.IO.TextReader"/> object that this <see cref="JKor.IO.TypeTextReader"/> object reads from.</value>
        public TextReader BaseReader
        {
            get { return underlyingReader; }
        }
        
        /// <summary>
        /// Reads the next integer from the stream as an Int64 to maximize data storage space.  This value can be safely cast to any other integral type as long as the value is in range.
        /// </summary>
        /// <returns>The next integer in the stream.</returns>
        public long ReadInt64()
        {
            long retVal = 0;
            int currentChar;
            var isNegative = (char)BaseReader.Peek() == '-';
            if (isNegative)
            {
                BaseReader.Read();
            }
            while ((currentChar = BaseReader.Peek()) != -1)//Keep the last character in the reader if it is an invalid character
            {
                var val = (char)currentChar;
                if (Char.IsDigit(val))
                {
                    if (retVal == 0)
                    {
                        retVal = Int64.Parse(new string(new[] { val }), CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        retVal = retVal * 10 + Int64.Parse(new string(new[] { val }), CultureInfo.CurrentCulture);
                    }
                }
                else
                {
                    break;
                }
                BaseReader.Read();//Remove the character from the stream
            }
            return retVal * (isNegative ? -1 : 1);
        }

        /// <summary>
        /// Reads the next decimal from the stream.
        /// </summary>
        /// <returns>The next decimal in the stream.</returns>
        public decimal ReadDecimal()
        {
            decimal retVal = 0;
            var decimalPlaces = 0;
            var increaseDecPlaces = false;
            int currentChar;
            var isNegative = (char)BaseReader.Peek() == '-';
            if (isNegative)
            {
                BaseReader.Read();
            }
            while ((currentChar = BaseReader.Peek()) != -1)
            {
                var val = (char)currentChar;
                if (Char.IsDigit(val))
                {
                    if (retVal == 0)
                    {
                        retVal = Decimal.Parse(new string(new[] { val }), CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        retVal = retVal * 10 + Decimal.Parse(new string(new[] { val }), CultureInfo.CurrentCulture);
                    }
                    BaseReader.Read();
                }
                else if (val == '.')
                {
                    increaseDecPlaces = true;
                    BaseReader.Read();
                    continue;
                }
                else
                {
                    break;
                }
                if (increaseDecPlaces)
                {
                    ++decimalPlaces;
                }
            }
            if (decimalPlaces == 0)
            {
                return retVal * (isNegative ? -1 : 1);
            }
            else
            {
                return retVal * (isNegative ? -1 : 1) / Decimal.Round((decimal)System.Math.Pow(10, decimalPlaces), decimalPlaces);
            }
        }

        /// <summary>
        /// Reads the next double from the stream.
        /// </summary>
        /// <returns>The next double in the stream.</returns>
        public double ReadDouble()
        {
            return (double)ReadDecimal();
        }

        /// <summary>
        /// Reads until one of the delimeters is found.
        /// </summary>
        /// <param name="delimeters">Any possible delimeters to stop reading at.</param>
        /// <returns>Returns the string read up to the delimeter.</returns>
        /// <remarks>Does not read the delimiter as well.</remarks>
        public string ReadToDelimiter(params char[] delimeters)
        {
            var builder = new StringBuilder();
            while (BaseReader.Peek() != -1 && !delimeters.Contains((char)BaseReader.Peek()))
            {
                builder.Append((char)BaseReader.Read());
            }
            return builder.ToString();
        }
    }
}
