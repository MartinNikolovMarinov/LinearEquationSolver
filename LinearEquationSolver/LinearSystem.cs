using System;
using System.Collections.Generic;

namespace LinearEquationSolver
{
    public class LinearSystem
    {
        private List<LinearEquation> equations;

        public LinearSystem(params LinearEquation[] eqs)
        {
            this.equations = new List<LinearEquation>(eqs);
        }

        public void AddEquation(LinearEquation eq)
        {
            this.equations.Add(eq);
        }

        public void SolveNextStep()
        {
            // ...
            this.equations.Sort();
            Console.WriteLine(string.Join("\n", this.equations));
        }
    }
}