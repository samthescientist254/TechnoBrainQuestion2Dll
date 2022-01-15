using System;
using System.Collections.Generic;
using Xunit;

namespace Employee.Domain.UnitTest
{
    public class EmployeesUnitTest
    {

        Employess employess;

        Response response = new Response();

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GetManagersBudget_WhenIdIsInvalid(string managerId)
        {
            var root = System.IO.Directory.GetCurrentDirectory();
            IReader reader = new CsvReader();
            employess = new Employess(root + @"\data.csv", reader);
            var emps = employess.GetEmployeeRecords();
            EmpServices services = new EmpServices(emps);
            Assert.Throws<ArgumentNullException>(nameof(managerId), () => services.GetManagersBudget(managerId));
        }

        [Theory]
        [InlineData("Employee2", 1800)]
        [InlineData("Employee3", 500)]
        [InlineData("Employee1", 3800)]
        public void GetManagersBudget(string managerId, long expected)
        {
            var root = System.IO.Directory.GetCurrentDirectory();
            IReader reader = new CsvReader();
            employess = new Employess(root + @"\data.csv", reader);
            var emps = employess.GetEmployeeRecords();
            EmpServices services = new EmpServices(emps);
            var result = services.GetManagersBudget(managerId);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CheckNumberOfCEOS_ReturnFalseStatus()
        {

            List<Employee> employees = new List<Employee>
            {
                Employee.Create("Employee1",null,100),
                Employee.Create("Employee2","",100)
            };
            EmpServices services = new EmpServices(employees);
            response = services.CheckNoOfCEOs();
            Assert.False(response.status);
            Assert.Contains(response.Description, "More than one CEO listed");
        }
        [Fact]
        public void CheckWhenSomeManagersAreNotListed()
        {

            List<Employee> employees = new List<Employee>
            {
                Employee.Create("Employee1","Employee2",900),
                Employee.Create("Employee2","Employee3",1000),
                Employee.Create("Employee3","Employee6",2000)
            };
            EmpServices services = new EmpServices(employees);
            response = services.CheckIfAllMgrAreListed();
            Assert.False(response.status);
            Assert.Contains(response.Description, "Some managers not listed as employees");
        }



        [Fact]
        public void Check_EmployeeHasMoreThanOneManager()
        {
            List<Employee> employees = new List<Employee>
            {
                Employee.Create("Employee1","",100),
                Employee.Create("Employee2","Employee1",100),
                Employee.Create("Employee5","Employee1",100),
                Employee.Create("Employee5","Employee2",100)
            };
            EmpServices services = new EmpServices(employees);
            response = services.EmployeeWithMoreThanOneMgr();
            Assert.False(response.status);
            Assert.Contains(response.Description, "Employee Employee5 has more than 1 Manager");
        }


        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_IdIsInvalid(string id)
        {
            Assert.Throws<ArgumentNullException>(nameof(id), () => Employee.Create(id, null, default(int)));
        }

        [Fact]
        public void Create_WhenSalaryIsInvalid()
        {

            int salary = 0;
            Assert.Throws<ArgumentOutOfRangeException>(nameof(salary), () => Employee.Create("E1", null, salary));
        }

        [Fact]
        public void CheckForCyclicReference()
        {
            List<Employee> employees = new List<Employee>
            {
                Employee.Create("Employee1","",100),
                Employee.Create("Employee3","Employee2",100),
                Employee.Create("Employee2","Employee3",100)
            };
            EmpServices services = new EmpServices(employees);
            response = services.CheckCyclicRef();
            Assert.False(response.status);
            Assert.Contains(response.Description, "Cyclic Reference Identified");

        }
    }
}

