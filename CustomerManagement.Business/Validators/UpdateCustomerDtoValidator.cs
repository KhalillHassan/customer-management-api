using CustomerManagement.Business.DTOs;
using FluentValidation;

namespace CustomerManagement.Business.Validators;

public class UpdateCustomerDtoValidator
    : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        RuleFor(customer => customer.FullName)
    .NotEmpty()
    .WithMessage("Full name is required.");

        RuleFor(customer => customer.Email)
    .NotEmpty()
    .WithMessage("Email is required.")
    .EmailAddress()
    .WithMessage(
        "Email must be a valid email address.");

        RuleFor(customer => customer.PhoneNumber)
    .NotEmpty()
    .WithMessage("Phone number is required.");
    }
}