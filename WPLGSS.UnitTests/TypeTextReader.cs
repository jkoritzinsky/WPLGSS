using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Xunit;

namespace WPLGSS.Utilities.UnitTests
{
    public class TypeTextReaderTest
    {
        [Fact]
        public void ReadInt()
        {
            var str = new StringReader("100s");
            using (var reader = new TypeTextReader(str))
            {
                var integer = reader.ReadInt64();
                Assert.Equal(100, integer);
                Assert.Equal('s', (char)reader.BaseReader.Peek());
            }
        }

        [Fact]
        public void ReadDecimalNoDecimalPlaces()
        {
            var str = new StringReader("100s");
            using (var reader = new TypeTextReader(str))
            {
                var testVal = reader.ReadDecimal();
                Assert.Equal(100M, testVal);
                Assert.Equal('s', (char)reader.BaseReader.Peek());
            }
        }

        [Fact]
        public void ReadDecimalWithPointNoPlaces()
        {
            var str = new StringReader("175.s");
            using (var reader = new TypeTextReader(str))
            {
                var testVal = reader.ReadDecimal();
                Assert.Equal(175M, testVal);
                Assert.Equal('s', (char)reader.BaseReader.Peek());
            }
        }

        [Fact]
        public void ReadDecimalWithDecimalPlaces()
        {
            var str = new StringReader("175.357s");
            using (var reader = new TypeTextReader(str))
            {
                var testVal = reader.ReadDecimal();
                Assert.Equal(175.357M, testVal);
                Assert.Equal('s', (char)reader.BaseReader.Peek());
            }
        }

        [Fact]
        public void ReadDecimalOnlyDecimalPlaces()
        {
            var str = new StringReader(".10057g");
            using (var reader = new TypeTextReader(str))
            {
                var testVal = reader.ReadDecimal();
                Assert.Equal(.10057M, testVal);
                Assert.Equal('g', (char)reader.BaseReader.Peek());
            }
        }

        [Fact]
        public void ReadNegativeInt()
        {
            var str = new StringReader("-20z");
            using (var reader = new TypeTextReader(str))
            {
                var integer = reader.ReadInt64();
                Assert.Equal(integer, -20L);
                Assert.Equal('z', (char)reader.BaseReader.Peek());
            }
        }

        [Fact]
        public void ReadNegativeDecimal()
        {
            var str = new StringReader("-20z");
            using (var reader = new TypeTextReader(str))
            {
                var value = reader.ReadDecimal();
                Assert.Equal(value, -20.0M);
                Assert.Equal('z', (char)reader.BaseReader.Peek());
            }
        }
    }
}
