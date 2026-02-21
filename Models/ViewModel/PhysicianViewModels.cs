using CAS.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CAS.Models.ViewModel
{
    public class PatientListViewModel
    {
        public List<Patient> Patients { get; set; } = new List<Patient>();
    }

    public class CreatePrescriptionViewModel
    {
        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
        public List<Drug> Drugs { get; set; } = new List<Drug>();
        // Optional pre-selected schedule id (used to pre-select patient/schedule in the form)
        public int? SelectedScheduleId { get; set; }
    }

    public class CreatePrescriptionFormModel
    {
        [Required]
        public int ScheduleId { get; set; }

        [Required]
        public int DrugId { get; set; }

        [Required]
        public string Prescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "This field is required")]
        public string Dosage { get; set; } = string.Empty;

        [Required(ErrorMessage = "This field is required")]
        public string Advice { get; set; } = string.Empty;

        [Required(ErrorMessage = "This field is required")]
        public string Note { get; set; } = string.Empty;
    }

    public class PrescriptionListViewModel
    {
        public List<PhysicianPrescrip> Prescriptions { get; set; } = new List<PhysicianPrescrip>();
    }

    public class AppointmentListViewModel
    {
        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    }

    public class DrugListViewModel
    {
        public List<Drug> Drugs { get; set; } = new List<Drug>();
    }

    public class DrugRequestListViewModel
    {
        public List<DrugRequest> Requests { get; set; } = new List<DrugRequest>();
    }

    public class OrderDrugsViewModel
    {
        public OrderDrugsFormModel Form { get; set; } = new OrderDrugsFormModel();
    }

    public class OrderDrugsFormModel
    {
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Drugs info")]
        public string DrugsInfoText { get; set; } = string.Empty;
    }

    public class PhysicianProfileViewModel
    {
        public int PhysicianId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string PhysicianName { get; set; } = string.Empty;
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Specialization")]
        public string Specialization { get; set; } = string.Empty;

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Phone")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Summary")]
        public string Summary { get; set; } = string.Empty;
    }
}
