using System.Collections.Generic;

namespace CAS.Models.ViewModel
{
    public class PhysicianHomeViewModel
    {
        public string PhysicianName { get; set; } = "Physician";

        public List<Patient> Patients { get; set; } = new();
    }
}
