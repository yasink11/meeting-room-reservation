using FluentValidation;
using MeetingRoomReservation.API.DTOs;

namespace MeetingRoomReservation.API.Validators
{
    public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
    {
        public CreateRoomDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Oda adı boş olamaz")
                .MaximumLength(100).WithMessage("Oda adı en fazla 100 karakter olabilir");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Kapasite 0'dan büyük olmalıdır")
                .LessThanOrEqualTo(200).WithMessage("Kapasite en fazla 200 olabilir");

            RuleFor(x => x.Floor)
                .GreaterThanOrEqualTo(0).WithMessage("Kat numarası 0'dan küçük olamaz")
                .LessThanOrEqualTo(50).WithMessage("Kat numarası en fazla 50 olabilir");

            RuleFor(x => x.Equipment)
                .NotNull().WithMessage("Donanım listesi boş olamaz");
        }
    }
}