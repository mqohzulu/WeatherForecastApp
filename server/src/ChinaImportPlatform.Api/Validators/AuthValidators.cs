using ChinaImportPlatform.Api.Dtos;
using FluentValidation;

namespace ChinaImportPlatform.Api.Validators;

public class RequestOtpValidator : AbstractValidator<RequestOtpDto>
{
    public RequestOtpValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("A mobile number is required.")
            .Matches(@"^(\+27|0)[0-9]{9}$")
            .WithMessage("Enter a valid South African mobile number (e.g. +2782xxxxxxx or 082xxxxxxx).");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Enter a valid email address.");
    }
}

public class VerifyOtpValidator : AbstractValidator<VerifyOtpDto>
{
    public VerifyOtpValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Otp)
            .NotEmpty()
            .Matches(@"^[0-9]{6}$").WithMessage("The OTP must be 6 digits.");
    }
}
