using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Employee.Domain
{
    public class CsvReader : IReader
    {
        public List<Employee> GetEmployees(string csvFPath)
        {
            List<Employee> employees = new List<Employee>();
            List<string> Lines = File.ReadAllLines(csvFPath).ToList();
            foreach (var data in Lines.Skip(1))
            {
                try
                {
                    string[] split = data.Split(",".ToCharArray());
                    Employee employee = new Employee();
                    bool isNumeric = int.TryParse(split[2], out int n);
                    if (split.Length == 3 && isNumeric)
                    {
                        employee = Employee.Create(split[0], split[1], n);
                        employees.Add(employee);
                    }
                }
                catch (Exception ex)
                {
                }



            }
            return employees;

        }
    }
}
