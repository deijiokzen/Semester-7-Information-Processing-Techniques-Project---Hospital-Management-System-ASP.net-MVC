using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hospital_Management_System.CollectionViewModels;
using Hospital_Management_System.Models;
using Microsoft.AspNet.Identity;

namespace Hospital_Management_System.Controllers
{
    public class PatientController : Controller
    {
        private ApplicationDbContext db;
        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        //Constructor
        public PatientController()
        {
            db = new ApplicationDbContext();
        }
        //Destructor
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI


        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI


        //Update Patient profile
        [Authorize(Roles = "Patient")]        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        public ActionResult UpdateProfile(string id)
        {        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            var patient = db.Patients.Single(c => c.ApplicationUserId == id);        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            return View(patient);
        }
        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        [HttpPost]
        [ValidateAntiForgeryToken]        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        public ActionResult UpdateProfile(string id, Patient model)
        {
            var patient = db.Patients.Single(c => c.ApplicationUserId == id);
            patient.FirstName = model.FirstName;
            patient.LastName = model.LastName;
            patient.Address = model.Address;
            patient.DateOfBirth = model.DateOfBirth;        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            patient.Gender = model.Gender;
            patient.BloodGroup = model.BloodGroup;
   
            patient.PhoneNo = model.PhoneNo;
            patient.FullName = model.FirstName + " " + model.LastName;        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            patient.Contact = model.Contact;
            //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            db.SaveChanges();
            return View();
        }
        [Authorize(Roles = "Patient")]
        public ActionResult Index(string message)
        {        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            ViewBag.Messege = message;
            string user = User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.ApplicationUserId == user);
            var date = DateTime.Now.Date;
            var model = new CollectionOfAll
            {
                Ambulances = db.Ambulances.ToList(),        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

                Departments = db.Department.ToList(),
                Patients = db.Patients.ToList(),
                Medicines = db.Medicines.ToList(),        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

                Doctors = db.Doctors.ToList(),

                ActiveAppointments = db.Appointments.Where(c => c.Status).Where(c => c.PatientId == patient.Id).Where(c => c.AppointmentDate >= date).ToList(),        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

                PendingAppointments = db.Appointments.Where(c => c.Status == false).Where(c => c.PatientId == patient.Id).Where(c => c.AppointmentDate >= date).ToList(),
                AmbulanceDrivers = db.AmbulanceDrivers.ToList(),
                Announcements = db.Announcements.Where(c => c.AnnouncementFor == "Patient").ToList()
            };
            return View(model);
        }
        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        //Start Appointment Section

        //Add Appointment
        [Authorize(Roles = "Patient")]        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        public ActionResult AddAppointment()
        {
            var collection = new AppointmentCollection
            {
                Appointment = new Appointment(),
                Doctors = db.Doctors.ToList()
            };
            return View(collection);
        }        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI



        //List of Appointments
        [Authorize(Roles = "Patient")]
        public ActionResult ListOfAppointments()
        {
            string user = User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.ApplicationUserId == user);
            var appointment = db.Appointments.Include(c => c.Doctor).Where(c => c.PatientId == patient.Id).ToList();
            return View(appointment);        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        }

        //Edit Appointment
        [Authorize(Roles = "Patient")]
        public ActionResult EditAppointment(int id)
        {
            var collection = new AppointmentCollection
            {
                Appointment = db.Appointments.Single(c => c.Id == id),
                Doctors = db.Doctors.ToList()
            };
            return View(collection);
        }
        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAppointment(AppointmentCollection model)
        {
            var collection = new AppointmentCollection
            {
                Appointment = model.Appointment,
                Doctors = db.Doctors.ToList()
            };        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            if (model.Appointment.AppointmentDate >= DateTime.Now.Date)
            {
                string user = User.Identity.GetUserId();
                var patient = db.Patients.Single(c => c.ApplicationUserId == user);
                var appointment = new Appointment();
                appointment.PatientId = patient.Id;
                appointment.AppointmentDate = model.Appointment.AppointmentDate;        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

                appointment.Problem = model.Appointment.Problem;
                appointment.Status = false;
                appointment.DoctorId = model.Appointment.DoctorId;


                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("ListOfAppointments");
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";
            //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            return View(collection);

        }




        //End Appointment Section        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAppointment(int id, AppointmentCollection model)
        {
            var collection = new AppointmentCollection
            {
                Appointment = model.Appointment,
                Doctors = db.Doctors.ToList()
            };
            if (model.Appointment.AppointmentDate >= DateTime.Now.Date)
            {
                var appointment = db.Appointments.Single(c => c.Id == id);
                appointment.AppointmentDate = model.Appointment.AppointmentDate;
                appointment.Problem = model.Appointment.Problem;        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI


                appointment.DoctorId = model.Appointment.DoctorId;

                db.SaveChanges();
                return RedirectToAction("ListOfAppointments");
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";
            //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            return View(collection);
        }

        //Delete Appointment
        [Authorize(Roles = "Patient")]
        public ActionResult DeleteAppointment(int? id)
        {
            var appointment = db.Appointments.Single(c => c.Id == id);
            return View(appointment);
        }        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI


        [HttpPost, ActionName("DeleteAppointment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAppointment(int id)
        {
            var appointment = db.Appointments.Single(c => c.Id == id);
            db.Appointments.Remove(appointment);        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            db.SaveChanges();
            return RedirectToAction("ListOfAppointments");
        }
        //Start Doctor Section

        //Show Doctor Schedule
        [Authorize(Roles = "Patient")]
        public ActionResult DoctorSchedule(int id)
        {
            var schedule = db.Schedules.Include(c => c.Doctor).Single(c => c.DoctorId == id);        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI
        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            return View(schedule);
        }
        //List of Available Doctors
        [Authorize(Roles = "Patient")]
        public ActionResult AvailableDoctors()
        {
            var doctor = db.Doctors.Include(c => c.Department).Where(c => c.Status == "Active").ToList();
            return View(doctor);
        }

        //Doctor Detail
        [Authorize(Roles = "Patient")]
        public ActionResult DoctorDetail(int id)        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        {
            var doctor = db.Doctors.Include(c => c.Department).Single(c => c.Id == id);
            return View(doctor);
        }

        //End Doctor Section

        //Start Complaint Section

        [Authorize(Roles = "Patient")]        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        public ActionResult AddComplain()
        {
            return View();
        }
        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        [Authorize(Roles = "Patient")]
        public ActionResult ListOfComplains()
        {
            var complain = db.Complaints.ToList();
            return View(complain);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComplain(Complaint model)        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        {
            var complain = new Complaint();
            complain.ComplainDate = DateTime.Now.Date;
            complain.Complain = model.Complain;

            db.Complaints.Add(complain);
            db.SaveChanges();        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            return RedirectToAction("ListOfComplains");
        }        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI


        [Authorize(Roles = "Patient")]
        public ActionResult EditComplain(int id)
        {
            var complain = db.Complaints.Single(c => c.Id == id);
            return View(complain);
        }
        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        [Authorize(Roles = "Patient")]
        public ActionResult DeleteComplain()
        {
            return View();        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComplain(int id, Complaint model)        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        {
            var complain = db.Complaints.Single(c => c.Id == id);
            complain.Complain = model.Complain;
            db.SaveChanges();
            return RedirectToAction("ListOfComplains");        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        }

        [HttpPost, ActionName("DeleteComplain")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComplain(int id)        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        {
            var complain = db.Complaints.Single(c => c.Id == id);
            db.Complaints.Remove(complain);
            db.SaveChanges();
            return RedirectToAction("ListOfComplains");
        }
        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        //End Complain Section

        //Start Prescription Section

        //List of Prescription        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

        [Authorize(Roles = "Patient")]
        public ActionResult ListOfPrescription()
        {
            string user = User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.ApplicationUserId == user);        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            var prescription = db.Prescription.Include(c => c.Doctor).Where(c => c.PatientId == patient.Id).ToList();
            return View(prescription);
        }

        //Prescription View
        public ActionResult PrescriptionView(int id)
        {
            var prescription = db.Prescription.Single(c => c.Id == id);        //THIS CODE IS THE PROPERTY OF SAUD AHMED ABBASI

            return View(prescription);
        }

        //End Prescription Section
    }
}