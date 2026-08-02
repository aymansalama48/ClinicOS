using ClinicOS.Application.Common.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicOS.Application.Features.Doctors.Queries.GetMyProfile
{
    public sealed record GetMyDoctorProfileQuery() : IQuery<DoctorProfileResponse>;

}
