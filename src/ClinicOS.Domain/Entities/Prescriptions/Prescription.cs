using ClinicOS.Domain.Common.Entities;
using ClinicOS.Domain.Entities.MedicalRecords;

namespace ClinicOS.Domain.Entities.Prescriptions;

/// <summary>
/// الروشتة الطبية
/// </summary>
public class Prescription : AuditableEntity
{
    public Guid MedicalRecordId { get; private set; }

    public DateTime IssueDate { get; private set; }
    public string? Notes { get; private set; }

    public virtual MedicalRecord MedicalRecord { get; private set; } = null!;
    
    private readonly List<PrescriptionItem> _items = new();
    public virtual IReadOnlyCollection<PrescriptionItem> Items => _items.AsReadOnly();

    protected Prescription() { }

    public static Prescription Create(string medications, string? notes)
    {
        var prescription = new Prescription
        {
            Id = Guid.NewGuid(),
            IssueDate = DateTime.UtcNow,
            Notes = notes
        };

        // Simple parsing or add as a single item
        prescription._items.Add(new PrescriptionItem
        {
            Id = Guid.NewGuid(),
            PrescriptionId = prescription.Id,
            MedicineName = medications,
            Instructions = "As directed by physician"
        });

        return prescription;
    }
}