using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPLGSS.Utilities;
using YamlDotNet.Serialization;

namespace WPLGSS.Models
{
    public class InputChannel : Channel
    {
        private static readonly MathEquationParser equationParser = new MathEquationParser();

        private string scaling;
        public double value;

        public string Scaling
        {
            get { return scaling; }
            set
            {
                SetProperty(ref scaling, value);
                ScalingFunction = equationParser.ParseFunction(value);
            }
        }

        [YamlIgnore]
        public Func<double, double> ScalingFunction { get; private set; }

        private string unit;

        public string Unit
        {
            get { return unit; }
            set
            {
                SetProperty(ref unit, value);
            }
        }

    }
}
