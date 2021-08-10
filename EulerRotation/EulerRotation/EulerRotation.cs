using System;
using System.Collections.Generic;
using System.Windows.Media;
using MatrixOperations;

namespace Euler
{
    /// <summary>
    /// Rotates a point given a certain convention
    /// </summary>

    public static partial class Rotation
    {
        /// <summary>
        /// Rotates a point (ep) around the origin, given a set of angles and a specified convention both of which are defined by the EulerAngle object (angle) 
        /// </summary>
        /// <param name="ep">EulerPoint object which contains positional vector in the original reference frame</param>
        /// <param name="angle">EulerAngles object which defines angles and convention of rotation </param>
        /// <returns></returns>
        public static EulerPoint RotatePoint(EulerPoint ep, EulerAngles angle)
        {

            // -- Compute Rotation Matrix -- //

            double[,] R;

            // Get basic rotation matrices
            double[,] R1 = BasicRotationMatrix(angle.convention.axis[0], angle.alpha);
            double[,] R2 = BasicRotationMatrix(angle.convention.axis[1], angle.beta);
            double[,] R3 = BasicRotationMatrix(angle.convention.axis[2], angle.gamma);

            // Calculate product of R1 and R2
            double[,] R1R2;
            if (angle.convention.isMobile(1))
                R1R2 = MatrixMultiplication.MatrixMultiply(R1, R2);
            else
                R1R2 = MatrixMultiplication.MatrixMultiply(R2, R1);

            // Calculate product of R1R2 and R3
            if (angle.convention.isMobile(2))
                R = MatrixMultiplication.MatrixMultiply(R1R2, R3);
            else
                R = MatrixMultiplication.MatrixMultiply(R3, R1R2);


            //Rotate and return ep
            var ep_prime = MatrixMultiplication.MatrixMultiply(R, ep.col_vector);
            return new EulerPoint(ep_prime[0,0], ep_prime[1,0], ep_prime[2,0]);
            
        }

        public static double[,] BasicRotationMatrix(string axis, double theta)
        {
            double[,] BMatrix = new double[3, 3];

            if (axis.Contains("X"))
                BMatrix = BasicXRotationMatrix(theta);
            if (axis.Contains("Y"))
                BMatrix = BasicYRotationMatrix(theta);
            if (axis.Contains("Z"))
                BMatrix = BasicZRotationMatrix(theta);

            return BMatrix;
        }



    }


    public class EulerPoint
    {

        public EulerPoint(double xd, double yd, double zd)
        {
            x = xd;
            y = yd;
            z = zd;
            col_vector = new double[3, 1];
            row_vector = new double[1, 3];
            col_vector[0, 0] = xd;
            col_vector[1, 0] = yd;
            col_vector[2, 0] = zd;
            row_vector[0, 0] = xd;
            row_vector[0, 1] = yd;
            row_vector[0, 2] = zd;

            //Notes: column and row vectors are declared as two dimensional arrays though they only contain one dimenson of data this is required by Matrix Multiplication fuction
        }
        public double x;
        public double y;
        public double z;
        public double[,] col_vector;
        public double[,] row_vector;
    }







}
