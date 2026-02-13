using System.Collections.Generic;

namespace CAS.Models.ViewModel
{
    public class PhysicianHomeViewModel
    {
        public string UserName { get; set; } = string.Empty;

        public List<Patient> Patients { get; set; } = new();
    }
}
