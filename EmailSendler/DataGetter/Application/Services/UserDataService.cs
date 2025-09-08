using System.Text.Json;
using System.Text.RegularExpressions;
using Application.Exceptions;
using Application.Interfaces;
using Application.Options;
using Domain.Models;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class UserDataService(
    IUserDataRepository userDataRepository,
    IOutboxMessageService outboxMessageService,
    HttpClient client,
    IOptions<EmailApiOptions> emailOptions,
    IOptions<RabbitOptions> rabbitOptions,
    IOptions<PhoneNumberApiOptions> phoneNumberOptions)
    : IUserDataService
{
    public async Task<UserData> GetData(string email,string address, string phoneNumber, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phoneNumber))
        {
            throw new NullValidationException("All fields must be filled");
        }
        if (!IsCorrectEmailFormat(email))
        {
            throw new IncorrectEmailException("Incorrect Email format");
        }
        // if (!await ValidateEmail(email))
        // {
        //     throw new IncorrectEmailException("Nonexistent Email");
        // }
        
        
        // if(!await ValidateAddress(address))
        // {
        //     throw new IncorrectAddressException("Unreal address");
        // }
         
         
        // if (!await ValidatePhoneNumber(phoneNumber))
        // {
        //     throw new IncorrectPhoneException("Unexisting Phone number");
        // }

        var existingUser = await userDataRepository.GetDataByEmailAsync(email, cancellationToken);
        if (existingUser is not null)
        {
            await outboxMessageService.EnqueueAsync(email,rabbitOptions.Value.QueueName ,cancellationToken);
            await userDataRepository.SaveChangesAsync(cancellationToken);
            await outboxMessageService.PendingMessagesAsync(cancellationToken);
            return existingUser;
        }

        var newUser = new UserData(email, address, phoneNumber);
        await userDataRepository.AddUserDataAsync(newUser, cancellationToken);
        await outboxMessageService.EnqueueAsync(email, rabbitOptions.Value.QueueName,cancellationToken);
        await userDataRepository.SaveChangesAsync(cancellationToken); 
        await outboxMessageService.PendingMessagesAsync(cancellationToken);

        return newUser;
    }

    private bool IsCorrectEmailFormat(string email)
    {

        var emailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        return emailRegex.IsMatch(email);
    }
    
     private async Task<bool> ValidateEmail(string email)
     {
         var baseUrl =  emailOptions.Value.BaseUrl;
         var apiKey = emailOptions.Value.ApiKey;
         
         var url = $"{baseUrl}?api_key={apiKey}&email={email}";
         var response = await client.GetAsync(url);
         response.EnsureSuccessStatusCode();
         
         var json = await response.Content.ReadAsStringAsync();
         var options = new JsonSerializerOptions()
         {
             PropertyNameCaseInsensitive = true
         };
         var result = JsonSerializer.Deserialize<EmailValidationResponse>(json, options);
         
         return string.Equals(result?.Deliverability, "DELIVERABLE", StringComparison.OrdinalIgnoreCase);
     }
    
    // private async Task<bool> ValidateAddress(string address)
    // {
    //     var baseUrl =  _addressOptions.Value.BaseUrl;
    //     var apiKey = _addressOptions.Value.ApiKey;
    //     
    //     var url = $"";
    //     var response = await _client.GetAsync(url);
    //     response.EnsureSuccessStatusCode();
    //     
    //     var json = await response.Content.ReadAsStringAsync();
    //     var options = new JsonSerializerOptions()
    //     {
    //         PropertyNameCaseInsensitive = true
    //     };
    //     var result = JsonSerializer.Deserialize<AddressValidationResponse>(json, options);
    //
    //     return ;
    // }
    
    private async Task<bool> ValidatePhoneNumber(string phoneNumber)
    {
        var baseUrl =   phoneNumberOptions.Value.BaseUrl;
        var apiKey = phoneNumberOptions.Value.ApiKey;
        
        var url = $"{baseUrl}?api_key={apiKey}&phone={phoneNumber}";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        var result = JsonSerializer.Deserialize<PhoneValidationResponse>(json, options);

        return result?.Valid == true;
    }
}


public class EmailValidationResponse(string email, string deliverability)
{
    public string Email { get; init; } = email;
    public string Deliverability { get; init; } = deliverability;
}
public class AddressValidationResponse(string address, bool isExists)
{
    public string Address { get; init; } = address;
    public bool IsExists { get; init; } = isExists;
}

public class PhoneValidationResponse(string phoneNumber, bool valid)
{
    public string PhoneNumber { get; init; } = phoneNumber;
    public bool Valid { get; init; } = valid;
}