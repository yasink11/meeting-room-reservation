using FluentValidation;
using MeetingRoomReservation.API.DTOs;
using System;

namespace MeetingRoomReservation.API.Validators
{
    public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
    {
        public CreateReservationDtoValidator()
        {
            RuleFor(x => x.RoomId)
                .GreaterThan(0).WithMessage("Geçerli bir oda seçmelisiniz");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz")
                .MaximumLength(100).WithMessage("Kullanıcı adı en fazla 100 karakter olabilir");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Toplantı başlığı boş olamaz")
                .MaximumLength(200).WithMessage("Başlık en fazla 200 karakter olabilir");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir");

            RuleFor(x => x.StartTime)
                .GreaterThan(DateTime.Now).WithMessage("Başlangıç zamanı geçmişte olamaz");

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime).WithMessage("Bitiş zamanı başlangıç zamanından sonra olmalıdır");

            RuleFor(x => x)
                .Must(HaveValidDuration).WithMessage("Rezervasyon süresi minimum 15 dakika, maksimum 8 saat olmalıdır");

            RuleFor(x => x)
                .Must(BeWithinThreeMonths).WithMessage("Rezervasyon en fazla 3 ay öncesinden yapılabilir");

            RuleFor(x => x.ParticipantCount)
                .GreaterThan(0).WithMessage("Katılımcı sayısı 0'dan büyük olmalıdır");
        }

        private bool HaveValidDuration(CreateReservationDto dto)
        {
            var duration = dto.EndTime - dto.StartTime;
            return duration.TotalMinutes >= 15 && duration.TotalHours <= 8;
        }

        private bool BeWithinThreeMonths(CreateReservationDto dto)
        {
            var threeMonthsLater = DateTime.Now.AddMonths(3);
            return dto.StartTime <= threeMonthsLater;
        }
    }
}