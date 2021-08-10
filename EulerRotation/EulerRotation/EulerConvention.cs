using System;
using System.Collections.Generic;


namespace Euler
{

  
  
    /// <summary>
    /// class to specify a set of Euler Angles and their associated convention
    /// </summary>
    public class EulerAngles
    {
        /// <summary>
        /// Initializes a set of Euler angles and their associated convention. 
        /// </summary>
        /// <param name="c">Euler angle convention e.g. XYZ, XY'Z' etc.</param>
        /// <param name="a">alpha: first angle of roation in radians</param>
        /// <param name="b">beta: second angle of roation in radians</param>
        /// <param name="g">gamma: third angle of roation in radians</param>
        public EulerAngles(string c, double a, double b, double g)
        {
            convention = new EulerConvention(c);
            alpha = a;
            beta = b;
            gamma = g;
        }
        public EulerConvention convention;
        public double alpha;
        public double beta;
        public double gamma;

        







    }

    public class EulerConvention
    {
        public EulerConvention(string c)
        {
            if (ConventionValid(c))
            {
                axis = new List<string>();
                ParseAndAssignValues(c);
            }
            else
                throw new ArgumentOutOfRangeException("convention", "Convention is not in a valid format\n\te.g. XYZ, X'Y'Z', YXZ', etc.");
        }

        public string sequence;
        public List<string> axis;

        /// <summary>
        /// Checks if a given axis of rotation should be considered mobile in the convention. 
        /// </summary>
        /// <param name="i">must be between 0 and 2 corresponding to each of the three euler angles</param>
        /// <returns>returns true if axis i is mobile</returns>
        public bool isMobile(int i)
        {
            if (axis[i].Contains("\'"))
                return true;
            else
                return false;
        }



        private void ParseAndAssignValues(string c)
        {
            sequence = c;

            // Parse Convention
            for (int i = 0; i < c.Length; i++)
            {
                if (i + 1 < c.Length)
                {

                    if (c[i + 1] == '\'')
                    {
                        axis.Add(c.Substring(i, 2));
                        i++;
                    }
                    else
                    {
                        axis.Add(c.Substring(i, 1));
                    }

                }
                else
                {
                    axis.Add(c.Substring(i, 1));
                }

            }

        }

        public bool ConventionValid(string c)
        {
            if (c.Length < 3 || c.Length > 6)
                return false;

            if (c[0] == '\'')
                return false;

            foreach (char character in c)
                if (character != 'X' && character != 'Y' && character != 'Z' && character != '\'')
                    return false;


            if (c.Contains("\'"))
                if (c.Replace("\'", "").Length != 3)
                    return false;

            if (c.Contains("\'\'"))
                return false;

            return true;

        }

    }
}
