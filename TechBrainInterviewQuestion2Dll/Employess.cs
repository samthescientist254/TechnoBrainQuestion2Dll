using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Domain
{
    public class Employess
    {
        List<Employee> _employees = new List<Employee>();
        private string _cSVPath;
        private readonly IReader _iReader;
        public Employess(string cSvPath, IReader iReader)
        {
            _cSVPath = string.IsNullOrWhiteSpace(cSvPath) ? throw new ArgumentNullException(nameof(cSvPath)) : cSvPath;
            _iReader = iReader;
        }

        private Employess() { }

        public List<Employee> GetEmployeeRecords()
        {
            var employees = _iReader.GetEmployees(_cSVPath);
            if (employees != null)
            {
            
                    _employees = employees;
                    return employees;
               

            }
            return null;
        }


    }
}
