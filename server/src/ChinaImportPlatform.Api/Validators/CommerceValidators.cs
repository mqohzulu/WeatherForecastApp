using ChinaImportPlatform.Api.Dtos;
using FluentValidation;

namespace ChinaImportPlatform.Api.Validators;

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequestDto>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.AmountCents).GreaterThan(0).WithMessage("The amount must be greater than zero.");
        RuleFor(x => x.DueDate).NotEmpty();
    }
}

public class CreateAnnouncementValidator : AbstractValidator<CreateAnnouncementDto>
{
    public CreateAnnouncementValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty();
    }
}

public class SendMessageValidator : AbstractValidator<SendMessageDto>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.SenderId).NotEmpty();
        RuleFor(x => x.Body)
            .NotEmpty().When(x => string.IsNullOrWhiteSpace(x.AttachmentS3Key))
            .WithMessage("A message must have text or an attachment.");
    }
}

public class RegisterDeviceValidator : AbstractValidator<RegisterDeviceDto>
{
    public RegisterDeviceValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FcmToken).NotEmpty();
        RuleFor(x => x.Platform).Must(p => p is "android" or "ios")
            .WithMessage("Platform must be 'android' or 'ios'.");
    }
}
