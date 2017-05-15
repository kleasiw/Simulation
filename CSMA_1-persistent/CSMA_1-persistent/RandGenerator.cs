using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMA_1_persistent
{
    class RandGenerator
    {
        
	    public  class UniformRandomGenerator
        {
            // Uchwyt do obiektu generatora równomiernego.
            private Generators.UniformGenerator uniform_generator_;

            public UniformRandomGenerator(int kernel)
            {
                uniform_generator_ = new Generators.UniformGenerator(kernel);
            }
            public double Rand()
            {
                return uniform_generator_.Rand();
            }
            public double Rand(int min, int max)
            {
                return uniform_generator_.Rand(min, max);
            }
            public int GetKernel()
            {
                return uniform_generator_.get_kernel();
            }
           
        }

        public  class ExponentialRandomGenerator
        {

            private Generators.ExpGenerator exp_generator_;

            public ExponentialRandomGenerator(double lambda, int kernel)
            {
                exp_generator_ = new Generators.ExpGenerator(lambda,new Generators.UniformGenerator(kernel));
            }

            public double Rand() {
                return exp_generator_.Rand();
            }
            
        }


    }
}

