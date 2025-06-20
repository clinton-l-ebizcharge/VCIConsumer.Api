using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VCIConsumer.Api.Models.Responses;
using Xunit;

namespace VCIConsumer.Api.UnitTests;
public class CustomerJsonDeserializationTests
{
    [Fact]
    public void Deserialize_ValidCustomerJson_Succeeds()
    {
        //var path = Path.Combine(AppContext.BaseDirectory, "TestData", "sample_customers.json");
        var json = "[\r\n  {\r\n    \"hrid\": \"RKQ2PL28V936BDV\",\r\n    \"uuid\": \"CUS_1253058024894410752900095XNTC\",\r\n    \"name\": \"customer003\",\r\n    \"email_truncated\": \"c**@123.com\",\r\n    \"phone_truncated\": \"XXX-XXX-0003\",\r\n    \"bank_account\": {\r\n      \"account_type\": \"Savings\",\r\n      \"account_validated\": false,\r\n      \"account_number_truncated\": \"0003\",\r\n      \"routing_number_truncated\": \"0006\"\r\n    },\r\n    \"created_at\": \"2025-06-19 14:45:13 ET\",\r\n    \"modified_at\": \"2025-06-19 14:45:13 ET\",\r\n    \"active\": true\r\n  },\r\n  {\r\n    \"hrid\": \"FLG4AHUTNYJQRFL\",\r\n    \"uuid\": \"CUS_1253055232913764352900095XNTC\",\r\n    \"name\": \"Customer002\",\r\n    \"email_truncated\": \"c**@123.com\",\r\n    \"phone_truncated\": \"XXX-XXX-0002\",\r\n    \"bank_account\": {\r\n      \"account_type\": \"CHECKING\",\r\n      \"account_validated\": false,\r\n      \"account_number_truncated\": \"0002\",\r\n      \"routing_number_truncated\": \"0006\"\r\n    },\r\n    \"created_at\": \"2025-06-19 14:34:08 ET\",\r\n    \"modified_at\": \"2025-06-19 14:34:08 ET\",\r\n    \"active\": true\r\n  },\r\n  {\r\n    \"hrid\": \"W8ZQQX4FYVMHVLS\",\r\n    \"uuid\": \"CUS_1253054927467499520900095XNTC\",\r\n    \"name\": \"Customer001\",\r\n    \"email_truncated\": \"c**@123.com\",\r\n    \"phone_truncated\": \"XXX-XXX-0001\",\r\n    \"bank_account\": {\r\n      \"account_type\": \"CHECKING\",\r\n      \"account_validated\": false,\r\n      \"account_number_truncated\": \"0001\",\r\n      \"routing_number_truncated\": \"0006\"\r\n    },\r\n    \"created_at\": \"2025-06-19 14:32:55 ET\",\r\n    \"modified_at\": \"2025-06-19 14:35:11 ET\",\r\n    \"active\": true\r\n  }\r\n]\r\n";

        // Act
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<List<CustomerCreationResponse>>(json, options);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result, customer =>
        {
            Assert.False(string.IsNullOrWhiteSpace(customer.HRId));
            Assert.False(string.IsNullOrWhiteSpace(customer.UUId));
            Assert.NotNull(customer.BankAccount);
        });
    }
}

