using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Euler
{
    /// <summary>
    /// Rotates a point given a certain convention
    /// </summary>

    public static partial class Rotation
    {


        /// <summary>
        /// returns the basic rotation matrix for rotating around the X axis (counterclockwise if axis is pointing toward observer i.e. right-handed)
        /// </summary>
        /// <param name="alpha">angle of rotation in radians</param>
        /// <returns></returns>
        public static double[,] BasicXRotationMatrix(double alpha)
        {
            double[,] XMatrix = new double[3, 3];

            // row 1
            XMatrix[0, 0] = 1.0;
            XMatrix[0, 1] = 0.0;
            XMatrix[0, 2] = 0.0;

            // row 2
            XMatrix[1, 0] = 0.0;
            XMatrix[1, 1] = Math.Cos(alpha);
            XMatrix[1, 2] = Math.Sin(alpha) * -1.0;

            // row 3
            XMatrix[2, 0] = 0.0;
            XMatrix[2, 1] = Math.Sin(alpha);
            XMatrix[2, 2] = Math.Cos(alpha);


            return XMatrix;
        }

        /// <summary>
        /// returns the basic rotation matrix for rotating around the Y axis (counterclockwise if axis is pointing toward observer i.e. right-handed)
        /// </summary>
        /// <param name="beta">angle of rotation in radians</param>
        /// <returns></returns>
        public static double[,] BasicYRotationMatrix(double beta)
        {
            double[,] YMatrix = new double[3, 3];

            // row 1
            YMatrix[0, 0] = Math.Cos(beta);
            YMatrix[0, 1] = 0.0;
            YMatrix[0, 2] = Math.Sin(beta);

            // row 2
            YMatrix[1, 0] = 0.0;
            YMatrix[1, 1] = 1.0;
            YMatrix[1, 2] = 0.0;

            // row 3
            YMatrix[2, 0] = Math.Sin(beta) * -1.0;
            YMatrix[2, 1] = 0.0;
            YMatrix[2, 2] = Math.Cos(beta);


            return YMatrix;
        }

        /// <summary>
        /// returns the basic rotation matrix for rotating around the Z axis (counterclockwise if axis is pointing toward observer i.e. right-handed)
        /// </summary>
        /// <param name="gamma">angle of rotation in radians</param>
        /// <returns></returns>
        public static double[,] BasicZRotationMatrix(double gamma)
        {
            double[,] ZMatrix = new double[3, 3];

            // row 1
            ZMatrix[0, 0] = Math.Cos(gamma);
            ZMatrix[0, 1] = Math.Sin(gamma) * -1.0;
            ZMatrix[0, 2] = 0.0;

            // row 2
            ZMatrix[1, 0] = Math.Sin(gamma);
            ZMatrix[1, 1] = Math.Cos(gamma);
            ZMatrix[1, 2] = 0.0;

            // row 3
            ZMatrix[2, 0] = 0.0;
            ZMatrix[2, 1] = 0.0;
            ZMatrix[2, 2] = 1.0;


            return ZMatrix;
        }

    }


}



