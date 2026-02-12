/* =========================================
   1️⃣ INSERT MASTER TABLES
========================================= */


INSERT INTO Physician 
(PhysicianName, Specialization, Address, Phone, Email, Summary, PhysicianStatus)
VALUES
('Dr. Amit Sharma', 'Cardiology', 'Lucknow', '9999999991', 'amit@medicure.com', 'Senior Cardiologist', 'Active'),
('Dr. Neha Verma', 'Orthopedics', 'Delhi', '9999999992', 'neha@medicure.com', 'Bone Specialist', 'Active');


INSERT INTO Supplier
(SupplierName, Address, Phone, Email, SupplierStatus)
VALUES
('ABC Pharma', 'Mumbai', '8888888881', 'abc@pharma.com', 'Active'),
('XYZ Medicos', 'Delhi', '8888888882', 'xyz@medicos.com', 'Active');


INSERT INTO Patient
(PatientName, DOB, Gender, Address, Phone, Email, Summary, PatientStatus)
VALUES
('Rohit Singh', '1998-05-10', 'Male', 'Lucknow', '7777777771', 'rohit@gmail.com', 'Regular Patient', 'Active'),
('Anjali Gupta', '1995-08-15', 'Female', 'Kanpur', '7777777772', 'anjali@gmail.com', 'First Visit', 'Active');


INSERT INTO Chemist
(ChemistName, Address, Phone, Email, Summary, ChemistStatus)
VALUES
('Ramesh', 'Lucknow', '6666666661', 'ramesh@chemist.com', 'Local medical store owner', 'Active');


INSERT INTO Drug
(DrugTitle, Description, Expiry, Dosage, DrugStatus)
VALUES
('Paracetamol', 'Pain & fever reducer', '2026-12-31', '500mg', 'Active'),
('Amoxicillin', 'Antibiotic', '2025-06-30', '250mg', 'Active'),
('Aspirin', 'Blood thinner', '2026-09-15', '75mg', 'Active'),
('Atorvastatin', 'Cholesterol control', '2027-03-10', '10mg', 'Active'),
('Pantoprazole', 'Acid reducer', '2026-11-20', '40mg', 'Active');


/* =========================================
   2️⃣ FETCH REQUIRED IDs
========================================= */

DECLARE @PatientID INT
DECLARE @PhysicianID INT
DECLARE @ScheduleID INT
DECLARE @AdviceID INT
DECLARE @POID INT

DECLARE @ParacetamolID INT
DECLARE @AspirinID INT
DECLARE @PantoprazoleID INT
DECLARE @SupplierID INT

-- Fetch Patient
SELECT @PatientID = PatientID 
FROM Patient 
WHERE PatientName = 'Rohit Singh';

-- Fetch Physician
SELECT @PhysicianID = PhysicianID 
FROM Physician 
WHERE PhysicianName = 'Dr. Amit Sharma';

-- Fetch Drugs
SELECT @ParacetamolID = DrugID FROM Drug WHERE DrugTitle = 'Paracetamol';
SELECT @AspirinID = DrugID FROM Drug WHERE DrugTitle = 'Aspirin';
SELECT @PantoprazoleID = DrugID FROM Drug WHERE DrugTitle = 'Pantoprazole';

-- Fetch Supplier
SELECT @SupplierID = SupplierID 
FROM Supplier 
WHERE SupplierName = 'ABC Pharma';


/* =========================================
   3️⃣ INSERT APPOINTMENT
========================================= */

INSERT INTO Appointment
(PatientID, AppointmentDate, Criticality, Reason, Note, ScheduleStatus)
VALUES
(@PatientID, GETDATE(), 'Medium', 'Chest Pain', 'Needs ECG', 'Scheduled');


DECLARE @AppointmentID INT = SCOPE_IDENTITY();



/* =========================================
   4️⃣ INSERT SCHEDULE
========================================= */

INSERT INTO Schedule
(PhysicianID, AppointmentID, ScheduleDate, ScheduleTime, ScheduleStatus)
VALUES
(@PhysicianID, @AppointmentID, CAST(GETDATE() AS DATE), '10:30', 'Confirmed');


SET @ScheduleID = SCOPE_IDENTITY();


/* =========================================
   5️⃣ INSERT PHYSICIAN ADVICE
========================================= */

INSERT INTO PhysicianAdvice
(ScheduleID, Advice, Note)
VALUES
(@ScheduleID, 'Low salt diet, daily walk, avoid stress', 'Follow-up after 7 days');


SET @AdviceID = SCOPE_IDENTITY();


/* =========================================
   6️⃣ INSERT PRESCRIPTION
========================================= */

INSERT INTO PhysicianPrescrip
(PhysicianAdviceID, DrugID, Prescription, Dosage)
VALUES
(@AdviceID, @ParacetamolID, 'Twice daily after meals', '500mg'),
(@AdviceID, @AspirinID, 'Once daily at night', '75mg'),
(@AdviceID, @PantoprazoleID, 'Before breakfast', '40mg');


/* =========================================
   7️⃣ INSERT DRUG REQUEST
========================================= */

INSERT INTO DrugRequest
(PhysicianID, DrugsInfoText, RequestDate, RequestStatus)
VALUES
(@PhysicianID, 'Requesting regular cardiac medicines', GETDATE(), 'Pending');


/* =========================================
   8️⃣ INSERT PURCHASE ORDER
========================================= */

INSERT INTO PurchaseOrderHeader
(PONo, PODate, SupplierID)
VALUES
('PO-001', GETDATE(), @SupplierID);


SET @POID = SCOPE_IDENTITY();


INSERT INTO PurchaseProductLine
(POID, DrugID, SlNo, Qty, Note)
VALUES
(@POID, @ParacetamolID, 1, 500, 'Pain relief stock'),
(@POID, @AspirinID, 2, 300, 'Cardiac medicine'),
(@POID, @PantoprazoleID, 3, 200, 'Gastric medicine');

INSERT INTO [User]
(UserName, [Password], Role, RoleReferenceID, Status)
VALUES
-- Admin
('admin', 'admin123', 'Admin', NULL, 'Active'),

-- Physicians
('dramit', 'password123', 'Physician', 1, 'Active'),
('drneha', 'password123', 'Physician', 2, 'Active'),

-- Patients
('rohit.p', 'password123', 'Patient', 1, 'Active'),
('anjali.p', 'password123', 'Patient', 2, 'Active'),

-- Suppliers
('abcpharma', 'password123', 'Supplier', 1, 'Active'),
('xyzmedicos', 'password123', 'Supplier', 2, 'Active'),

-- Chemist
('ramesh', 'password123', 'Chemist', 1, 'Active');