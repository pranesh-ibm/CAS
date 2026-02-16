namespace CAS.Models.ViewModel
{
    public class AdminDashboardViewModel {
        public int TotalPatients { get; set; }
        public int TotalPhysicians { get; set; }
        public int TotalChemists { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalAppointments { get; set; }

        // Pending Counts
        public int PendingPatients { get; set; }
        public int PendingAppointments { get; set; }

        // Today
        public int TodayAppointments { get; set; }
    }
}
