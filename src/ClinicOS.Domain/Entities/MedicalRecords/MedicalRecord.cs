using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.Appointments;
using ClinicOS.Domain.Entities.Prescriptions;

namespace ClinicOS.Domain.Entities.MedicalRecords;

/// <summary>
/// السجل الطبي للزيارة
/// </summary>
public class MedicalRecord : AuditableEntity
{
    public Guid AppointmentId { get; private set; }
    public string? Diagnosis { get; private set; }
    public string? Notes { get; private set; }
    public Guid? CreatedByDoctorId { get; private set; }

    public virtual Appointment Appointment { get; private set; } = null!;

    private readonly List<Prescription> _prescriptions = new();
    public virtual IReadOnlyCollection<Prescription> Prescriptions => _prescriptions.AsReadOnly();

    private readonly List<Test> _tests = new();
    public virtual IReadOnlyCollection<Test> Tests => _tests.AsReadOnly();

    protected MedicalRecord() { }

    public static MedicalRecord Create(Guid appointmentId, string? diagnosis, string? notes, Guid? createdByDoctorId)
    {
        return new MedicalRecord
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointmentId,
            Diagnosis = diagnosis,
            Notes = notes,
            CreatedByDoctorId = createdByDoctorId
        };
    }

    public void AddPrescription(Prescription prescription)
    {
        _prescriptions.Add(prescription);
    }

    public void AddTest(Test test)
    {
        _tests.Add(test);
    }
}