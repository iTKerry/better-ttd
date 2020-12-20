using System;
using BetterTTD.Domain.Entities;
using CSharpFunctionalExtensions;

namespace BetterTTD.WPF.Models
{
    public class ClientModel : BaseModel
    {
        private ClientModel(Client client)
        {
            Id = client.Id;
            Name = client.Name;
        }

        public static Result<ClientModel> Create(Maybe<Client> maybeClient)
        {
            return maybeClient.HasNoValue
                ? Result.Failure<ClientModel>($"[{nameof(ClientModel)}] Unknown client handled.")
                : new ClientModel(maybeClient.Value);
        }

        public long Id
        {
            get => Get((long) -1);
            set => Set(value);
        }
        
        public string Name
        {
            get => Get("Unknown");
            set => Set(value);
        }
        
        public Company Company
        {
            get => Get<Company>();
            set => Set(value);
        }
    }

    public class CompanyModel : BaseModel
    {
        public CompanyModel(Maybe<Company> maybeCompany)
        {
            if (maybeCompany.HasNoValue)
            {
                Console.WriteLine($"[{nameof(CompanyModel)}] Unknown company handled.");
                return;
            }

            var company = maybeCompany.Value;
            Name = company.Name;
        }

        public string Name
        {
            get => Get("Unknown");
            set => Set(value);
        }
    }
}