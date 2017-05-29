using System;

namespace CSMA_1_persistent
{
    public class Generators
    {
        public class UniformGenerator
        {
            private int kernel;
            private double M;
            private int A;
            private int Q;
            private int R;

            public UniformGenerator(int kern)
            {
                kernel = kern;
                M = 2147483647.0;
                A = 16807;
                Q = 127773;
                R = 2836;
            }

            
            /// <summary>
            /// Draws number between [0,1]
            /// </summary>
            /// <returns>new random number</returns>
            public double Rand()
            {
                int h = kernel / Q;
                kernel = A * (kernel - Q * h) - R * h;
                if (kernel < 0)
                    kernel = kernel + (int)M;
                return kernel / M;
            }
            /// <summary>
            /// Draws number between [start, end]
            /// </summary>
            public double Rand(int start, int end)
            {
                return Rand() * (end - start) + start;
            }

            public int get_kernel() { return kernel; }
            
        }


        public class ExpGenerator
        {
            private double lambda;
            private UniformGenerator uniform;
            public ExpGenerator(double L, UniformGenerator ug)
            {
                lambda = L;
                uniform = ug;
            }
            public double Rand() {
                double k = uniform.Rand();
                return -(1.0 / lambda) * Math.Log(k);
            }
        }
    }
}
