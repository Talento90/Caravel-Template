using FluentValidation;

namespace CaravelTemplate.WebApi.Models.Devices
{
    public class CreateDeviceModel
    {
        public string? Name { get; set; }
    }

    public class CreateDeviceModelValidator : AbstractValidator<CreateDeviceModel>
    {
        public CreateDeviceModelValidator()
        {
            RuleFor(d => d.Name).NotEmpty().MaximumLength(20);
        }
    }
}