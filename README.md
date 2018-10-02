# LoginAndRegistrationAPI
Made in the summer of 2018 - It's a login and registration system that uses HTTPS and JWT tokens for security. - Made in .NET Core with C#, Swagger, Microsoft Identity, EntityFrameworkCore and your choice of MySQL or SQLServer (EFCore can use both).

To make it work there are 2 steps you must take.

1. You need to set up the settings in the AppSettings.json file. More specifically these sections:

```
  "ConnectionStrings": {
    "DefaultConnection": "INSERT YOUR CONNECTION STRING HERE"
  },
  "Jwt": {
    "SecretKey": "INSERT JWT SECRET KEY HERE",
    "Audience": "INSERT JWT AUDIENCE SITE HERE",
    "Issuer": "INSERT JWT ISSUING SITE HERE"
  },
  "API": {
    "APIKey": "INSERT YOUR API KEY HERE OR GENERATE IT SOMEWHERE, CAN ALSO STORE IN DATABASE OR SIMILAR."
  }
```

2. In the Startup.cs file, change the options in ConfigureServices to use the SQL type of your choice, e.g. MySql or SqlServer.
Note that there are 2 mysql choices, MySQL or MySql, use MySql.