namespace LinearEquationSolver
{
    using System.Collections.Generic;

    public class LinearSystem
    {
        private List<LinearEquation> equations;

        public LinearSystem(params LinearEquation[] eqs)
        {
            this.equations = new List<LinearEquation>(eqs);
        }

        // TODO: implement
    }
}
