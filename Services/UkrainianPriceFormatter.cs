using Homework.Services.Abstractions;

namespace Homework.Services;

public class UkrainianPriceFormatter : IPriceFormatter
{
    public string Format(double price) => $"{price:N0} UAH";
}