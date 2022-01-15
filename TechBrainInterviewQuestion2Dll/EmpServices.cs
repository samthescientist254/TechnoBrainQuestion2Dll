using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Employee.Domain
{
    public class EmpServices
    {
        private List<Employee> _emps;

        Response response = new Response();
        public bool ValidStmt { get; private set; } = true;
        public EmpServices(List<Employee> employees)
        {
            _emps = employees ?? throw new ArgumentNullException(nameof(employees));

        }
        public void ValidateEmp() {

            var task1 = Task.Factory.StartNew(() => CheckNoOfCEOs());
            var task2 = Task.Factory.StartNew(() => CheckIfAllMgrAreListed());
            var task3 = Task.Factory.StartNew(() => EmployeeWithMoreThanOneMgr());
            var task4 = Task.Factory.StartNew(() => CheckCyclicRef());

            Task.WaitAll(task1, task2, task3);
        }

        public Response CheckNoOfCEOs()
        {
            if (_emps.Where(e => e.ManagerId == string.Empty || e.ManagerId == null).Count() > 1)
            {
                ValidStmt = false;
                response.status = false;
                response.Description = "More than one CEO listed";
            }
            else
            {
                response.status = true;
                response.Description = "One CEO listed";
            }

            return response;
        }

        public long GetManagersBudget(string managerId)
        {
            if (string.IsNullOrWhiteSpace(managerId)) throw new ArgumentNullException(nameof(managerId));
            long total = 0;
            total += _emps.FirstOrDefault(e => e.Id == managerId).Salary;
            foreach (Employee item in _emps.Where(e => e.ManagerId == managerId))
            {
                if (IsMgr(item.Id))
                {
                    total += GetManagersBudget(item.Id);
                }
                else
                {
                    total += item.Salary;
                }
            }
            return total;
        }


        public Response CheckIfAllMgrAreListed()
        {
            var empIds = _emps.Select(l => l.Id).Distinct().ToList();
            var mgrIds = _emps.Select(l => l.ManagerId).Distinct().ToList();

            var MgrNotEmp = mgrIds.Except(empIds, StringComparer.OrdinalIgnoreCase).ToList();
            if (MgrNotEmp.Count > 0)
            {
                ValidStmt = false;
                response.status = false;
                response.Description = "Some managers not listed as employees";
            }
            else
            {
                response.status = true;
                response.Description = "All managers present";
            }
            return response;

        }
        private bool IsMgr(string id) => _emps.Where(e => e.ManagerId == id).Count() > 0;


        public Response EmployeeWithMoreThanOneMgr()
        {
            List<Response> responses = new List<Response>();
            foreach (var id in _emps.Select(u => u.Id).Distinct().Where(id => _emps.Where(x => x.Id == id)
            .Select(m => m.ManagerId).Distinct().Count() > 1).Select(id => id))
            {
                ValidStmt = false;
                response.status = false;
                response.Description = $"Employee {id} has more than 1 Manager";

            }
            return response;
        }
        public Response CheckCyclicRef()
        {
            foreach (var _ in from emp in _emps.Where(e => e.ManagerId != string.Empty && e.ManagerId != null)
                              let mgr = _emps.Where(e => e.ManagerId != string.Empty && e.ManagerId != null)
                        .FirstOrDefault(e => e.Id == emp.ManagerId)
                              where mgr != null
                              where mgr.ManagerId == emp.Id
                              select new { })
            {
                ValidStmt = false;
                response.status = false;
                response.Description = "Cyclic Reference Identified";

            }
            return response;
        }

    }
}
