using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Domain
{
    public interface IReader
    {
        List<Employee> GetEmployees(string csvFilePath);
    }
}
